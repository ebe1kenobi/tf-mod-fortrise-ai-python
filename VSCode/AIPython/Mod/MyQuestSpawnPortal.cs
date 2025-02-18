using System.Reflection;
using MonoMod.Utils;
using TowerFall;

namespace TFModFortRiseAiPython {
  public static class MyQuestSpawnPortal
  {
    public static StateEntity GetState(this QuestSpawnPortal ent) {
      var dynData = DynamicData.For(ent);
      bool value = false;
      dynData.TryGet<bool>("appeared", out value);
      //if (!(bool)Util.GetFieldValue("appeared", typeof(QuestSpawnPortal), ent, BindingFlags.Instance | BindingFlags.Public)) {
      if (!value) {
        return null;
      }

      var aiState = new StateEntity {
        type = "portal",
      };

      ExtEntity.SetAiState(ent, aiState);
      return aiState;
    }
  }
}
