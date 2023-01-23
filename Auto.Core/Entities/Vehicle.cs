using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Auto.Core.Entities;

public class Vehicle
{
    public string Registration { get; set; }
    public string ModelCode { get; set; }
    public string Color { get; set; }
    public int Year { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    public virtual Model VehicleModel { get; set; }
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual Owner Owner { get; set; }
}