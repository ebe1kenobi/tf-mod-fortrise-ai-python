from .agent import Agent
from typing import Any, Mapping
import logging
import random
from typing import Any, Mapping
from common.logging_options import default_logging

from towerfall import Connection
import logging

default_logging()
_VERBOSE = 0
_TIMEOUT = 10

class TrainingAgent(Agent):

  def act(self, game_state: Mapping[str, Any]):
    # logging.info('TrainAgent.act')

    return super().act(game_state)
