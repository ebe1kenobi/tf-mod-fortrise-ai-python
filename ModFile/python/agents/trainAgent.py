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

class TrainAgent(Agent):

  def act(self, game_state: Mapping[str, Any]):
    logging.info('TrainAgent.act')

    # is game state scenario et not start train

    # wait game start
    # get agent

    # Création de l'environnement
    env = TowerFallEnv(agent)  # Remplace par ton environnement Gym

    # Création du modèle PPO
    model = PPO("MlpPolicy", env, verbose=1)

    # Nombre d'épisodes à entraîner
    num_episodes = 15

    # Boucle d'entraînement sur 15 parties
    for episode in range(num_episodes):
      obs = env.reset()  # Réinitialiser l'environnement (prend en compte le scenario)
      done = False
      total_reward = 0

      while not done:
      action, _states = model.predict(obs)  # Prédire une action avec le modèle
      obs, reward, done, info = env.step(action)  # Envoyer l'action et récupérer l'état suivant
      total_reward += reward  # Cumuler les récompenses

      # Vérifier si on reçoit un JSON avec "type": "end"
      if "type" in info and info["type"] == "end":
          done = True  # Forcer la fin de l'épisode

      print(f"Épisode {episode + 1} terminé avec un score de {total_reward}")

    # Sauvegarde du modèle entraîné
    model.save("agent_entraine")

    return True
