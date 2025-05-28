using System.Text.Json;
using System.Text.Json.Nodes;
using API.Utils.Http;
using Microsoft.EntityFrameworkCore;
using Streamling.Data;
using Streamling.Model.Entities;

namespace Streamling.Utils.EntityUtils
{
    public static class PropertyUtils
    {
        public async static Task<List<Property>> CreatePropertiesFromHostawayJson(JsonObject propertyJson)
        {
            var propertyDataList = propertyJson["result"]?.AsArray()!;
            List<Property> properties = [];


            foreach (var propertyData in propertyDataList)
            {
                var platformPropertyId = propertyData["id"]?.ToString()!;
                var name = propertyData["internalListingName"]?.ToString()!;
                var platformName = "Hostaway";
                var accountId = "18671";
                var id = $"{platformName}-{accountId}-{platformPropertyId}";
                Property property = new()
                {
                    Id = id,
                    PlatformPropertyId = platformPropertyId,
                    Name = name,
                    PlatformName = platformName,
                    AccountId = accountId
                };
                properties.Add(property);
            }
            return properties;
        }

        public async static Task<List<Property>> CreatePropertiesFromUplistingJson(JsonObject propertyJson)
        {
            var propertyDataList = propertyJson["data"]?.AsArray()!;
            List<Property> properties = [];

            foreach (var propertyData in propertyDataList)
            {
                var platformPropertyId = propertyData["id"]?.ToString()!;
                var name = propertyData["attributes"]?["nickname"]?.ToString()!;
                var platformName = "Uplisting";
                var accountId = GlobalSettings.UplistingRequestObj_BedBooka.UserCredential.AccountId;
                var id = $"{platformName}-{accountId}-{platformPropertyId}";
                Property property = new()
                {
                    Id = id,
                    PlatformPropertyId = platformPropertyId,
                    Name = name,
                    PlatformName = platformName,
                    AccountId = accountId
                };
                properties.Add(property);
            }
            return properties;
        }
    }
};