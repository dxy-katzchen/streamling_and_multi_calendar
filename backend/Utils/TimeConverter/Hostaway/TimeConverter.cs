using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Streamling.Utils.TimeConverter.Hostaway
{
    public static class TimeConverter
    {
        public static int ConvertNzdtToUnixTimestamp10am(string nzdtDateString)
        {
            // Parse the date string to a DateTime object
            DateTime nzdtDateTime = DateTime.ParseExact(nzdtDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);

            // Set the time to 10:00:00 AM
            nzdtDateTime = nzdtDateTime.AddHours(10);

            // Convert to DateTimeOffset in NZDT timezone
            DateTimeOffset nzdtDateTimeOffset = new DateTimeOffset(nzdtDateTime, TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time").GetUtcOffset(nzdtDateTime));

            // Convert to Unix timestamp
            int unixTimestamp = (int)nzdtDateTimeOffset.ToUnixTimeSeconds();

            return unixTimestamp;
        }

        public static string ConvertNzdtToUTCString(string nzdtDateString, int AddHours)
        {
            DateTime nzLocalTime = DateTime.ParseExact(nzdtDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            // Set the time to 10:00 AM in New Zealand local time
            nzLocalTime = nzLocalTime.AddHours(AddHours);

            // Convert to UTC time
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(nzLocalTime, TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time"));

            // Format the UTC time in ISO 8601 format
            return utcTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        public static string GetTimeStringOneYearAgo()
        {
            var oneYearAgo = DateTime.Now.AddYears(-1);
            return oneYearAgo.ToString("yyyy-MM-dd");
        }

        public static string GetTimeStringTwoYearAfter()
        {
            var twoYearAfter = DateTime.Now.AddYears(2);
            return twoYearAfter.ToString("yyyy-MM-dd");
        }

    }
}