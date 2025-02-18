using Microsoft.Xna.Framework;
using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython {
  public static class MySpikeball
  {
    public static StateEntity GetState(this Spikeball ent) {
      var aiState = new StateSpikeBall { type = Types.SpikeBall };

      ExtEntity.SetAiState(ent, aiState);
      var dynData = DynamicData.For(ent);
      aiState.center = new Vec2 {
        x = ((Vector2)dynData.Get("pivot")).X,
        y = ((Vector2)dynData.Get("pivot")).Y
      };

      aiState.radius = ((float)dynData.Get("radius"));
      dynData.Dispose();

      return aiState;
    }
  }
}
