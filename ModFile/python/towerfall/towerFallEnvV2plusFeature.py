# https://chatgpt.com/c/69027548-7a3c-832a-b1ca-b550cc1ee177
# utiliser cnn avec 4canaus, level/joueur/ennemy/fleche  -> voir pour al position des x y avec le level x y dans quel sens est le level
# Normalisation des valeurs d’état => plus besoin car tout est en 10dans les 4 canaux
# apprendre par partie plutot qu en continue  voir Exemple concret (à mettre dans TrainingAgent2.run()))
# Action space plus précis, => Recommandation — “Action Set” raisonné (≈ 25 actions utiles)
import logging
import gymnasium as gym
import numpy as np
import time


class TowerFallEnvV2plusFeature(gym.Env):
    """
    Version CNN + Features du TowerFallEnv
    Observation = Dict({
        "image": 4 canaux (grid, player, enemies, arrows),
        "features": [x, y, vx, vy]
    })
    Action space = Discrete(23) (combinaisons utiles de touches)
    """

    def __init__(self, agent, game_state):
        super(TowerFallEnvV2plusFeature, self).__init__()
        logging.info("Init TowerFallEnvV2plusFeature (CNN + features + Discrete(23))")

        self.agent = agent
        self.game_state = game_state
        self.previous_game_state = game_state
        self.first = True

        self.total_step = 0
        self.total_game = 0
        self.game_reward = 0

        # 🎮 Actions combinées
        self.action_space = gym.spaces.Discrete(23)

        # 🧠 Observation : image (4,24,32) + features (4,)
        self.observation_space = gym.spaces.Dict({
            "image": gym.spaces.Box(low=0, high=1, shape=(4, 24, 32), dtype=np.float32),
            "features": gym.spaces.Box(low=-1, high=1, shape=(4,), dtype=np.float32)
        })

    # ----------------------------------------------------------
    # 🔄 Step Gym
    # ----------------------------------------------------------
    def step(self, action):
        self.total_step += 1
        self._send_action(action)
        obs = self._get_observation()

        reward = self._calculate_reward()
        done = self._check_done()
        truncated = False

        return obs, reward, done, truncated, {}

    # ----------------------------------------------------------
    # ♻️ Reset entre deux matchs
    # ----------------------------------------------------------
    def reset(self, seed=None, **kwargs):
        self.total_game += 1
        self.game_reward = 0
        self.first = True
        game_restart = 0

        self.agent.rematch = True
        while True:
            if not self.agent.act(self.game_state):
                self.agent.send_actions()
            self.agent.rematch = False
            self.game_state = self.agent.connection.read_json()
            if self.game_state["type"] == "init":
                game_restart += 1
            if self.game_state["type"] == "scenario":
                game_restart += 1
                self.agent.state_scenario = self.game_state
            if self.game_state["type"] == "update" and game_restart == 2:
                self.previous_game_state = self.game_state
                break

        obs = self._get_observation()
        return obs, {}

    # ----------------------------------------------------------
    # 🧠 Observation CNN + features normalisées
    # ----------------------------------------------------------
    def _get_observation(self):
        # 1️⃣ Grid de base
        grid = np.array(self.agent.state_scenario["grid"], dtype=np.float32)
        player_map = np.zeros_like(grid)
        enemy_map = np.zeros_like(grid)
        arrow_map = np.zeros_like(grid)

        player_x = player_y = 0
        player_vx = player_vy = 0

        # 2️⃣ Entités du jeu
        for e in self.game_state["entities"]:
            pos = e.get("pos", {"x": 0, "y": 0})
            vel = e.get("vel", {"x": 0, "y": 0})
            x = int(pos["x"] // 10)
            y = int(pos["y"] // 10)

            if not (0 <= y < 24 and 0 <= x < 32):
                continue

            t = e.get("type")
            if t == "archer":
                if e.get("playerIndex") == self.agent.id:
                    player_map[y, x] = 1.0
                    # Normalisation position / vitesse
                    player_x = pos["x"] / 320.0  # niveau = 32 cases * 10px
                    player_y = pos["y"] / 240.0
                    player_vx = np.clip(vel["x"] / 50.0, -1, 1)
                    player_vy = np.clip(vel["y"] / 50.0, -1, 1)
                else:
                    enemy_map[y, x] = 1.0
            elif t == "arrow":
                arrow_map[y, x] = 1.0
            elif t in ["crackedPlatform", "crumbleBlock", "movingPlatform"]:
                # Mise à jour dynamique des blocs
                size = e.get("size", {"x": 10, "y": 10})
                x_end = int((pos["x"] + size["x"]) // 10)
                y_end = int((pos["y"] + size["y"]) // 10)
                for yy in range(y, min(y_end + 1, 24)):
                    for xx in range(x, min(x_end + 1, 32)):
                        grid[yy, xx] = 1.0

        # 3️⃣ Image et features
        image_obs = np.stack([grid, player_map, enemy_map, arrow_map], axis=0)
        features = np.array([player_x, player_y, player_vx, player_vy], dtype=np.float32)

        return {"image": image_obs, "features": features}

    # ----------------------------------------------------------
    # 🎮 Envoi d'une action (Discrete(23))
    # ----------------------------------------------------------
    def _send_action(self, action):
        self.agent.release_all()  # relâche toutes les touches avant

        # Dictionnaire des actions
        action_map = {
            0: [],
            1: ["l"], 2: ["r"], 3: ["u"], 4: ["d"],
            5: ["j"], 6: ["l", "j"], 7: ["r", "j"], 8: ["u", "j"], 9: ["d", "j"],
            10: ["s"], 11: ["l", "s"], 12: ["r", "s"], 13: ["u", "s"], 14: ["d", "s"],
            15: ["l", "j", "s"], 16: ["r", "j", "s"], 17: ["u", "j", "s"], 18: ["d", "j", "s"],
            19: ["z"], 20: ["l", "z"], 21: ["r", "z"], 22: ["u", "z"]
        }

        for key in action_map[action]:
            self.agent.press(key)

        self.agent.send_actions()

    # ----------------------------------------------------------
    # 💰 Récompenses
    # ----------------------------------------------------------
    def _calculate_reward(self):
        reward = 0

        # Joueur mort
        player_corpse = next(
            (e for e in self.game_state["entities"]
             if e["type"] == "playerCorpse" and e["playerIndex"] == self.agent.id),
            None
        )
        player_archer = next(
            (e for e in self.game_state["entities"]
             if e["type"] == "archer" and e["playerIndex"] == self.agent.id),
            None
        )

        if player_corpse or (player_archer and player_archer.get("dead", False)):
            reward -= 5

        # Kills des ennemis
        for e in self.game_state["entities"]:
            if e["type"] in ["archer", "playerCorpse"]:
                killer = e.get("killer", -1)
                if killer == self.agent.id and e.get("playerIndex") != self.agent.id:
                    reward += 10

        self.game_reward += reward
        return reward

    # ----------------------------------------------------------
    # ✅ Fin de partie
    # ----------------------------------------------------------
    def _check_done(self):
        for e in self.game_state["entities"]:
            if e["type"] in ["archer", "playerCorpse"] and e.get("playerIndex") == self.agent.id:
                if e.get("dead", False) or e["type"] == "playerCorpse":
                    return True
        return False
