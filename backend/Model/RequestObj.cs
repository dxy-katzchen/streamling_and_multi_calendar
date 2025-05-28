

namespace Streamling.Model
{
    public class RequestObj
    {
        public string BaseURL { get; set; }

        public Credential UserCredential { get; set; }

        public class Credential
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public string AccountId { get; set; }
        }
    }
}