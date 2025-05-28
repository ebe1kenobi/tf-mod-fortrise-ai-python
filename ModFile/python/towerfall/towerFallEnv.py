import logging
import gymnasium as gym
import numpy as np
import json
import socket
from stable_baselines3 import PPO
import time

class TowerFallEnv(gym.Env):
    def __init__(self, agent, game_state):
        logging.info('__init__***************************')
        super(TowerFallEnv, self).__init__()

        self.agent = agent
        self.game_state = game_state
        self.previous_game_state = game_state

        # self.agent.state_update = self.game_state
        # self.agent.update_state_scenario_with_new_data()
        # self.game_state = self.agent.state_update


        self.first = True
        self.info = {}
        self.player_index = 3
        self.enemy1_index = 0
        self.enemy2_index = 1
        self.enemy3_index = 2

        self.total_step = 0
        self.total_game = 0
        self.game_reward = 0

        # self.step = "left"

        # Définition des actions possibles ([0 Gauche 1 Droite 2 rien ], [0 haut 1 bas 2 rien ], 0 Saut 1 rien, 0 Tir 1 rien, 0 Dash 1 rien, 0 Dash 1 rien)

        # if self.step == "left":
        self.action_space = gym.spaces.MultiDiscrete([3, 3, 2, 2, 2])

        # self.action_space = gym.spaces.MultiDiscrete([3, 3, 2, 2, 2])

        # Définition de l'espace d'observation (Position + Vitesse du joueur + Flèches)
        # self.observation_space = gym.spaces.Box(low=-500, high=500, shape=(10,), dtype=np.float32)
        self.observation_space = gym.spaces.Box(low=-500, high=500, shape=(908,), dtype=np.float32)

        self.state = self._get_game_state()


    def _send_action(self, action):
        """Envoie une action au jeu."""
        # logging.info('_send_action***************************')
        # [2 1 0 0 1]
        # logging.info(str(action))
        #todo end action to agent
        # // self.game_state =
        # [0 Gauche 1 Droite 2 rien ],
        if action[0] == 0:
            self.agent.press("l")
        elif action[0] == 1:
            self.agent.press("r")

        # [0 haut 1 bas 2 rien ]
        if action[1] == 0:
            self.agent.press("u")
        elif action[1] == 1:
            self.agent.press("d")

        # [ 0 Saut 1 rien,
        if action[2] == 0:
            self.agent.press("j")

        #  0 Tir 1 rien
        if action[3] == 0:
            self.agent.press("s")

        #  0 Dash 1 rien
        if action[4] == 0:
            self.agent.press("z")

        self.agent.send_actions()

    def _get_player_state(self, state):
        # logging.info('_get_player_state***************************')
        if state == 'normal':
            return 0
        if state == 'ledgeGrab':
            return 1
        if state == 'ducking':
            return 2
        if state == 'dodging':
            return 3
        if state == 'dying':
            return 4
        if state == 'froze':
            return 5
        return 0

    def _get_arrow_state(self, state):
        # logging.info('_get_arrow_state***************************')
        if state == 'shooting':
            return 0
        if state == 'drilling':
            return 1
        if state == 'gravity':
            return 2
        if state == 'falling':
            return 3
        if state == 'stuck':
            return 4
        if state == 'layingOnGround':
            return 5
        if state == 'buried':
            return 6
        return 0

    def _get_game_state(self):
        """Récupère et traite le JSON de l’état du jeu."""
        # logging.info('_get_game_state***************************')
        #todo get agent state
        self.previous_game_state = self.game_state

        if not self.first:
            # logging.info('not first***************************')
            self.game_state = self.agent.connection.read_json()
            # logging.info(f'{self.game_state}')
            # self.agent.state_update = self.game_state
            # self.agent.update_state_scenario_with_new_data()
            # self.game_state = self.agent.state_update

        self.first = False

        # logging.info('init_info 1**************************')
        self.init_info()
        # logging.info('init_info 2***************************')

        # player_x, player_y, player_vx, player_vy, player_nb_arrow, player_on_ground, player_on_wall, player_state, player_facing, player_aimDirectionX, player_aimDirectionY = 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        # enemy1_x, enemy1_y, enemy1_vx, enemy1_vy, enemy1_nb_arrow, enemy1_on_ground, enemy1_on_wall, enemy1_state, enemy1_facing, enemy1_aimDirectionX, enemy1_aimDirectionY = 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        # enemy2_x, enemy2_y, enemy2_vx, enemy2_vy, enemy2_nb_arrow, enemy2_on_ground, enemy2_on_wall, enemy2_state, enemy2_facing, enemy2_aimDirectionX, enemy2_aimDirectionY = 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        # enemy3_x, enemy3_y, enemy3_vx, enemy3_vy, enemy3_nb_arrow, enemy3_on_ground, enemy3_on_wall, enemy3_state, enemy3_facing, enemy3_aimDirectionX, enemy3_aimDirectionY = 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        # arrow1_x,  arrow1_y,  arrow1_vx,  arrow1_vy = 0, 0, 0, 0
        # arrow2_x,  arrow2_y,  arrow2_vx,  arrow2_vy = 0, 0, 0, 0
        # arrow3_x,  arrow3_y,  arrow3_vx,  arrow3_vy = 0, 0, 0, 0
        # arrow4_x,  arrow4_y,  arrow4_vx,  arrow4_vy = 0, 0, 0, 0
        # arrow5_x,  arrow5_y,  arrow5_vx,  arrow5_vy = 0, 0, 0, 0
        # arrow6_x,  arrow6_y,  arrow6_vx,  arrow6_vy = 0, 0, 0, 0
        # arrow7_x,  arrow7_y,  arrow7_vx,  arrow7_vy = 0, 0, 0, 0
        # arrow8_x,  arrow8_y,  arrow8_vx,  arrow8_vy = 0, 0, 0, 0
        # arrow9_x,  arrow9_y,  arrow9_vx,  arrow9_vy = 0, 0, 0, 0
        # arrow10_x, arrow10_y, arrow10_vx, arrow10_vy= 0, 0, 0, 0
        # arrow11_x, arrow11_y, arrow11_vx, arrow11_vy= 0, 0, 0, 0
        # arrow12_x, arrow12_y, arrow12_vx, arrow12_vy= 0, 0, 0, 0
        # arrow13_x, arrow13_y, arrow13_vx, arrow13_vy= 0, 0, 0, 0
        # arrow14_x, arrow14_y, arrow14_vx, arrow14_vy= 0, 0, 0, 0
        # arrow15_x, arrow15_y, arrow15_vx, arrow15_vy= 0, 0, 0, 0
        # arrow16_x, arrow16_y, arrow16_vx, arrow16_vy= 0, 0, 0, 0
        # arrow17_x, arrow17_y, arrow17_vx, arrow17_vy= 0, 0, 0, 0
        # arrow18_x, arrow18_y, arrow18_vx, arrow18_vy= 0, 0, 0, 0
        # arrow19_x, arrow19_y, arrow19_vx, arrow19_vy= 0, 0, 0, 0
        # arrow20_x, arrow20_y, arrow20_vx, arrow20_vy= 0, 0, 0, 0
        # arrow21_x, arrow21_y, arrow21_vx, arrow21_vy= 0, 0, 0, 0
        # arrow22_x, arrow22_y, arrow22_vx, arrow22_vy= 0, 0, 0, 0
        # arrow23_x, arrow23_y, arrow23_vx, arrow23_vy= 0, 0, 0, 0
        # arrow24_x, arrow24_y, arrow24_vx, arrow24_vy= 0, 0, 0, 0

        # Extraction des infos du joueur
        player = next((e for e in self.game_state["entities"] if e["type"] == "archer" and e["playerIndex"] == self.player_index), None)
        # logging.info(' player = next')
        if player:
            self.info["player_x"] = player["pos"]["x"]
            self.info["player_y"] = player["pos"]["y"]
            self.info["player_vx"] = player["vel"]["x"]
            self.info["player_vy"] = player["vel"]["y"]
            self.info["player_nb_arrow"] = len(player["arrows"])
            self.info["player_on_ground"] = 1 if player["onGround"] else 0
            self.info["player_on_wall"] = 1 if player["onWall"] else 0
            self.info["player_state"] = self._get_player_state(player["state"])
            self.info["player_facing"] = player["facing"]
            self.info["player_aimDirectionX"] = player["aimDirection"]["x"]
            self.info["player_aimDirectionY"] = player["aimDirection"]["y"]

        enemy1 = next((e for e in self.game_state["entities"] if e["type"] == "archer" and e["playerIndex"] == self.enemy1_index), None)
        # logging.info(' enemy1 = next')
        if enemy1:
            self.info["enemy1_x"] = enemy1["pos"]["x"]
            self.info["enemy1_y"] = enemy1["pos"]["y"]
            self.info["enemy1_vx"] = enemy1["vel"]["x"]
            self.info["enemy1_vy"] = enemy1["vel"]["y"]
            self.info["enemy1_nb_arrow"] = len(enemy1["arrows"])
            self.info["enemy1_on_ground"] = 1 if enemy1["onGround"] else 0
            self.info["enemy1_on_wall"] = 1 if enemy1["onWall"] else 0
            self.info["enemy1_state"] = self._get_player_state(enemy1["state"])
            self.info["enemy1_facing"] = enemy1["facing"]
            self.info["enemy1_aimDirectionX"] = enemy1["aimDirection"]["x"]
            self.info["enemy1_aimDirectionY"] = enemy1["aimDirection"]["y"]

        enemy2 = next((e for e in self.game_state["entities"] if e["type"] == "archer" and e["playerIndex"] == self.enemy2_index), None)
        # logging.info(' enemy2 = next')
        if enemy2:
            self.info["enemy2_x"] = enemy2["pos"]["x"]
            self.info["enemy2_y"] = enemy2["pos"]["y"]
            self.info["enemy2_vx"] = enemy2["vel"]["x"]
            self.info["enemy2_vy"] = enemy2["vel"]["y"]
            self.info["enemy2_nb_arrow"] = len(enemy2["arrows"])
            self.info["enemy2_on_ground"] = 1 if enemy2["onGround"] else 0
            self.info["enemy2_on_wall"] = 1 if enemy2["onWall"] else 0
            self.info["enemy2_state"] = self._get_player_state(enemy2["state"])
            self.info["enemy2_facing"] = enemy2["facing"]
            self.info["enemy2_aimDirectionX"] = enemy2["aimDirection"]["x"]
            self.info["enemy2_aimDirectionY"] = enemy2["aimDirection"]["y"]

        enemy3 = next((e for e in self.game_state["entities"] if e["type"] == "archer" and e["playerIndex"] == self.enemy3_index), None)
        # logging.info(' enemy3 = next')
        if enemy3:
            self.info["enemy3_x"] = enemy3["pos"]["x"]
            self.info["enemy3_y"] = enemy3["pos"]["y"]
            self.info["enemy3_vx"] = enemy3["vel"]["x"]
            self.info["enemy3_vy"] = enemy3["vel"]["y"]
            self.info["enemy3_nb_arrow"] = len(enemy3["arrows"])
            self.info["enemy3_on_ground"] = 1 if enemy3["onGround"] else 0
            self.info["enemy3_on_wall"] = 1 if enemy3["onWall"] else 0
            self.info["enemy3_state"] = self._get_player_state(enemy3["state"])
            self.info["enemy3_facing"] = enemy3["facing"]
            self.info["enemy3_aimDirectionX"] = enemy3["aimDirection"]["x"]
            self.info["enemy3_aimDirectionY"] = enemy3["aimDirection"]["y"]

        # Extraction des flèches en vol
        # logging.info(' for i, arrow in enumerate')
        for i, arrow in enumerate((e for e in self.game_state["entities"] if e["type"] == "arrow"), start=1):
            self.info[f"arrow{i}_x"] =  arrow["pos"]["x"]
            self.info[f"arrow{i}_y"] =  arrow["pos"]["y"]
            self.info[f"arrow{i}_vx"] = arrow["vel"]["x"]
            self.info[f"arrow{i}_vy"] = arrow["vel"]["y"]
        # logging.info(' after for')

        # todo "left":11.5945253,"right":308.4055

        # arrows = [e for e in game_data["entities"] if e["type"] == "arrow"]
        # if arrows:
        #     arrow_1  = _get_arrow(arrows[0])
        #     arrow_2  = _get_arrow(arrows[1])
        #     arrow_3  = _get_arrow(arrows[2])
        #     arrow_4  = _get_arrow(arrows[3])
        #     arrow_5  = _get_arrow(arrows[4])
        #     arrow_6  = _get_arrow(arrows[5])
        #     arrow_7  = _get_arrow(arrows[6])
        #     arrow_8  = _get_arrow(arrows[7])
        #     arrow_9  = _get_arrow(arrows[8])
        #     arrow_10 = _get_arrow(arrows[9])
        #     arrow_11 = _get_arrow(arrows[10])
        #     arrow_12 = _get_arrow(arrows[11])
        #     arrow_13 = _get_arrow(arrows[12])
        #     arrow_14 = _get_arrow(arrows[13])
        #     arrow_15 = _get_arrow(arrows[14])
        #     arrow_16 = _get_arrow(arrows[15])
        #     arrow_17 = _get_arrow(arrows[16])
        #     arrow_18 = _get_arrow(arrows[17])
        #     arrow_19 = _get_arrow(arrows[18])
        #     arrow_20 = _get_arrow(arrows[19])
        #     arrow_21 = _get_arrow(arrows[20])
        #     arrow_22 = _get_arrow(arrows[21])
        #     arrow_23 = _get_arrow(arrows[22])
        #     arrow_24 = _get_arrow(arrows[23])
        #     first_arrow = arrows[0]
        #     arrow_x = arrow_1["pos"]["x"]
        #     arrow_y = arrow_1["pos"]["y"]
        #     arrow_vx = arrow_1["vel"]["x"]
        #     arrow_vy= arrow_1["vel"]["y"]

        # Construire l'observation
        # nb array per player
        # direction player enemy
        # player x
        # enemy1 x
        # enemy2 x
        # enemy3 x
        # arrow 6 par per player  6 x 4 x y velocity direction
        # on wall
        # on ground
        # return np.array([player_x, player_y, player_vx, player_vy, on_ground, on_wall, arrow_x, arrow_y, arrow_vx, arrow_vy], dtype=np.float32)
        # 32 lines of 24 block => 768
        # each player enemy has 7 val => 4 * 11 => 44
        # max 6 arrow per player -> 6  * 4 = 24 * 4 value  = 96
        # vector of 768 + 44 + 96 => 908 value
        # logging.info('getgamestate : *************state_scenario**************')
        # logging.info(str(self.agent.state_scenario))
        # logging.info('getgamestate : ***********grid****************')
        # logging.info(str(self.agent.state_scenario['grid']))
        # logging.info('getgamestate : ***********level****************')
        level = np.array(self.agent.state_scenario['grid'], dtype=np.float32)
        # logging.info(str(level))
        # logging.info('getgamestate : ***********level.flatten()****************')
        # logging.info(str(level.flatten()))
        # logging.info("np.concatenate")
        return np.concatenate([
            level.flatten(),
            np.array(
            [
                self.info["player_x"],
                self.info["player_y"],
                self.info["player_vx"],
                self.info["player_vy"],
                self.info["player_nb_arrow"],
                self.info["player_on_ground"],
                self.info["player_on_wall"],
                self.info["player_state"],
                self.info["player_facing"],
                self.info["player_aimDirectionX"],
                self.info["player_aimDirectionY"],
                self.info["enemy1_x"],
                self.info["enemy1_y"],
                self.info["enemy1_vx"],
                self.info["enemy1_vy"],
                self.info["enemy1_nb_arrow"],
                self.info["enemy1_on_ground"],
                self.info["enemy1_on_wall"],
                self.info["enemy1_state"],
                self.info["enemy1_facing"],
                self.info["enemy1_aimDirectionX"],
                self.info["enemy1_aimDirectionY"],
                self.info["enemy2_x"],
                self.info["enemy2_y"],
                self.info["enemy2_vx"],
                self.info["enemy2_vy"],
                self.info["enemy2_nb_arrow"],
                self.info["enemy2_on_ground"],
                self.info["enemy2_on_wall"],
                self.info["enemy2_state"],
                self.info["enemy2_facing"],
                self.info["enemy2_aimDirectionX"],
                self.info["enemy2_aimDirectionY"],
                self.info["enemy3_x"],
                self.info["enemy3_y"],
                self.info["enemy3_vx"],
                self.info["enemy3_vy"],
                self.info["enemy3_nb_arrow"],
                self.info["enemy3_on_ground"],
                self.info["enemy3_on_wall"],
                self.info["enemy3_state"],
                self.info["enemy3_facing"],
                self.info["enemy3_aimDirectionX"],
                self.info["enemy3_aimDirectionY"],
                self.info["arrow1_x"],  self.info["arrow1_y"],  self.info["arrow1_vx"],  self.info["arrow1_vy"] ,
                self.info["arrow2_x"],  self.info["arrow2_y"],  self.info["arrow2_vx"],  self.info["arrow2_vy"] ,
                self.info["arrow3_x"],  self.info["arrow3_y"],  self.info["arrow3_vx"],  self.info["arrow3_vy"] ,
                self.info["arrow4_x"],  self.info["arrow4_y"],  self.info["arrow4_vx"],  self.info["arrow4_vy"] ,
                self.info["arrow5_x"],  self.info["arrow5_y"],  self.info["arrow5_vx"],  self.info["arrow5_vy"] ,
                self.info["arrow6_x"],  self.info["arrow6_y"],  self.info["arrow6_vx"],  self.info["arrow6_vy"] ,
                self.info["arrow7_x"],  self.info["arrow7_y"],  self.info["arrow7_vx"],  self.info["arrow7_vy"] ,
                self.info["arrow8_x"],  self.info["arrow8_y"],  self.info["arrow8_vx"],  self.info["arrow8_vy"] ,
                self.info["arrow9_x"],  self.info["arrow9_y"],  self.info["arrow9_vx"],  self.info["arrow9_vy"] ,
                self.info["arrow10_x"], self.info["arrow10_y"], self.info["arrow10_vx"], self.info["arrow10_vy"],
                self.info["arrow11_x"], self.info["arrow11_y"], self.info["arrow11_vx"], self.info["arrow11_vy"],
                self.info["arrow12_x"], self.info["arrow12_y"], self.info["arrow12_vx"], self.info["arrow12_vy"],
                self.info["arrow13_x"], self.info["arrow13_y"], self.info["arrow13_vx"], self.info["arrow13_vy"],
                self.info["arrow14_x"], self.info["arrow14_y"], self.info["arrow14_vx"], self.info["arrow14_vy"],
                self.info["arrow15_x"], self.info["arrow15_y"], self.info["arrow15_vx"], self.info["arrow15_vy"],
                self.info["arrow16_x"], self.info["arrow16_y"], self.info["arrow16_vx"], self.info["arrow16_vy"],
                self.info["arrow17_x"], self.info["arrow17_y"], self.info["arrow17_vx"], self.info["arrow17_vy"],
                self.info["arrow18_x"], self.info["arrow18_y"], self.info["arrow18_vx"], self.info["arrow18_vy"],
                self.info["arrow19_x"], self.info["arrow19_y"], self.info["arrow19_vx"], self.info["arrow19_vy"],
                self.info["arrow20_x"], self.info["arrow20_y"], self.info["arrow20_vx"], self.info["arrow20_vy"],
                self.info["arrow21_x"], self.info["arrow21_y"], self.info["arrow21_vx"], self.info["arrow21_vy"],
                self.info["arrow22_x"], self.info["arrow22_y"], self.info["arrow22_vx"], self.info["arrow22_vy"],
                self.info["arrow23_x"], self.info["arrow23_y"], self.info["arrow23_vx"], self.info["arrow23_vy"],
                self.info["arrow24_x"], self.info["arrow24_y"], self.info["arrow24_vx"], self.info["arrow24_vy"],
            ], dtype=np.float32)
            ]
            )
        # return np.array([1], dtype=np.float32)

    def step(self, action):
        """Envoie une action et récupère l’état suivant."""
        # logging.info('step***************************')
        self.total_step += 1
        self._send_action(action)
        obs = self._get_game_state()

        # reward = self._calculate_reward_step1(obs)
        reward = self._calculate_reward(obs)

        terminated  = self._check_done(obs)
        truncated = False
        self.state = obs
        return obs, reward, terminated, terminated, {}  # ✅ Must return 5 values!

    # step 1
    # def _calculate_reward_step1(self, state_json):
        # logging.info('_calculate_reward***************************')

    #     #  TODO if arrow no speed -> not a catch, check arrow.state
    #     # si bouge pas minus

    #     # todo remplir les case avec ds solid si patforme!!

    def _calculate_reward(self, state_json):
        # logging.info('st_calculate_rewardep***************************')
        reward = 0

        player = next((e for e in self.game_state["entities"] if e["type"] == "archer" and e["playerIndex"] == self.player_index), None)
        previous_player = next((e for e in self.previous_game_state["entities"] if e["type"] == "archer" and e["playerIndex"] == self.player_index), None)
        if not previous_player:
            previous_player = player

        player_dead = False

        playercorpse = next((e for e in self.game_state["entities"] if e["type"] == "playerCorpse" and e["playerIndex"] == self.player_index), None)
        if playercorpse:
            logging.info(f"PREWARD -5 player {playercorpse['playerIndex'] + 1} is dead (playerCorpse) by killer {playercorpse['killer']}")
            player_dead = True
            reward -= 5

        if player:
            # if player["catchArrow"]:
            #     logging.info(f"PREWARD +5 player {player['playerIndex'] + 1} catchArrow")
            #     reward += 5

            # if player["stealArrow"]:
            #     logging.info(f"PREWARD +5 player {player['playerIndex'] + 1} stealArrow")
            #     reward += 2

            if player["dead"]:
                logging.info(f"PREWARD -5 player {player['playerIndex'] + 1} is dead (archer) by killer {player['killer']}")
                player_dead = True
                reward -= 5
            # else:
            #     if len(previous_player["arrows"]) < len(player["arrows"]):
            #         logging.info(f"PREWARD +0.1 player")
            #         reward += 0.1
        else:
            player_dead = True
            reward -= 5

        enemycorpse1 = next((e for e in self.game_state["entities"] if e["type"] == "playerCorpse" and e["playerIndex"] == self.enemy1_index), None)
        if enemycorpse1:
            if enemycorpse1["killer"] == self.player_index:
                logging.info(f"PREWARD 10 enemy {enemycorpse1['playerIndex'] + 1} is dead (playerCorpse) by killer {enemycorpse1['killer']}")
                player_kill = True
                reward += 10

        enemy1 = next((e for e in self.game_state["entities"] if e["type"] == "archer" and e["playerIndex"] == self.enemy1_index), None)
        if enemy1:
            if enemy1["dead"] and enemy1["killer"] == self.player_index:
                logging.info(f"PREWARD 10 enemy {enemy1['playerIndex'] + 1} is dead (archer) by killer {enemy1['killer']}")
                player_kill = True
                reward += 10

        enemycorpse2 = next((e for e in self.game_state["entities"] if e["type"] == "playerCorpse" and e["playerIndex"] == self.enemy2_index), None)
        if enemycorpse2:
            if enemycorpse2["killer"] == self.player_index:
                logging.info(f"PREWARD 10 enemy {enemycorpse2['playerIndex'] + 1} is dead (playerCorpse) by killer {enemycorpse2['killer']}")
                player_kill = True
                reward += 10

        enemy2 = next((e for e in self.game_state["entities"] if e["type"] == "archer" and e["playerIndex"] == self.enemy2_index), None)
        if enemy2:
            if enemy2["dead"] and enemy2["killer"] == self.player_index:
                logging.info(f"PREWARD 10 enemy {enemy2['playerIndex'] + 1} is dead (archer) by killer {enemy2['killer']}")
                player_kill = True
                reward += 10

        enemycorpse3 = next((e for e in self.game_state["entities"] if e["type"] == "playerCorpse" and e["playerIndex"] == self.enemy3_index), None)
        if enemycorpse3:
            if enemycorpse3["killer"] == self.player_index:
                logging.info(f"PREWARD 10 enemy {enemycorpse3['playerIndex'] + 1} is dead (playerCorpse) by killer {enemycorpse3['killer']}")
                player_kill = True
                reward += 10

        enemy3 = next((e for e in self.game_state["entities"] if e["type"] == "archer" and e["playerIndex"] == self.enemy3_index), None)
        if enemy3:
            if enemy3["dead"] and enemy3["killer"] == self.player_index:
                logging.info(f"PREWARD 10 enemy {enemy3['playerIndex'] + 1} is dead (archer) by killer {enemy3['killer']}")
                player_kill = True
                reward += 10




        # gain an arrow (keep previous state)


        # dead and killer

        # # 1️⃣ BONUS : Toucher un ennemi avec une flèche
        # for entity in state_json["entities"]:
        #     if entity["type"] == "archer" and entity["isEnemy"] and entity["isDead"]:
        #         reward += 10  # Grosse récompense pour avoir éliminé un ennemi

        # # 2️⃣ MALUS : Si le joueur meurt
        # for entity in state_json["entities"]:
        #     if entity["type"] == "archer" and entity["playerIndex"] == 0:  # Joueur IA
        #         if entity["isDead"]:
        #             reward -= 5  # Pénalité pour être mort

        #         # 3️⃣ MALUS : Si le joueur spamme les tirs sans toucher
        #         if self.last_action == "shoot" and not self.hit_something:
        #             reward -= 1  # Petit malus pour éviter le spam

        #         # 4️⃣ MALUS : Si le joueur reste immobile trop longtemps
        #         if abs(entity["vel"]["x"]) < 0.1 and abs(entity["vel"]["y"]) < 0.1:
        #             self.temps_inactif += 1
        #             if self.temps_inactif > 50:  # Ex: après 50 frames sans bouger
        #                 reward -= 2  # Pénalité pour camping
        #         else:
        #             self.temps_inactif = 0  # Reset si le joueur bouge
        self.game_reward += reward
        return reward

    def _check_done(self, state):
        # logging.info('_check_done***************************')
        # """Vérifie si le joueur est mort."""
        # if state[0] == 0 and state[1] == 0:  # Exemple : si le joueur a disparu
        #     return True
        nbPlayer = 0
        for entity in self.game_state['entities']:

            if entity['type'] == 'playerCorpse' and entity['playerIndex'] == self.player_index:
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                logging.info(f"PDEAD player {entity['playerIndex'] + 1} is dead (playerCorpse) by killer {entity['killer']}")
                return True

            if entity['type'] == 'archer' and entity['playerIndex'] == self.player_index:
                nbPlayer += 1
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                if entity['dead'] == True:
                    logging.info(f"PDEAD player {entity['playerIndex'] + 1} is dead (archer) by killer {entity['killer']}")
                    # return True

            if entity['type'] == 'playerCorpse' and entity['playerIndex'] == self.enemy1_index:
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                logging.info(f"PDEAD enemy {entity['playerIndex'] + 1} is dead (playerCorpse) by killer {entity['killer']}")
                # return True


                # return entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True
            if entity['type'] == 'archer' and entity['playerIndex'] == self.enemy1_index:
                nbPlayer += 1
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                if entity['dead'] == True:
                    logging.info(f"PDEAD enemy {entity['playerIndex'] + 1} is dead (archer) by killer {entity['killer']}")
                    # return True



            if entity['type'] == 'playerCorpse' and entity['playerIndex'] == self.enemy2_index:
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                logging.info(f"PDEAD enemy {entity['playerIndex'] + 1} is dead (playerCorpse) by killer {entity['killer']}")
                # return True


                # return entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True
            if entity['type'] == 'archer' and entity['playerIndex'] == self.enemy2_index:
                nbPlayer += 1
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                if entity['dead'] == True:
                    logging.info(f"PDEAD enemy {entity['playerIndex'] + 1} is dead (archer) by killer {entity['killer']}")
                    # return True

            if entity['type'] == 'playerCorpse' and entity['playerIndex'] == self.enemy3_index:
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                logging.info(f"PDEAD enemy {entity['playerIndex'] + 1} is dead (playerCorpse) by killer {entity['killer']}")
                # return True


                # return entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True
            if entity['type'] == 'archer' and entity['playerIndex'] == self.enemy3_index:
                nbPlayer += 1
                # if entity['state'] == 'dying' or entity['dead'] == True or entity['isDead'] == True:
                if entity['dead'] == True:
                    logging.info(f"PDEAD enemy {entity['playerIndex'] + 1} is dead (archer) by killer {entity['killer']}")
                    # return True

        if nbPlayer < 2:
            return True

        return False
        # return self.game_state['state'] == 'dying'


    def reset(self, seed=None, **kwargs):
        """Réinitialise le jeu."""
        # logging.info('reset***************************')

        # self.towerfall.send_rematch()
        self.total_game += 1
        self.game_reward = 0

        game_restart=0
        self.agent.rematch = True
        while True:
            if False == self.agent.act(self.game_state):
                self.agent.send_actions() # to not block
            self.agent.rematch = False
            self.game_state = self.agent.connection.read_json()
            if self.game_state['type'] == 'init':
                game_restart += 1
            if self.game_state['type'] == 'scenario':
                game_restart += 1
                self.agent.state_scenario = self.game_state

            # logging.info(f'game_restart={game_restart}')

            if self.game_state['type'] == 'update' and game_restart == 2:
                self.previous_game_state = self.game_state
                break


        logging.info(f'ok fin reset match restart with new data game_state == {self.game_state['type']} {self.game_state}')
        self.first = True
        return self._get_game_state(), {}

    def init_info(self):
        # logging.info(f'init_info')
        self.info["player_x"] = 0
        self.info["player_y"] = 0
        self.info["player_vx"] = 0
        self.info["player_vy"] = 0
        self.info["player_nb_arrow"] = 0
        self.info["player_on_ground"] = 0
        self.info["player_on_wall"] = 0
        self.info["player_state"] = 0
        self.info["player_facing"] = 0
        self.info["player_aimDirectionX"] = 0
        self.info["player_aimDirectionY"] = 0
        self.info["enemy1_x"] = 0
        self.info["enemy1_y"] = 0
        self.info["enemy1_vx"] = 0
        self.info["enemy1_vy"] = 0
        self.info["enemy1_nb_arrow"] = 0
        self.info["enemy1_on_ground"] = 0
        self.info["enemy1_on_wall"] = 0
        self.info["enemy1_state"] = 0
        self.info["enemy1_facing"] = 0
        self.info["enemy1_aimDirectionX"] = 0
        self.info["enemy1_aimDirectionY"] = 0
        self.info["enemy2_x"] = 0
        self.info["enemy2_y"] = 0
        self.info["enemy2_vx"] = 0
        self.info["enemy2_vy"] = 0
        self.info["enemy2_nb_arrow"] = 0
        self.info["enemy2_on_ground"] = 0
        self.info["enemy2_on_wall"] = 0
        self.info["enemy2_state"] = 0
        self.info["enemy2_facing"] = 0
        self.info["enemy2_aimDirectionX"] = 0
        self.info["enemy2_aimDirectionY"] = 0
        self.info["enemy3_x"] = 0
        self.info["enemy3_y"] = 0
        self.info["enemy3_vx"] = 0
        self.info["enemy3_vy"] = 0
        self.info["enemy3_nb_arrow"] = 0
        self.info["enemy3_on_ground"] = 0
        self.info["enemy3_on_wall"] = 0
        self.info["enemy3_state"] = 0
        self.info["enemy3_facing"] = 0
        self.info["enemy3_aimDirectionX"] = 0
        self.info["enemy3_aimDirectionY"] = 0
        self.info["arrow1_x"] = 0
        self.info["arrow1_y"] = 0
        self.info["arrow1_vx"] = 0
        self.info["arrow1_vy"] = 0
        self.info["arrow2_x"] = 0
        self.info["arrow2_y"] = 0
        self.info["arrow2_vx"] = 0
        self.info["arrow2_vy"] = 0
        self.info["arrow3_x"] = 0
        self.info["arrow3_y"] = 0
        self.info["arrow3_vx"] = 0
        self.info["arrow3_vy"] = 0
        self.info["arrow4_x"] = 0
        self.info["arrow4_y"] = 0
        self.info["arrow4_vx"] = 0
        self.info["arrow4_vy"] = 0
        self.info["arrow5_x"] = 0
        self.info["arrow5_y"] = 0
        self.info["arrow5_vx"] = 0
        self.info["arrow5_vy"] = 0
        self.info["arrow6_x"] = 0
        self.info["arrow6_y"] = 0
        self.info["arrow6_vx"] = 0
        self.info["arrow6_vy"] = 0
        self.info["arrow7_x"] = 0
        self.info["arrow7_y"] = 0
        self.info["arrow7_vx"] = 0
        self.info["arrow7_vy"] = 0
        self.info["arrow8_x"] = 0
        self.info["arrow8_y"] = 0
        self.info["arrow8_vx"] = 0
        self.info["arrow8_vy"] = 0
        self.info["arrow9_x"] = 0
        self.info["arrow9_y"] = 0
        self.info["arrow9_vx"] = 0
        self.info["arrow9_vy"] = 0
        self.info["arrow10_x"] = 0
        self.info["arrow10_y"] = 0
        self.info["arrow10_vx"] = 0
        self.info["arrow10_vy"] = 0
        self.info["arrow11_x"] = 0
        self.info["arrow11_y"] = 0
        self.info["arrow11_vx"] = 0
        self.info["arrow11_vy"] = 0
        self.info["arrow12_x"] = 0
        self.info["arrow12_y"] = 0
        self.info["arrow12_vx"] = 0
        self.info["arrow12_vy"] = 0
        self.info["arrow13_x"] = 0
        self.info["arrow13_y"] = 0
        self.info["arrow13_vx"] = 0
        self.info["arrow13_vy"] = 0
        self.info["arrow14_x"] = 0
        self.info["arrow14_y"] = 0
        self.info["arrow14_vx"] = 0
        self.info["arrow14_vy"] = 0
        self.info["arrow15_x"] = 0
        self.info["arrow15_y"] = 0
        self.info["arrow15_vx"] = 0
        self.info["arrow15_vy"] = 0
        self.info["arrow16_x"] = 0
        self.info["arrow16_y"] = 0
        self.info["arrow16_vx"] = 0
        self.info["arrow16_vy"] = 0
        self.info["arrow17_x"] = 0
        self.info["arrow17_y"] = 0
        self.info["arrow17_vx"] = 0
        self.info["arrow17_vy"] = 0
        self.info["arrow18_x"] = 0
        self.info["arrow18_y"] = 0
        self.info["arrow18_vx"] = 0
        self.info["arrow18_vy"] = 0
        self.info["arrow19_x"] = 0
        self.info["arrow19_y"] = 0
        self.info["arrow19_vx"] = 0
        self.info["arrow19_vy"] = 0
        self.info["arrow20_x"] = 0
        self.info["arrow20_y"] = 0
        self.info["arrow20_vx"] = 0
        self.info["arrow20_vy"] = 0
        self.info["arrow21_x"] = 0
        self.info["arrow21_y"] = 0
        self.info["arrow21_vx"] = 0
        self.info["arrow21_vy"] = 0
        self.info["arrow22_x"] = 0
        self.info["arrow22_y"] = 0
        self.info["arrow22_vx"] = 0
        self.info["arrow22_vy"] = 0
        self.info["arrow23_x"] = 0
        self.info["arrow23_y"] = 0
        self.info["arrow23_vx"] = 0
        self.info["arrow23_vy"] = 0
        self.info["arrow24_x"] = 0
        self.info["arrow24_y"] = 0
        self.info["arrow24_vx"] = 0
        self.info["arrow24_vy"] = 0