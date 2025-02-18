using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython {
  public static class MyCrackedPlatform
  {
    public static StateEntity GetState(this CrackedPlatform ent) {
      var state = new StateEntity { type = Types.CrackedPlatform };
      ExtEntity.SetAiState(ent, state);
      var dynData = DynamicData.For(ent);
      state.state = dynData.Get("state").ToString().FirstLower();
      dynData.Dispose();
      return state;
    }
  }
}
