from towerfall import Towerfall

def main():
  # Creates or reuse a Towerfall game.
  towerfall = Towerfall(
    verbose = 0,
    config = dict(
      matchLengths='Standard', #Instant Quick Standard Epic
      mode='HeadHunters', #Quest, DarkWorld, Trials, LastManStanding, HeadHunters, TeamDeathmatch, PlayTag( not supported = Warlord)
      level=0,
      subLevel=1,
      training=True,
      # fps=60,
      agentTimeout='24:00:00',
      agents=[
        dict(type='remote', archer='white', ai='SimpleAgentLevel1'),
        dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
        dict(type='remote', archer='purple', ai='SimpleAgentLevel1'),
        dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
        dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
        dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
        dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
        dict(type='remote', archer='orange', ai='SimpleAgentLevel1'),
        ],
    )
  )

  towerfall.run()

if __name__ == '__main__':
  main()