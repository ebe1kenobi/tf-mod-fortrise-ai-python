﻿using System.Xml;


namespace TFModFortRiseAiPython
{
  public class MyLevel {

    public static int nbUpdate = 0;

    internal static void Load()
    {
      On.TowerFall.Level.Update += Update_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.Level.Update -= Update_patch;
    }


		public static void Update_patch(On.TowerFall.Level.orig_Update orig, global::TowerFall.Level self) {
      nbUpdate++;
      if (!(self.Ending))
      {
        AIPython.update(self);

      }

      orig(self);
    }
  }
}
