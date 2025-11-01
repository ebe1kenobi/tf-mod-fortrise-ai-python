from .agent import Agent
from typing import Any, Mapping
import logging
import random
from typing import Any, Mapping
from common.logging_options import default_logging

import os
import numpy as np
from stable_baselines3 import PPO
# from towerfall import TowerFallEnv, Connection
from common.logging_options import default_logging
import time
from datetime import datetime

# from towerfall import Connection
import towerfall
import logging

default_logging()
_VERBOSE = 0
_TIMEOUT = 10

class TrainingAgent3(Agent):

  def __init__(self, id : id,  connection: towerfall.Connection):
    # logging.info(f'TrainingAgent2.__init__ {id}')
    self.counterStart = 0
    self.game_state = {}
    super().__init__(id, connection)

  def run(self):

    self.counterStart += 1
    game_state= {}
    # get game_state update
    while not 'type' in game_state or game_state['type'] != 'update':
      game_state = self.connection.read_json()
      self.act(game_state)
      self.counterStart += 1

    print(f"towerfall.TowerFallEnvV2")
    self.env = towerfall.TowerFallEnvV2(self, game_state)  # Remplace par ton environnement Gym

    self.timestamp = datetime.now().strftime("%Y%m%d%H%M%S")

    self.model_name = "jimmy"
    self.model_name_last = f"{self.model_name}.last.zip"
    self.model_path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\TowerFall\\Mods\\"
    self.last = f"{self.model_path}{self.model_name_last}"
    # # # Création du modèle PPO
    if os.path.exists(self.last):
      print(f"load model existant")
      self.model = PPO.load(self.last)
      print(f"set_env")
      self.model.set_env(self.env)
      print(f"end set_env")
    else:
      print("Création d’un modèle CNN (4 canaux)")
      self.model = PPO("CnnPolicy", self.env, verbose=1, learning_rate=3e-4, batch_size=64)

    print(f"avant while true")

    # i = 0
    # while True:
    #   print(f"while True")
    #   i += 1
    #   if i > 1:
    #     time.sleep(5)
    #     self.send_actions()
    #   # logging.info('self.connection.read_json()')
    #   game_state = self.connection.read_json()
    #   # logging.info('towerfall.run : agent.act')
    #   self.model.learn(total_timesteps=2000)
    #   print(f"after learn: {i}")
    #   self.model.save(f"{self.model_path}{self.model_name}.{self.timestamp}.{self.env.total_game}.{self.env.total_step}.{self.env.game_reward}.zip")
    #   self.model.save(self.last)

    # option1 :pour voir sans entrainer
    # for episode in range(100):
    #     print(f"Début du match {episode}")
    #     obs, _ = self.env.reset()
    #     done = False
    #     total_reward = 0

    #     while not done:
    #         # print(f"not done match {episode}")
    #         action, _ = self.model.predict(obs, deterministic=False)
    #         # print(f"predicted action {action} match {episode}")
    #         obs, reward, done, _, _ = self.env.step(action)
    #         # print(f"after step match {episode}")
    #         # if not done:
    #         #   print("not done")
    #         total_reward += reward

    #     print(f"Fin du match {episode} — reward total = {total_reward:.2f}")

    # option 2.pour entrainer
    for session in range(100000):
        print(f"Début de la session {session}")
        self.model.learn(total_timesteps=2048, reset_num_timesteps=False)
        print(f"After learn session {session}")
        if session % 10 == 0:
            self.model.save(f"{self.model_path}{self.model_name}.{self.timestamp}.{self.env.total_game}.{self.env.total_step}.{self.env.game_reward}.{session}.zip")
            self.model.save(self.last)
            print(f"Modèle sauvegardé (session {session})")

  def release_all(self):
    self.pressed.clear()

  def act(self, game_state: Mapping[str, Any], rematch = False):
    # logging.info(f'TrainingAgent2.act  {self.id}')
    if not 'type' in game_state:
      # logging.info(f'act not type in game_state return True  {self.id}')
      return True
    if False == super().act(game_state, rematch):
      # logging.info(f'False == super().act(game_state)')
      self.send_actions(rematch)

