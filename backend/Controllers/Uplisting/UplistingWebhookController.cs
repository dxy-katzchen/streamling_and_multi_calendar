
using API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Streamling.Controllers.Uplisting
{
    public class UplistingWebhookController : BaseApiController
    {
        [HttpPost("Uplisting/reservation/create")]
        public ActionResult HandleCreateReservation()
        {
            Console.WriteLine("Received new booking from Uplisting");
            return Ok();
        }

        [HttpPost("Uplisting/reservation/update")]
        public ActionResult HandleUpdateReservation()
        {
            Console.WriteLine("Received updated booking from Uplisting");
            return Ok();
        }

        [HttpPost("Uplisting/reservation/remove")]
        public ActionResult HandleDeleteReservation()
        {
            Console.WriteLine("Received deleted booking from Uplisting");
            return Ok();
        }
    }
}