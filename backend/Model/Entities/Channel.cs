using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Streamling.Model.Entities
{
    public class Channel
    {

        public required string Name { get; set; }
        [Column(TypeName = "varchar(191)")]
        public string AccountId { get; set; }
        [Column(TypeName = "varchar(191)")]
        public required string PlatformName { get; set; }


        public required string ApiKey { get; set; }

        public string? WebhookKey { get; set; }

        public string? Token { get; set; }

        public Platform Platform { get; set; }

        public ICollection<Property> Properties { get; set; }

        public ICollection<Reservation> Reservations { get; set; }


    }
}