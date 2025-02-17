namespace TFModFortRiseAiPython
{
  public class MySession {

    internal static void Load()
    {
      On.TowerFall.Session.OnLevelLoadFinish += OnLevelLoadFinish_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.Session.OnLevelLoadFinish -= OnLevelLoadFinish_patch;
    }

    public MySession() { }

    public static void OnLevelLoadFinish_patch(On.TowerFall.Session.orig_OnLevelLoadFinish orig, global::TowerFall.Session self) {
      orig(self);
      Logger.Info("OnLevelLoadFinish_patch");
      AIPython.NotifyLevelLoad(self.CurrentLevel);
    }
  }
}
