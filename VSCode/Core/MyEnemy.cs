using TowerFall;
using System.Reflection;
using MonoMod.Utils;

namespace TFModFortRiseAiPython
{
  public static class ExtEnemy {
    public static bool IsDead(this Enemy e) {
      var dynData = DynamicData.For(e);
      bool value = false;
      dynData.TryGet<bool>("dead", out value);
      //return (bool)Util.GetFieldValue("dead", typeof(Enemy), e, BindingFlags.Instance | BindingFlags.Public);
      return value;
    }
  }
}
