using System;
using TowerFall;
using Monocle;
using MonoMod.Utils;

namespace TFModFortRiseAiPython {
  public static class MyBombArrow
  {
    public static StateEntity GetState(this BombArrow ent) {
      var state = (StateArrow)ExtEntity.GetStateArrow(ent);

      var dynData = DynamicData.For(ent);
      state.timeLeft = (float)Math.Ceiling(((Alarm)dynData.Get("explodeAlarm")).FramesLeft);
      dynData.Dispose();
      return state;
    }
  }
}
