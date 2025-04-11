using TowerFall;

namespace TFModFortRiseAiPython {
  public static class MyPlayerCorpse
  {
    public static StateEntity GetState(this PlayerCorpse ent) {
      if (ent.PlayerIndex < 0) return null;
      //if (AIPython.Training) {
      //  var state = new StateArcher { type = "archer" };
      //  ExtEntity.SetAiState(ent, state);
      //  state.dead = true;
      //  state.killer = AIPython.agents[ent.PlayerIndex].killer;
      //  state.playerIndex = ent.PlayerIndex;
      //  return state;
      //}
      //else
      //{
        var state = new StatePlayerCorpse { type = "playerCorpse" };
        ExtEntity.SetAiState(ent, state);
        //state.killer = AIPython.agents[ent.PlayerIndex].killer;
        state.killer = ent.KillerIndex;
        state.playerIndex = ent.PlayerIndex;
        return state;
      //}
    }
  }
}
