from towerfall import Towerfall

def main():
  # Creates or reuse a Towerfall game.
  towerfall = Towerfall(
    verbose = 0,
    config = dict(
      matchLengths='Instant', #Instant Quick Standard Epic
      mode='HeadHunters', #Quest, DarkWorld, Trials, LastManStanding, HeadHunters, TeamDeathmatch, PlayTag( not supported = Warlord)
      level=1,
      subLevel=0,
      noTreasure=True,
      noHazards=True,
      speed=10,
      # speed=1,
      training=True,
      # training=False,
      # fps=60,
      agentTimeout='24:00:00',
      nbAgents = 4, # 4 or 8 number of agent Towerfall will wait to connect before start game, the number in agents blow + number of agent if another client will connect
      # nbHuman = 0, # number of human playing (used when Training=True only)
      #always set all the agent
      trainingPlayer=[
        dict(type='none'), # human MUST always be in the beginning of the array!
        dict(type='none'),
        dict(type='none'),
        dict(type='remote'), # IA to train, will connect after
      ],
      agents=[
        dict(archer='white', ai='NoMoveAgent'),
        dict(archer='orange', ai='NoMoveAgent'),
        dict(archer='purple', ai='NoMoveAgent'),
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