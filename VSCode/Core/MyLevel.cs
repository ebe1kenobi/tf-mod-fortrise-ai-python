using System.Xml;


namespace TFModFortRiseAiPython
{
  public class MyLevel {

    public static int nbUpdate = 0;

    internal static void Load()
    {
      On.TowerFall.Level.Update += Update_patch;
      On.TowerFall.Level.HandlePausing += HandlePausing_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.Level.Update -= Update_patch;
      On.TowerFall.Level.HandlePausing -= HandlePausing_patch;
    }


		public static void Update_patch(On.TowerFall.Level.orig_Update orig, global::TowerFall.Level self) {
      nbUpdate++;
      if (!(self.Ending))
      {
        AIPython.Update(self);

      }

      orig(self);
    }

    public static void HandlePausing_patch(On.TowerFall.Level.orig_HandlePausing orig, global::TowerFall.Level self)
    {
      // Avoid pausing when no human is playing and the screen goes out of focus.
      if (AIPython.Training)
      //if (AIPython.Training && !AIPython.IsHumanPlaying())
      {
        return;
      }

      orig(self);
    }
  }
}
