using System.ComponentModel.DataAnnotations;

namespace UnitedAirlinesAPI.Models
{
    public class CargoRequest
    {
        [MaxLength(3, ErrorMessage = "Cannot exceed 3 letters")]
        public string AirportCode { get; set; }
        public string FlightDate { get; set; }
    }
}
