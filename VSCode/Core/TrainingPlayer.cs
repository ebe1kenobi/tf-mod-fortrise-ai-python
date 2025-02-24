using System;
using System.Runtime.Serialization;

namespace TFModFortRiseAiPython
{
  [DataContract]
  public class TrainingPlayer
  {
    public static class Type {
      public static string Human = "human";
      public static string Remote = "remote";
      public static string None = "none";
    }


    [DataMember(EmitDefaultValue = true)]
    public string type;


    public override String ToString()
    {
      return "{type:" + type + "}";
    }
  }
}
