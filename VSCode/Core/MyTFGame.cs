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
      On.TowerFall.TFGame.Draw += Draw_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.TFGame.Update -= Update_patch;
      On.TowerFall.TFGame.Load -= Load_patch;
      On.TowerFall.TFGame.Draw -= Draw_patch;
    }

    public static void Load_patch(On.TowerFall.TFGame.orig_Load orig)
    {
      orig();
      TaskHelper.Run("WAITING FOR THE AI PYTHON TO CONNECT", () =>
      {
        try
        {
          Task.Run(() => AIPython.StartServer());
          Logger.Info("Server is running...");

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
      orig(self, gameTime);
      if (LoaderAIImport.CanAddAgent() && AIPython.isAgentReady && !agentAdded)
      {
        LoaderAIImport.addAgent(AIPython.AINAME, AIPython.agents, AIPython.Training);
        agentAdded = true;
      }

      if (agentAdded == true) {
        if (AIPython.Training)
          AIPython.PreUpdate();
      }
    }

    public static void Draw_patch(On.TowerFall.TFGame.orig_Draw orig, global::TowerFall.TFGame self, GameTime gameTime)
    {
      //if (AIPython.Training && (!AIPython.IsMatchRunning() || AIPython.NoGraphics))
      //{
      //  Monocle.Engine.Instance.GraphicsDevice.SetRenderTarget(null);
      //  return;
      //}

      orig(self, gameTime);

      //if (AIPython.Training)
      //{
      //  Monocle.Draw.SpriteBatch.Begin();
      //  Agents.Draw();
      //  Monocle.Draw.SpriteBatch.End();
      //}
    }
  }
}
