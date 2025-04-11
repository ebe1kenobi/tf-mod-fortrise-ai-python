using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TFModFortRiseAiPython
{
  [DataContract]
  public class MatchConfig {

    [DataMember(EmitDefaultValue = false)]
    public bool training;

    [DataMember(EmitDefaultValue = false)]
    public int level;

    [DataMember(EmitDefaultValue = false)]
    public int subLevel;

    [DataMember(EmitDefaultValue = false)]
    public bool noTreasure;

    [DataMember(EmitDefaultValue = false)]
    public bool noHazards;
    
    [DataMember(EmitDefaultValue = false)]
    public float speed;

    [DataMember(EmitDefaultValue = false)]
    public int skipWaves;

    [DataMember(EmitDefaultValue = false)]
    public int nbAgents;

    //[DataMember(EmitDefaultValue = false)]
    //public int nbHuman; 

    [DataMember(EmitDefaultValue = false)]
    public List<AgentConfig> agents;

    [DataMember(EmitDefaultValue = false)]
    public List<TrainingPlayer> trainingPlayer;

    [DataMember(EmitDefaultValue = false)]
    public string mode;

    [DataMember(EmitDefaultValue = false)]
    public string difficulty;

    [DataMember(EmitDefaultValue = false)]
    public string matchLengths;

    [DataMember(EmitDefaultValue = false)]
    public bool randomLevel;

    [DataMember(EmitDefaultValue = false)]
    public TimeSpan agentTimeout;

    [DataMember(EmitDefaultValue = false)]
    public int fps;

    [DataMember(EmitDefaultValue = false)]
    public int[,] solids;

    public override String ToString()
    {
      String s = "{training:" + training 
      + ",fps:" + fps 
      + ",level:" + level 
      + ",subLevel:" + subLevel 
      + ",subLevel:" + randomLevel
      + ",matchLengths:" + matchLengths
      + ",difficulty:" + difficulty 
      + ",matchLengths:" + matchLengths 
      + ", mode:" + mode
      + ", trainingPlayer:\n";
      foreach (var player in trainingPlayer)
      {
        s += player.ToString() + "\n";
      }
      s += ", agents:\n";
      foreach (var agent in agents) {
        s += agent.ToString() + "\n";
      }
      s += "}";
      return s;
    }
  }
}
