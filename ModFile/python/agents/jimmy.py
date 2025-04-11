from .agent import Agent
from typing import Any, Mapping
import logging
import random
from typing import Any, Mapping
from common.logging_options import default_logging
from stable_baselines3 import PPO
# from towerfall import Connection, TowerFallEnv

default_logging()
_VERBOSE = 0
_TIMEOUT = 10

def get_towerfall_env():
    from towerfall import TowerFallEnv
    return  TowerFallEnv

class Jimmy(Agent):

  def __init__(self, id : id,  connection):
    super().__init__(id, connection)
    model_name = "jimmy"
    model_name_last = f"{model_name}.last"
    model_path =  "C:\\Program Files (x86)\\Steam\\steamapps\\common\\TowerFall\\Mods\\"
    self.model_name = f"{model_path}{model_name_last}.zip"
    self.model = PPO.load(self.model_name)
    self.env = None

  def act(self, game_state: Mapping[str, Any]):
    logging.info('Jimmy.act')

    if super().act(game_state):
      return True

    if self.env == None:
      TowerFallEnv = get_towerfall_env()
      self.env = TowerFallEnv(self, game_state)

    # use model jimmy
    self.env.first = True
    self.env.game_state = game_state
    obs = self.env._get_game_state()
    action, _states = self.model.predict(obs, deterministic=True)
    self.env._send_action(action)
    return True
