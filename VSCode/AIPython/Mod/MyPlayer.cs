using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython
{
  public static class MyPlayer
  {
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
      aiState.dead = ent.Dead;
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
      //aiState.team = "neutral"; //todo
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
