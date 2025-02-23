using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerFall;
using Monocle;
using MonoMod.Utils;
using Microsoft.Xna.Framework;

namespace TFModFortRiseAiPython
{
  internal class MyPauseMenu
  {
    internal static void Load()
    {
      On.TowerFall.PauseMenu.VersusRematch += VersusRematch_patch;
      On.TowerFall.PauseMenu.Update += Update_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.PauseMenu.VersusRematch -= VersusRematch_patch;
      On.TowerFall.PauseMenu.Update -= Update_patch;
    }

    public static void VersusRematch_patch(On.TowerFall.PauseMenu.orig_VersusRematch orig, global::TowerFall.PauseMenu self)
    {
      //TODO endless match
      //TODO Update_patch + VersusRematch_patch => restart match
      //if (AIPython.Training) AIPython.Rematch = true;
      orig(self);
    }

    public static void Update_patch(On.TowerFall.PauseMenu.orig_Update orig, global::TowerFall.PauseMenu self)
    {
      orig(self);
      //TODO endless match
      //var dynData = DynamicData.For(self);
      //dynData.Invoke("VersusRematch");
    }
  }
}
