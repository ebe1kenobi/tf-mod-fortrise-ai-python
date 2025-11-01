using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython
{
  public static class MyPlayer
  {
    internal static void Load()
    {
      On.TowerFall.Player.Die_DeathCause_int_bool_bool += Die_DeathCause_int_bool_bool_patch;
      On.TowerFall.Player.StealArrow_Player += StealArrow_Player_patch;
      On.TowerFall.Player.CatchArrow += CatchArrow_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.Player.Die_DeathCause_int_bool_bool -= Die_DeathCause_int_bool_bool_patch;
      On.TowerFall.Player.StealArrow_Player -= StealArrow_Player_patch;
      On.TowerFall.Player.CatchArrow -= CatchArrow_patch;
    }

    public static PlayerCorpse Die_DeathCause_int_bool_bool_patch(On.TowerFall.Player.orig_Die_DeathCause_int_bool_bool orig, global::TowerFall.Player self, DeathCause deathCause, int killerIndex, bool brambled, bool laser)
    {
      //todo training
      AIPython.agents[self.PlayerIndex].dead = true;
      AIPython.agents[self.PlayerIndex].deathCause = deathCause;
      AIPython.agents[self.PlayerIndex].killer = killerIndex;
      if (AIPython.Training) {
        Logger.Info("player " + self.PlayerIndex + " dead"); 
        AIPython.Update(self.Level);
        AIPython.agents[self.PlayerIndex].Move(); // send last state because Move() will not be called after this point when match end
      }
      return orig(self, deathCause, killerIndex, brambled, laser);
    }

    public static void StealArrow_Player_patch(On.TowerFall.Player.orig_StealArrow_Player orig, global::TowerFall.Player self, global::TowerFall.Player victim) {
      AIPython.agents[self.PlayerIndex].stealArrow = true;
    }

    public static void CatchArrow_patch(On.TowerFall.Player.orig_CatchArrow orig, global::TowerFall.Player self, global::TowerFall.Arrow arrow) {
      AIPython.agents[self.PlayerIndex].catchArrow = true;
    }


    public static StateEntity GetState(this Player ent)
    {
      var aiState = new StateArcher() { type = "archer" };

      ExtEntity.SetAiState(ent, aiState);
      aiState.playerIndex = ent.PlayerIndex;
      aiState.shield = ent.HasShield;
      aiState.wing = ent.HasWings;
      aiState.state = ent.State.ToString().FirstLower();
      aiState.arrows = new List<string>();
      List<ArrowTypes> arrows = ent.Arrows.Arrows;
      for (int i = 0; i < arrows.Count; i++)
      {
        aiState.arrows.Add(arrows[i].ToString().FirstLower());
      }
      aiState.canHurt = ent.CanHurt;
      //aiState.dead = ent.Dead;
      aiState.dead = AIPython.agents[ent.PlayerIndex].dead;
      aiState.killer = AIPython.agents[ent.PlayerIndex].killer;

      aiState.catchArrow = AIPython.agents[ent.PlayerIndex].catchArrow;
      aiState.stealArrow = AIPython.agents[ent.PlayerIndex].stealArrow;
      AIPython.agents[ent.PlayerIndex].catchArrow = false;
      AIPython.agents[ent.PlayerIndex].stealArrow = false;

      aiState.facing = (int)ent.Facing;
      aiState.onGround = ent.OnGround;
      var dynData = DynamicData.For(ent);
      aiState.onWall = dynData.Invoke<bool>("CanWallJump", Facing.Left) || dynData.Invoke<bool>("CanWallJump", Facing.Right);
      Vector2 aim = Calc.AngleToVector(ent.AimDirection, 1);
      aiState.aimDirection = new Vec2
      {
        x = aim.X,
        y = -aim.Y
      };
      aiState.team = AgentConfigExtension.GetTeam(ent.TeamColor); 
      if (PlayTagImport.IsPlayerPlayTag != null)
      {
        aiState.playTagOn = PlayTagImport.IsPlayTagCountDownOn(ent.PlayerIndex);
        aiState.playTag = PlayTagImport.IsPlayerPlayTag(ent.PlayerIndex);
      }
      else
      {
        aiState.playTagOn = false;
        aiState.playTag = false;
      }

      dynData.Dispose();

      return aiState;
    }
  }
}
