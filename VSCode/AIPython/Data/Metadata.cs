using System.Runtime.Serialization;

namespace TFModFortRiseAiPython
{
  [DataContract]
  public class Metadata {
    [DataMember(EmitDefaultValue = true)]
    public int port;

    [DataMember(EmitDefaultValue = true)]
    public bool fastrun;

    [DataMember(EmitDefaultValue = true)]
    public bool nographics;
  }
}
