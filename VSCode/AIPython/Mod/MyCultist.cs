using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython {
  public static class MyCultist
  {
    public static StateEntity GetState(this Cultist ent) {
      var aiState = new StateEntity();

      var dynData = DynamicData.For(ent);
      aiState.type = ConversionTypes.CultistTypes.GetB((TowerFall.Cultist.CultistTypes)dynData.Get("type"));
      ExtEntity.SetAiState(ent, aiState);
      dynData.Dispose();
      return aiState;
    }
  }
}
