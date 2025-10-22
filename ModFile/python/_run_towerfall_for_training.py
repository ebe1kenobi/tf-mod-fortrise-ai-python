from towerfall import Towerfall

def main():
  # Creates or reuse a Towerfall game.
  towerfall = Towerfall(
    verbose = 0,
    config = dict(
      matchLengths='Epic', #Instant Quick Standard Epic
      # matchLengths='Epic',
      # mode='HeadHunters', #Quest, DarkWorld, Trials, LastManStanding, HeadHunters, TeamDeathmatch, PlayTag( not supported = Warlord)
      # mode='Respawn',

      # for training specific action : sandbox(todo change MyLevel.Update_patch  create the number of player from the config!)
      mode='sandbox',
      solids=[[0] * 32]*14 + [[1]*32] + [[0] * 32]*9,

      level=0,
      subLevel=1,
      noTreasure=True,
      noHazards=True,
      # speed=10,
      speed=1,
      training=True,
      # training=False,
      # fps=60,
      agentTimeout='24:00:00',
      nbAgents = 2, # 4 or 8 number of agent Towerfall will wait to connect before start game, the number in agents blow + number of agent if another client will connect
      # nbAgents = 4, # 4 or 8 number of agent Towerfall will wait to connect before start game, the number in agents blow + number of agent if another client will connect
      # nbHuman = 0, # number of human playing (used when Training=True only)
      #always set all the agent
      trainingPlayer=[
        # dict(type='remote'), # ai
        # dict(type='remote'), # ai
        dict(type='remote'), # ai
        dict(type='remote'), # IA to train
      ],
      agents=[
        # dict(archer='white', ai='NoMoveAgent'),
        # dict(archer='orange', ai='NoMoveAgent'),
        # dict(archer='purple', ai='NoMoveAgent'),

        # dict(archer='orange', ai='SimpleAgentLevel1'),
        # dict(archer='orange', ai='SimpleAgentLevel1'),
        dict(archer='orange', ai='NoMoveAgent'),
        dict(archer='purple', ai='TrainingAgent2'),

        # dict(archer='red', ai='NoMoveAgent'),
        # dict(archer='red', ai='Jimmy'),
        # dict(archer='orange', ai='SimpleAgentLevel1'),
        #dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
        #dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
        #dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
        #dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
      ],
    )
  )

  towerfall.run()

if __name__ == '__main__':
  main()