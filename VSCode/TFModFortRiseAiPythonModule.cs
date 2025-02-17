using System;
using FortRise;
using MonoMod.ModInterop;
using TFModFortRiseLoaderAI;

namespace TFModFortRiseAiPython
{
  [Fort("com.ebe1.kenobi.TFModFortRiseAiPython", "TFModFortRiseAiPython")]
  public class TFModFortRiseAiPythonModule : FortModule
  {
    public static TFModFortRiseAiPythonModule Instance;

    public override Type SettingsType => typeof(TFModFortRiseAiPythonSettings);
    public static TFModFortRiseAiPythonSettings Settings => (TFModFortRiseAiPythonSettings)Instance.InternalSettings;

    public TFModFortRiseAiPythonModule()
    {
      Instance = this;
      Logger.Init("TFModFortRiseAiPythonLOG");
    }

    public override void LoadContent()
    {
    }

    public override void Load()
    {
      MyTFGame.Load();
      MySession.Load();
      MyLevel.Load();
      typeof(LoaderAIImport).ModInterop();
      typeof(EigthPlayerImport).ModInterop();
      typeof(PlayTagImport).ModInterop();
    }

    public override void Unload()
    {
      MyTFGame.Unload();
      MySession.Unload();
      MyLevel.Unload();
    }
  }

  [ModImportName("com.fortrise.TFModFortRiseLoaderAI")]
  public static class LoaderAIImport
  {
    public static Func<String, Agent[], bool> addAgent;
  }
}
