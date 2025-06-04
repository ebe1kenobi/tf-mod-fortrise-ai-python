# import gymnasium as gym
import os
import numpy as np
from stable_baselines3 import PPO
from towerfall import TowerFallTraining, TowerFallEnv, Connection
from common.logging_options import default_logging
import time
from datetime import datetime
# connect to TowerFall game
towerfall = TowerFallTraining(
  verbose = 0,
  # timeout = 99999,
  config = dict(
    agentTimeout='24:00:00',
    agents=[
      dict(archer='blue', ai='TrainingAgent'),
    ],
  )
)

agent = towerfall.join_game()
game_state = agent.connection.read_json()
while True:
  print(f"while true ")
  print(f"game_state {game_state["type"]}")
  if game_state['type'] == 'update':
    print(f"break")
    break
  agent.act(game_state)
  game_state = agent.connection.read_json()

# here, towerfall wait the agent to send an action for the update
print(f"******** game_state {game_state["type"]}")

# # # Création de l'environnement
env = TowerFallEnv(agent, game_state)  # Remplace par ton environnement Gym

timestamp = datetime.now().strftime("%Y%m%d%H%M%S")

model_name = "jimmy"
model_name_last = f"{model_name}.last.zip"
model_path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\TowerFall\\Mods\\"
last = f"{model_path}{model_name_last}"
# # # Création du modèle PPO
if os.path.exists(last):
  print(f"load model existant")
  model = PPO.load(last)
  model.set_env(env)
else:
  print(f"create nouveau model")
  model = PPO("MlpPolicy", env, verbose=1)



# i == 200 1heure  1254 parties, 400 000 step
i = 0
while True:
  i += 1
  print(f"while True: {i}")
  print(f"before learn: {i}")
  model.learn(total_timesteps=2000)
  print(f"after learn: {i}")
  model.save(f"{model_path}{model_name}.{timestamp}.{env.total_game}.{env.total_step}.{env.game_reward}.zip")
  model.save(last)
  # if i == 2:
    # break
  # time.sleep(2)
        # total_timesteps = 0

    # print(f"**************************Épisode {episode + 1} terminé avec un score de {total_reward}")
    # print(f"**************************Épisode {episode + 1} terminé avec un score de {total_reward}")
    # print(f"**************************Épisode {episode + 1} terminé avec un score de {total_reward}")
    # print(f"**************************Épisode {episode + 1} terminé avec un score de {total_reward}")
    # print(f"**************************Épisode {episode + 1} terminé avec un score de {total_reward}")
    # print(f"**************************Épisode {episode + 1} terminé avec un score de {total_reward}")
    # print(f"**************************Épisode {episode + 1} terminé avec un score de {total_reward}")
    # print(f"**************************Épisode {episode + 1} terminé avec un score de {total_reward}")

  # model.save("C:\\Program Files (x86)\\Steam\\steamapps\\common\\TowerFall\\Mods\\tf-mod-fortrise-ai-python\\agent_entraine.{num_episodes}.{total_step}")

# Sauvegarde du modèle entraîné
# model.save("C:\\Program Files (x86)\\Steam\\steamapps\\common\\TowerFall\\Mods\\tf-mod-fortrise-ai-python\\agent_entraine")