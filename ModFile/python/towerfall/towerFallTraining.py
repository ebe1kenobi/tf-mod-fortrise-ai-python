import json
import logging
import os
import random
import signal
import time
import threading
from io import TextIOWrapper
from typing import Any, Callable, Dict, List, Mapping, Optional

import psutil
from psutil import Popen

from .connection import Connection
from agents import TrainingAgent

_DEFAULT_STEAM_PATH_WINDOWS = 'C:/Program Files (x86)/Steam/steamapps/common/TowerFall'
_ENV_TOWERFALL_PATH = 'TOWERFALL_PATH'

class TowerfallError(Exception):
  pass

class TowerFallTraining:
  def __init__(self,
      config: Mapping[str, Any] = {},
      towerfall_path: str = _DEFAULT_STEAM_PATH_WINDOWS,
      timeout: float = 2,
      verbose: int = 0):
    logging.info('Towerfall.__init__')

    self.verbose = 1
    if not os.path.exists(towerfall_path):
      env_towerfall_path = os.environ.get(_ENV_TOWERFALL_PATH)
      if env_towerfall_path:
        if not os.path.exists(env_towerfall_path):
          raise TowerfallError(f'\n\nInstallation path defined in env variable {_ENV_TOWERFALL_PATH} does not exist: {env_towerfall_path}. Make sure {_ENV_TOWERFALL_PATH} is set to the installation path of Towerfall.')
        towerfall_path = env_towerfall_path
      else:
        raise TowerfallError(f'\n\nThe default installation path does not exist: {towerfall_path}. You have 2 options:\n  a) Set env variable {_ENV_TOWERFALL_PATH} with the installation path.\n  b) Set the towerfall_path parameter in Towerfall constructor with the installation path.')
    self.config: Mapping[str, Any] = config
    self.towerfall_path = towerfall_path
    self.towerfall_path_exe = os.path.join(self.towerfall_path, 'TowerFall.exe')
    #self.pool_name = 'default'
    #self.pool_path = os.path.join(self.towerfall_path, 'pools', self.pool_name)
    self.pool_path = os.path.join(self.towerfall_path, 'pools')
    logging.info(self.pool_path)
    self.timeout = timeout
    self.verbose = verbose
    tries = 0
    while True:
      logging.info('while true')

      self.port = self._attain_game_port()
      try:
        self.open_connection = Connection(self.port, timeout=timeout, verbose=verbose)
        logging.info('Towerfall. CONNECT PROCESS')
        break
      except TowerfallError:
        if tries > 3:
          raise TowerfallError('Could not config a Towerfall process.')
        tries += 1

  def join_game(self):
    # logging.info('Towerfall.logging')
    connections = []
    agents = []
    i = 0
    for agent in self.config['agents']:
      # if agent['type'] == 'human':
        # continue
      connections.append(self.join(timeout=6000, verbose=self.verbose))
      if 'ai' in agent and agent['ai'] == 'TrainingAgent':
        logging.info('TrainingAgent')
        agents.append(TrainingAgent(3, connections[i]))
      else:
        raise TowerfallError('Only TrainingAgent can be used')

      # i += 1

    return agents[i]

    ##################################################
    # EAch agent is a thread (game still freeze sometime when playing)
    ##################################################
    # threads = []
    # for agent in agents:
    #   thread = threading.Thread(target=agent.run)
    #   threads.append(thread)
    #   thread.start()

    # for thread in threads:
    #   thread.join()

    ##################################################
    # To use training comment the thread above and decomment the code below :
    ##################################################
    # while True: #TODO : use thread and each agent read from its connection : each agent -> thread
    #  # Read the state of the game then replies with an action.
    #  for connection, agent in zip(connections, agents):
    #   #  logging.info('towerfall.run : connection.read_json')
    #    game_state = connection.read_json()
    #   #  logging.info('towerfall.run : agent.act')
    #    agent.act(game_state)


  def join(self, timeout: float = 2, verbose: int = 0) -> Connection:
    '''
    Joins a towerfall game.

    params timeout: Timeout in seconds to wait for a response. The same timeout will be used on calls to get the observations.

    returns: A connection to a Towerfall game. This should be used by the agent to interact with the game.
    '''
    # logging.info('ToxerFall.join')
    connection = Connection(self.port, timeout=timeout, verbose=verbose, log_cap=1000, record_path='capture.log')
    connection.send_json(dict(type='join'))
    response = connection.read_json()
    if response['type'] != 'result':
      raise TowerfallError(f'Unexpected response type: {response["type"]}')
    if not response['success']:
      raise TowerfallError(f'Failed to join the game. Port: {self.port}, Response: {response}')
    self._try_log(logging.info, f'Successfully joined the game. Port: {self.port}')
    return connection

  def send_request_json(self, obj: Mapping[str, Any]):
    #logging.info("TowerFall.send_request_json")
    self.open_connection.send_json(obj)
    logging.info("self.open_connection.read_json()")
    return self.open_connection.read_json()

  def _attain_game_port(self) -> int:
    logging.info('_attain_game_port')
    metadata = self._find_compatible_metadata()

    tries = 0
    self._try_log(logging.info, f'Waiting for available process.')
    while not metadata and tries < 10:
      time.sleep(5)
      metadata = self._find_compatible_metadata()
      tries += 1
    if not metadata:
      raise TowerfallError('Could not find a Towerfall process.')

    return metadata['port']

  def _find_compatible_metadata(self) -> Optional[Mapping[str, Any]]:
    logging.info("TowerFall._find_compatible_metadata")
    if not os.path.exists(self.pool_path):
      return None
    dirs = list(os.listdir(self.pool_path))
    random.shuffle(dirs)
    for file_name in os.listdir(self.pool_path):
      try:
        pid = int(file_name)
        psutil.Process(pid)
      except (ValueError, psutil.NoSuchProcess):
        os.remove(os.path.join(self.pool_path, file_name))
        continue
      try:
        with open(os.path.join(self.pool_path, file_name), 'r') as file:
          try:
            metadata = TowerFallTraining._load_metadata(file)
          except (ValueError, json.JSONDecodeError, FileNotFoundError) as ex:
            self._try_log(logging.warning, f'Invalid metadata file {file_name}. Exception: {ex}')
            continue
          return metadata
      except PermissionError as ex:
        self._try_log(logging.warning, f'Failed to access {file_name}. Exception: {ex}')
        continue
    return None

  @staticmethod
  def _load_metadata(file: TextIOWrapper) -> Mapping[str, Any]:
    logging.info("TowerFallTraining._load_metadata")
    metadata = json.load(file)
    if 'port' not in metadata:
      raise ValueError('Port not found in metadata.')
    try:
      metadata['port'] = int(metadata['port'])
    except ValueError:
      raise ValueError(f'Port is not an integer. Port: {metadata["port"]}')

    if 'fastrun' not in metadata:
      metadata['fastrun'] = False
    if 'nographics' not in metadata:
      metadata['nographics'] = False
    return metadata

  def _try_log(self, log_fn: Callable[[str], None], message: str):
    # logging.info("TowerFall._try_log")
    if self.verbose > 0:
      log_fn(message)

  def send_rematch(self):
    logging.info('send_rematch')
    response = self.send_request_json(dict(type='rematch'))
    # logging.info('response = '+ str(response))
    # if response['type'] != 'result':
    #   raise TowerfallError(f'Unexpected response type: {response["type"]}')
    # if not response['success']:
    #   raise TowerfallError(f'Failed to send rematch message. Port: {self.port}, Response: {response["message"]}')

    # Keep only the agent authorized by the Game
    logging.info('Rematch ongoing, waiting scenario')

