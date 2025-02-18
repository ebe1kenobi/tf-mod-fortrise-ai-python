using MonoMod.Utils;
using TowerFall;
namespace TFModFortRiseAiPython {
  public static class MyTreasureChest {
    public static StateEntity GetState(this TreasureChest ent) {
      if (ent.State == TreasureChest.States.WaitingToAppear) return null;

      var aiState = new StateChest { type = Types.Chest };
      ExtEntity.SetAiState(ent, aiState);
      aiState.state = ent.State.ToString().FirstLower();

      var dynData = DynamicData.For(ent);
      aiState.chestType = dynData.Get("type").ToString().FirstLower();

      dynData.Dispose();
      return aiState;
    }
  }
}
