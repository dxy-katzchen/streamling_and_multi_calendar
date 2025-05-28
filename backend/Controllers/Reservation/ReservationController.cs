
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Streamling.Data;
using Streamling.Service;


namespace Streamling.Controllers.Reservation
{
    public class ReservationController(StoreContext storeContext) : BaseApiController
    {
        [HttpPost("update")]
        public async Task<ActionResult> FetchReservations()
        {
            ReservationService reservationService = new(storeContext);
            await reservationService.UpdateReservations();
            return Ok();
        }

    }
}