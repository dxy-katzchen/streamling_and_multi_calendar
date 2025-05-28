using API.Utils.Http;
using Microsoft.EntityFrameworkCore;
using Streamling.Model.Entities;
using Streamling.Data;
using Streamling.Utils;
using Streamling.Utils.EntityUtils;

namespace Streamling.Service
{
    public class PropertyService(StoreContext context)
    {
        private readonly StoreContext _context = context;
        public async Task FlushProperties()
        {
            var hostaway_fr_properties = await GetHostawayProperties();
            var uplisting_bedbooka_properties = await GetUplistingProperties();

            await UpdateProperties(hostaway_fr_properties, "Hostaway");
            await UpdateProperties(uplisting_bedbooka_properties, "Uplisting");
        }

        public async Task<List<Property>> GetUplistingProperties()
        {
            var jsonObject = await HttpRequestHelper.SendRequestWithRetryAsync<string>(HttpMethod.Get, GlobalSettings.UplistingRequestObj_BedBooka, $"/properties");

            var newProperties = await PropertyUtils.CreatePropertiesFromUplistingJson(jsonObject);

            return newProperties;
        }

        private async Task<List<Property>> GetHostawayProperties()
        {
            var jsonObject = await HttpRequestHelper.SendRequestWithRetryAsync<string>(HttpMethod.Get, GlobalSettings.HostawayRequestObj_FR, $"/listings?limit=500");


            var newProperties = await PropertyUtils.CreatePropertiesFromHostawayJson(jsonObject);

            return newProperties;
        }

        public async Task UpdateProperties(List<Property> newProperties, string platformName)
        {
            // Filter properties by PlatformName first
            var existingProperties = await _context.Properties.Where(p => p.PlatformName == platformName).ToListAsync();

            var newPropertyIds = newProperties.Select(p => p.Id).ToHashSet();

            var existingPropertyIds = existingProperties.Select(p => p.Id).ToHashSet();

            // Add new properties
            var propertiesToAdd = newProperties.Where(p => !existingPropertyIds.Contains(p.Id)).ToList();
            _context.Properties.AddRange(propertiesToAdd);

            // Update existing properties
            foreach (var existingProperty in existingProperties)
            {
                var newProperty = newProperties.FirstOrDefault(p => p.Id == existingProperty.Id);
                if (newProperty != null && newProperty.Name != existingProperty.Name)
                {
                    existingProperty.Name = newProperty.Name;
                    _context.Properties.Update(existingProperty);
                }
            }

            // Delete properties that are not in the new data
            var propertiesToDelete = existingProperties.Where(p => !newPropertyIds.Contains(p.Id)).ToList();
            _context.Properties.RemoveRange(propertiesToDelete);

            await _context.SaveChangesAsync();
        }


    }
}