

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Streamling.Exceptions;
using Streamling.Utils.EmailHelper;

namespace Streamling.Filter
{
    public class DateUnspecifiedExceptionFilter : ExceptionFilterAttribute
    {
        public async override Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is HostawayDateUnspecifiedException exception)
            {
                var hostawayReservationDto = exception._hostawayReservationDto;
                var reservationId = hostawayReservationDto.ReservationId;
                var propertyName = hostawayReservationDto.ListingName;
                var departureDate = hostawayReservationDto.DepartureDate;
                var platform = "Hostaway";
                var guestName = hostawayReservationDto.GuestName;


                string toEmail = "dxyrun2023@gmail.com";
                string subject = "Date Unspecified Reservation has been created - HostAway";
                string body = $@"
                Hi,<br/>
                A reservation has been created with unspecified date on hostaway. this record won't appear in connecteam, but will appear in calendar. The departure date and arrival date is set to the same day. Please check and add it manually.<br/>
                Reservation Details:<br/>
                <br/>
                Reservation ID: {reservationId}<br/>
                Property Name: {propertyName}<br/>
                Platform: {platform}<br/>
                Guest Name: {guestName}<br/>
                DepartureDate: {departureDate}<br/>
                <br/>
                From CleanBoss<br/>
                ";

                await EmailHelper.SendEmailAsync(toEmail, subject, body);

                context.Result = new ObjectResult(exception.Message) { StatusCode = (int)HttpStatusCode.OK };

                context.ExceptionHandled = true;
            }
        }
    }
}