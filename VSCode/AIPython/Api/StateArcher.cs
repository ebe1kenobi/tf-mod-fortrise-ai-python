using System.Collections.Generic;

namespace TFModFortRiseAiPython
{
  public class StateArcher : StateEntity {
    public int playerIndex;
    public bool shield;
    public bool wing;
    public List<string> arrows;
    public bool dead;
    public int killer;
    public bool catchArrow;
    public bool stealArrow;
    public bool onGround;
    public bool onWall;
    public Vec2 aimDirection;
    public string team;
    public bool dodgeCooldown;
    public bool playTagOn;
    public bool playTag;
  }
}
