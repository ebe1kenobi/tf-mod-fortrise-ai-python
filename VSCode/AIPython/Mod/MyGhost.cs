using TowerFall;

namespace TFModFortRiseAiPython {
  public static class MyGhost
  {
    public static StateEntity GetState(this Ghost ent) {
      var aiState = new StateSubType { type = "ghost" };
      ExtEntity.SetAiState(ent, aiState);
      aiState.subType = ConversionTypes.GhostTypes.GetB((Ghost.GhostTypes)Util.GetPrivateFieldValue("ghostType", ent));
      return aiState;
    }
  }
}
