from towerfall import Towerfall

def parse_solids_from_string(s):
    """
    Convertit une chaîne contenant des '1' et '0' séparés par des retours à la ligne
    en une liste de listes d'entiers (solids).
    """
    # On enlève les espaces inutiles et on sépare les lignes
    lines = s.strip().splitlines()

    # Pour chaque ligne, on transforme chaque caractère ('0' ou '1') en int
    solids = [[int(c) for c in line.strip()] for line in lines if line.strip()]

    return solids

def make_solid_border_screen(cols=32, rows=24):
    """
    Crée une grille (liste de listes) de dimensions rows x cols
    avec un bord solide (1) tout autour et l'intérieur vide (0).
    """
    solids = []
    for y in range(rows):
        if y == 0 or y == rows - 1:
            # Ligne du haut ou du bas
            solids.append([1] * cols)
        else:
            # Ligne intermédiaire : bord gauche et droite solides
            solids.append([1] + [0] * (cols - 2) + [1])
    return solids


s= """11111111111111111111111111111111
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000100000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
10000000000000000000000000000001
11111111111111111111111111111111"""
mode='sandbox'
solids=parse_solids_from_string(s)
nbAgents=2
trainingPlayer=[
  dict(type='remote'), # ai
  dict(type='remote'), # IA to train
]
agents=[
  dict(archer='orange', ai='NoMoveAgent', X=300, Y=200),
  dict(archer='purple', ai='TrainingAgent2', X=50, Y=200),
]
#####################################################""
def main():


  # nbAgents = 2
  # trainingPlayer=[
  #   dict(type='remote'), # ai
  #   dict(type='remote'), # IA to train
  # ]
  # agents=[
  #   dict(archer='orange', ai='NoMoveAgent', X=300, Y=200),
  #   dict(archer='purple', ai='TrainingAgent2', X=50, Y=200),
  # ]

  # Creates or reuse a Towerfall game.
  towerfall = Towerfall(
    verbose = 0,
    config = dict(
      matchLengths='Epic', #Instant Quick Standard Epic
      # matchLengths='Epic',
      # mode='HeadHunters', #Quest, DarkWorld, Trials, LastManStanding, HeadHunters, TeamDeathmatch, PlayTag( not supported = Warlord)
      # mode='Respawn',

      # for training specific action : sandbox(todo change MyLevel.Update_patch  create the number of player from the config!)
      # mode='sandbox',
      mode=mode,
      # solids=[[0] * 32]*14 + [[1]*32] + [[0] * 32]*9,
      # solids=make_solid_border_screen(),
      # solids=parse_solids_from_string(s),
      solids=solids,

      level=0,
      subLevel=1,
      noTreasure=True,
      noHazards=True,
      # speed=10,
      speed=10,
      training=True,
      # training=False,
      # fps=60,
      agentTimeout='24:00:00',
      nbAgents = nbAgents, # 4 or 8 number of agent Towerfall will wait to connect before start game, the number in agents blow + number of agent if another client will connect
      # nbAgents = 2, # 4 or 8 number of agent Towerfall will wait to connect before start game, the number in agents blow + number of agent if another client will connect
      # nbAgents = 4, # 4 or 8 number of agent Towerfall will wait to connect before start game, the number in agents blow + number of agent if another client will connect
      # nbHuman = 0, # number of human playing (used when Training=True only)
      #always set all the agent
      trainingPlayer=trainingPlayer,
      # trainingPlayer=[
      #   # dict(type='remote'), # ai
      #   # dict(type='remote'), # ai
      #   dict(type='remote'), # ai
      #   dict(type='remote'), # IA to train
      # ],
      agents=agents,
      # agents=[
      #   # dict(archer='white', ai='NoMoveAgent'),
      #   # dict(archer='orange', ai='NoMoveAgent'),
      #   # dict(archer='purple', ai='NoMoveAgent'),

      #   # dict(archer='orange', ai='SimpleAgentLevel1'),
      #   # dict(archer='orange', ai='SimpleAgentLevel1'),
      #   dict(archer='orange', ai='NoMoveAgent', X=300, Y=200),
      #   dict(archer='purple', ai='TrainingAgent2', X=50, Y=200),

      #   # dict(archer='red', ai='NoMoveAgent'),
      #   # dict(archer='red', ai='Jimmy'),
      #   # dict(archer='orange', ai='SimpleAgentLevel1'),
      #   #dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
      #   #dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
      #   #dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
      #   #dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
      # ],
    )
  )

  towerfall.run()

if __name__ == '__main__':
  main()