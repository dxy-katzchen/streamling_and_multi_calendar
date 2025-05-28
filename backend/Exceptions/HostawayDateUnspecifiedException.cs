using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Streamling.Model.DTOs.Hostaway;

namespace Streamling.Exceptions
{
    public class HostawayDateUnspecifiedException : Exception
    {
        public ReceivingReservationDto _hostawayReservationDto { get; set; }

        public HostawayDateUnspecifiedException(ReceivingReservationDto hostawayReservationDto) : base("Hostaway reservation date is unspecified, an email has been sent to the admin.")
        {
            _hostawayReservationDto = hostawayReservationDto;
        }
    }

}