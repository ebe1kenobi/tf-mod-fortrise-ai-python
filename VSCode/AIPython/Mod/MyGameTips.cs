using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using TowerFall;
using System.Xml.Linq;

namespace TFModFortRiseAiPython
{
  internal class GameTips
  {
    internal static void Load()
    {
      On.TowerFall.GameTips.GetVersusTip += GetVersusTip_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.GameTips.GetVersusTip -= GetVersusTip_patch;
    }

    public static SlideData GetVersusTip_patch(On.TowerFall.GameTips.orig_GetVersusTip orig)
    {
      //orig(self);
      return null;
    }
  }
}
