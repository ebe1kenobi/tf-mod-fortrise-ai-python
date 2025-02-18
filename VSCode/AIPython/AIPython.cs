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
using TFModFortRiseLoaderAI;

namespace TFModFortRiseAiPython
{
  internal class AIPython
  {
    public static readonly Random Random = new Random((int)DateTime.UtcNow.Ticks);
    public static TcpListener server;
    public static int nbRemoteAgentConnected = 0;
    public static int nbRemoteAgentWaited = 0;
    //public static bool isAgentReady = false;
    public static bool isAgentReady = false;
    public static AIPythonAgent[] agents = new AIPythonAgent[8];
    public static PlayerInput[] AgentInputs = new PlayerInput[8];
    static string scenarioMessage;
    //static bool scenarioSent = false;
    //static bool levelLoaded = false;
    //static List<int> listPlayerAIIndexPlaying = new List<int>();
    static StateUpdate stateUpdate = new StateUpdate();
    static int frame = 0;
    public static String serializedStateUpdate = "";
    static List<String> listEntityToIgnore = new List<string> 
    {
        //still freeze with :
      "TowerFall.Lantern",
      "TowerFall.Chain",
      "TowerFall.Cobwebs",
      "TowerFall.LevelTiles",
      "TowerFall.DefaultArrow",
      "TowerFall.DefaultHat",
      "TowerFall.LightFade",
      "TowerFall.PlayerCorpse",
      "TowerFall.DeathSkull",
      "TowerFall.CatchShine",
      "TowerFall.TreasureChest",
      "TowerFall.BGSkeleton",
      "TowerFall.JumpPad",
      "TowerFall.CrackedPlatform",
      "TowerFall.Spikeball",
      "TowerFall.OrbPickup",
      "TowerFall.Orb",
      "TowerFall.Crown",
      "TowerFall.BGBigMushroom",
      "TowerFall.CrackedWall",
      "TowerFall.ArrowTypePickup",
      "TowerFall.FloatText",
      "TowerFall.ShieldPickup",
      "TowerFall.BombArrow",
      "TowerFall.BombParticle",
      "TowerFall.Explosion",
      "TowerFall.BombPickup",
      "TowerFall.LaserArrow",
      "TowerFall.MovingPlatform",
      "TowerFall.LavaControl",
      "TowerFall.Lava",
      "TowerFall.WingsPickup",
      "TowerFall.Icicle",
      "TowerFall.SnowClump",
      "TowerFall.Ice",
      "TowerFall.PlayerBreath",
      "TowerFall.DrillArrow",
      "TowerFall.SpeedBootsPickup",
      "TowerFall.Miasma",
      "TowerFall.KingIntro",
      "TowerFall.SwitchBlockControl",
      "TowerFall.SwitchBlock",
      "TowerFall.LevelEntity",
      "TowerFall.ShiftBlock",
      "TowerFall.MirrorPickup",
      "TowerFall.BoltArrow",
      "TowerFall.SmallShock",
      "TowerFall.WaterDrop",
      "TowerFall.ProximityBlock",
      "TowerFall.MoonGlassBlock",
      "TowerFall.HotCoals",
      "TowerFall.RainDrops",
      "TowerFall.LoopPlatform",
      "TowerFall.PirateBanner",
      "TowerFall.GhostShipWindow",
      "TowerFall.RotatePlatform",
      "TowerFall.Mud",
      "TowerFall.SensorBlock",
      "TowerFall.BrambleArrow",
      "TowerFall.Brambles",
      "TowerFall.CrumbleBlock",
      "TowerFall.TriggerArrow",
      "TowerFall.PrismArrow",
      "TowerFall.CrumbleWall",
      "Monocle.ParticleSystem",
      //"TowerfallAi.Mod.PlayTag",
      "TowerFall.FeatherArrow",
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
      StateScenario stateScenario = new StateScenario();

      int xSize = level.Tiles.Grid.CellsX;
      int ySize = level.Tiles.Grid.CellsY;

      stateScenario.mode = MainMenu.CurrentMatchSettings.Mode.ToString();
      stateScenario.grid = new int[xSize, ySize];

      for (int x = 0; x < xSize; x++)
      {
        for (int y = 0; y < ySize; y++)
        {
          stateScenario.grid[x, ySize - y - 1] = level.Tiles.Grid[x, y] ? 1 : 0;
        }
      }

      scenarioMessage = JsonConvert.SerializeObject(stateScenario);

      //todo detect player type and player select player[x] == true
      for (int i = 0; i < TFGame.Players.Length; i++) //todo use everywhere
      {
        //Logger.Info("player i = " + i + " playing ?");
        if (!TFGame.Players[i]) continue;
        if ("AIP" != LoaderAIImport.GetPlayerTypePlaying(i)) continue;
        //Logger.Info("player i = " + i + " playing ? YES");
        agents[i].SendScenario(level, scenarioMessage);
      }
      //for (var i = 2; i < 4; i++) //todo
      //{
      //    agents[i].SendScenario(level, scenarioMessage);
      //}
    }

    public static void update(Level level) {
      //Logger.Info("AIpython.update");
      frame++;
      RefreshStateUpdate(level);
      stateUpdate.dt = Engine.TimeMult;
      stateUpdate.id = frame;

      serializedStateUpdate = JsonConvert.SerializeObject(stateUpdate);
    }


    static Dictionary<Type, Func<Entity, StateEntity>> getStateFunctions = new Dictionary<Type, Func<Entity, StateEntity>>() {
      //{ typeof(AmaranthBoss), ExtEntity.GetState}, // Investigate
      //{ typeof(AmaranthShot), ExtEntity.GetState}, // Investigate
      //{ typeof(ArrowTypePickup),  (e) => MyArrowTypePickup.GetState((ArrowTypePickup)e) },
      ////{ typeof(ArrowTypePickup), null },
      //{ typeof(Bat),  (e) => MyBat.GetState((Bat)e) },
      ////{ typeof(Bat), null },
      //{ typeof(Birdman),  (e) => MyBirdman.GetState((Birdman)e) },
      ////{ typeof(Birdman), null },

      //{ typeof(BoltArrow), ExtEntity.GetStateArrow },
      //{ typeof(BombArrow),  (e) => MyBombArrow.GetState((BombArrow)e) },
      ////{ typeof(BombArrow), null },

      //{ typeof(BombPickup), (e) => ExtEntity.GetStateItem(e, TypesItems.Bomb) },
      //{ typeof(BrambleArrow), ExtEntity.GetStateArrow },
      //{ typeof(Brambles), ExtEntity.GetState },
      //{ typeof(CataclysmBlade), ExtEntity.GetState }, // Investigate
      //{ typeof(CataclysmBlock), ExtEntity.GetState }, // Investigate
      //{ typeof(CataclysmBullet), ExtEntity.GetState }, // Investigate
      //{ typeof(CataclysmEye), ExtEntity.GetState }, // Investigate
      //{ typeof(CataclysmMissile), ExtEntity.GetState }, // Investigate
      //{ typeof(CataclysmShieldOrb), ExtEntity.GetState }, // Investigate
      //{ typeof(CrackedPlatform),  (e) => MyCrackedPlatform.GetState((CrackedPlatform)e) },
      ////{ typeof(CrackedPlatform), null },
      //{ typeof(CrackedWall),  (e) => MyCrackedWall.GetState((CrackedWall)e) },
      ////{ typeof(CrackedWall), null },

      //{ typeof(Crown), ExtEntity.GetState },
      //{ typeof(CrumbleBlock), ExtEntity.GetState }, // Investigate
      //{ typeof(CrumbleWall), ExtEntity.GetState }, // Investigate
      //{ typeof(Cultist), ExtEntity.GetState },
      //{ typeof(CyclopsEye), ExtEntity.GetState }, // Investigate
      //{ typeof(CyclopsFist), ExtEntity.GetState }, // Investigate
      //{ typeof(CyclopsPlatform), ExtEntity.GetState }, // Investigate
      //{ typeof(CyclopsShot), ExtEntity.GetState }, // Investigate
      //{ typeof(DefaultArrow), ExtEntity.GetStateArrow },
      //{ typeof(DefaultHat), (e) => ExtEntity.GetState(e, Types.Hat) },
      //{ typeof(DreadEye), ExtEntity.GetState }, // Investigate
      //{ typeof(DreadFlower), ExtEntity.GetState }, // Investigate
      //{ typeof(DreadTentacle), ExtEntity.GetState }, // Investigate
      //{ typeof(DrillArrow), ExtEntity.GetStateArrow },
      //{ typeof(Dummy), ExtEntity.GetState }, // Investigate
      //{ typeof(EnemyAttack),  (e) => MyEnemyAttack.GetState((EnemyAttack)e) },
      ////{ typeof(EnemyAttack), null },
      //{ typeof(EvilCrystal),  (e) => MyEvilCrystal.GetState((EvilCrystal)e) },
      ////{ typeof(EvilCrystal), null },

      //{ typeof(Exploder), ExtEntity.GetState }, // Investigate
      //{ typeof(Explosion), ExtEntity.GetState },
      //{ typeof(FakeWall), ExtEntity.GetState },
      //{ typeof(FeatherArrow), ExtEntity.GetStateArrow },
      //{ typeof(FlamingSkull), ExtEntity.GetState }, // Investigate
      //{ typeof(FloorMiasma),  (e) => MyFloorMiasma.GetState((FloorMiasma)e) },
      ////{ typeof(FloorMiasma), null },
      //{ typeof(Ghost),  (e) => MyGhost.GetState((Ghost)e) }, // Investigate
      ////{ typeof(Ghost), null }, // Investigate

      //{ typeof(GhostPlatform), ExtEntity.GetState }, // Investigate
      //{ typeof(GraniteBlock), ExtEntity.GetState }, // Investigate
      //{ typeof(HotCoals), ExtEntity.GetState }, // Investigate
      //{ typeof(Ice), ExtEntity.GetState },
      //{ typeof(Icicle),  (e) => MyIcicle.GetState((Icicle)e) },
      ////{ typeof(Icicle), null },

      //{ typeof(JumpPad), ExtEntity.GetState }, // Investigate
      //{ typeof(KingReaper),  (e) => MyKingReaper.GetState((KingReaper)e) },
      ////{ typeof(KingReaper), null },
      //{ typeof(KingReaper.ReaperBeam),  (e) => MyReaperBeam.GetState((KingReaper.ReaperBeam)e) },
      ////{ typeof(KingReaper.ReaperBeam), null },
      //{ typeof(KingReaper.ReaperBomb),  (e) => MyReaperBomb.GetState((KingReaper.ReaperBomb)e) },
      ////{ typeof(KingReaper.ReaperBomb), null },
      //{ typeof(KingReaper.ReaperCrystal),  (e) => MyReaperCrystal.GetState((KingReaper.ReaperCrystal)e) }, // Investigate
      ////{ typeof(KingReaper.ReaperCrystal), null }, // Investigate
      //{ typeof(Lantern),  (e) => MyLantern.GetState((Lantern)e) },
      ////{ typeof(Lantern), null },

      //{ typeof(LaserArrow), ExtEntity.GetStateArrow },
      //{ typeof(Lava),  (e) => MyLava.GetState((Lava)e) },
      ////{ typeof(Lava), null },

      //{ typeof(LoopPlatform), ExtEntity.GetState }, // Investigate
      //{ typeof(Miasma),  (e) => MyMiasma.GetState((Miasma)e) },
      ////{ typeof(Miasma), null },

      //{ typeof(MirrorPickup), (e) => ExtEntity.GetStateItem(e, TypesItems.Mirror) }, // Investigate
      //{ typeof(Mole), ExtEntity.GetState }, // Investigate
      //{ typeof(MoonGlassBlock), ExtEntity.GetState }, // Investigate
      //{ typeof(MovingPlatform), ExtEntity.GetState }, // Investigate
      //{ typeof(Orb),  (e) => MyOrb.GetState((Orb)e) },
      ////{ typeof(Orb), null },
      //{ typeof(OrbPickup),  (e) => MyOrbPickup.GetState((OrbPickup)e) },
      ////{ typeof(OrbPickup), null },
      { typeof(Player),  (e) => MyPlayer.GetState((Player)e) },
      ////{ typeof(Player), null },
      //{ typeof(PlayerCorpse),  (e) => MyPlayerCorpse.GetState((PlayerCorpse)e) },
      ////{ typeof(PlayerCorpse), null },

      //{ typeof(Prism), ExtEntity.GetState }, // Investigate
      //{ typeof(PrismArrow), ExtEntity.GetStateArrow },
      //{ typeof(ProximityBlock), ExtEntity.GetState }, // Investigate
      //{ typeof(PurpleArcherPortal), ExtEntity.GetState }, // Investigate
      //{ typeof(QuestSpawnPortal),  (e) => MyQuestSpawnPortal.GetState((QuestSpawnPortal)e) }, // Investigate
      ////{ typeof(QuestSpawnPortal), null }, // Investigate

      //{ typeof(SensorBlock), ExtEntity.GetState }, // Investigate
      //{ typeof(ShieldPickup), (e) => ExtEntity.GetStateItem(e, TypesItems.Shield) }, // Investigate
      //{ typeof(ShiftBlock),  (e) => MyShiftBlock.GetState((ShiftBlock)e) }, // Investigate
      ////{ typeof(ShiftBlock), null }, // Investigate

      //{ typeof(ShockCircle), ExtEntity.GetState }, // Investigate
      //{ typeof(Skeleton),  (e) => MySkeleton.GetState((Skeleton)e) },
      ////{ typeof(Skeleton), null },
      //{ typeof(Slime),  (e) => MySlime.GetState((Slime)e) },
      ////{ typeof(Slime), null },

      //{ typeof(SpeedBootsPickup), (e) => ExtEntity.GetStateItem(e, "speedBoots") },
      //{ typeof(Spikeball),  (e) => MySpikeball.GetState((Spikeball)e) },
      ////{ typeof(Spikeball), null },

      //{ typeof(SteelHat), ExtEntity.GetState }, // Investigate
      //{ typeof(SuperBombArrow),  (e) => MySuperBombArrow.GetState((SuperBombArrow)e) },
      ////{ typeof(SuperBombArrow), null },
      //{ typeof(SwitchBlock),  (e) => MySwitchBlock.GetState((SwitchBlock)e) },
      ////{ typeof(SwitchBlock), null },

      //{ typeof(TechnoMage), ExtEntity.GetState },
      //{ typeof(TechnoMage.TechnoMissile), ExtEntity.GetState },
      //{ typeof(Tornado), ExtEntity.GetState }, // Investigate
      //{ typeof(ToyArrow), ExtEntity.GetStateArrow },
      //{ typeof(TreasureChest),  (e) => MyTreasureChest.GetState((TreasureChest)e) },
      ////{ typeof(TreasureChest), null },

      //{ typeof(TriggerArrow), ExtEntity.GetStateArrow },
      //{ typeof(WingsPickup), (e) => ExtEntity.GetStateItem(e, TypesItems.Wings) },
      //{ typeof(WoodenHat), ExtEntity.GetState }, // Investigate
      //{ typeof(Worm), ExtEntity.GetState }, // Investigate
    };

    private static void RefreshStateUpdate(Level level)
    {
      //Logger.Info("RefreshStateUpdate");
      stateUpdate.entities.Clear();

      foreach (var ent in level.Layers[0].Entities)
      {
        Type type = ent.GetType();
        if (level.Session.MatchSettings.Mode != Modes.Quest && level.Session.MatchSettings.Mode != Modes.DarkWorld)
        {
          if (type.ToString() != "TowerFall.Player" && !listEntityToIgnore.Contains(type.ToString()))
          {
            //get entity to ignore on log
            listEntityToIgnore.Add(type.ToString());
            // cd "/c/Program Files (x86)/Steam/steamapps/common/TowerFall/modcompilkenobi/logs"; grep "1 ==" *
          }
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
      //Logger.Info("RegisterInPool");
      string poolPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\TowerFall\\pools\\default\\"; //todo
      string metadataPathFileName = poolPath + Process.GetCurrentProcess().Id.ToString();

      Util.CreateDirectory(poolPath);

      //Logger.Info("Util.CreateDirectory");

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
          //Logger.Info("Client connected!");
          // Handle client in a separate thread
          Task.Run(() => HandleConnectionClient(client));
        }
        catch (Exception ex)
        {
          Logger.Info($"Error: {ex.Message}");
          break;
        }
      }
      //Logger.Info("server.Stop()");
      server.Stop();
    }

    private static void HandleConnectionClient(TcpClient client)
    {
      try
      {
        NetworkStream stream = client.GetStream();
        Message message = Read(stream);

        if (message.type == "join")
        {
          Logger.Info($"Received: {message.type}");
          HandleJoinMessage(message, stream);
          Logger.Info($"HandleJoinMessage");
        }
        else if (message.type == "config")
        {
          Logger.Info($"Received: {message.type}");
          HandleConfigConnection(message, stream);
          Logger.Info($"HandleNewConfigConnection");
        }
        //else if (message.type == "reset")
        //{
        //  Logger.Info($"Received: {message.type}");
        //  //HandleResetMessage(connection, message);
        //}
        else
        {
          Logger.Info("Message type not supported: {0}".Format(message.type));
        }
      }
      catch (Exception ex)
      {
        Logger.Info($"Client error: {ex.Message}");
      }
    }

    public static void HandleJoinMessage(Message message, NetworkStream stream)
    {
      //Logger.Info($"in HandleJoinMessage");
      if (isAgentReady) {
        Logger.Info("Warning, All agent Waited ("+ nbRemoteAgentWaited + ") are already connected");
        return;
      }
      AgentInputs[nbRemoteAgentConnected] = new Input(nbRemoteAgentConnected);
      agents[nbRemoteAgentConnected] = new AIPythonAgent(nbRemoteAgentConnected, "AIP", AgentInputs[nbRemoteAgentConnected], stream);
      nbRemoteAgentConnected++;
      if (nbRemoteAgentConnected == nbRemoteAgentWaited)
      {
        isAgentReady = true;
        LoaderAIImport.addAgent("AIP", AIPython.agents);
      }

      Write(JsonConvert.SerializeObject(new Message
      {
        type = Message.Type.Result,
        success = true,
        message = "Game will start once all agents join."
      }), stream);
    }

    public static void HandleConfigConnection(Message message, NetworkStream stream)
    {
      //Logger.Info("HandleConfigConnection " + message.config.agents.Count);
      nbRemoteAgentWaited = message.config.agents.Count > 8 ? 8 : message.config.agents.Count;
      Write(JsonConvert.SerializeObject(new Message
      {
        type = Message.Type.Result,
        maxAgent = nbRemoteAgentWaited,
        success = true,
      }), stream);
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
          if (bytesRead == 0) break; // Connection closed

          int bytesToReceive = header[0] << 8 | header[1];

          bytesRead = stream.Read(buffer, 0, bytesToReceive);
          if (bytesRead == 0) break; // Connection closed

          rawMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
          //Logger.Info($"Received: {rawMessage}");
          break;
        }

        Message message = JsonConvert.DeserializeObject<Message>(rawMessage);
        return message;
      }
      catch (Exception ex)
      {
        Logger.Info($"Client error: {ex.Message}");
        throw ex;
      }
    }
  }
}
