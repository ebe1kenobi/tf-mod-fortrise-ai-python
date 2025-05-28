using Monocle;
using System.Collections.Generic;
using TowerFall;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MonoMod.Utils;
using TFModFortRiseLoaderAI;
using System.Text.RegularExpressions;
using FortRise;

namespace TFModFortRiseAiPython
{
  internal class AIPython
  {
    public const string AINAME = "AIPy";
    
    public static readonly Random Random = new Random((int)DateTime.UtcNow.Ticks);
    //private static readonly object _lockJoin = new object();
    //private static readonly object _lockNbRemote = new object();
    //private static readonly object _lockIsAgentReady = new object();
    //private static readonly object _lockNbRemoteAgentWaited = new object();
    //private static readonly object _lockRematch = new object();
    //public static readonly object _lockSend = new object();
    //private static readonly TimeSpan ellapsedGameTime = new TimeSpan(10000000 / 60);
    

    //AIPython.Training
    public static int maxAgent = 4;
    public static TcpListener server;
    public static int nbRemoteAgentConnected = 0;
    public static int nbRemoteAgentWaited = 0;
    //public static bool isAgentReady = false;
    public static bool isAgentReady = false;
    public static AIPythonAgent[] agents;// = new AIPythonAgent[8];
    public static PlayerInput[] AgentInputs;// = new PlayerInput[8];
    static string scenarioMessage;
    //static bool scenarioSent = false;
    //static bool levelLoaded = false;
    //static List<int> listPlayerAIIndexPlaying = new List<int>();
    static StateUpdate stateUpdate = new StateUpdate();
    public static int frame = 0;
    public static String serializedStateUpdate = "";

    public static bool Training = false;
    public static MatchConfig Config { get; private set; }
    private static MatchSettings matchSettings;
    public static bool IsNoConfig { get { return true; } }
    public static bool IsFastrun { get { return true; } }
    public static bool NoGraphics { get { return false; } }
    public static readonly TimeSpan DefaultAgentTimeout = new TimeSpan(0, 0, 10);
    public static ReconfigOperation reconfigOperation;
    public static bool Rematch = false;

    public class ReconfigOperation
    {
      public MatchConfig Config { get; set; }
    }

    static List<String> listEntityToIgnore = new List<string> 
    {
        //still freeze with :
      "TowerFall.Lantern",
      "TowerFall.Chain",
      "TowerFall.Cobwebs",
      //"TowerFall.LevelTiles",
      //"TowerFall.DefaultArrow",
      "TowerFall.DefaultHat",
      "TowerFall.LightFade",
      //"TowerFall.PlayerCorpse",
      "TowerFall.DeathSkull",
      "TowerFall.CatchShine",
      "TowerFall.TreasureChest",
      "TowerFall.BGSkeleton",
      "TowerFall.JumpPad",
      //"TowerFall.CrackedPlatform",
      "TowerFall.Spikeball",
      "TowerFall.OrbPickup",
      "TowerFall.Orb",
      "TowerFall.Crown",
      "TowerFall.BGBigMushroom",
      //"TowerFall.CrackedWall",
      "TowerFall.ArrowTypePickup",
      "TowerFall.FloatText",
      "TowerFall.ShieldPickup",
      //"TowerFall.BombArrow",
      "TowerFall.BombParticle",
      "TowerFall.Explosion",
      "TowerFall.BombPickup",
      //"TowerFall.LaserArrow",
      //"TowerFall.MovingPlatform",
      "TowerFall.LavaControl",
      "TowerFall.Lava",
      "TowerFall.WingsPickup",
      "TowerFall.Icicle",
      "TowerFall.SnowClump",
      "TowerFall.Ice",
      "TowerFall.PlayerBreath",
      //"TowerFall.DrillArrow",
      "TowerFall.SpeedBootsPickup",
      //"TowerFall.Miasma",
      "TowerFall.KingIntro",
      "TowerFall.SwitchBlockControl",
      //"TowerFall.SwitchBlock",
      "TowerFall.LevelEntity",
      //"TowerFall.ShiftBlock",
      "TowerFall.MirrorPickup",
      //"TowerFall.BoltArrow",
      "TowerFall.SmallShock",
      "TowerFall.WaterDrop",
      //"TowerFall.ProximityBlock",
      //"TowerFall.MoonGlassBlock",
      "TowerFall.HotCoals",
      "TowerFall.RainDrops",
      //"TowerFall.LoopPlatform",
      "TowerFall.PirateBanner",
      "TowerFall.GhostShipWindow",
      //"TowerFall.RotatePlatform",
      "TowerFall.Mud",
      //"TowerFall.SensorBlock",
      //"TowerFall.BrambleArrow",
      "TowerFall.Brambles",
      //"TowerFall.CrumbleBlock",
      //"TowerFall.TriggerArrow",
      //"TowerFall.PrismArrow",
      //"TowerFall.CrumbleWall",
      "Monocle.ParticleSystem",
      //"TowerfallAi.Mod.PlayTag",
      //"TowerFall.FeatherArrow",
"TowerFall.Prism",
"TowerFall.PrismParticle",
"TowerFall.CrumbleWallChunk",
"TowerFall.PrismVanish",
"TowerFall.ShockCircle",

      ////still freeze with :
      //"TowerFall.Lantern",
      //"TowerFall.Chain",
      //"TowerFall.Cobwebs",
      //"TowerFall.LevelTiles",
      ////"TowerFall.DefaultArrow",
      //"TowerFall.DefaultHat",
      //"TowerFall.LightFade",
      //"TowerFall.PlayerCorpse",
      //"TowerFall.DeathSkull",
      //"TowerFall.CatchShine",
      ////"TowerFall.TreasureChest",
      //"TowerFall.BGSkeleton",
      ////"TowerFall.JumpPad",
      ////"TowerFall.CrackedPlatform",
      ////"TowerFall.Spikeball",
      ////"TowerFall.OrbPickup",
      ////"TowerFall.Orb",
      //"TowerFall.Crown",
      //"TowerFall.BGBigMushroom",
      ////"TowerFall.CrackedWall",
      ////"TowerFall.ArrowTypePickup",
      //"TowerFall.FloatText",
      ////"TowerFall.ShieldPickup",
      ////"TowerFall.BombArrow",
      //"TowerFall.BombParticle",
      //"TowerFall.Explosion",
      ////"TowerFall.BombPickup",
      ////"TowerFall.LaserArrow",
      ////"TowerFall.MovingPlatform",
      //"TowerFall.LavaControl",
      //"TowerFall.Lava",
      ////"TowerFall.WingsPickup",
      ////"TowerFall.Icicle",
      ////"TowerFall.SnowClump",
      //"TowerFall.Ice",
      //"TowerFall.PlayerBreath",
      ////"TowerFall.DrillArrow",
      ////"TowerFall.SpeedBootsPickup",
      //"TowerFall.Miasma",
      //"TowerFall.KingIntro",
      //"TowerFall.SwitchBlockControl",
      ////"TowerFall.SwitchBlock",
      //"TowerFall.LevelEntity",
      ////"TowerFall.ShiftBlock",
      ////"TowerFall.MirrorPickup",
      ////"TowerFall.BoltArrow",
      //"TowerFall.SmallShock",
      //"TowerFall.WaterDrop",
      ////"TowerFall.ProximityBlock",
      ////"TowerFall.MoonGlassBlock",
      //"TowerFall.HotCoals",
      //"TowerFall.RainDrops",
      ////"TowerFall.LoopPlatform",
      //"TowerFall.PirateBanner",
      //"TowerFall.GhostShipWindow",
      ////"TowerFall.RotatePlatform",
      //"TowerFall.Mud",
      ////"TowerFall.SensorBlock",
      ////"TowerFall.BrambleArrow",
      ////"TowerFall.Brambles",
      ////"TowerFall.CrumbleBlock",
      ////"TowerFall.TriggerArrow",
      ////"TowerFall.PrismArrow",
      ////"TowerFall.CrumbleWall",
      //"Monocle.ParticleSystem",
      ////"TowerfallAi.Mod.PlayTag",

      //from doc
      //amaranthBoss
      //amaranthShot
      //arrow
      //bat
      //batBomb
      //batSuperBomb
      //bird
      //birdman
      //brambles
      //cataclysmBlade
      //cataclysmBlock
      //cataclysmBullet
      //cataclysmEye
      //cataclysmMissile
      //cataclysmShieldOrb
      //chain
      //chest
      //crackedPlatform
      //crackedWall
      //crown
      //crumbleBlock
      //crumbleWall
      //cultist
      //cyclopsEye
      //cyclopsFist
      //cyclopsPlatform
      //cyclopsShot
      //dreadEye
      //dreadFlower
      //dreadTentacle
      //dummy
      //enemyAttack
      //evilCrystal
      //exploder
      //explosion
      //fakeWall
      //flamingSkull
      //floorMiasma
      //ghost
      //ghostPlatform
      //graniteBlock
      //hat
      //hotCoals
      //ice
      //icicle
      //jumpPad
      //kingReaper
      //kingReaperBeam
      //kingReaperBomb
      //kingReaperCrystal
      //lantern
      //laserArrow
      //lava
      //loopPlatform
      //miasma
      //mirrorPickup
      //mole
      //moonGlassBlock
      //movingPlatform
      //mud
      //orb
      //player
      //playerCorpse
      //portal
      //prism
      //prismArrow
      //proximityBlock
      //purpleArcherPortal
      //sensorBlock
      //shiftBlock
      //shockCircle
      //slime
      //spikeball
      //switchBlock
      //technoMage
      //technoMissile
      //tornado
      //worm

    };

    public static void NotifyLevelLoad(Level level)
    {
      //Logger.Info("NotifyLevelLoad");
      StateScenario stateScenario = new StateScenario();
      //Logger.Info("1");

      int xSize = level.Tiles.Grid.CellsX;
      int ySize = level.Tiles.Grid.CellsY;
      //Logger.Info("2");
      //Logger.Info("MainMenu.CurrentMatchSettings = " + MainMenu.CurrentMatchSettings);

      //stateScenario.mode = MainMenu.CurrentMatchSettings.Mode.ToString();
      stateScenario.mode = Training ? Config.mode : MainMenu.CurrentMatchSettings.Mode.ToString();
      //Logger.Info("3");
      stateScenario.grid = new int[xSize, ySize];

      for (int x = 0; x < xSize; x++)
      {
        for (int y = 0; y < ySize; y++)
        {
          stateScenario.grid[x, ySize - y - 1] = level.Tiles.Grid[x, y] ? 1 : 0;
        }
      }
      //Logger.Info("SerializeObject");

      scenarioMessage = JsonConvert.SerializeObject(stateScenario);
      //Logger.Info("SerializeObject2");

      for (int i = 0; i < TFGame.Players.Length; i++) 
      {
      //Logger.Info("i");
        if (!TFGame.Players[i]) continue;
        if (AINAME != LoaderAIImport.GetPlayerTypePlaying(i)) continue;
        agents[i].SendScenario(level, scenarioMessage);
      }
    }

    public static void Update(Level level) {
      frame++;
      RefreshStateUpdate(level);
      stateUpdate.dt = Engine.TimeMult;
      stateUpdate.id = frame;
      serializedStateUpdate = JsonConvert.SerializeObject(stateUpdate);
    }


    static Dictionary<Type, Func<Entity, StateEntity>> getStateFunctions = new Dictionary<Type, Func<Entity, StateEntity>>() {
      { typeof(AmaranthBoss), ExtEntity.GetState}, // Investigate
      { typeof(AmaranthShot), ExtEntity.GetState}, // Investigate
      { typeof(ArrowTypePickup),  (e) => MyArrowTypePickup.GetState((ArrowTypePickup)e) },
      //{ typeof(ArrowTypePickup), null },
      { typeof(Bat),  (e) => MyBat.GetState((Bat)e) },
      //{ typeof(Bat), null },
      { typeof(Birdman),  (e) => MyBirdman.GetState((Birdman)e) },
      //{ typeof(Birdman), null },

      { typeof(BoltArrow), ExtEntity.GetStateArrow },
      { typeof(BombArrow),  (e) => MyBombArrow.GetState((BombArrow)e) },
      //{ typeof(BombArrow), null },

      { typeof(BombPickup), (e) => ExtEntity.GetStateItem(e, TypesItems.Bomb) },
      { typeof(BrambleArrow), ExtEntity.GetStateArrow },
      { typeof(Brambles), ExtEntity.GetState },
      { typeof(CataclysmBlade), ExtEntity.GetState }, // Investigate
      { typeof(CataclysmBlock), ExtEntity.GetState }, // Investigate
      { typeof(CataclysmBullet), ExtEntity.GetState }, // Investigate
      { typeof(CataclysmEye), ExtEntity.GetState }, // Investigate
      { typeof(CataclysmMissile), ExtEntity.GetState }, // Investigate
      { typeof(CataclysmShieldOrb), ExtEntity.GetState }, // Investigate
      { typeof(CrackedPlatform),  (e) => MyCrackedPlatform.GetState((CrackedPlatform)e) },
      //{ typeof(CrackedPlatform), null },
      { typeof(CrackedWall),  (e) => MyCrackedWall.GetState((CrackedWall)e) },
      //{ typeof(CrackedWall), null },

      { typeof(Crown), ExtEntity.GetState },
      { typeof(CrumbleBlock), ExtEntity.GetState }, // Investigate
      { typeof(CrumbleWall), ExtEntity.GetState }, // Investigate
      { typeof(Cultist), ExtEntity.GetState },
      { typeof(CyclopsEye), ExtEntity.GetState }, // Investigate
      { typeof(CyclopsFist), ExtEntity.GetState }, // Investigate
      { typeof(CyclopsPlatform), ExtEntity.GetState }, // Investigate
      { typeof(CyclopsShot), ExtEntity.GetState }, // Investigate
      { typeof(DefaultArrow), ExtEntity.GetStateArrow },
      { typeof(DefaultHat), (e) => ExtEntity.GetState(e, Types.Hat) },
      { typeof(DreadEye), ExtEntity.GetState }, // Investigate
      { typeof(DreadFlower), ExtEntity.GetState }, // Investigate
      { typeof(DreadTentacle), ExtEntity.GetState }, // Investigate
      { typeof(DrillArrow), ExtEntity.GetStateArrow },
      { typeof(Dummy), ExtEntity.GetState }, // Investigate
      { typeof(EnemyAttack),  (e) => MyEnemyAttack.GetState((EnemyAttack)e) },
      //{ typeof(EnemyAttack), null },
      { typeof(EvilCrystal),  (e) => MyEvilCrystal.GetState((EvilCrystal)e) },
      //{ typeof(EvilCrystal), null },

      { typeof(Exploder), ExtEntity.GetState }, // Investigate
      { typeof(Explosion), ExtEntity.GetState },
      { typeof(FakeWall), ExtEntity.GetState },
      { typeof(FeatherArrow), ExtEntity.GetStateArrow },
      { typeof(FlamingSkull), ExtEntity.GetState }, // Investigate
      { typeof(FloorMiasma),  (e) => MyFloorMiasma.GetState((FloorMiasma)e) },
      //{ typeof(FloorMiasma), null },
      { typeof(Ghost),  (e) => MyGhost.GetState((Ghost)e) }, // Investigate
      //{ typeof(Ghost), null }, // Investigate

      { typeof(GhostPlatform), ExtEntity.GetState }, // Investigate
      { typeof(GraniteBlock), ExtEntity.GetState }, // Investigate
      { typeof(HotCoals), ExtEntity.GetState }, // Investigate
      { typeof(Ice), ExtEntity.GetState },
      { typeof(Icicle),  (e) => MyIcicle.GetState((Icicle)e) },
      //{ typeof(Icicle), null },

      { typeof(JumpPad), ExtEntity.GetState }, // Investigate
      { typeof(KingReaper),  (e) => MyKingReaper.GetState((KingReaper)e) },
      //{ typeof(KingReaper), null },
      { typeof(KingReaper.ReaperBeam),  (e) => MyReaperBeam.GetState((KingReaper.ReaperBeam)e) },
      //{ typeof(KingReaper.ReaperBeam), null },
      { typeof(KingReaper.ReaperBomb),  (e) => MyReaperBomb.GetState((KingReaper.ReaperBomb)e) },
      //{ typeof(KingReaper.ReaperBomb), null },
      { typeof(KingReaper.ReaperCrystal),  (e) => MyReaperCrystal.GetState((KingReaper.ReaperCrystal)e) }, // Investigate
      //{ typeof(KingReaper.ReaperCrystal), null }, // Investigate
      { typeof(Lantern),  (e) => MyLantern.GetState((Lantern)e) },
      //{ typeof(Lantern), null },

      { typeof(LaserArrow), ExtEntity.GetStateArrow },
      { typeof(Lava),  (e) => MyLava.GetState((Lava)e) },
      //{ typeof(Lava), null },

      { typeof(LoopPlatform), ExtEntity.GetState }, // Investigate
      { typeof(Miasma),  (e) => MyMiasma.GetState((Miasma)e) },
      //{ typeof(Miasma), null },

      { typeof(MirrorPickup), (e) => ExtEntity.GetStateItem(e, TypesItems.Mirror) }, // Investigate
      { typeof(Mole), ExtEntity.GetState }, // Investigate
      { typeof(MoonGlassBlock), ExtEntity.GetState }, // Investigate
      { typeof(MovingPlatform), ExtEntity.GetState }, // Investigate
      { typeof(Orb),  (e) => MyOrb.GetState((Orb)e) },
      //{ typeof(Orb), null },
      { typeof(OrbPickup),  (e) => MyOrbPickup.GetState((OrbPickup)e) },
      //{ typeof(OrbPickup), null },
      { typeof(Player),  (e) => MyPlayer.GetState((Player)e) },
      //{ typeof(Player), null },
      { typeof(PlayerCorpse),  (e) => MyPlayerCorpse.GetState((PlayerCorpse)e) },
      //{ typeof(PlayerCorpse), null },

      { typeof(Prism), ExtEntity.GetState }, // Investigate
      { typeof(PrismArrow), ExtEntity.GetStateArrow },
      { typeof(ProximityBlock), ExtEntity.GetState }, // Investigate
      { typeof(PurpleArcherPortal), ExtEntity.GetState }, // Investigate
      { typeof(QuestSpawnPortal),  (e) => MyQuestSpawnPortal.GetState((QuestSpawnPortal)e) }, // Investigate
      //{ typeof(QuestSpawnPortal), null }, // Investigate

      { typeof(SensorBlock), ExtEntity.GetState }, // Investigate
      { typeof(ShieldPickup), (e) => ExtEntity.GetStateItem(e, TypesItems.Shield) }, // Investigate
      { typeof(ShiftBlock),  (e) => MyShiftBlock.GetState((ShiftBlock)e) }, // Investigate
      //{ typeof(ShiftBlock), null }, // Investigate

      { typeof(ShockCircle), ExtEntity.GetState }, // Investigate
      { typeof(Skeleton),  (e) => MySkeleton.GetState((Skeleton)e) },
      //{ typeof(Skeleton), null },
      { typeof(Slime),  (e) => MySlime.GetState((Slime)e) },
      //{ typeof(Slime), null },

      { typeof(SpeedBootsPickup), (e) => ExtEntity.GetStateItem(e, "speedBoots") },
      { typeof(Spikeball),  (e) => MySpikeball.GetState((Spikeball)e) },
      //{ typeof(Spikeball), null },

      { typeof(SteelHat), ExtEntity.GetState }, // Investigate
      { typeof(SuperBombArrow),  (e) => MySuperBombArrow.GetState((SuperBombArrow)e) },
      //{ typeof(SuperBombArrow), null },
      { typeof(SwitchBlock),  (e) => MySwitchBlock.GetState((SwitchBlock)e) },
      //{ typeof(SwitchBlock), null },

      { typeof(TechnoMage), ExtEntity.GetState },
      { typeof(TechnoMage.TechnoMissile), ExtEntity.GetState },
      { typeof(Tornado), ExtEntity.GetState }, // Investigate
      { typeof(ToyArrow), ExtEntity.GetStateArrow },
      { typeof(TreasureChest),  (e) => MyTreasureChest.GetState((TreasureChest)e) },
      //{ typeof(TreasureChest), null },

      { typeof(TriggerArrow), ExtEntity.GetStateArrow },
      { typeof(WingsPickup), (e) => ExtEntity.GetStateItem(e, TypesItems.Wings) },
      { typeof(WoodenHat), ExtEntity.GetState }, // Investigate
      { typeof(Worm), ExtEntity.GetState }, // Investigate
    };

    private static void RefreshStateUpdate(Level level)
    {
      stateUpdate.entities.Clear();

      foreach (var ent in level.Layers[0].Entities)
      {
        Type type = ent.GetType();
        if (level.Session.MatchSettings.Mode != Modes.Quest && level.Session.MatchSettings.Mode != Modes.DarkWorld)
        {
          if (type.ToString() != "TowerFall.Player" && !listEntityToIgnore.Contains(type.ToString()))
          {
            //get entity to ignore on log
            //listEntityToIgnore.Add(type.ToString()); //training todo
            // cd "/c/Program Files (x86)/Steam/steamapps/common/TowerFall/modcompilkenobi/logs"; grep "1 ==" *
          }
          //TODO
          if (listEntityToIgnore.Contains(type.ToString())) continue; //ignore all except player, TODO modify
        }

        Func<Entity, StateEntity> getState;
        if (!getStateFunctions.TryGetValue(type, out getState)) continue;

        StateEntity state;
        if (getState != null)
        {
          state = getState(ent);
        }
        else
        {
          MethodInfo methodInfo = type.GetMethod("GetState");
          if (methodInfo == null) throw new Exception("Type {0} does not have GetState".Format(type));
          var getStateFromEntityFuncPtr = methodInfo.MethodHandle.GetFunctionPointer();
          Func<StateEntity> getStateFromEntity = (Func<StateEntity>)Activator.CreateInstance(typeof(Func<StateEntity>), ent, getStateFromEntityFuncPtr);
          state = getStateFromEntity();
        }

        if (state != null)
        {
          stateUpdate.entities.Add(state);
        }
      }
    }

    public static void WriteConfigFile()
    {
      string poolPath = "pools/";
      string metadataPathFileName = poolPath + Process.GetCurrentProcess().Id.ToString();

      Util.CreateDirectory(poolPath);

      using (var f = File.Open(metadataPathFileName, FileMode.OpenOrCreate))
      {
        using (var w = new StreamWriter(f))
        {
          Logger.Info($"Writing metadata to {metadataPathFileName}");
          w.Write(JsonConvert.SerializeObject(new Metadata
          {
            port = ((IPEndPoint)server.LocalEndpoint).Port,
            fastrun = false,
            nographics = false,
          }));
        }
      }
    }

    public static void StartServer()
    {
      server = new TcpListener(IPAddress.Any, 0);
      server.Start();
      while (((IPEndPoint)server.LocalEndpoint).Port == 0) Thread.Sleep(200);
      WriteConfigFile();
      Logger.Info("Server started, waiting for connections...");
      TcpClient client;
      while (true)
      {
        try
        {
          client = server.AcceptTcpClient(); // Blocking call, waits for a client
          // Handle client in a separate thread
          Task.Run(() => HandleConnectionClient(client));
        }
        catch (Exception ex)
        {
          Logger.Info($"Error: {ex.Message}");
          //throw ex;
          break;
        }
      }
      server.Stop();
    }

    private static async Task HandleConnectionClient(TcpClient client)
    {
      try
      {
        NetworkStream stream = client.GetStream();
        Message message = await ReadAsync(stream); // Use async Read

        if (message == null) return;

        Logger.Info($"Received: {message.type}");

        switch (message.type)
        {
          case "join":
            await HandleJoinMessageAsync(message, stream);
            break;
          case "config":
            await HandleConfigConnectionAsync(message, stream);
            break;
          case "rematch":
            await HandleRematchMessageAsync(message, stream);
            break;
          default:
            Logger.Info($"Message type not supported: {message.type}");
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.Info($"Client error: {ex.Message}");
      }
    }


    //private static void HandleConnectionClient(TcpClient client)
    //{
    //  //lock (AIPython._lockSend)
    //  {
    //    try
    //    {
    //      NetworkStream stream = client.GetStream();
    //      Message message = Read(stream);

    //      if (message.type == "join")
    //      {
    //        Logger.Info($"Received: {message.type}");
    //        HandleJoinMessage(message, stream);
    //        Logger.Info($"HandleJoinMessage");
    //      }
    //      else if (message.type == "config")
    //      {
    //        Logger.Info($"Received: {message.type}");
    //        HandleConfigConnection(message, stream);
    //        Logger.Info($"HandleNewConfigConnection");
    //      }
    //      else if (message.type == "rematch")
    //      {
    //        Logger.Info($"Received: {message.type}");
    //        HandleRematchMessage(message, stream);
    //        Logger.Info($"HandleRematchMessage");
    //      }
    //      //else if (message.type == "reset")
    //      //{
    //      //  Logger.Info($"Received: {message.type}");
    //      //  //HandleResetMessage(connection, message);
    //      //}
    //      else
    //      {
    //        Logger.Info("Message type not supported: {0}".Format(message.type));
    //      }
    //    }
    //    catch (Exception ex)
    //    {
    //      Logger.Info($"Client error: {ex.Message}");
    //      //throw ex;
    //    }
    //  }
    //}

    public static int getNbRemoteAgentConnected() {
      //lock (_lockNbRemote) // Empêche l'accès simultané
      {
        return nbRemoteAgentWaited;
      }
    }

    public static int getNbRemoteAgentWaited()
    {
      //lock (_lockNbRemoteAgentWaited) // Empêche l'accès simultané
      {
        return nbRemoteAgentWaited;
      }
    }

    public static bool getIsAgentReady()
    {
      //lock (_lockNbRemote) // Empêche l'accès simultané
      {
        return isAgentReady;
      }
    }

    public static void HandleJoinMessage(Message message, NetworkStream stream)
    {
      //lock (_lockNbRemote) // Empêche l'accès simultané
      {
        if (isAgentReady)
        {
          Logger.Info("Warning, All agent Waited (" + nbRemoteAgentWaited + ") are already connected");
          return;
        }

        //nbRemoteAgent = getNbRemoteAgentConnected();
        AgentInputs[nbRemoteAgentConnected] = new Input(nbRemoteAgentConnected);
        agents[nbRemoteAgentConnected] = new AIPythonAgent(nbRemoteAgentConnected, AINAME, AgentInputs[nbRemoteAgentConnected], stream);

        if (Training)
        {
          //the agent will not play for this training game
          if (reconfigOperation.Config.trainingPlayer[nbRemoteAgentConnected].type == TrainingPlayer.Type.None)
          {
            agents[nbRemoteAgentConnected].setPlaying(false);
          }
        }

        nbRemoteAgentConnected++;

        if (nbRemoteAgentConnected == nbRemoteAgentWaited)
        {
          isAgentReady = true;
        }

        Write(JsonConvert.SerializeObject(new Message
        {
          type = Message.Type.Result,
          success = true,
          message = "Game will start once all agents join."
        }), stream);
      }
    }

    private static readonly object _lock = new object(); // Protection des variables partagées

    public static async Task HandleJoinMessageAsync(Message message, NetworkStream stream)
    {
      //lock (_lock)
      {
        if (isAgentReady)
        {
          Logger.Info($"Warning: Tous les agents attendus ({nbRemoteAgentWaited}) sont déjà connectés.");
          return;
        }

        AgentInputs[nbRemoteAgentConnected] = new Input(nbRemoteAgentConnected);
        agents[nbRemoteAgentConnected] = new AIPythonAgent(nbRemoteAgentConnected, AINAME, AgentInputs[nbRemoteAgentConnected], stream);

        if (Training)
        {
          if (reconfigOperation.Config.trainingPlayer[nbRemoteAgentConnected].type == TrainingPlayer.Type.None)
          {
            agents[nbRemoteAgentConnected].setPlaying(false);
          }
        }

        nbRemoteAgentConnected++;

        if (nbRemoteAgentConnected == nbRemoteAgentWaited)
        {
          isAgentReady = true;
        }
      }

      await WriteAsync(JsonConvert.SerializeObject(new Message
      {
        type = Message.Type.Result,
        success = true,
        message = "Game will start once all agents join."
      }), stream);
    }


    public static bool isRematch() {
      //lock (_lockRematch)
      {
        return Rematch;
      }
    }

    public static void HandleRematchMessage(Message message, NetworkStream stream)
    {
      //lock (_lockRematch) {
        Rematch = true;
      //}
      Write(JsonConvert.SerializeObject(new Message
      {
        type = Message.Type.Result,
        success = true,
        message = "Rematch will start"
      }), stream);
    }

    public static async Task HandleRematchMessageAsync(Message message, NetworkStream stream)
    {
      //lock (_lock)
      {
        Rematch = true;
      }

      await WriteAsync(JsonConvert.SerializeObject(new Message
      {
        type = Message.Type.Result,
        success = true,
        message = "Rematch will start"
      }), stream);
    }

    public static void HandleConfigConnection(Message message, NetworkStream stream)
    {
      maxAgent = TFModFortRiseAiPythonModule.EightPlayerMod ? 8 : 4;
      
      ValidateConfig(message.config);

      //nbRemoteAgentWaited = message.config.agents.Count > maxAgent ? maxAgent : message.config.agents.Count;
      nbRemoteAgentWaited = message.config.nbAgents > maxAgent ? maxAgent : message.config.nbAgents;
      //if (message.config.nbAgents > 0) nbRemoteAgentWaited += message.config.nbAgents;

      agents = new AIPythonAgent[maxAgent];
      AgentInputs = new PlayerInput[maxAgent];

      reconfigOperation = new ReconfigOperation() 
      {
        Config = message.config
      };
      if (message.config.training)
        AIPython.Training = true; 
      else
        AIPython.Training = false;


      Write(JsonConvert.SerializeObject(new Message
      {
        type = Message.Type.Result,
        maxAgent = nbRemoteAgentWaited,
        success = true,
      }), stream);
    }

    public static async Task HandleConfigConnectionAsync(Message message, NetworkStream stream)
    {
      maxAgent = TFModFortRiseAiPythonModule.EightPlayerMod ? 8 : 4;

      ValidateConfig(message.config);

      nbRemoteAgentWaited = Math.Min(message.config.nbAgents, maxAgent);

      agents = new AIPythonAgent[maxAgent];
      AgentInputs = new PlayerInput[maxAgent];

      reconfigOperation = new ReconfigOperation()
      {
        Config = message.config
      };

      Training = message.config.training;

      await WriteAsync(JsonConvert.SerializeObject(new Message
      {
        type = Message.Type.Result,
        maxAgent = nbRemoteAgentWaited,
        success = true
      }), stream);
    }


    public static bool PreUpdate()
    {
      //lock (ongoingOperationLock)
      //{
        // All changes happen in the main thread to avoid race condition during Updates.
        //if (ctsSession.IsCancellationRequested)
        //{
        //  ctsSession = new CancellationTokenSource();
        //}

        if (reconfigOperation != null)
        {
          // Reconfig without a new config works as a Rematch.
          if (reconfigOperation.Config != null)
          {
            Config = reconfigOperation.Config;
            //Agents.PrepareAgentConnections(Config.agents);
            //Agents.AssignRemoteConnections(reconfigOperation.Connections, ctsSession.Token);
          }

          reconfigOperation = null;
          if (Training)
          {
            StartNewSession();
          }
        }
      else if (isRematch())
      {
        if (Training)
        {
          StartNewSession();
        }
      }

      //if (resetOperation != null)
      //{
      //  Agents.Reset(resetOperation.Entities, ctsSession.Token);
      //  resetOperation = null;
      //}
      //}

      //if (!IsMatchRunning())
      //{
      //  Sound.StopSound();
      //  return false;
      //}
      //else
      //{
      //  Sound.ResumeSound();
      //}

      //totalFrame++;
      //totalGameTime += ellapsedGameTime;
      return true;
    }

    public static void Write(string text, NetworkStream stream)
    {
      byte[] payload = Encoding.ASCII.GetBytes(text);
      int size = payload.Length;
      //if (size > maxMessageSize)
      //{
      //  throw new Exception("Message exceeds limit: {0}.".Format(maxMessageSize));
      //}
      byte[] header = new Byte[2];
      header[0] = (byte)(size >> 8);
      header[1] = (byte)(size & 0x00FF);
      //socket.Send(header);
      stream.Write(header, 0, header.Length);
      //socket.Send(payload);
      stream.Write(payload, 0, payload.Length);
    }

    public static async Task WriteAsync(string text, NetworkStream stream)
    {
      byte[] payload = Encoding.ASCII.GetBytes(text);
      int size = payload.Length;

      byte[] header = new byte[2];
      header[0] = (byte)(size >> 8);
      header[1] = (byte)(size & 0x00FF);

      await stream.WriteAsync(header, 0, header.Length);
      await stream.WriteAsync(payload, 0, payload.Length);
    }


    public static Message Read(NetworkStream stream)
    {
      try
      {
        byte[] header = new byte[2];
        byte[] buffer = new byte[10000];
        string rawMessage = "";
        while (true)
        {
          int bytesRead = stream.Read(header, 0, header.Length);
          if (bytesRead == 0)
          {
            break; // Connection closed
          }

          int bytesToReceive = header[0] << 8 | header[1];

          bytesRead = stream.Read(buffer, 0, bytesToReceive);
          if (bytesRead == 0)
          {
            break; // Connection closed
          }

          rawMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
          Logger.Info("rawMessage");
          Logger.Info(rawMessage);
          break;
        }
        Logger.Info("rawMessage = " + rawMessage);
        Message message = JsonConvert.DeserializeObject<Message>(rawMessage);
        return message;
      }
      catch (Exception ex)
      {
        Logger.Info($"Client error: {ex.Message}");
        throw ex;
      }
    }

    public static async Task<Message> ReadAsync(NetworkStream stream)
    {
      try
      {
        byte[] header = new byte[2];
        byte[] buffer = new byte[10000];
        string rawMessage = "";

        int bytesRead = await stream.ReadAsync(header, 0, header.Length);
        if (bytesRead == 0)
        {
          return null;
        }

        int bytesToReceive = header[0] << 8 | header[1];

        bytesRead = await stream.ReadAsync(buffer, 0, bytesToReceive);
        if (bytesRead == 0)
        {
          return null;
        }

        rawMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        return JsonConvert.DeserializeObject<Message>(rawMessage);
      }
      catch (Exception ex)
      {
        Logger.Info($"Client error: {ex.Message}");
        return null;
      }
    }



    public static void ValidateConfig(MatchConfig config)
    {

      if (config.mode == null && IsNoConfig)
      {
        throw new ConfigException("Game mode need to be specified in config request.");
      }
      if (config.mode == null && IsFastrun)
      {
        throw new ConfigException("Fastrun can only be enabled when game mode is selected.");
      }

      switch (config.mode)
      {
        case "sandbox":
          if (config.agents == null || config.agents.Count <= 0)
          {
            throw new ConfigException("No agent in config, starting normal game.");
          }
          if (config.level < 0)
          {
            throw new ConfigException("Invalid level {0}.");
          }
          break;
        case "LastManStanding":
        case "HeadHunters":
        case "TeamDeathmatch":
        case "Quest":
        case "DarkWorld":
        case "Trials":
        case "PlayTag":
        case "Respawn":
          //TODO
          //skipWaves
          //solids

          if (config.agentTimeout == null)
          {
            Logger.Info($"Agent timeout not specified. Using default {DefaultAgentTimeout}.");
            config.agentTimeout = DefaultAgentTimeout;
          }

          
          if (config.nbAgents <= 0)
          {
            throw new ConfigException("nbAgents config parameter is mandatory.");
          }
          if (config.agents.Count > 8)
          {
            throw new ConfigException("Too many agents. Only 8 players are supported.");
          }
          if (TFGame.Players.Length > 4 && Training && config.agents.Count > 6 && config.mode == "TeamDeathmatch")
          {
            throw new ConfigException("Too many agents. Only 6 players are supported for TeamDeathmatch.");
          }

          //If not training, the matchsettings will be set in the game interface
          if (!Training)
          {
            break;
          }
          if (!config.randomLevel)
          {
            if (config.level < 0)
            {
              throw new ConfigException("Invalid level {0}.".Format(config.level));
            }

            if (config.mode == "Trials" && config.subLevel <= 0)
            {
              throw new ConfigException("Invalid subLevel {0}.".Format(config.subLevel));
            }
          }

          if ((config.mode == "LastManStanding" || config.mode == "HeadHunters"
            || config.mode == "TeamDeathmatch" || config.mode == "PlayTag") &&
                config.matchLengths != "Instant" && config.matchLengths != "Quick" &&
                config.matchLengths != "Standard" && config.matchLengths != "Epic")
          {
            throw new ConfigException("matchLengths invalid.");
          }

          if (config.mode == "DarkWorld" &&
              (config.difficulty != "Normal" && config.difficulty != "Hardcore" && config.difficulty != "Legendary"))
          {
            throw new ConfigException("difficulty invalid.");
          }
          if (config.mode == "Quest" && config.agents.Count > 4)
          {
            throw new ConfigException("DarkWorld mode is for 1-4 player.");
          }

          if (config.mode == "Quest" &&
              (config.difficulty != "Normal" && config.difficulty != "Hardcore"))
          {
            throw new ConfigException("difficulty invalid.");
          }
          if (config.mode == "Quest" && config.agents.Count > 2)
          {
            throw new ConfigException("Quest mode is for 1-2 player.");
          }

          if (config.mode == "Trial" && config.agents.Count > 1)
          {
            throw new ConfigException("Trials mode is for 1 player.");
          }
          break;
        default:
          throw new ConfigException("Mode value unknown.");
      }
    }

    private static void StartNewSession()
    {
      Logger.Info("Starting a new session.");
      CreateMatchSettings();

      
      Session session = new Session(matchSettings);
      //var dynData = DynamicData.For(session);
      //dynData.Set("RoundIndex", 1);
      //dynData.Dispose();
      session.QuestTestWave = Config.skipWaves;
      session.StartGame();
      //session.StartRound();
      Logger.Info("Session started.");
      //sessionEnded = false;
      //lock (_lockRematch)
      {
        Rematch = false;
      }
      //Agents.SessionRestarted();
    }


    private static void CreateMatchSettings()
    {
      Logger.Info("CreateMatchSettings.");
      Logger.Info("Config = " + Config);
      //if (!IsNoConfig)
      //{
      //  Config = JsonConvert.DeserializeObject<MatchConfig>(File.ReadAllText(ConfigPath));
      //}
      MatchSettings.MatchLengths matchLength;
      if (Config.matchLengths == "Instant")
      {
        matchLength = MatchSettings.MatchLengths.Instant;
      }
      else if (Config.matchLengths == "Quick")
      {
        matchLength = MatchSettings.MatchLengths.Quick;
      }
      else if (Config.matchLengths == "Epic")
      {
        matchLength = MatchSettings.MatchLengths.Epic;
      }
      else
      {
        matchLength = MatchSettings.MatchLengths.Standard;

      }
      LevelSystem levelSystem = getLevel(Config);

      if (Config.mode == GameModes.HeadHunters)
      {
        Logger.Info("Configuring HeadHunters mode.");
        matchSettings = new MatchSettings(levelSystem, Modes.HeadHunters, matchLength);
        matchSettings.Variants.TournamentRules();
      }
      else if (Config.mode == GameModes.TeamDeathmatch)
      {
        Logger.Info("Configuring TeamDeathmatch mode.");
        matchSettings = new MatchSettings(levelSystem, Modes.TeamDeathmatch, matchLength);
        matchSettings.Variants.TournamentRules();
      }
      else if (Config.mode == GameModes.LastManStanding)
      {
        Logger.Info("Configuring LastManStanding mode.");
        matchSettings = new MatchSettings(levelSystem, Modes.LastManStanding, matchLength);
        matchSettings.Variants.TournamentRules();
      }
      else if (Config.mode == GameModes.Respawn)
      {
        Logger.Info("Configuring Respawn mode.");
        matchSettings = new MatchSettings(levelSystem, ModRegisters.GameModeType<Respawn>(), matchLength);
        matchSettings.Variants.TournamentRules();
      }
      //else if (TFModFortRiseAIModule.IsModPlaytagExists && Config.mode == GameModes.PlayTag)
      //{
      //  Logger.Info("Configuring PlayTag mode.");
      //  matchSettings = new MatchSettings(levelSystem, Modes.PlayTag, matchLength);
      //  matchSettings.Variants.TournamentRules();
      //}
      else if (Config.mode == GameModes.Quest)
      {
        Logger.Info("Configuring Quest mode.");
        matchSettings = new MatchSettings(levelSystem, Modes.Quest, matchLength);
        if (Config.difficulty == "Hardcore")
        {
          matchSettings.QuestHardcoreMode = true;
        }
        else
        {
          matchSettings.QuestHardcoreMode = false;
        }
      }
      else if (Config.mode == GameModes.DarkWorld)
      {
        Logger.Info("Configuring DarkWorld mode.");
        matchSettings = new MatchSettings(levelSystem, Modes.DarkWorld, matchLength);

        if (Config.difficulty == "Legendary")
        {
          matchSettings.DarkWorldDifficulty = DarkWorldDifficulties.Legendary;
        }
        else if (Config.difficulty == "Hardcore")
        {
          matchSettings.DarkWorldDifficulty = DarkWorldDifficulties.Hardcore;
        }
        else
        {
          matchSettings.DarkWorldDifficulty = DarkWorldDifficulties.Normal;
        }
      }
      else if (Config.mode == GameModes.Trials)
      {
        Logger.Info("Configuring Trials mode.");
        matchSettings = new MatchSettings(levelSystem, Modes.Trials, matchLength);
      }
      else if (Config.mode == GameModes.Sandbox)
      {
        Logger.Info("Configuring Sandbox mode.");
        matchSettings = MatchSettings.GetDefaultTrials();
        matchSettings.Mode = Modes.LevelTest;
        matchSettings.LevelSystem = new SandboxLevelSystem(GameData.QuestLevels[Config.level], Config.solids);
      }
      else
      {
        throw new Exception("Game mode not supported: {0}".Format(Config.mode));
      }

      if (Config.noTreasure) {
        Logger.Info("set NoTreasure");
        matchSettings.Variants.NoTreasure.Value = true;
      }

      

      //if (Config.speed > 1)
      //{
      //  Logger.Info("set speed");
      //  Engine.TimeRate = Config.speed;
      //}

      //int indexHuman = 0;
      //int indexRemote = Config.nbHuman;
      //int indexForTeam = 0;



      for (int i = 0; i < nbRemoteAgentConnected; i++)
      {
        //var agent = Config.agents[i];
        AgentConfig agent;
        if (AIPython.Training && Config.agents.Count < i + 1)
        {
          agent = new AgentConfig();
        }
        else
        {
          agent = Config.agents[i];
        }

        matchSettings.Teams[i] = agent.GetTeam();
      }

      //hide the intro control for level 0 or trigger controle for tower N
      var dynData = DynamicData.For(matchSettings.LevelSystem);
      dynData.Set("ShowControls", false);
      dynData.Set("ShowTriggerControls", false);
      dynData.Dispose();
    }

    public static int getSubLevel() {
      return Config.subLevel;
    }

    //public static bool IsMatchRunning()
    //{

    //  if (IsNoConfig)
    //  {
    //    if (Config == null) return false;
    //    if (Config.agents == null) return false;
    //    if (Config.agents.Count == 0) return false;
    //    //if (Config.mode == GameModes.Sandbox && !Agents.IsReset) return false;
    //    if (Config.mode == GameModes.Sandbox) return false;
    //  }

    //  if (Config != null &&
    //      Config.agents != null &&
    //      Config.agents.Count > 0
    //      //!Agents.Ready) return false;
    //      ) return false;

    //  return true;
    //}

    //public static int CountHumanConnections(List<AgentConfig> agentConfigs)
    //{
    //  if (agentConfigs == null || agentConfigs.Count == 0) return 0;

    //  int count = 0;
    //  foreach (AgentConfig agentConfig in agentConfigs)
    //  {
    //    if (agentConfig.type == AgentConfig.Type.Human) count++;
    //  }
    //  return count;
    //}

    private static LevelSystem getLevel(MatchConfig Config)
    {
      if (Config.mode == GameModes.LastManStanding || Config.mode == GameModes.HeadHunters
          || Config.mode == GameModes.TeamDeathmatch || Config.mode == GameModes.PlayTag
          || Config.mode == GameModes.Respawn)
      {
        if (Config.randomLevel)
        {
          System.Random rnd = new Random();
          return GameData.VersusTowers[rnd.Next(1, 17)].GetLevelSystem(); //16 levels
        }
        else if (Config.level >= 0)
        {
          Logger.Info("Config.level != 0 " + Config.level);
          return GameData.VersusTowers[Config.level].GetLevelSystem();
        }
        else
        {
          return GameData.VersusTowers[1].GetLevelSystem();
        }
      }
      else if (Config.mode == GameModes.Quest)
      {
        if (Config.randomLevel)
        {
          System.Random rnd = new Random();
          return GameData.QuestLevels[rnd.Next(1, 13)].GetLevelSystem(); //12 levels
        }
        else if (Config.level != 0)
        {
          return GameData.QuestLevels[Config.level].GetLevelSystem();
        }
        else
        {
          return GameData.QuestLevels[1].GetLevelSystem();
        }
      }
      else if (Config.mode == GameModes.DarkWorld)
      {
        if (Config.randomLevel)
        {
          System.Random rnd = new Random();
          return GameData.DarkWorldTowers[rnd.Next(1, 5)].GetLevelSystem(); // 4 levels
        }
        else if (Config.level != 0)
        {
          return GameData.DarkWorldTowers[Config.level].GetLevelSystem();
        }
        else
        {
          return GameData.DarkWorldTowers[1].GetLevelSystem();
        }
      }
      else if (Config.mode == GameModes.Trials)
      {
        if (Config.randomLevel)
        {
          System.Random rnd = new Random();
          return GameData.TrialsLevels[rnd.Next(1, 17), rnd.Next(1, 4)].GetLevelSystem(); //16 levels with 3 sublevels
        }
        else if (Config.level != 0)
        {
          return GameData.TrialsLevels[Config.level, Config.subLevel].GetLevelSystem();
        }
        else
        {
          return GameData.TrialsLevels[1, 1].GetLevelSystem();
        }
      }
      else //default QuestLevels
      {
        return GameData.QuestLevels[0].GetLevelSystem();
      }
    }

    public static int getNbHumanTraining() {
      int nb = 0;
      for (int i = 0; i < reconfigOperation.Config.trainingPlayer.Count; i++) {
        if (reconfigOperation.Config.trainingPlayer[i].type == TrainingPlayer.Type.Human) nb++;
      }
      return nb;
    }
    //public static bool IsHumanPlaying()
    //{
    //  if (Config.mode == null) return true;
    //  if (NoGraphics) return false;

    //  foreach (AgentConfig agent in Config.agents)
    //  {
    //    if (agent.type == AgentConfig.Type.Human)
    //    {
    //      return true;
    //    }
    //  }

    //  return false;
    //}

  }
}
