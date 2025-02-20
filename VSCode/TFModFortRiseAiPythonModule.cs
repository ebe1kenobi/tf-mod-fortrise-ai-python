using System;
using FortRise;
using MonoMod.ModInterop;
using TFModFortRiseAIModule;

namespace TFModFortRiseAiPython
{
  [Fort("com.ebe1.kenobi.TFModFortRiseAiPython", "TFModFortRiseAiPython")]
  public class TFModFortRiseAiPythonModule : FortModule
  {
    public static TFModFortRiseAiPythonModule Instance;
    public static bool EightPlayerMod;
    public static bool PlayTagMod;

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
      MyPlayerIndicator.Load();
      
      typeof(LoaderAIImport).ModInterop();
      typeof(EigthPlayerImport).ModInterop();
      typeof(PlayTagImport).ModInterop();

      EightPlayerMod = IsModExists("WiderSetMod");
      PlayTagMod = IsModExists("PlayTag");
    }

    public override void Unload()
    {
      MyTFGame.Unload();
      MySession.Unload();
      MyLevel.Unload();
      MyPlayerIndicator.Unload();
    }
  }
}
