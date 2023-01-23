using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Auto.Website.Models
{

    public class OwnerDTO
    {
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [DisplayName("Email")]
        public string Email { get; set; }
        private string _vc;

        private static string NormalizeRegistration(string _vc)
        {
            return _vc == null ? _vc : Regex.Replace(_vc.ToUpperInvariant(), "[^A-Z0-9]", "");
        }

        [Required]
        [DisplayName("Registration Plate")]
        public string VehicleCode
        {
            get => NormalizeRegistration(_vc);
            set => _vc = value;
        }
    }
}