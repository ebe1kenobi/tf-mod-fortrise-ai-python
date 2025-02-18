using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython {
  public static class MyFloorMiasma
  {
    public static StateEntity GetState(this FloorMiasma ent) {
      var dynData = DynamicData.For(ent);
      if ((FloorMiasma.States)dynData.Get("state") == FloorMiasma.States.Invisible) return null;

      var aiState = new StateEntity { type = Types.FloorMiasma };
      
      if ((FloorMiasma.States)dynData.Get("state") == FloorMiasma.States.Dangerous) {
        aiState.canHurt = true;
      }

      ExtEntity.SetAiState(ent, aiState);
      dynData.Dispose();
      return aiState;
    }
  }
}
