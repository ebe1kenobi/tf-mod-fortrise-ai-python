from towerfall import Towerfall

def main():
  # Creates or reuse a Towerfall game.
  towerfall = Towerfall(
    verbose = 0,
    config = dict(
      matchLengths='Instant', #Instant Quick Standard Epic
      mode='HeadHunters', #Quest, DarkWorld, Trials, LastManStanding, HeadHunters, TeamDeathmatch, PlayTag( not supported = Warlord)
      level=0,
      subLevel=1,
      training=False,
      # fps=60,
      agentTimeout='24:00:00',
      nbAgents = 4, # 4 or 8 number of agent Towerfall will wait to connect before start game, the number in agents blow + number of agent if another client will connect
      nbHuman = 0, # number of human playing (used when Training=True only)
      #always set all the agent
      agents=[
        dict(type='remote', archer='white', ai='SimpleAgentLevel1'),
        dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
        dict(type='remote', archer='purple', ai='SimpleAgentLevel1'),
        dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
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