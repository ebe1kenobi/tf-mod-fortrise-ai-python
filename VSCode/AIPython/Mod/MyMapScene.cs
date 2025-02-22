using TowerFall;

namespace TFModFortRiseAiPython
{
  internal class MyMapScene
  {
    internal static void Load()
    {
      On.TowerFall.MapScene.Update += Update_patch;
      On.TowerFall.MapScene.InitButtons += InitButtons_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.MapScene.Update -= Update_patch;
      On.TowerFall.MapScene.InitButtons -= InitButtons_patch;
    }

    public static void InitButtons_patch(On.TowerFall.MapScene.orig_InitButtons orig, global::TowerFall.MapScene self, global::TowerFall.MapButton startSelection)
    {
      orig(self, startSelection);
      self.Selection = self.Buttons[3];
    }

    public static void Update_patch(On.TowerFall.MapScene.orig_Update orig, global::TowerFall.MapScene self)
    {
      orig(self);
      //SaveData.Instance.Options.ReplayMode = Options.ReplayModes.Off;
      if (AIPython.Training) {
        //self.Selection = self.Buttons[1];
        //self.Selection = self.Buttons[3]; //random -> problem
        self.Selection.Confirmed();
      }
    }
  }
}
