
namespace TFModFortRiseAiPython
{
  public class StateInit : State {
    public StateInit() {
      type = "init";
      version = "1";
    }

    // The version of the mod.
    public string version;

    // The index of the player
    public int index;
  }
}
