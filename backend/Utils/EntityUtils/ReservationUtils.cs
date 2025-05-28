
using System.Text.Json.Nodes;
using Streamling.Model.DTOs.Hostaway;


namespace Streamling.Utils.EntityUtils
{
    public class ReservationUtils
    {
        public static Model.Entities.Reservation CreateReservation(ReceivingReservationDto reservationDto, string? shiftId)
        {
            var reservation = new Model.Entities.Reservation
            {
                Id = $"{reservationDto.Platform}-{reservationDto.AccountId}-{reservationDto.PropertyId}-{reservationDto.ReservationId}",
                AccountId = reservationDto.AccountId,
                PlatformPropertyId = reservationDto.PropertyId,
                ArrivalDate = reservationDto.ArrivalDate,
                PlatformName = reservationDto.Platform,
                PlatformReservationId = reservationDto.ReservationId,
                DepartureDate = reservationDto.DepartureDate,
                ShiftId = shiftId,
                ShiftTitle = reservationDto.ListingName + " - " + reservationDto.Platform,
                ShiftStartTime = TimeConverter.Hostaway.TimeConverter.ConvertNzdtToUnixTimestamp10am(reservationDto.DepartureDate),
                ShiftEndTime = TimeConverter.Hostaway.TimeConverter.ConvertNzdtToUnixTimestamp10am(reservationDto.DepartureDate) + 60,
                GuestName = reservationDto.GuestName,
                Status = reservationDto.Status,
                HostNote = reservationDto.HostNote
            };
            return reservation;
        }

        public static ReceivingReservationDto CreateHostawayReservationDto(JsonObject jsonPayload)
        {
            var data = jsonPayload["data"]!;
            string accountId = jsonPayload["accountId"]?.ToString()!;
            var hostawayReservationDto = ConvertJsonToReservationDto((JsonObject)data, accountId);

            return hostawayReservationDto;
        }

        public static ReceivingReservationDto ConvertJsonToReservationDto(JsonObject data, string accountId)
        {
            return new ReceivingReservationDto
            {
                AccountId = accountId,
                PropertyId = data["listingMapId"]?.ToString()!,
                ReservationId = data["id"]?.ToString()!,
                IsDatesUnspecified = CheckIfDateUnSpecified(data),
                ArrivalDate = data["arrivalDate"]?.ToString()!,
                DepartureDate = data["departureDate"]?.ToString()!,
                ListingName = data["listingName"]?.ToString()!,
                Status = data["status"]?.ToString()!,
                GuestName = data["guestName"]?.ToString()!,
                HostNote = data["hostNote"]?.ToString() ?? "",
                Platform = "Hostaway",
                Id = $"Hostaway-{accountId}-{data["listingMapId"]?.ToString()!}-{data["id"]?.ToString()!}"
            };


        }

        public static ReceivingReservationDto CreateUplistingReservationDto(JsonNode booking, string accountId, string propertyId)
        {

            var uplistingReservationDto = new ReceivingReservationDto
            {
                AccountId = accountId,
                PropertyId = propertyId,
                ReservationId = booking["id"]!.GetValue<int>().ToString(),
                IsDatesUnspecified = false,
                ArrivalDate = booking["check_in"]?.GetValue<string>() ?? "",
                DepartureDate = booking["check_out"]?.GetValue<string>() ?? "",
                ListingName = booking["property_name"]?.GetValue<string>() ?? "",
                Status = booking["status"]?.GetValue<string>() ?? "",
                GuestName = booking["guest_name"]?.GetValue<string>() ?? "",
                HostNote = booking["note"]?.ToString() ?? "",
                Platform = "Uplisting",
                Id = $"Uplisting-{accountId}-{propertyId}-{booking["id"]!.GetValue<int>()}"
            };

            return uplistingReservationDto;
        }

        public static bool CheckIfReservationisNew(JsonObject jsonPayload)
        {
            string reservationStatus = jsonPayload["data"]?["status"]?.ToString()!;

            if (reservationStatus == "new")
            {
                return true;
            }
            return false;
        }

        public static bool CheckIfReservationisModified(JsonObject jsonPayload)
        {
            string reservationStatus = jsonPayload["data"]?["status"]?.ToString()!;

            if (reservationStatus == "modified")
            {
                return true;
            }
            return false;
        }

        public static bool CheckIfDateUnSpecified(JsonObject jsonPayload)
        {
            string isDatesUnspecified = jsonPayload["isDatesUnspecified"]?.ToString()!;
            if (isDatesUnspecified == "1")
            {
                return true;
            }
            return false;
        }
    }
}