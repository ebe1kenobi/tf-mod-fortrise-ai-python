using IL.MonoMod;
using Microsoft.Xna.Framework;
using Monocle;
using Newtonsoft.Json.Linq;

namespace TFModFortRiseAiPython
{
  public class MyLevel {

    public static int nbUpdate = 0;
    public static bool sandboxEntityCreated = false;

    internal static void Load()
    {
      On.TowerFall.Level.Update += Update_patch;
      On.TowerFall.Level.HandlePausing += HandlePausing_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.Level.Update -= Update_patch;
      On.TowerFall.Level.HandlePausing -= HandlePausing_patch;
    }


		public static void Update_patch(On.TowerFall.Level.orig_Update orig, global::TowerFall.Level self) {
      nbUpdate++;
      if (AIPython.Config.mode == GameModes.Sandbox && !sandboxEntityCreated)
      {
        //int playerIndex = 0;

        //TODO create the number of player from the config!

        //var player = EntityCreator.CreatePlayer(e, playerIndex, self.Session.MatchSettings.GetPlayerAllegiance(playerIndex));
        for (int i = 0; i < AIPython.Config.agents.Count; i++) {
          var player1 = EntityCreator.CreatePlayer(i, self.Session.MatchSettings.GetPlayerAllegiance(i), AIPython.Config.agents[i].X, AIPython.Config.agents[i].Y);
          self.Add(player1);
        }

        //var player1 = EntityCreator.CreatePlayer(playerIndex, self.Session.MatchSettings.GetPlayerAllegiance(playerIndex), AIPython.Config.agents[playerIndex]X, AIPython.Config.agents[playerIndex].Y);
        //self.Add(player1);
        //playerIndex++;
        //var player2 = EntityCreator.CreatePlayer(playerIndex, self.Session.MatchSettings.GetPlayerAllegiance(playerIndex));
        //self.Add(player2);
        //playerIndex++;
        //var player3 = EntityCreator.CreatePlayer(playerIndex, self.Session.MatchSettings.GetPlayerAllegiance(playerIndex));
        //self.Add(player3);
        //playerIndex++;
        //var player4 = EntityCreator.CreatePlayer(playerIndex, self.Session.MatchSettings.GetPlayerAllegiance(playerIndex));
        //self.Add(player4);
        //playerIndex++;


        //Entity entity = EntityCreator.CreateSlime(new JObject());
        //self.Add(entity);

        self.UpdateEntityLists();
        MyLevel.sandboxEntityCreated = true;
      }


      orig(self);
      if (AIPython.Training && AIPython.Config.speed > 1)
      {
        Engine.TimeRate = AIPython.Config.speed;
      }
      if (!(self.Ending))
      {
        AIPython.Update(self);
      }
    }

    public static void HandlePausing_patch(On.TowerFall.Level.orig_HandlePausing orig, global::TowerFall.Level self)
    {
      // Avoid pausing when no human is playing and the screen goes out of focus.
      if (AIPython.Training)
      //if (AIPython.Training && !AIPython.IsHumanPlaying())
      {
        return; //todo trining
      }

      orig(self);
    }
  }
}
