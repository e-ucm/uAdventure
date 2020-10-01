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
  public class Activity {
    /// <summary>
    /// Gets or Sets ExtraData
    /// </summary>
    [DataMember(Name="extra_data", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "extra_data")]
    public Object ExtraData { get; set; }

    /// <summary>
    /// Gets or Sets ExtraData
    /// </summary>
    [DataMember(Name = "details", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "details")]
    public Dictionary<string, object> Details { get; set; }

    /// <summary>
    /// Gets or Sets Id
    /// </summary>
    [DataMember(Name="_id", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "_id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or Sets Name
    /// </summary>
    [DataMember(Name="name", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or Sets Owners
    /// </summary>
    [DataMember(Name="owners", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "owners")]
    public List<string> Owners { get; set; }

    /// <summary>
    /// Gets or Sets Test
    /// </summary>
    [DataMember(Name="test", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "test")]
    public string Test { get; set; }

    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [DataMember(Name="type", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [DataMember(Name = "copysurvey", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "copysurvey")]
    public string CopySurvey { get; set; }

    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [DataMember(Name = "backup", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "backup")]
    public bool Backup { get; set; }

    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [DataMember(Name = "trace_storage", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "trace_storage")]
    public bool TraceStorage { get; set; }

    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [DataMember(Name = "realtime", EmitDefaultValue = false)]
    [JsonProperty(PropertyName = "realtime")]
    public bool Realtime { get; set; }

    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      var sb = new StringBuilder();
      sb.Append("class Activity {\n");
      sb.Append("  ExtraData: ").Append(ExtraData).Append("\n");
      sb.Append("  Id: ").Append(Id).Append("\n");
      sb.Append("  Name: ").Append(Name).Append("\n");
      sb.Append("  Owners: ").Append(Owners).Append("\n");
      sb.Append("  Test: ").Append(Test).Append("\n");
      sb.Append("  Type: ").Append(Type).Append("\n");
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
