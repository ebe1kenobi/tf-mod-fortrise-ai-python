import gym
import numpy as np
import json
import socket
from stable_baselines3 import PPO

class TowerFallEnv(gym.Env):
    def __init__(self, agent):
        super(TowerFallEnv, self).__init__()

        # Définition des actions possibles ([Gauche stop Droite], [haut rien bas], Saut, Tir, Dash)
        self.action_space = gym.spaces.MultiDiscrete([3, 3, 2, 2, 2])

        # Définition de l'espace d'observation (Position + Vitesse du joueur + Flèches)
        self.observation_space = gym.spaces.Box(low=-500, high=500, shape=(10,), dtype=np.float32)

        self.state = self._get_game_state()

    def _send_action(self, action):
        """Envoie une action au jeu."""
        #todo end action to agent

    def _get_game_state(self):
        """Récupère et traite le JSON de l’état du jeu."""
        #todo get agent state

        # Extraction des infos du joueur
        player = next((e for e in game_data["entities"] if e["type"] == "archer" and e["playerIndex"] == 0), None)
        if player:
            player_x = player["pos"]["x"]
            player_y = player["pos"]["y"]
            player_vx = player["vel"]["x"]
            player_vy = player["vel"]["y"]
            on_ground = 1 if player["onGround"] else 0
            on_wall = 1 if player["onWall"] else 0
        else:
            player_x, player_y, player_vx, player_vy, on_ground, on_wall = 0, 0, 0, 0, 0, 0

        # Extraction des flèches en vol
        arrows = [e for e in game_data["entities"] if e["type"] == "arrow" and e["state"] == "shooting"]
        if arrows:
            first_arrow = arrows[0]
            arrow_x = first_arrow["pos"]["x"]
            arrow_y = first_arrow["pos"]["y"]
            arrow_vx = first_arrow["vel"]["x"]
            arrow_vy = first_arrow["vel"]["y"]
        else:
            arrow_x, arrow_y, arrow_vx, arrow_vy = 0, 0, 0, 0

        # Construire l'observation
        return np.array([player_x, player_y, player_vx, player_vy, on_ground, on_wall, arrow_x, arrow_y, arrow_vx, arrow_vy], dtype=np.float32)

    def step(self, action):
        """Envoie une action et récupère l’état suivant."""
        self._send_action(action)
        new_state = self._get_game_state()

        reward = self._calculate_reward(new_state)
        done = self._check_done(new_state)

        self.state = new_state
        return new_state, reward, done, {}

    def _calculate_reward(self, state_json):
        """Calcule la récompense en fonction de l'état du jeu"""

        reward = 0

        # 1️⃣ BONUS : Toucher un ennemi avec une flèche
        for entity in state_json["entities"]:
            if entity["type"] == "archer" and entity["isEnemy"] and entity["isDead"]:
                reward += 10  # Grosse récompense pour avoir éliminé un ennemi

        # 2️⃣ MALUS : Si le joueur meurt
        for entity in state_json["entities"]:
            if entity["type"] == "archer" and entity["playerIndex"] == 0:  # Joueur IA
                if entity["isDead"]:
                    reward -= 5  # Pénalité pour être mort

                # 3️⃣ MALUS : Si le joueur spamme les tirs sans toucher
                if self.last_action == "shoot" and not self.hit_something:
                    reward -= 1  # Petit malus pour éviter le spam

                # 4️⃣ MALUS : Si le joueur reste immobile trop longtemps
                if abs(entity["vel"]["x"]) < 0.1 and abs(entity["vel"]["y"]) < 0.1:
                    self.temps_inactif += 1
                    if self.temps_inactif > 50:  # Ex: après 50 frames sans bouger
                        reward -= 2  # Pénalité pour camping
                else:
                    self.temps_inactif = 0  # Reset si le joueur bouge

        return reward

    def _check_done(self, state):
        """Vérifie si le joueur est mort."""
        if state[0] == 0 and state[1] == 0:  # Exemple : si le joueur a disparu
            return True
        return False

    def reset(self):
        """Réinitialise le jeu."""
        # todo envoyer un message reconfigOperation
        self.state = self._get_game_state()
        return self.state

