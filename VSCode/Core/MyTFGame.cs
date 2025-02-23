using System.Threading;
using Monocle;
using System;
using FortRise;
//using Microsoft.Xna.Framework;
using TowerFall;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TFModFortRiseLoaderAI;
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

    private static void SetPlayerControl() {
      for (var i = TFGame.Players.Length - 1; i >= 0; i--)
      {
        //disable keyboard
        if (TFGame.PlayerInputs[i] == null) continue;
        //Logger.Info(TFGame.PlayerInputs[i].GetType().ToString());


        if (TFGame.PlayerInputs[i].GetType().ToString() != "TowerFall.KeyboardInput"
            && TFGame.PlayerInputs[i].GetType().ToString() != "TowerFall.XGamepadInput")
        {
          continue;
        }

        if (i + 1 > AIPython.reconfigOperation.Config.nbHuman)
        {
          TFModFortRiseLoaderAIModule.currentPlayerType[i] = "NONE";
          TFModFortRiseLoaderAIModule.nbPlayerType[i] = 0;
          TFGame.PlayerInputs[i] = null;
          continue;
        }
      }

      for (int i = 0; i < AIPython.nbRemoteAgentConnected; i++)
      {
        var agent = AIPython.reconfigOperation.Config.agents[i];

        TFGame.Players[i] = true;
        TFGame.Characters[i] = agent.GetArcherIndex();
        TFGame.AltSelect[i] = agent.GetArcherType();
      }
    }

    public static void Update_patch(On.TowerFall.TFGame.orig_Update orig, global::TowerFall.TFGame self, GameTime gameTime)
    {
      orig(self, gameTime);
      if (LoaderAIImport.CanAddAgent() && AIPython.isAgentReady && !agentAdded)
      {
        //keeps only the human controller needed and delete the other for the ia input 
        if (AIPython.Training)
        {
          SetPlayerControl();
        }

        LoaderAIImport.addAgent(AIPython.AINAME, AIPython.agents, false);
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
