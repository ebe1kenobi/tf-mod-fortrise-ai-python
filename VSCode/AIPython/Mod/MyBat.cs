using MonoMod.Utils;
using TowerFall;
using static TowerFall.Bat;

namespace TFModFortRiseAiPython {
  public static class MyBat
  {
    public static StateEntity GetState(this Bat ent) {
      var aiState = new StateEntity();
      var dynData = DynamicData.For(ent);

      aiState.type = ConversionTypes.BatTypes.GetB((BatType)dynData.Get("batType"));
      ExtEntity.SetAiState(ent, aiState);
      dynData.Dispose();
      return aiState;
    }
  }
}
