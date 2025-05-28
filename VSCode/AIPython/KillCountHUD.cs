using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using MonoMod.Utils;
using TowerFall;
using Microsoft.Xna.Framework;

namespace TFModFortRiseAiPython
{

  public class KillCountHUD : Entity
  {
    private int playerIndex;
    private List<Sprite<int>> skullIcons = new List<Sprite<int>>();

    public int Count { get { return this.skullIcons.Count; } }

    public KillCountHUD(int playerIndex)
        : base(3)
    {
      this.playerIndex = playerIndex;
    }

    public void Increase()
    {
      Sprite<int> sprite = DeathSkull.GetSprite();
      sprite.Color = ArcherData.GetColorA(playerIndex);

      var width = EightPlayerUtils.GetScreenWidth();

      if (this.playerIndex % 2 == 0)
        sprite.X = 8 + 10 * skullIcons.Count;
      else
        sprite.X = width - 8 - 10 * skullIcons.Count;
      float offset = 0;
      if (playerIndex > 4)
        offset = 20;

      sprite.Y = this.playerIndex % 2 == 0 ? 20 + offset : (240 - 20) - offset;
      sprite.Stop();
      this.skullIcons.Add(sprite);
      base.Add(sprite);
    }

    public void Decrease()
    {
      if (this.skullIcons.Any())
      {
        base.Remove(this.skullIcons.Last());
        this.skullIcons.Remove(this.skullIcons.Last());
      }
    }

    public override void Render()
    {
      foreach (Sprite<int> sprite in this.skullIcons)
      {
        sprite.DrawOutline(1);
      }
      base.Render();
    }
  }
}
