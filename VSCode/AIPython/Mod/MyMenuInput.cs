//using System;
//using TowerFall;


//namespace TFModFortRiseAiPython
//{
//  public static class MyMenuInput
//  {
//    internal static void Load()
//    {
//      On.TowerFall.MenuInput.cctor += cctor_patch;
//    }

//    internal static void Unload()
//    {
//      On.TowerFall.MenuInput.cctor -= cctor_patch;
//    }

//    static void cctor_patch(On.TowerFall.MenuInput.orig_cctor orig) {
//    }

//    public static bool Confirm { //TODO
//      get {
//        // Makes the bot automatically confirm all menus.
//        if (AIPython.Training && !AIPython.IsHumanPlaying()) return true;
//        var ptr = typeof(MenuInput).GetMethod("$original_get_Confirm").MethodHandle.GetFunctionPointer();
//        var orginalGetConfirm = (Func<bool>)Activator.CreateInstance(typeof(Func<bool>), null, ptr);
//        return orginalGetConfirm();
//      }
//    }

//    public static bool ConfirmOrStart //TODO
//    {
//      get {
//        // Makes the bot automatically confirm all menus.
//        if (AIPython.Training && !AIPython.IsHumanPlaying()) return true;
//        var ptr = typeof(MenuInput).GetMethod("$original_get_ConfirmOrStart").MethodHandle.GetFunctionPointer();
//        var orginalGetConfirm = (Func<bool>)Activator.CreateInstance(typeof(Func<bool>), null, ptr);
//        return orginalGetConfirm();
//      }
//    }
//    public static bool ReplaySkip //TODO
//    {
//      get
//      {
//        // Makes the bot automatically confirm all menus.
//        if (AIPython.Training && !AIPython.IsHumanPlaying()) return true;
//        var ptr = typeof(MenuInput).GetMethod("$original_get_ReplaySkip").MethodHandle.GetFunctionPointer();
//        var orginalGetReplaySkip = (Func<bool>)Activator.CreateInstance(typeof(Func<bool>), null, ptr);
//        return orginalGetReplaySkip();
//      }
//    }
//  }
//}
