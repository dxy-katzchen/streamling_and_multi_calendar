

using Microsoft.EntityFrameworkCore;
using Streamling.Data;
using Streamling.Model.Entities;

namespace Streamling.Repository
{
    public class ChannelRepository(StoreContext storeContext)
    {
        private readonly StoreContext _storeContext = storeContext;


        public async Task<List<Channel>> GetChannels()
        {
            return await _storeContext.Channels.AsNoTracking().ToListAsync();
        }


    }
}