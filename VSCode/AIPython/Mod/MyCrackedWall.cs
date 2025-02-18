using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython {
  public static class MyCrackedWall
  {
    public static StateEntity GetState(this CrackedWall ent) {
      var state = new StateCrackedWall { type = Types.CrackedWall };
      ExtEntity.SetAiState(ent, state);
      var dynData = DynamicData.For(ent);
      state.count = (float)dynData.Get("explodeCounter");
      dynData.Dispose();
      return state;
    }
  }
}
