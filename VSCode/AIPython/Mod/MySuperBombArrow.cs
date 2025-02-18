using System;
using Monocle;
using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython {
  public static class MySuperBombArrow
  {
    public static StateEntity GetState(this SuperBombArrow ent) {
      var aiState = new StateArrow { type = Types.Arrow };

      ExtEntity.SetAiState(ent, aiState);
      aiState.state = ent.State.ToString().FirstLower();
      aiState.arrowType = ent.ArrowType.ToString().FirstLower();
      var dynData = DynamicData.For(ent);
      aiState.timeLeft = (int)Math.Ceiling(((Alarm)dynData.Get("explodeAlarm")).FramesLeft);

      dynData.Dispose();
      return aiState;
    }
  }
}
