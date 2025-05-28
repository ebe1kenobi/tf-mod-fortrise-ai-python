using FortRise;

namespace TFModFortRiseAiPython
{
  public class TFModFortRiseAiPythonSettings: ModuleSettings
  {
    public const int Instant = 0;
    public const int Delayed = 1;
    [SettingsOptions("Instant", "Delayed")]
    public int RespawnMode;
  }
}
