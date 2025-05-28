using System.ComponentModel.DataAnnotations.Schema;

namespace Streamling.Model.Entities
{
    public class Property
    {
        // public string Id => $"{PlatformName}-{AccountId}-{PlatformPropertyId}";
        public required string Id { get; set; }

        [Column(TypeName = "varchar(191)")]
        public required string AccountId { get; set; }
        public required string Name { get; set; }
        [Column(TypeName = "varchar(191)")]
        public required string PlatformPropertyId { get; set; }
        [Column(TypeName = "varchar(191)")]
        public string PlatformName { get; set; }

        public Platform Platform { get; set; }
        public Channel Channel { get; set; }
        public ICollection<Reservation> Reservations { get; set; }

    }
}