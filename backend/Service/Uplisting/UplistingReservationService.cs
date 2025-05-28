
using System.Net;
using System.Text.Json.Nodes;
using API.Utils.Http;
using Microsoft.EntityFrameworkCore;
using Streamling.Data;
using Streamling.Model;
using Streamling.Model.DTOs.Connecteam;
using Streamling.Model.DTOs.Hostaway;
using Streamling.Model.Entities;
using Streamling.Repository;
using Streamling.Utils;
using Streamling.Utils.EntityUtils;
using Streamling.Utils.TimeConverter.Hostaway;

namespace Streamling.Service.Uplisting
{
    public class UplistingReservationService(StoreContext storeContext)
    {
        private readonly ConnectTeamService _connecteamService = new();
        private readonly ReservationRepository _reservationRepository = new(storeContext);
        private readonly StoreContext _storeContext = storeContext;

        private readonly ReservationService _reservationService = new(storeContext);

        public async Task UpdateReservations()
        {

            List<ReceivingReservationDto> newReservationsList = await GetAllReservations();
            List<Reservation> currentReservationList = await _reservationRepository.GetReservations();

            //add new reservations
            await _reservationService.AddNewReservations(newReservationsList, currentReservationList);

            //update existing reservations
            await _reservationService.UpdateExistingReservations(newReservationsList, currentReservationList);

        }


        public async Task<List<ReceivingReservationDto>> GetAllReservations()
        {
            var properties = await _storeContext.Properties.Where(p => p.PlatformName == "Uplisting").ToListAsync();
            var properties_ids = properties.Select(p => p.PlatformPropertyId).ToList();
            List<ReceivingReservationDto> reservations = [];
            foreach (var propertyId in properties_ids)
            {
                List<ReceivingReservationDto> reservationsById = await LoadReservationsForProperty(propertyId);
                reservations.AddRange(reservationsById);

            }

            //filter out reservations status that are "cancelled"
            reservations = reservations.Where(r => r.Status != "cancelled").ToList();

            return reservations;
        }


        private async Task<List<ReceivingReservationDto>> LoadReservationsForProperty(string propertyId)
        {
            var allBookings = await RetrieveReservationsByPropertyId(propertyId);
            return ConvertBookingsToReservations(allBookings, propertyId);
        }

        private async Task<List<JsonNode>> RetrieveReservationsByPropertyId(string propertyId)
        {
            string oneYearAgoTimeStr = TimeConverter.GetTimeStringOneYearAgo();
            string twoYearAfterTimeStr = TimeConverter.GetTimeStringTwoYearAfter();
            int currentPage = 0;
            int totalPages = 1;
            int retryDelay = 10000; // 10 seconds between property requests

            var allBookings = new List<JsonNode>();

            try
            {
                do
                {
                    try
                    {
                        var jsonObj = await HttpRequestHelper.SendRequestWithRetryAsync<JsonObject>(
                            HttpMethod.Get,
                            GlobalSettings.UplistingRequestObj_BedBooka,
                            $"/bookings/{propertyId}?from={oneYearAgoTimeStr}&to={twoYearAfterTimeStr}&page={currentPage}",
                            10, // More retries
                            2000 // Longer initial delay
                        );

                        if (jsonObj.ContainsKey("bookings") && jsonObj["bookings"] != null)
                        {
                            var bookings = jsonObj["bookings"]!.AsArray();
                            allBookings.AddRange(bookings);
                        }
                        else
                        {
                            Console.WriteLine($"No bookings found for property {propertyId} on page {currentPage}");
                        }

                        var meta = jsonObj["meta"]?.AsObject();
                        totalPages = meta?["total_pages"]?.GetValue<int>() ?? 0;
                        currentPage++;

                        // Add a delay between page requests to avoid rate limiting
                        if (currentPage < totalPages)
                        {
                            await Task.Delay(1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error retrieving page {currentPage} for property {propertyId}: {ex.Message}");
                        if (ex is HttpRequestException && (ex as HttpRequestException)?.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            // Special handling for rate limiting
                            await Task.Delay(retryDelay);
                            retryDelay *= 2; // Double the delay for next failure
                            currentPage--; // Try the same page again
                        }
                        else
                        {
                            // For other errors, log and continue with next page
                            Console.WriteLine($"Skipping page {currentPage} due to error");
                        }
                    }
                } while (currentPage < totalPages && currentPage < 10); // Added safety limit of 10 pages
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error retrieving bookings for property {propertyId}: {ex.Message}");
                // Return whatever bookings we've collected so far rather than failing completely
            }

            return allBookings;
        }
        private List<ReceivingReservationDto> ConvertBookingsToReservations(List<JsonNode> allBookings, string propertyId)
        {
            var reservations = new List<ReceivingReservationDto>();

            foreach (var booking in allBookings)
            {
                var reservationDto = ReservationUtils.CreateUplistingReservationDto(booking, GlobalSettings.UplistingRequestObj_BedBooka.UserCredential.AccountId, propertyId);
                reservations.Add(reservationDto);
            }

            return reservations;
        }


    }
}