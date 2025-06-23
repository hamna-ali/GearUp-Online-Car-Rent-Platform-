using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GearUp.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        public string? Username { get; set; }

        public string? VehiclePlateNumber { get; set; }

        public DateTime BookingDateTime { get; set; }

        public DateTime ReturnDate { get; set; }
    }
}