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
using System.Collections;
using System.Runtime.Remoting.Messaging;

namespace TFModFortRiseAiPython
{
  internal class MyVersusStart
  {
    internal static void Load()
    {
      //On.TowerFall.VersusStart.Render += Render_patch;
      On.TowerFall.VersusStart.SessionIntroSequence += SessionIntroSequence_patch;
      //On.TowerFall.VersusStart.IntroSequence += IntroSequence_patch;
      
    }

    internal static void Unload()
    {
      //On.TowerFall.VersusStart.Render -= Render_patch;
      On.TowerFall.VersusStart.SessionIntroSequence -= SessionIntroSequence_patch;
      //On.TowerFall.VersusStart.IntroSequence -= IntroSequence_patch;
    }

    //public static void Render_patch(On.TowerFall.VersusStart.orig_Render orig, global::TowerFall.VersusStart self)
    //{
    //  //TFGame.PlayerInputs[0].MenuConfirm = true;
    //  //var dynData = DynamicData.For(TFGame.PlayerInputs[0]);
    //  //dynData.Set("MenuConfirm", true);
    //  //dynData.Dispose();
    //  orig(self);

    //}

    //todo TEST for 
    //System.NullReferenceException: Object reference not set to an instance of an object.
    //at TowerFall.VersusStart.<IntroSequence>d__15.MoveNext()
    //at Monocle.Coroutine.Update()
    //at Monocle.Entity.Update()
    //at TowerFall.HUD.Update()
    //at Monocle.Layer.Update()
    //at Monocle.Scene.Update()
    public static IEnumerator SessionIntroSequence_patch(On.TowerFall.VersusStart.orig_SessionIntroSequence orig, global::TowerFall.VersusStart self)
    {
      //TFGame.PlayerInputs[0].MenuConfirm = true;
      //var dynData = DynamicData.For(TFGame.PlayerInputs[0]);
      //dynData.Set("MenuConfirm", true);
      //dynData.Dispose();
      //orig(self);
      //yield return orig(self);
      //yield return orig(self);
      //yield return orig(self);
      //yield return orig(self);
      yield break;
    }

    //public static IEnumerator IntroSequence_patch(On.TowerFall.VersusStart.orig_IntroSequence orig, global::TowerFall.VersusStart self)
    //{
    //  //TFGame.PlayerInputs[0].MenuConfirm = true;
    //  var dynData = DynamicData.For(self);
    //  dynData.Invoke("StartRound");
    //  self.RemoveSelf();
    //  dynData.Dispose();
    //  //orig(self);
    //  //self.session.StartRound();
    //  yield break;
    //}
  }
}
