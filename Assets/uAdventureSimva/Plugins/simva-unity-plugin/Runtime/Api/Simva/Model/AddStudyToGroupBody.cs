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
  public class AddStudyToGroupBody {
    /// <summary>
    /// Gets or Sets Study
    /// </summary>
    [DataMember(Name="study", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "study")]
    public string Study { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      var sb = new StringBuilder();
      sb.Append("class AddStudyToGroupBody {\n");
      sb.Append("  Study: ").Append(Study).Append("\n");
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
