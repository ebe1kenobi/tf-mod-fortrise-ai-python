using System.Threading;
using Monocle;
using System;
using FortRise;
//using Microsoft.Xna.Framework;
using TowerFall;
using System.Threading.Tasks;
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
    internal static void Load()
    {
      On.TowerFall.TFGame.Load += Load_patch;
    }

    internal static void Unload()
    {
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

    //public static void Update_patch(On.TowerFall.TFGame.orig_Update orig, global::TowerFall.TFGame self, GameTime gameTime)
    //{
    //  //AIPython.CreateAgent();
    //  orig(self, gameTime);
    //}

    //public static void RegisterInPool()
    //{
    //  //Logger.Info("RegisterInPool");
    //  string poolPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\TowerFall\\pools\\default\\";
    //  string metadataPathFileName = poolPath + Process.GetCurrentProcess().Id.ToString();

    //  Util.CreateDirectory(poolPath);

    //  //Logger.Info("Util.CreateDirectory");

    //  using (var f = File.Open(metadataPathFileName, FileMode.OpenOrCreate))
    //  {
    //    using (var w = new StreamWriter(f))
    //    {
    //      Logger.Info($"Writing metadata to {metadataPathFileName}");
    //      w.Write(JsonConvert.SerializeObject(new Metadata
    //      {
    //        port = ((IPEndPoint)server.LocalEndpoint).Port,
    //        fastrun = false,
    //        nographics = false,
    //      }));
    //    }
    //  }
    //}

    //static void StartServer()
    //{
    //  server = new TcpListener(IPAddress.Any, 0);
    //  server.Start();
    //  while (((IPEndPoint)server.LocalEndpoint).Port == 0) Thread.Sleep(200);
    //  RegisterInPool();
    //  Logger.Info("Server started, waiting for connections...");
    //  TcpClient client;
    //  while (true)
    //  {
    //    try
    //    {
    //      client = server.AcceptTcpClient(); // Blocking call, waits for a client
    //      Logger.Info("Client connected!");

    //      // Handle client in a separate thread
    //      Task.Run(() => HandleClient(client));
    //    }
    //    catch (Exception ex)
    //    {
    //      Logger.Info($"Error: {ex.Message}");
    //      break;
    //    }
    //  }

    //  Logger.Info("server.Stop()");
    //  server.Stop();
    //}

    //static void HandleClient(TcpClient client)
    //{
    //  try
    //  {
    //    NetworkStream stream = client.GetStream();
    //    Message message = Read(stream);

    //    if (message.type == "join")
    //    {
    //      Logger.Info($"Received: {message.type}");
    //      HandleJoinMessage(message, stream);
    //      Logger.Info($"HandleJoinMessage");
    //    }
    //    else if (message.type == "config")
    //    {
    //      Logger.Info($"Received: {message.type}");
    //      HandleNewConfigConnection(message, stream);
    //      Logger.Info($"HandleNewConfigConnection");
    //    }
    //    //else if (message.type == "reset")
    //    //{
    //    //  Logger.Info($"Received: {message.type}");
    //    //  //HandleResetMessage(connection, message);
    //    //}
    //    else
    //    {
    //      Logger.Info("Message type not supported: {0}".Format(message.type));
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    Logger.Info($"Client error: {ex.Message}");
    //  }
    //}

    //public static void HandleJoinMessage(Message message, NetworkStream stream)
    //{
    //  Logger.Info($"in HandleJoinMessage");
    //  AIPython.AgentInputs[nbRemoteAgentConnected] = new Input(nbRemoteAgentConnected);
    //  AIPython.agents[nbRemoteAgentConnected] = new AIPythonAgent(nbRemoteAgentConnected, "AIP", AIPython.AgentInputs[nbRemoteAgentConnected], stream);
    //  nbRemoteAgentConnected++;
    //  if (nbRemoteAgentConnected == nbRemoteAgentWaited) {
    //    isAgentReady = true;
    //    LoaderAIImport.addAgent("AIP", AIPython.agents);
    //  }

    //  Write(JsonConvert.SerializeObject(new Message
    //  {
    //    type = Message.Type.Result,
    //    success = true,
    //    message = "Game will start once all agents join."
    //  }), stream);
    //}

    //public static void HandleNewConfigConnection(Message message, NetworkStream stream)
    //{
    //  Logger.Info("HandleNewConfigConnection " + message.config.agents.Count);
    //  nbRemoteAgentWaited = message.config.agents.Count > 8 ? 8 : message.config.agents.Count;
    //  Write(JsonConvert.SerializeObject(new Message
    //  {
    //    type = Message.Type.Result,
    //    maxAgent = nbRemoteAgentWaited,
    //    success = true,
    //  }), stream);
    //}

    //public static void Write(string text, NetworkStream stream)
    //{
    //  byte[] payload = Encoding.ASCII.GetBytes(text);
    //  int size = payload.Length;
    //  //if (size > maxMessageSize)
    //  //{
    //  //  throw new Exception("Message exceeds limit: {0}.".Format(maxMessageSize));
    //  //}
    //  byte[] header = new Byte[2];
    //  header[0] = (byte)(size >> 8);
    //  header[1] = (byte)(size & 0x00FF);
    //  //socket.Send(header);
    //  stream.Write(header, 0, header.Length);

    //  //socket.Send(payload);
    //  stream.Write(payload, 0, payload.Length);


    //}

    //public static Message Read(NetworkStream stream) {
    //  try
    //  {
    //    byte[] header = new byte[2];
    //    byte[] buffer = new byte[10000];
    //    string rawMessage = "";
    //    while (true)
    //    {
    //      int bytesRead = stream.Read(header, 0, header.Length);
    //      if (bytesRead == 0) break; // Connection closed

    //      int bytesToReceive = header[0] << 8 | header[1];

    //      bytesRead = stream.Read(buffer, 0, bytesToReceive);
    //      if (bytesRead == 0) break; // Connection closed

    //      rawMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    //      //rawMessage = new UTF8Encoding(false).GetString(buffer, 0, bytesRead); //todo ... 
    //      Logger.Info($"Received: {rawMessage}");
    //      break;
    //    }

    //    Message message = JsonConvert.DeserializeObject<Message>(rawMessage);
    //    return message;
    //  } 
    //  catch (Exception ex)
    //  {
    //    Logger.Info($"Client error: {ex.Message}");
    //    throw ex;
    //  }
    //}
  }
}
