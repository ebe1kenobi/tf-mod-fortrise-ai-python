using TowerFall;

namespace TFModFortRiseAiPython {
  public static class MyDefaultHat
  {
    public static StateEntity GetState(this DefaultHat ent) {
      var aiState = new StateHat { type = Types.Hat };
     
      aiState.playerIndex = ent.PlayerIndex;
      ExtEntity.SetAiState(ent, aiState);
      return aiState;
    }
  }
}
