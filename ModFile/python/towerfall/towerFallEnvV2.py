# https://chatgpt.com/c/69027548-7a3c-832a-b1ca-b550cc1ee177
import logging
import gymnasium as gym
import numpy as np
import time
import cv2

class TowerFallEnvV2(gym.Env):
    """
    TowerFall Environment (CNN only)
    Observation : image (4, 24, 32)
        [0] = grid (murs / plateformes)
        [1] = player
        [2] = enemies
        [3] = arrows
    Action space : Discrete(19) (combinations of movement, jump, shoot, dash)
    """
    # ok
    def __init__(self, agent, game_state):
        super(TowerFallEnvV2, self).__init__()
        logging.info("Init TowerFallEnvV2 (CNN 4 channels + Discrete(19))")

        self.agent = agent
        self.game_state = game_state
        self.previous_game_state = game_state
        self.first = True

        self.total_step = 0
        self.total_game = 0
        self.game_reward = 0

        self.enemy1_index = 0 # always min 2 agent
        self.enemy2_index = 1 if agent.id > 1 else 9
        self.enemy3_index = 2 if agent.id > 2 else 9

        # 🎮 Actions combinées
        self.action_space = gym.spaces.Discrete(19)

        # 🧠 Observation : image 4 canaux
        # self.observation_space = gym.spaces.Box(
        #     low=0, high=255, shape=(4, 24, 32), dtype=np.uint8
        # )
        self.observation_space = gym.spaces.Box(low=0, high=255, shape=(4, 84, 84), dtype=np.uint8)

    # ----------------------------------------------------------
    # 🔄 Étape Gym
    # ok
    # ----------------------------------------------------------
    def step(self, action):
        # print("step call")
        self.total_step += 1
        self._send_action(action)
        obs = self._get_observation()

        reward = self._calculate_reward()
        done = self._check_done()
        truncated = False

        return obs, reward, done, truncated, {}

    # ----------------------------------------------------------
    # ♻️ Reset entre deux matchs
    # ok
    # ----------------------------------------------------------
    def reset(self, seed=None, **kwargs):
        # print("reset call")
        self.total_game += 1
        # print("reset=> self.game_reward:{self.game_reward}")
        # self.game_reward = 0
        self.first = True
        game_restart = 0

        self.agent.rematch = True
        reset_send = False
        # print("before while True")
        while True:
            # print("in while True")
            # if not self.agent.act(self.game_state):
                # print("not self.agent.act")
                # print("not self.agent.act")
                # self.agent.send_actions(True)
            if not reset_send:
                self.agent.send_actions(True)
                reset_send= True
            else:
                self.agent.send_actions()
            self.agent.rematch = False  #sert plus
            # print("self.agent.connection.read_json")
            self.game_state = self.agent.connection.read_json()
            if self.game_state["type"] == "init":
                # print("init")
                game_restart += 1
            if self.game_state["type"] == "scenario":
                # print("scenario")
                game_restart += 1
                self.agent.state_scenario = self.game_state
            if self.game_state["type"] == "update" and game_restart == 2:
                # print("update")
                player_archer = next(
                    (e for e in self.game_state["entities"]
                    if e["type"] == "archer" and e["playerIndex"] == self.agent.id),
                    None
                )
                # print(f"{player_archer}")
                if (player_archer and player_archer.get("state", False) != 'frozen'):
                    self.previous_game_state = self.game_state
                    break

        # print("ici")
        obs = self._get_observation()
        return obs, {}

    # ----------------------------------------------------------
    # 🧠 Observation CNN : 4 canaux
    # oka voir first = true
    # ----------------------------------------------------------
    def _get_observation(self):
        # print("_get_observation")

        self.previous_game_state = self.game_state
        if not self.first:
            self.game_state = self.agent.connection.read_json()

        self.first = False

        grid = np.array(self.agent.state_scenario["grid"], dtype=np.float32)
        player_map = np.zeros_like(grid)
        enemy_map = np.zeros_like(grid)
        arrow_map = np.zeros_like(grid)

        for e in self.game_state["entities"]:
            pos = e.get("pos", {"x": 0, "y": 0})
            x = int(pos["x"] // 10)
            y = int(pos["y"] // 10)

            # ✅ sécurité : ignore les entités hors limites
            if not (0 <= y < grid.shape[0] and 0 <= x < grid.shape[1]):
                continue

            t = e.get("type")

            if t == "archer":
                if e.get("playerIndex") == self.agent.id:
                    player_map[y, x] = 1.0
                else:
                    enemy_map[y, x] = 1.0

            elif t == "arrow":
                arrow_map[y, x] = 1.0

            elif t in ["crackedPlatform", "crumbleBlock", "movingPlatform"]:
                size = e.get("size", {"x": 10, "y": 10})
                x_end = int((pos["x"] + size["x"]) // 10)
                y_end = int((pos["y"] + size["y"]) // 10)
                for yy in range(y, min(y_end + 1, 24)):
                    for xx in range(x, min(x_end + 1, 32)):
                        grid[yy, xx] = 1.0

        obs = np.stack([grid, player_map, enemy_map, arrow_map], axis=0)
        obs = (obs * 255).astype(np.uint8)

        # resize each channel to 84x84
        resized = np.zeros((4, 84, 84), dtype=np.uint8)
        for i in range(4):
            resized[i] = cv2.resize(obs[i], (84, 84), interpolation=cv2.INTER_NEAREST)

        # print("return resized")
        return resized

    # ----------------------------------------------------------
    # 🎮 Envoi d'une action (Discrete(23))
    # ok
    # ----------------------------------------------------------
    # def _send_action(self, action):
    #     self.agent.release_all()

    #     action_map = {
    #         0: [],
    #         1: ["l"], 2: ["r"], 3: ["u"], 4: ["d"],
    #         5: ["j"], 6: ["l", "j"], 7: ["r", "j"], 8: ["u", "j"], 9: ["d", "j"],
    #         10: ["s"], 11: ["l", "s"], 12: ["r", "s"], 13: ["u", "s"], 14: ["d", "s"],
    #         15: ["l", "j", "s"], 16: ["r", "j", "s"], 17: ["u", "j", "s"], 18: ["d", "j", "s"],
    #         19: ["z"], 20: ["l", "z"], 21: ["r", "z"], 22: ["u", "z"]
    #     }

    #     keys_to_press = action_map.get(action, [])
    #     for key in keys_to_press:
    #         self.agent.press(key)

    #     self.agent.send_actions()

    def _send_action(self, action):
        """
        Envoie les actions au jeu TowerFall.
        Gère les cas où 'action' peut être un int, un np.array(7) ou np.array([7]).
        """
        import numpy as np
        # print("_send_action")

        # --- Correction robuste ---
        if isinstance(action, np.ndarray):
            action = int(action.item())  # ✅ convertit proprement tous les cas en int
        elif isinstance(action, list):
            action = int(action[0])
        else:
            action = int(action)

        # --- Relâche toutes les touches avant ---
        self.agent.release_all()

        # --- Mapping des actions ---
        action_map = {
            # 0: [],
            # 1: ["l"], 2: ["r"], 3: ["u"], 4: ["d"],
            # 5: ["j"], 6: ["l", "j"], 7: ["r", "j"], 8: ["u", "j"], 9: ["d", "j"],
            # 10: ["s"], 11: ["l", "s"], 12: ["r", "s"], 13: ["u", "s"], 14: ["d", "s"],
            # 15: ["l", "j", "s"], 16: ["r", "j", "s"], 17: ["u", "j", "s"], 18: ["d", "j", "s"],
            # 19: ["z"], 20: ["l", "z"], 21: ["r", "z"], 22: ["u", "z"]
            # rien
            0: [],
            #deplacement
            # gauche
            1: ["l"],
            # droite
            2: ["r"],
            # haut
            3: ["u"],
            # bas
            4: ["d"],
            # dash gauche
            5: ["l", "z"],
            # dash droite
            6: ["r", "z"],
            # dash haut
            7: ["u", "z"],
            # dash bas
            8: ["u", "d"],
            #saut
            # saut
            9: ["j"],
            # saut gauche
            10: ["j", "l"],
            # saut droite
            11: ["j", "r"],
            #tir
            # tir haut
            12: ["s", "u"],
            # tir droite
            13: ["s", "r"],
            # tir gauche
            14: ["s", "l"],
            # tir haut gauche
            15: ["s", "u", "l"],
            # tir haut droite
            16: ["s", "u", "r"],
            # tir bas droite
            17: ["s", "d", "r"],
            # tir bas gauche
            18: ["s", "d", "l"],
        }

        # --- Appuie sur chaque touche ---
        keys_to_press = action_map.get(action, [])
        for key in keys_to_press:
            self.agent.press(key)

        # --- Envoie les actions ---
        self.agent.send_actions()


    # ----------------------------------------------------------
    # 💰 Récompenses
    # ok
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
            print("reward -10")
            reward -= 10

        # Kills des ennemis
        for e in self.game_state["entities"]:
            if e["type"] in ["archer", "playerCorpse"]:
                killer = e.get("killer", -1)
                if killer == self.agent.id and e.get("playerIndex") != self.agent.id:
                    print("reward +10")
                    reward += 10

        self.game_reward += reward
        return reward

    # ----------------------------------------------------------
    # ✅ Fin de partie
    # todo replace par v1 si marche pas
    # ----------------------------------------------------------
    # def _check_done(self):
    #     foundPlayer = False
    #     for e in self.game_state["entities"]:
    #         if e["type"] in ["archer", "playerCorpse"] and e.get("playerIndex") == self.agent.id:
    #             foundPlayer = True
    #             if e.get("dead", False) or e["type"] == "playerCorpse":
    #                 return True

    #     return foundPlayer

    def _check_done(self):
        # logging.info('_check_done***************************')
        # """Vérifie si le joueur est mort."""
        # if state[0] == 0 and state[1] == 0:  # Exemple : si le joueur a disparu
        #     return True
        nbPlayer = 0
        archer = False
        # logging.info(f" self.agent.id {self.player_index}")
        for entity in self.game_state['entities']:

            if entity['type'] == 'playerCorpse' and entity['playerIndex'] ==  self.agent.id:
                # logging.info(f"archer detecté corpse")
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                logging.info(f"PDEAD player {entity['playerIndex']} is dead (playerCorpse) by killer {entity['killer']}")
                return True

            if entity['type'] == 'archer' and entity['playerIndex'] ==  self.agent.id:
                # nbPlayer += 1
                # logging.info(f"archer detecté")
                archer = True
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                if entity['dead'] == True:
                    logging.info(f"PDEAD player {entity['playerIndex']} is dead (archer) by killer {entity['killer']}")
                    return True
                else:
                    nbPlayer += 1

            if entity['type'] == 'playerCorpse' and entity['playerIndex'] == self.enemy1_index:
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                logging.info(f"PDEAD enemy {entity['playerIndex']} is dead (playerCorpse) by killer {entity['killer']}")
                # return True


                # return entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True
            if entity['type'] == 'archer' and entity['playerIndex'] == self.enemy1_index:
                # nbPlayer += 1
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                if entity['dead'] == True:
                    logging.info(f"PDEAD enemy {entity['playerIndex']} is dead (archer) by killer {entity['killer']}")
                    # return True
                else:
                    nbPlayer += 1



            if entity['type'] == 'playerCorpse' and entity['playerIndex'] == self.enemy2_index:
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                logging.info(f"PDEAD enemy {entity['playerIndex']} is dead (playerCorpse) by killer {entity['killer']}")
                # return True


                # return entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True
            if entity['type'] == 'archer' and entity['playerIndex'] == self.enemy2_index:
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                if entity['dead'] == True:
                    logging.info(f"PDEAD enemy {entity['playerIndex']} is dead (archer) by killer {entity['killer']}")
                    # return True
                else:
                    nbPlayer += 1


            if entity['type'] == 'playerCorpse' and entity['playerIndex'] == self.enemy3_index:
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                logging.info(f"PDEAD enemy {entity['playerIndex']} is dead (playerCorpse) by killer {entity['killer']}")
                # return True


                # return entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True
            if entity['type'] == 'archer' and entity['playerIndex'] == self.enemy3_index:
                # nbPlayer += 1
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                if entity['dead'] == True:
                    logging.info(f"PDEAD enemy {entity['playerIndex']} is dead (archer) by killer {entity['killer']}")
                    # return True
                else:
                    nbPlayer += 1

        if nbPlayer < 2:
            return True


        # logging.info(f"nbPlayer: {nbPlayer} archer: {archer}  entity: {self.game_state['entities']}")

        # if nbPlayer > 0 and archer == False:
        #     return True

        return False
