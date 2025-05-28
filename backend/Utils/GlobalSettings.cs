using System.Text;
using Streamling.Model;

namespace Streamling.Utils
{
    public static class GlobalSettings
    {

        public static readonly int FR_SCHEDULER_ID = 1;// FR Scheduler ID for the first scheduler - mockup
        public static readonly int FRANCHIESE_SCHEDULER_ID = 2;// FRANCHIESE Scheduler ID for the second scheduler - mockup

        public static readonly string CONNECTEAM_BASE_URL = "https://api.connecteam.com/scheduler/v1/schedulers";

        public static readonly string HOSTAWAY_BASE_URL = "https://api.hostaway.com/v1";

        public static readonly string UPLISTING_BASE_URL = "https://connect.uplisting.io";

        public static readonly RequestObj ConnectTeamRequestObj = new()
        {
            BaseURL = CONNECTEAM_BASE_URL,
            UserCredential = new RequestObj.Credential
            {
                Key = "X-API-KEY",
                Value = "xxx"
            }
        };

        public static readonly RequestObj HostawayRequestObj_FR = new()
        {
            BaseURL = HOSTAWAY_BASE_URL,
            UserCredential = new RequestObj.Credential
            {
                Key = "Authorization",
                Value = "Bearer xxx",// This is the Bearer token for the FR Hostaway account
                AccountId = "xxx"// This is the account ID for the FR Hostaway account
            }
        };
        public static readonly RequestObj UplistingRequestObj_BedBooka = new()
        {
            BaseURL = UPLISTING_BASE_URL,

            UserCredential = new RequestObj.Credential
            {
                Key = "Authorization",
                Value = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("xxx"))}",// This is the Basic Auth for the BedBooka Uplisting account
                AccountId = "xxx"// This is the account ID for the BedBooka Uplisting account
            }
        };

    }
}