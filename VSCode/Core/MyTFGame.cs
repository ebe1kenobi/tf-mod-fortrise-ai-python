using System.Threading;
using Monocle;
using System;
using FortRise;
//using Microsoft.Xna.Framework;
using TowerFall;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
//using System.Net.Sockets;
//using System.Net;
//using System.Text;
//using System.IO;
//using System.Diagnostics;
//using Newtonsoft.Json;
//using TFModFortRiseLoaderAI;
//using System.Runtime.InteropServices.ComTypes;

namespace TFModFortRiseAiPython
{
  public class MyTFGame {

    public static bool agentAdded = false;
    internal static void Load()
    {
      On.TowerFall.TFGame.Update += Update_patch;
      On.TowerFall.TFGame.Load += Load_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.TFGame.Update -= Update_patch;
      On.TowerFall.TFGame.Load -= Load_patch;
    }

    public static void Load_patch(On.TowerFall.TFGame.orig_Load orig)
    {
      orig();
      TaskHelper.Run("WAITING FOR THE AI PYTHON TO CONNECT", () =>
      {
        try
        {
          Task.Run(() => AIPython.StartServer());
          Logger.Info("Server is running... Press Enter to exit.");

          //TODO
          while (AIPython.nbRemoteAgentWaited == 0 || AIPython.nbRemoteAgentWaited != AIPython.nbRemoteAgentConnected)
          {
            Thread.Sleep(1000);
          }
        }
        catch (Exception ex)
        {
          TFGame.Log(ex, true);
          TFGame.OpenLog();
          Engine.Instance.Exit();
        }
      });
    }

    public static void Update_patch(On.TowerFall.TFGame.orig_Update orig, global::TowerFall.TFGame self, GameTime gameTime)
    {
      //AIPython.CreateAgent();
      orig(self, gameTime);
      if (LoaderAIImport.CanAddAgent() && AIPython.isAgentReady && !agentAdded)
      {
        LoaderAIImport.addAgent(AIPython.AINAME, AIPython.agents);
        agentAdded = true;
      }
    }
  }
}
