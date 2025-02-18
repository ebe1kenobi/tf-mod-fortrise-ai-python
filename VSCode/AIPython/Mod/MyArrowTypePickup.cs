using Monocle;
using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseAiPython {
  public static class MyArrowTypePickup
  {
    public static StateEntity GetState(this Entity ent) {
      var item = ent as ArrowTypePickup;

      var dynData = DynamicData.For(item);

      var state = new StateItem {
        type = Types.Item,
        itemType = "arrow" + dynData.Get("arrowType").ToString()
      };
      ExtEntity.SetAiState(ent, state);
      dynData.Dispose();
      return state;
    }
  }
}
