using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamling.Model.Entities
{
    public class Platform
    {


        public required string Name { get; set; }

        public ICollection<Channel> Channels { get; set; }

        public ICollection<Property> Properties { get; set; }

        public ICollection<Reservation> Reservations { get; set; }

    }
}