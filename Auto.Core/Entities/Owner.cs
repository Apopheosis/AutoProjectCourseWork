using System.Text.Json.Serialization;
using Auto.Core.Entities;

namespace Auto.Core.Entities
{
    public class Owner
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        
        public string VehicleRegistration { get; set; }

        [JsonIgnore] public virtual Vehicle Vehicle { get; set; }
    }
}