

using Streamling.Model.Entities;

namespace Streamling.Data
{
    public class DBInitializer
    {
        public static void Initialize(StoreContext context)
        {
            context.Database.EnsureCreated();

            // Look for any properties.
            if (context.Properties.Any() || context.Channels.Any() || context.Platforms.Any() || context.Reservations.Any())
            {
                return;
            }

            var Platforms = new Platform[]
            {
            new() { Name = "Hostaway" },
            new() { Name = "Uplisting" },
            };
            var Channels = new Channel[]
            {
            new() { Name = "FR", AccountId = "xx", PlatformName = "Hostaway",ApiKey="xx", Token="xx"},
            };

            context.Platforms.AddRange(Platforms);
            context.Channels.AddRange(Channels);

            context.SaveChanges();

        }
    }
}