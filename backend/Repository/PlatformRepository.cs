
using Microsoft.EntityFrameworkCore;
using Streamling.Data;

namespace Streamling.Repository
{
    public class PlatformRepository(StoreContext storeContext)
    {
        private readonly StoreContext _storeContext = storeContext;


    }
}