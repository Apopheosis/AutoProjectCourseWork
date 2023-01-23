using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Auto.Core.Entities;

public class Manufacturer
{
    public Manufacturer() {
        Models = new HashSet<Model>();
    }

    public string Code { get; set; }
    public string Name { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<Model> Models { get; set; }
}