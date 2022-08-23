using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Simva.Model {

  /// <summary>
  /// 
  /// </summary>
  [DataContract]
  public class AddGroupBody {
    /// <summary>
    /// Gets or Sets Name
    /// </summary>
    [DataMember(Name="name", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// The amount of codes to be created randomly when the group is created 
    /// </summary>
    /// <value>The amount of codes to be created randomly when the group is created </value>
    [DataMember(Name="random_participants", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "random_participants")]
    public int? RandomParticipants { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      var sb = new StringBuilder();
      sb.Append("class AddGroupBody {\n");
      sb.Append("  Name: ").Append(Name).Append("\n");
      sb.Append("  RandomParticipants: ").Append(RandomParticipants).Append("\n");
      sb.Append("}\n");
      return sb.ToString();
    }

    /// <summary>
    /// Get the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson() {
      return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

}
}
