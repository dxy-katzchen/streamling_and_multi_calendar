

namespace Streamling.Model.DTOs.Hostaway
{
    public class ReceivingReservationDto
    {
        public required string ReservationId { get; set; }

        public required string PropertyId { get; set; }

        public required bool IsDatesUnspecified { get; set; }

        public required string ArrivalDate { get; set; }

        public required string DepartureDate { get; set; }

        public required string ListingName { get; set; }

        public required string Status { get; set; }

        public required string GuestName { get; set; }

        public required string HostNote { get; set; } = "";

        public required string Platform { get; set; }

        public required string AccountId { get; set; }

        public required string Id { get; set; }
    }
}