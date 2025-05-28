
using Streamling.Model.DTOs.Calendar;
using Streamling.Model.Entities;
using Streamling.Repository;
using Streamling.Utils.TimeConverter.Hostaway;

namespace Streamling.Service
{
    public class CalendarService(ReservationRepository reservationRepository, PropertyRepository propertyRepository, ChannelRepository channelRepository)
    {
        private readonly ReservationRepository _reservationRepository = reservationRepository;
        private readonly PropertyRepository _propertyRepository = propertyRepository;
        private readonly ChannelRepository _channelRepository = channelRepository;

        public async Task<List<SchedulerDto>> GetSchedulerData()
        {
            var reservations = await _reservationRepository.GetReservations();
            var properties = await _propertyRepository.GetPropertiesByName();
            var channels = await _channelRepository.GetChannels();
            var schedulerData = new List<SchedulerDto>();

            foreach (var property in properties)
            {
                var propertyId = property.Id;
                var propertyReservations = GetReservationsByPropertyId(reservations, propertyId);
                var schedulerDataDto = await GetSchedulerDataDto(propertyReservations);
                var channelName = channels.FirstOrDefault(c => c.PlatformName == property.PlatformName && c.AccountId == property.AccountId)?.Name ?? string.Empty;

                var schedulerDto = new SchedulerDto
                {
                    id = propertyId,
                    label = new SchedulerLabelDto
                    {
                        title = property.Name,
                        subtitle = $"{property.PlatformName} - {channelName}"
                    },
                    data = schedulerDataDto
                };

                schedulerData.Add(schedulerDto);
            }
            return schedulerData;
        }

        private List<Reservation> GetReservationsByPropertyId(List<Reservation> reservations, string propertyId)
        {
            return reservations.Where(r => $"{r.PlatformName}-{r.AccountId}-{r.PlatformPropertyId}" == propertyId).ToList();
        }

        private async Task<List<SchedulerDataDto>> GetSchedulerDataDto(List<Reservation> reservations)
        {
            var schedulerDataDtos = new List<SchedulerDataDto>();
            var properties = await _propertyRepository.GetProperties();


            foreach (var reservation in reservations)
            {

                string propertyName = properties.FirstOrDefault(p => p.PlatformName == reservation.PlatformName && p.AccountId == reservation.AccountId && p.PlatformPropertyId == reservation.PlatformPropertyId)?.Name ?? string.Empty;
                var schedulerData = new SchedulerDataDto
                {
                    id = reservation.Id,
                    startDate = TimeConverter.ConvertNzdtToUTCString(reservation.ArrivalDate, 14),
                    endDate = TimeConverter.ConvertNzdtToUTCString(reservation.DepartureDate, 10),
                    title = propertyName,
                    subtitle = reservation.Status,
                    description = reservation.HostNote,
                    bgColor = reservation.Status switch
                    {
                        "new" => "#a18cd1",
                        "ownerStay" => "rgb(133, 169, 71)",
                        "modified" => "rgb(122, 178, 211)",
                        "confirmed" => "rgb(180, 0, 200)",
                        "checked_in" => "rgb(27, 147, 158)",
                        "checked_out" => "rgb(28, 90, 117)",
                        "needs_check_in" => "rgb(224, 119, 232)",
                        "needs_check_out" => "rgb(90, 114, 231)",
                        _ => "rgb(103, 8, 8)"
                    },

                };

                schedulerDataDtos.Add(schedulerData);
            }

            return schedulerDataDtos;
        }
    }
}