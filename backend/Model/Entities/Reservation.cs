

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Streamling.Model.Entities
{
    public class Reservation
    {
        public required string Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(191)")]
        public required string PlatformReservationId { get; set; }
        public Property Property { get; set; }

        [Required]
        [Column(TypeName = "varchar(191)")]
        public required string PlatformPropertyId { get; set; }
        [Column(TypeName = "varchar(191)")]
        public required string PlatformName { get; set; }
        public Platform Platform { get; set; }
        [Required]
        [Column(TypeName = "varchar(191)")]
        public required string AccountId { get; set; }

        public Channel Channel { get; set; }

        [Required]
        public required string ArrivalDate { get; set; }
        [Required]
        public required string DepartureDate { get; set; }
        [Required]
        public required string? ShiftId { get; set; }
        [Required]
        public required string ShiftTitle { get; set; }
        [Required]
        public required int ShiftStartTime { get; set; }
        [Required]
        public required int ShiftEndTime { get; set; }
        [Required]
        public required string GuestName { get; set; }
        [Required]
        public required string Status { get; set; }
        public required string HostNote { get; set; }


    }
}