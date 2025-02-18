using TowerFall;

namespace TFModFortRiseAiPython {
  public static class MyPlayerCorpse
  {
    public static StateEntity GetState(this PlayerCorpse ent) {
      if (ent.PlayerIndex < 0) return null;
      var state = new StateEntity { type = "playerCorpse" };
      ExtEntity.SetAiState(ent, state);
      return state;
    }
  }
}
