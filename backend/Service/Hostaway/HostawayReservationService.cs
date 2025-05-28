using System.Text.Json.Nodes;
using API.Utils.Http;
using Streamling.Data;
using Streamling.Exceptions;
using Streamling.Model.DTOs.Hostaway;
using Streamling.Model.Entities;
using Streamling.Repository;
using Streamling.Utils;
using Streamling.Utils.EntityUtils;
using Streamling.Utils.TimeConverter.Hostaway;
namespace Streamling.Service
{
    public class HostawayReservationService
    {
        private readonly ConnectTeamService _connecteamService = new();
        private readonly ReservationRepository _reservationRepository;
        private readonly StoreContext _storeContext;
        private readonly ReservationService _reservationService;


        public HostawayReservationService(StoreContext storeContext)
        {
            _reservationRepository = new(storeContext);
            _storeContext = storeContext;
            _reservationService = new(storeContext);
        }

        public async Task UpdateReservations()
        {
            List<ReceivingReservationDto> newReservationsList = await GetAllReservations();

            List<Reservation> currentReservationList = await _reservationRepository.GetReservationsByPlatformNameAndAccountId(GlobalSettings.HostawayRequestObj_FR.UserCredential.AccountId, "Hostaway");

            //add new reservations
            await _reservationService.AddNewReservations(newReservationsList, currentReservationList);
            //update existing reservations
            await _reservationService.UpdateExistingReservations(newReservationsList, currentReservationList);
            //delete reservations that are not in the new reservations list
            await _reservationService.DeleteReservations(newReservationsList, currentReservationList);
            //delete dates unspecified reservations in connecteam
            await DeleteDateUnspecifiedReservationsInConnecteam(currentReservationList);
        }

        public async Task DeleteDateUnspecifiedReservationsInConnecteam(List<Reservation> currentReservationList)
        {
            List<Reservation> dateUnspecifiedReservations = [.. currentReservationList.Where(r => r.ArrivalDate == r.DepartureDate)];
            List<Reservation> dateUnspecifiedReservationsFR = [.. dateUnspecifiedReservations.Where(r => r.AccountId == GlobalSettings.HostawayRequestObj_FR.UserCredential.AccountId)];
            List<Reservation> dateUnspecifiedReservationsFRANCHIESE = [.. dateUnspecifiedReservations.Where(r => r.AccountId != GlobalSettings.HostawayRequestObj_FR.UserCredential.AccountId)];
            if (dateUnspecifiedReservationsFR.Count > 0)
            {
                await _connecteamService.DeleteShifts(dateUnspecifiedReservationsFR, GlobalSettings.FR_SCHEDULER_ID);
                dateUnspecifiedReservationsFR.ForEach(r => r.ShiftId = null);
                await _reservationRepository.UpdateReservations(dateUnspecifiedReservationsFR);
            }
            if (dateUnspecifiedReservationsFRANCHIESE.Count > 0)
            {
                await _connecteamService.DeleteShifts(dateUnspecifiedReservationsFRANCHIESE, GlobalSettings.FRANCHIESE_SCHEDULER_ID);
                dateUnspecifiedReservationsFRANCHIESE.ForEach(r => r.ShiftId = null);
                await _reservationRepository.UpdateReservations(dateUnspecifiedReservationsFRANCHIESE);
            }
        }


        public async Task<List<ReceivingReservationDto>> GetAllReservations()
        {
            string oneYearAgoTimeStr = TimeConverter.GetTimeStringOneYearAgo();
            string twoYearAfterTimeStr = TimeConverter.GetTimeStringTwoYearAfter();
            int currentPage = 0;
            int limit = 500;
            int totalCount;

            var allBookings = new List<ReceivingReservationDto>();

            do
            {
                var jsonObj = await HttpRequestHelper.SendRequestWithRetryAsync<JsonObject>(HttpMethod.Get, GlobalSettings.HostawayRequestObj_FR, $"/reservations?sortOrder=arrivalDateDesc&limit={limit}&departureStartDate={oneYearAgoTimeStr}&departureEndDate={twoYearAfterTimeStr}&offset={currentPage * limit}");

                var reservations = jsonObj["result"]!.AsArray();

                var filteredReservations = FilterReservations(reservations);

                var newReservations = filteredReservations.Select(r => ReservationUtils.ConvertJsonToReservationDto(r, GlobalSettings.HostawayRequestObj_FR.UserCredential.AccountId)).ToList();

                allBookings.AddRange(newReservations);

                totalCount = jsonObj["count"]!.GetValue<int>();

                currentPage++;
            } while (currentPage * limit < totalCount);

            return allBookings;

        }

        private static List<JsonObject> FilterReservations(JsonArray reservations)
        {
            return reservations.Where(r => r?["status"]?.ToString() == "new" || r?["status"]?.ToString() == "modified" || r?["status"]?.ToString() == "ownerStay")
                                .Where(r => r != null)
                                .Cast<JsonObject>()
                                .ToList();
        }

    }

}