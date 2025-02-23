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
  internal class MyVersusRoundResults
  {
    internal static void Load()
    {
      On.TowerFall.VersusRoundResults.Update += Update_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.VersusRoundResults.Update -= Update_patch;
    }

    public static void Update_patch(On.TowerFall.VersusRoundResults.orig_Update orig, global::TowerFall.VersusRoundResults self)
    {

      //SaveData.Instance.Options.ReplayMode = Options.ReplayModes.UseCPU;
      orig(self);
      //self.TweenOut();
      //self.MatchResults.TweenIn();
      var dynData = DynamicData.For(self);
      var dynData2 = DynamicData.For(dynData.Get<Session>("session"));
      //session.EndlessContinue todo
      //TODO endless match
      //dynData2.Invoke("GotoNextRound"); // continue even if match end , round with no player... or overtime
      //SaveData.Instance.Options.ReplayMode = Options.ReplayModes.Off;

    }
  }
}
