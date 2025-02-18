using FortRise;
using Microsoft.Xna.Framework;
using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython {
  public static class MyOrbPickup
  {
    public static StateEntity GetState(this OrbPickup ent) {
      var aiState = new StateItem { type = Types.Item };
      ExtEntity.SetAiState(ent, aiState);
      var dynData = DynamicData.For(ent);
      aiState.itemType = "orb" + dynData.Get("orbType").ToString();
      dynData.Dispose();
      return aiState;
    }
  }
}
