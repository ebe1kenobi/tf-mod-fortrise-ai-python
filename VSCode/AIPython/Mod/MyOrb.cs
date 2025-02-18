using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython {
  public static class MyOrb
  {
    public static StateEntity GetState(this Orb ent) {
      var aiState = new StateFalling { type = Types.Orb };
      ExtEntity.SetAiState(ent, aiState);
      var dynData = DynamicData.For(ent);
      aiState.falling = (bool)dynData.Get("falling");
      dynData.Dispose();
      return aiState;
    }
  }
}
