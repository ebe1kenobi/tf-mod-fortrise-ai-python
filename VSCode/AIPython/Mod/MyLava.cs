using FortRise;
using Microsoft.Xna.Framework;
using TowerFall;


namespace TFModFortRiseAiPython {
  public static class MyLava
  {
    public static StateEntity GetState(this Lava ent) {
      var aiState =  new StateLava { type = Types.Lava };
      ExtEntity.SetAiState(ent, aiState);
      aiState.height = ent.Collider.Top;
      return aiState;
    }
  }
}
