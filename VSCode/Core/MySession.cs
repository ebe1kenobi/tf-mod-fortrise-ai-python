namespace TFModFortRiseAiPython
{
  public class MySession
  {

    internal static void Load()
    {
      On.TowerFall.Session.OnLevelLoadFinish += OnLevelLoadFinish_patch;
      On.TowerFall.Session.LevelLoadStart += LevelLoadStart_patch;
      On.TowerFall.Session.EndRound += EndRound_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.Session.OnLevelLoadFinish -= OnLevelLoadFinish_patch;
      On.TowerFall.Session.LevelLoadStart -= LevelLoadStart_patch;
      On.TowerFall.Session.EndRound -= EndRound_patch;
    }

    public MySession() { }

    public static void OnLevelLoadFinish_patch(On.TowerFall.Session.orig_OnLevelLoadFinish orig, global::TowerFall.Session self)
    {
      orig(self);
      AIPython.NotifyLevelLoad(self.CurrentLevel);
    }

    public static void LevelLoadStart_patch(On.TowerFall.Session.orig_LevelLoadStart orig, global::TowerFall.Session self, global::TowerFall.Level level)
    {
      Logger.Info("LevelLoadStart_patch");
      orig(self, level);
      if (AIPython.Training)
      {
        if (AIPython.Config.noHazards)
        {
          Logger.Info("set NoMiasma");
          self.RoundLogic.CanMiasma = false;
          MyLevel.sandboxEntityCreated = false;
        }
      }
    }
    public static void EndRound_patch(On.TowerFall.Session.orig_EndRound orig, global::TowerFall.Session self)
    {
      Logger.Info("EndRound_patch");
      if (AIPython.Training)
      {
        //Logger.Info("player 3 dead");
        //AIPython.Rematch = true;
        AIPython.Update(self.CurrentLevel);
        // send a last Move to the training agent
        Logger.Info("send last move " + (AIPython.agents.Length - 1));
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end
        AIPython.agents[AIPython.agents.Length - 1].Move(); // send last state because Move() will not be called after this point when match end

      }
      orig(self);
    }
  }
}
