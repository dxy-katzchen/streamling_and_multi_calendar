
using API.Controllers;
using API.Utils.Http;
using Microsoft.AspNetCore.Mvc;
using Streamling.Data;
using Streamling.Model.DTOs.Hostaway;
using Streamling.Service;


namespace Streamling.Controllers.Connecteam
{
    public class ConnecteamController(StoreContext storeContext) : BaseApiController
    {
        private readonly ConnectTeamService connectTeamService = new();

        // [HttpPost]
        // public async Task<ActionResult> CreateShift(ReceivingReservationDto receivingReservationDto)
        // {
        //     //send request to connecteam
        //     var res = await connectTeamService.addShiftToScheduler(receivingReservationDto);
        //     //get response
        //     string shiftId = res[0]?["id"]?.ToString()!;
        //     //add object into database

        //     return Ok(shiftId);
        // }


    }


}