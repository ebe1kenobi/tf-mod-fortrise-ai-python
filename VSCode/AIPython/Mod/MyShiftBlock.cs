using Microsoft.Xna.Framework;
using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython {
  public static class MyShiftBlock
  {
    public static StateEntity GetState(this ShiftBlock ent) {
      var aiState = new StateShiftBlock { type = Types.ShiftBlock };
      ExtEntity.SetAiState(ent, aiState);
      var dynData = DynamicData.For(ent);
      aiState.startPosition = new Vec2 {
        x = ((Vector2)dynData.Get("startPosition")).X,
        y = ((Vector2)dynData.Get("startPosition")).Y
      };

      aiState.endPosition = new Vec2 {
        x = ((Vector2)dynData.Get("node")).X,
        y = ((Vector2)dynData.Get("node")).Y
      };
      aiState.state = ((ShiftBlock.States)Util.GetPrivateFieldValue("state", ent)).ToString().FirstLower();
      dynData.Dispose();
      return aiState;
    }
  }
}
