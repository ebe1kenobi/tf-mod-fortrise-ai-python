using System;
using FortRise;
using MonoMod.ModInterop;

//todo on level end -> restart session with same
//send end message-> win
//end round -> continue, bloquer actuellemtn car peut pas faire diff avec fin match ?

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

      MyPauseMenu.Load();
      MyVersusRoundResults.Load();
      MyVersusMatchResults.Load();
      MyMapScene.Load();
      GameTips.Load();
      MyVersusStart.Load();
      MyVersusLevelSystem.Load();
      MyPlayer.Load();

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
      MyPauseMenu.Unload();
      MyVersusRoundResults.Unload();
      MyVersusMatchResults.Unload();
      MyMapScene.Unload();
      GameTips.Unload();
      MyVersusStart.Unload();
      MyVersusLevelSystem.Unload();
      MyPlayer.Unload();

    }

  }
}
