using TowerFall;
using System;
//using System.IO;
using System.Net.Sockets;
//using Monocle;
using System.Collections.Generic;
//using System.Threading.Tasks;
using Newtonsoft.Json;
//using System.Runtime.Remoting.Messaging;

namespace TFModFortRiseAiPython
{
  public class AIPythonAgent : TFModFortRiseLoaderAI.Agent
  {
    NetworkStream stream;
    public InputState prevInputState;

    public AIPythonAgent(int index, String type, PlayerInput input, NetworkStream stream) : base(index, type, input)
    {
      this.stream = stream;
    }

    public override void SetLevel(Level level)
    {
      this.level = level;
      shoot.Clear();
    }

    protected override void Move()
    {
      //Logger.Info("Play to agent {0}.".Format(index));
      if (AIPython.serializedStateUpdate == "") return;
      //Logger.Info("Sending serializedStateUpdate to agent {0}.".Format(index) + AIPython.serializedStateUpdate);
      AIPython.Write(AIPython.serializedStateUpdate, stream);
      //Logger.Info("Wait serializedStateUpdate ack to agent {0}.".Format(index));
      Message message = AIPython.Read(stream);
      //Logger.Info("Receive serializedStateUpdate ack to agent {0}.".Format(index));
      UpdateGameInput(message.actions);
    }

    public void SendScenario(Level level, String scenarioMessage)
    {
      Logger.Info("Send scenario to agent. " + index);
      //int frame = 0;
      string initMessage = JsonConvert.SerializeObject(new StateInit { index = index });
      //Logger.Info("Sending stateInit to agent {0}.".Format(index));
      //connection.Send(initMessage, frame);
      AIPython.Write(initMessage, stream);
      //Logger.Info("Wait stateInit ack to agent {0}.".Format(index));

      Message message = AIPython.Read(stream);
      //Logger.Info("Receive stateInit ack to agent {0}.".Format(index));

      //Logger.Info("Sending Scenario to agent {0}.".Format(index));
      AIPython.Write(scenarioMessage, stream);
      //Logger.Info("Wait Scenario ack to agent {0}.".Format(index));
      message = AIPython.Read(stream);
      Logger.Info("Receive Scenario ack to agent {0}.".Format(index));
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
