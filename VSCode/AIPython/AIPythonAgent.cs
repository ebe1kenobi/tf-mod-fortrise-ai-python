using TowerFall;
using System;
using System.IO;
using System.Net.Sockets;
using Monocle;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Remoting.Messaging;

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
      //sendScenario(level);

    }

    public override void Play()
    {
      //Logger.Info("Play to agent {0}.".Format(index));

      if (level.Paused) return;
      if (level.Frozen) return;
      if (level.Ending) return;

      if (AIPython.serializedStateUpdate == "") return;
      //Logger.Info("Sending serializedStateUpdate to agent {0}.".Format(index) + AIPython.serializedStateUpdate);
      MyTFGame.Write(AIPython.serializedStateUpdate, stream);
      //Logger.Info("Wait serializedStateUpdate ack to agent {0}.".Format(index));
      Message message = MyTFGame.Read(stream);
      //Logger.Info("Receive serializedStateUpdate ack to agent {0}.".Format(index));
      UpdateGameInput(message.actions);
      //Agents.RefreshInputFromAgents(self);

      //if (!scenarioSent)
      //{

      //  SendScenario(level);
      //}

      //RefreshStateUpdate(level);


      //this.input.inputState = new InputState();
      //this.input.inputState.AimAxis.X = 0;
      //this.input.inputState.MoveX = 0;
      //this.input.inputState.AimAxis.Y = 0;
      //this.input.inputState.MoveY = 0;

      //if (shoot.Count == 0 && 0 == random.Next(0, 19))
      ////if (0 == random.Next(0, 19))
      //{
      //  this.input.inputState.JumpCheck = true;
      //  this.input.inputState.JumpPressed = !this.input.prevInputState.JumpCheck;
      //}

      //this.input.prevInputState = this.input.GetCopy(this.input.inputState);

    }

    public void sendScenario(Level level, String scenarioMessage)
    {
      
      Logger.Info("Send scenario to agent. " + index);

      //List<Task> tasks = new List<Task>();

      //List<Entity> listPlayer = level.Session.CurrentLevel[GameTags.Player];
      //listPlayerAIIndexPlaying.Clear();
      //for (var i = 0; i < listPlayer.Count; i++)
      //{
      //  // save player at the first frame, because when player died, the player will change in session , died player disappear
      //  if (TFModFortRiseAIModule.currentPlayerType[((Player)listPlayer[i]).PlayerIndex] == PlayerType.Human) continue;

      //  listPlayerAIIndexPlaying.Add(((Player)listPlayer[i]).PlayerIndex);
      //}

      // Send all state inits.
      //for (int i = 0; i < AgentConnections.Count; i++)
      //{
      //var connection = AgentConnections[i];
      //if (connection == null) continue;

      //if (!listPlayerAIIndexPlaying.Contains(i))
      //{
      //  continue;
      //}
      //else
      //{
      int frame = 0;
      string initMessage = JsonConvert.SerializeObject(new StateInit { index = index });
      Logger.Info("Sending stateInit to agent {0}.".Format(index));
      //connection.Send(initMessage, frame);
      MyTFGame.Write(initMessage, stream);
      Logger.Info("Wait stateInit ack to agent {0}.".Format(index));

      Message message = MyTFGame.Read(stream);
      Logger.Info("Receive stateInit ack to agent {0}.".Format(index));

      Logger.Info("Sending Scenario to agent {0}.".Format(index));
      MyTFGame.Write(scenarioMessage, stream);
      Logger.Info("Wait Scenario ack to agent {0}.".Format(index));
      message = MyTFGame.Read(stream);
      Logger.Info("Receive Scenario ack to agent {0}.".Format(index));

      //connection.Send(scenarioMessage, frame);
      //Message message = MyTFGame.Read(stream);

      //}
      //var task = Task.Run(async () =>
      //{
      //  Message reply = await connection.ReceiveAsync(AiMod.Config.agentTimeout, cancelAgentCommunication);
      //  if (!reply.success)
      //  {
      //    throw new Exception("Agent didn't ack state init: {0}".Format(reply.message));
      //  }
      //});
      //tasks.Add(task);
      //}

      //Logger.Info("Wait for all agents to ack state init.");
      //WaitAllAndClear(tasks);

      //for (int i = 0; i < AgentConnections.Count; i++)
      //{
      //  var connection = AgentConnections[i];
      //  if (connection == null) continue;


      //  if (!listPlayerAIIndexPlaying.Contains(i))
      //  {
      //    continue;
      //  }

      //  Logger.Info("Notify level load to agent {0}.".Format(connection.index));
      //  connection.Send(scenarioMessage, frame);
      //  var task = Task.Run(async () => {
      //    Message reply = await connection.ReceiveAsync(AiMod.Config.agentTimeout, cancelAgentCommunication);
      //    if (!reply.success)
      //    {
      //      throw new Exception("Agent didn't ack state init: {0}".Format(reply.message));
      //    }
      //  });
      //  tasks.Add(task);
      //}

      //Logger.Info("Wait for all agents to ack state scenario.");
      //WaitAllAndClear(tasks);

      //scenarioSent = true;
      //Logger.Info("All agents received scenario.");
    }

    //public void sendFrame() {

    //}


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
