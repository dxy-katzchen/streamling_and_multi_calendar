using Microsoft.EntityFrameworkCore;
using Streamling.Data;
using Streamling.Model.Entities;

namespace Streamling.Repository
{
    public class PropertyRepository(StoreContext storeContext)
    {
        private readonly StoreContext _storeContext = storeContext;

        public async Task<List<Property>> GetProperties()
        {

            return await _storeContext.Properties.AsNoTracking().ToListAsync();
        }

        public async Task<List<Property>> GetPropertiesByName()
        {

            return await _storeContext.Properties.OrderBy(p => p.PlatformName).ToListAsync();
        }




    }
}