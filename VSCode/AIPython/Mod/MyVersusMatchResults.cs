using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
//using IL.Monocle;
using MonoMod.Utils;
using TowerFall;
using Microsoft.Xna.Framework;

namespace TFModFortRiseAiPython
{
  internal class MyVersusMatchResults
  {
    //private List<AwardInfo> awards;
    //VersusAwards.GetAwards(session.MatchSettings, session.MatchStats);
    //List<AwardInfo>[] result = VersusAwards.playerAwards;

    internal static void Load()
    {
      On.TowerFall.VersusMatchResults.ctor += ctor_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.VersusMatchResults.ctor -= ctor_patch;
    }

    public static void ctor_patch(On.TowerFall.VersusMatchResults.orig_ctor orig, global::TowerFall.VersusMatchResults self, global::TowerFall.Session session, global::TowerFall.VersusRoundResults roundResults)
    {
      //var dynData = DynamicData.For(self);
      //dynData.Set("finished", true);
      orig(self, session, roundResults);
      // on ne voit plus le resultat des match!

      //TODO endless match
      //session.CurrentLevel.Add(new PauseMenu(session.CurrentLevel, new Vector2(160f, 200f), PauseMenu.MenuType.VersusMatchEnd));
      //self.TweenIn();


      //((KeyboardInput)TFGame.PlayerInputs[0]).MenuLeft = true;
      //MInput.Keyboard.Pressed(((KeyboardInput)TFGame.PlayerInputs[1]).Config.Jump);
      //PuaseMenu
      //this.AddItem("REMATCH!", new Action(this.VersusRematch));
    }
  }
}
