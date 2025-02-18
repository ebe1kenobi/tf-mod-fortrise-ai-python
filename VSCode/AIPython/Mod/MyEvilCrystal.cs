using TowerFall;

namespace TFModFortRiseAiPython {
  public static class MyEvilCrystal {
    public static StateEntity GetState(this EvilCrystal ent) {
      var aiState = new StateSubType { type = "evilCrystal" };
      ExtEntity.SetAiState(ent, aiState);

      aiState.subType = ConversionTypes.CrystalTypes.GetB((EvilCrystal.CrystalColors)Util.GetPrivateFieldValue("crystalColor", ent));
      return aiState;
    }
  }
}
