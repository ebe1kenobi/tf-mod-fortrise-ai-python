using FortRise;
using Microsoft.Xna.Framework;
using TowerFall;

namespace TFModFortRiseAiPython
{
  public class Respawn : CustomGameMode
  {
    public override void StartGame(Session session)
    {
    }

    public override RoundLogic CreateRoundLogic(Session session)
    {
      return new RespawnRoundLogic(session);
    }

    public override void Initialize()
    {
      Icon = TFGame.MenuAtlas["gameModes/warlord"];
      //Icon = TFModFortRiseGameModeRespawnModule.RespawnAtlas["gamemodes/respawn"];
      NameColor = Color.LightPink;
      CoinOffset = 12;
    }

    public override void InitializeSounds() { }
  }
}
