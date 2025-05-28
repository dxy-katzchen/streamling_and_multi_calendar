
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Streamling.Repository;
using Streamling.Service;

namespace Streamling.Controllers.Calendar
{
    public class CalendarController(PropertyRepository propertyRepository, ReservationRepository reservationRepository, ChannelRepository channelRepository) : BaseApiController
    {
        private readonly CalendarService _calendarService = new(reservationRepository, propertyRepository, channelRepository);
        [HttpGet]
        public async Task<ActionResult> GetSchedulerData()
        {
            var schedulerData = await _calendarService.GetSchedulerData();
            return Ok(schedulerData);
        }
    }
}