using Monocle;
using TowerFall;
using TowerFall.Editor;
using System;
//using System.IO;
using System.Net.Sockets;
//using Monocle;
using System.Collections.Generic;
//using System.Threading.Tasks;
using Newtonsoft.Json;
using TeuJson.Attributes;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
//using System.Runtime.Remoting.Messaging;
using Microsoft.Xna.Framework;

namespace TFModFortRiseAiPython
{
  public class AIPythonAgent : TFModFortRiseLoaderAI.Agent
  {
    NetworkStream stream;
    public InputState prevInputState;
    public bool dead = false;
    public DeathCause deathCause;
    
    public int killer = -1;
    public bool stealArrow = false;
    public bool catchArrow = false;
    public AIPythonAgent(int index, String type, PlayerInput input, NetworkStream stream) : base(index, type, input)
    {
      this.stream = stream;
    }

    public override void SetLevel(TowerFall.Level level)
    {
      this.level = level;
      this.dead = false;
      this.killer = -1;
      this.stealArrow = false;
      this.catchArrow = false;
      shoot.Clear();
    }

    public override void Move()
    {
      //lock (AIPython._lockSend)
      {
        if (AIPython.serializedStateUpdate == "") return;
        //save screen image
        //SavePreviewPng();  //TODO move at the end of the game 
        AIPython.Write(AIPython.serializedStateUpdate, stream);
        Message message = AIPython.Read(stream);
        if (message.rematch) { // Yes I know, pretty ugly :)
          AIPython.Rematch = true;
        }
        UpdateGameInput(message.actions);
      }
    }

    public void SavePreviewPng()
    {
      // Créer une nouvelle texture 2D pour stocker la capture
      RenderTarget2D screenCapture = new RenderTarget2D(
          Engine.Instance.GraphicsDevice,
          320, // largeur du jeu
          240  // hauteur du jeu
      );

      // Copier le contenu de l'écran dans la nouvelle texture
      Engine.Instance.GraphicsDevice.SetRenderTarget(screenCapture);
      Draw.SpriteBatch.Begin();
      Draw.SpriteBatch.Draw(Engine.Instance.Screen.RenderTarget, Vector2.Zero, Color.White);
      Draw.SpriteBatch.End();
      Engine.Instance.GraphicsDevice.SetRenderTarget(null);

      // Sauvegarder la texture en fichier PNG
      using (FileStream stream = File.Create("screenshot" + AIPython.frame + ".png"))
      {
        screenCapture.SaveAsPng(stream, screenCapture.Width, screenCapture.Height);
      }

      // Libérer la ressource
      screenCapture.Dispose();
    }

    public void SendScenario(TowerFall.Level level, String scenarioMessage)
    {
      //int frame = 0;
      string initMessage = JsonConvert.SerializeObject(new StateInit { index = index });
      AIPython.Write(initMessage, stream);
      Message message = AIPython.Read(stream);
      AIPython.Write(scenarioMessage, stream);
      message = AIPython.Read(stream);
    }

    protected InputState GetCopy(InputState inputState)
    {
      return new InputState
      {
        AimAxis = inputState.AimAxis,
        ArrowsPressed = inputState.ArrowsPressed,
        DodgeCheck = inputState.DodgeCheck,
        DodgePressed = inputState.DodgePressed,
        JumpCheck = inputState.JumpCheck,
        JumpPressed = inputState.JumpPressed,
        MoveX = inputState.MoveX,
        MoveY = inputState.MoveY,
        ShootCheck = inputState.ShootCheck,
        ShootPressed = inputState.ShootPressed
      };
    }

    protected void ProcessResponse(string response)
    {
      if (response == null || response.Length == 0) return;

      this.input.inputState.AimAxis.X = 0;
      this.input.inputState.MoveX = 0;
      this.input.inputState.AimAxis.Y = 0;
      this.input.inputState.MoveY = 0;

      var responseChars = new HashSet<char>();
      foreach (char c in response)
      {
        responseChars.Add(c);
      }

      foreach (char c in responseChars)
      {
        ProcessAction(c);
      }
    }

    protected void ProcessAction(char c)
    {
      switch (c)
      {
        case 'j':
          this.input.inputState.JumpCheck = true;
          this.input.inputState.JumpPressed = !this.prevInputState.JumpCheck;
          break;
        case 's':
          this.input.inputState.ShootCheck = true;
          this.input.inputState.ShootPressed = !this.prevInputState.ShootCheck;
          break;
        case 'z':
          this.input.inputState.DodgeCheck = true;
          this.input.inputState.DodgePressed = !this.prevInputState.DodgeCheck;
          break;
        case 'a':
          this.input.inputState.ArrowsPressed = true;
          break;
        case 'u':
          this.input.inputState.AimAxis.Y = -1;
          this.input.inputState.MoveY = -1;
          break;
        case 'd':
          this.input.inputState.AimAxis.Y += 1;
          this.input.inputState.MoveY += 1;
          break;
        case 'l':
          this.input.inputState.AimAxis.X -= 1;
          this.input.inputState.MoveX -= 1;
          break;
        case 'r':
          this.input.inputState.MoveX += 1;
          this.input.inputState.AimAxis.X += 1;
          break;
      }
    }

    public void UpdateGameInput(string response)
    {
      this.input.inputState = new InputState();
      ProcessResponse(response);
      this.prevInputState = GetCopy(this.input.inputState);
    }
  }
}
