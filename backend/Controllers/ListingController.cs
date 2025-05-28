using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Streamling.Data;
using Streamling.Service;
using Streamling.Utils.EntityUtils;

namespace Streamling.Controllers.Hostaway
{
    public class ListingController : BaseApiController
    {
        private readonly PropertyService _propertyService;

        public ListingController(PropertyService propertyService)
        {
            _propertyService = propertyService;

        }

        [HttpGet]
        public async Task<ActionResult> GetAllPropertiesAsync()
        {
            await _propertyService.FlushProperties();
            return Ok();
        }
    }
}