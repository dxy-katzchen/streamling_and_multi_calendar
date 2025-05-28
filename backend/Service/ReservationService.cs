
using System.Text.Json.Nodes;
using Streamling.Data;
using Streamling.Model;
using Streamling.Model.DTOs.Connecteam;
using Streamling.Model.DTOs.Hostaway;
using Streamling.Model.Entities;
using Streamling.Repository;
using Streamling.Service.Uplisting;
using Streamling.Utils;
using Streamling.Utils.EntityUtils;
using Streamling.Utils.TimeConverter.Hostaway;

namespace Streamling.Service
{
    public class ReservationService(StoreContext storeContext)
    {
        private readonly ConnectTeamService _connecteamService = new();
        private readonly ReservationRepository _reservationRepository = new(storeContext);
        public async Task UpdateReservations()
        {
            PropertyService propertyService = new(storeContext);
            await propertyService.FlushProperties();
            UplistingReservationService uplistingReservationService = new(storeContext);
            HostawayReservationService hostawayReservationService = new(storeContext);
            await hostawayReservationService.UpdateReservations();
            await uplistingReservationService.UpdateReservations();
        }
        public async Task AddNewReservations(List<ReceivingReservationDto> newReservationsList, List<Reservation> currentReservationList)
        {

            var newReservationsId = newReservationsList.Select(r => r.Id).ToList();
            var currentReservationsId = currentReservationList.Select(r => r.Id).ToList();
            //add new reservations
            var reservationsToAdd = newReservationsList.Where(r => !currentReservationsId.Contains(r.Id)).ToList();
            List<CreateShiftDto> shiftDtos = await _connecteamService.CreateShiftsByReservationDto(reservationsToAdd);
            JsonArray createdShifts;
            if (newReservationsList[0].Platform == "Hostaway" && newReservationsList[0].AccountId == GlobalSettings.HostawayRequestObj_FR.UserCredential.AccountId)
            {
                createdShifts = await _connecteamService.AddShiftsToScheduler(shiftDtos, GlobalSettings.FR_SCHEDULER_ID);
            }
            else
            {
                createdShifts = await _connecteamService.AddShiftsToScheduler(shiftDtos, GlobalSettings.FRANCHIESE_SCHEDULER_ID);
            }
            List<Reservation> reservationsEntitiesToAdd = [];
            foreach (var shift in createdShifts)
            {
                var shiftId = shift["id"]!.GetValue<string>();
                //find the reservation that corresponds to this shift
                var reservation = reservationsToAdd.Find(r => TimeConverter.ConvertNzdtToUnixTimestamp10am(r.DepartureDate) == shift["startTime"]!.GetValue<int>() && shift["title"]!.GetValue<string>() == $"{r.ListingName} - {r.Platform}")!;
                var reservationEntityToAdd = ReservationUtils.CreateReservation(reservation, shiftId);
                reservationsEntitiesToAdd.Add(reservationEntityToAdd);
            }
            await _reservationRepository.AddReservations(reservationsEntitiesToAdd);
        }

        public async Task UpdateExistingReservations(List<ReceivingReservationDto> newReservationsList, List<Reservation> currentReservationList)
        {
            foreach (Reservation currentReservation in currentReservationList)
            {
                ReceivingReservationDto? newReservation = newReservationsList.Find(r => r.Id == currentReservation.Id);

                if (newReservation != null)
                {
                    if (currentReservation.DepartureDate != newReservation.DepartureDate)
                    {
                        var shiftId = currentReservation.ShiftId;
                        int schedulerId;

                        if (newReservationsList[0].Platform == "Hostaway" && newReservationsList[0].AccountId == GlobalSettings.HostawayRequestObj_FR.UserCredential.AccountId)
                        {
                            schedulerId = GlobalSettings.FR_SCHEDULER_ID;
                        }
                        else
                        {
                            schedulerId = GlobalSettings.FRANCHIESE_SCHEDULER_ID;
                        }

                        try
                        {
                            await _connecteamService.updateShift(shiftId, newReservation.DepartureDate, schedulerId);
                        }
                        catch (HttpRequestException ex) when (ex.Message.Contains("shift_ids that not exist"))
                        {
                            Console.WriteLine($"Shift ID {shiftId} not found. Creating a new shift for reservation {newReservation.Id}");
                            //1. create a new shift in connecteam
                            List<ReceivingReservationDto> newShiftReservation = [newReservation];
                            List<CreateShiftDto> newShift = await _connecteamService.CreateShiftsByReservationDto(newShiftReservation);
                            //2. update reservation in db
                            JsonArray createdShifts = await _connecteamService.AddShiftsToScheduler(newShift, schedulerId);

                            // Update the reservation record with the new shift ID
                            if (createdShifts.Count > 0)
                            {
                                var newShiftId = createdShifts[0]["id"]!.GetValue<string>();
                                currentReservation.ShiftId = newShiftId;

                                // Update shift time values
                                currentReservation.ShiftStartTime = TimeConverter.ConvertNzdtToUnixTimestamp10am(newReservation.DepartureDate);
                                currentReservation.ShiftEndTime = currentReservation.ShiftStartTime + 60;

                                Console.WriteLine($"Created new shift with ID {newShiftId} for reservation {newReservation.Id}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log other exceptions but don't crash
                            Console.WriteLine($"Error updating shift {shiftId}: {ex.Message}");
                            throw; // Rethrow if it's not a "shift not found" error
                        }

                        currentReservation.DepartureDate = newReservation.DepartureDate;
                        await _reservationRepository.UpdateReservation(currentReservation);
                    }
                    if (currentReservation.Status != newReservation.Status || currentReservation.ArrivalDate != newReservation.ArrivalDate || currentReservation.GuestName != newReservation.GuestName || currentReservation.HostNote != newReservation.HostNote)
                    {
                        currentReservation.Status = newReservation.Status;
                        currentReservation.ArrivalDate = newReservation.ArrivalDate;
                        currentReservation.GuestName = newReservation.GuestName;
                        currentReservation.HostNote = newReservation.HostNote;
                        await _reservationRepository.UpdateReservation(currentReservation);
                    }
                }
            }
        }

        public async Task DeleteReservations(List<ReceivingReservationDto> newReservationsList, List<Reservation> currentReservationList)
        {
            var newReservationsId = newReservationsList.Select(r => r.Id).ToList();
            var currentReservationsId = currentReservationList.Select(r => r.Id).ToList();


            var reservationsToDelete = currentReservationList.Where(r => !newReservationsId.Contains(r.Id)).ToList();

            if (reservationsToDelete.Count > 0)
            {
                if (newReservationsList[0].Platform == "Hostaway" && newReservationsList[0].AccountId == GlobalSettings.HostawayRequestObj_FR.UserCredential.AccountId)
                {
                    await _connecteamService.DeleteShifts(reservationsToDelete, GlobalSettings.FR_SCHEDULER_ID);
                }
                else
                {
                    await _connecteamService.DeleteShifts(reservationsToDelete, GlobalSettings.FRANCHIESE_SCHEDULER_ID);
                }

                await _reservationRepository.DeleteReservations(reservationsToDelete);
            }
        }
    }
}