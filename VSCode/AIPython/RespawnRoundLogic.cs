using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using MonoMod.Utils;
using TowerFall;
using Microsoft.Xna.Framework;

namespace TFModFortRiseAiPython
{
  [CustomRoundLogic("RespawnRoundLogic")]
  internal class RespawnRoundLogic : RoundLogic
  {
    private KillCountHUD[] killCountHUDs;
    private bool wasFinalKill;
    private Counter endDelay;
    private float[] autoReviveCounters;


    public static RoundLogicInfo Create()
    {
      return new RoundLogicInfo
      {
        Name = "Respawn",
      Icon = TFGame.MenuAtlas["gameModes/warlord"],
        //Icon = TFModFortRiseAiPythonModule.RespawnAtlas["gamemodes/respawn"],
        RoundType = RoundLogicType.HeadHunters
      };
    }

    internal static void Load()
    {
      On.TowerFall.RoundLogic.FFACheckForAllButOneDead += FFACheckForAllButOneDead_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.RoundLogic.FFACheckForAllButOneDead -= FFACheckForAllButOneDead_patch;
    }

    private static bool FFACheckForAllButOneDead_patch(On.TowerFall.RoundLogic.orig_FFACheckForAllButOneDead orig, RoundLogic self)
    {
      if (self is RespawnRoundLogic)
        return false;
      return orig(self);
    }


    public RespawnRoundLogic(Session session) : base(session, false)
    {
      var playerCount = EightPlayerUtils.GetPlayerCount();
      killCountHUDs = new KillCountHUD[playerCount];
      for (int i = 0; i < playerCount; i++)
      {
        if (TFGame.Players[i])
        {
          killCountHUDs[i] = new KillCountHUD(i);
          this.Session.CurrentLevel.Add(killCountHUDs[i]);
        }
      }
      this.endDelay = new Counter();
      this.endDelay.Set(90);
      autoReviveCounters = new float[playerCount];
    }

    public override void OnLevelLoadFinish()
    {
      base.OnLevelLoadFinish();
      Session.CurrentLevel.Add<VersusStart>(new VersusStart(base.Session));
      Players = SpawnPlayersFFA();
    }

    public override void OnUpdate()
    {
      base.OnUpdate();
      if (base.RoundStarted && base.Session.CurrentLevel.Ending && base.Session.CurrentLevel.CanEnd)
      {
        if (this.endDelay)
        {
          this.endDelay.Update();
          return;
        }
        base.Session.EndRound();
      }
      if (TFModFortRiseAiPythonModule.Settings.RespawnMode == TFModFortRiseAiPythonSettings.Delayed && !Session.CurrentLevel.Ending)
      {
        for (int i = 0; i < autoReviveCounters.Length; i++)
        {
          if (this.autoReviveCounters[i] > 0f)
          {
            this.autoReviveCounters[i] -= Engine.TimeMult;
            if (this.autoReviveCounters[i] <= 0f)
            {
              this.DoAutoRevive(i);
            }
          }
        }
      }
    }

    private void DoAutoRevive(int playerIndex)
    {
      TeamReviver selectedTeamReviver = null;
      foreach (TeamReviver teamReviver in base.Session.CurrentLevel[GameTags.TeamReviver])
      {
        if (teamReviver.Corpse.PlayerIndex == playerIndex)
        {
          selectedTeamReviver = teamReviver;
          break;
        }
      }
      if (selectedTeamReviver != null && !selectedTeamReviver.AutoRevive)
      {
        selectedTeamReviver.AutoRevive = true;
      }
    }

    protected Player RespawnPlayer(int playerIndex)
    {
      List<Vector2> spawnPositions = this.Session.CurrentLevel.GetXMLPositions("PlayerSpawn");

      var player = new Player(playerIndex, new Random().Choose(spawnPositions), Allegiance.Neutral, Allegiance.Neutral,
                      this.Session.GetPlayerInventory(playerIndex), this.Session.GetSpawnHatState(playerIndex),
                      frozen: false, flash: false, indicator: true);
      this.Session.CurrentLevel.Add(player);
      player.Flash(120, null);
      Alarm.Set(player, 60, player.RemoveIndicator, Alarm.AlarmMode.Oneshot);
      return player;
    }

    protected virtual void AfterOnPlayerDeath(Player player, PlayerCorpse corpse)
    {
      if (TFModFortRiseAiPythonModule.Settings.RespawnMode == TFModFortRiseAiPythonSettings.Instant)
        this.RespawnPlayer(player.PlayerIndex);
      else
        Session.CurrentLevel.Add(new TeamReviver(corpse, TeamReviver.Modes.Quest));
    }

    public override void OnPlayerDeath(Player player, PlayerCorpse corpse, int playerIndex, DeathCause cause, Vector2 position, int killerIndex)
    {
      base.OnPlayerDeath(player, corpse, playerIndex, cause, position, killerIndex);

      if (killerIndex == playerIndex || killerIndex == -1)
      {
        killCountHUDs[playerIndex].Decrease();
        base.AddScore(playerIndex, -1);
      }
      else if (killerIndex != -1)
      {
        killCountHUDs[killerIndex].Increase();
        base.AddScore(killerIndex, 1);
      }

      int winner = base.Session.GetWinner();
      if (this.wasFinalKill && winner == -1)
      {
        this.wasFinalKill = false;
        base.Session.CurrentLevel.Ending = false;
        base.CancelFinalKill();
        this.endDelay.Set(90);
      }
      if (!this.wasFinalKill && winner != -1)
      {
        base.Session.CurrentLevel.Ending = true;
        this.wasFinalKill = true;
        base.FinalKill(corpse, winner);
      }

      autoReviveCounters[playerIndex] = 60f;

      this.AfterOnPlayerDeath(player, corpse);
    }
  }
}
