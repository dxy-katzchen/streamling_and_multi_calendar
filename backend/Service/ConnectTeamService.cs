using API.Utils.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Streamling.Model.DTOs.Connecteam;
using Streamling.Model.DTOs.Hostaway;
using Streamling.Model.Entities;
using Streamling.Utils;
using Streamling.Utils.TimeConverter.Hostaway;
using Streamling.Model.DTOs.Hostaway;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Streamling.Service
{
    public class ConnectTeamService()
    {
        public async Task<JsonArray> AddShiftsToScheduler(List<CreateShiftDto> shifts, int schedulerId)
        {
            const int batchSize = 100; // Define the batch size
            JsonArray allShiftsJsonArr = [];

            for (int i = 0; i < shifts.Count; i += batchSize)
            {
                var batch = shifts.Skip(i).Take(batchSize).ToList();
                var responseJson = await HttpRequestHelper.SendRequestAsync(HttpMethod.Post, GlobalSettings.ConnectTeamRequestObj, $"/{schedulerId}/shifts", batch);
                var shiftsJsonArr = responseJson["data"]?["shifts"]?.AsArray();

                if (shiftsJsonArr != null)
                {
                    foreach (var shift in shiftsJsonArr)
                    {
                        allShiftsJsonArr.Add(shift.DeepClone());
                    }
                }
            }
            await EliminateCreatedDuplicateShifts(allShiftsJsonArr, schedulerId);
            return allShiftsJsonArr;
        }

        private async Task EliminateCreatedDuplicateShifts(JsonArray shifts, int schedulerId)
        {
            // Find duplicates and get the array of IDs of the 2nd, 3rd, 4th, 5th, etc., duplicated members
            List<string> duplicateIds = shifts.GroupBy(s => new { startTime = s["startTime"]!.GetValue<int>(), endTime = s["endTime"]!.GetValue<int>(), title = s["title"]!.GetValue<string>() })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1)) // Skip the first element in each group
                .Select(s => s["id"]!.GetValue<string>())
                .ToList();
            if (duplicateIds.Count > 0)
            {
                // Delete the duplicates in connecteam
                await HttpRequestHelper.SendRequestAsync(HttpMethod.Delete, GlobalSettings.ConnectTeamRequestObj, $"/{schedulerId}/shifts", duplicateIds);
                // Remove the duplicates from the array
                for (int i = shifts.Count - 1; i >= 0; i--)
                {
                    if (duplicateIds.Contains(shifts[i]["id"]!.GetValue<string>()))
                    {
                        shifts.RemoveAt(i);
                    }
                }
            }

        }

        public async Task<List<CreateShiftDto>> CreateShiftsByReservationDto(List<ReceivingReservationDto> reservations)
        {
            List<CreateShiftDto> shifts = new List<CreateShiftDto>();

            foreach (var reservation in reservations)
            {
                var shiftStartTime = TimeConverter.ConvertNzdtToUnixTimestamp10am(reservation.DepartureDate);
                var shiftEndTime = shiftStartTime + 60;

                CreateShiftDto shift = new CreateShiftDto
                {
                    startTime = shiftStartTime,
                    endTime = shiftEndTime,
                    title = $"{reservation.ListingName} - {reservation.Platform}",
                    isOpenShift = true // Set the required property
                };
                shifts.Add(shift);
            }
            return shifts;
        }

        public async Task updateShift(string shiftId, string DepartureTime, int schedulerId)
        {
            var shiftStartTime = TimeConverter.ConvertNzdtToUnixTimestamp10am(DepartureTime);
            var shiftEndTime = shiftStartTime + 60;

            UpdateShiftDto shift = new()
            {
                shiftId = shiftId,
                startTime = shiftStartTime,
                endTime = shiftEndTime,
                isOpenShift = true // Set the required property
            };

            var shifts = new List<UpdateShiftDto> { shift };


            await HttpRequestHelper.SendRequestAsync(HttpMethod.Put, GlobalSettings.ConnectTeamRequestObj, $"/{schedulerId}/shifts", shifts);
        }

        public async Task DeleteShifts(List<Reservation> reservations, int schedulerId)
        {
            // Filter out null shift IDs first
            var validShiftIds = reservations
                .Where(r => r.ShiftId != null)
                .Select(r => r.ShiftId)
                .ToList();

            // Process deletions in batches of 20 (API limit)
            const int batchSize = 20;

            for (int i = 0; i < validShiftIds.Count; i += batchSize)
            {
                // Take up to 20 shift IDs for this batch
                var batchIds = validShiftIds
                    .Skip(i)
                    .Take(batchSize)
                    .ToArray();

                if (batchIds.Length > 0)
                {
                    DeleteObj deleteObj = new(batchIds);
                    string deleteUrl = $"/{schedulerId}/shifts";

                    try
                    {
                        await HttpRequestHelper.SendRequestAsync(
                            HttpMethod.Delete,
                            GlobalSettings.ConnectTeamRequestObj,
                            deleteUrl,
                            deleteObj
                        );

                        Console.WriteLine($"Successfully deleted batch {i / batchSize + 1} " +
                            $"({batchIds.Length} shifts)");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting batch {i / batchSize + 1}: {ex.Message}");
                        // Continue with next batch rather than failing completely
                    }
                }
            }
        }

        private class DeleteObj(string[] shiftsIds)
        {
            public string[] shiftsIds { get; set; } = shiftsIds;

        }
    }
}