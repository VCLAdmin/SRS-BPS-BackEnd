using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCLWebAPI.Models.Account
{
    public class DecryptedToken
    {
        [JsonProperty(PropertyName = "security-token")]
        public string securitytoken { get; set; }

        public string time { get; set; }
        public string name { get; set; }
        public string givenName { get; set; }
        public string email { get; set; }
        public string loginName { get; set; }
        public string userType { get; set; }
        public List<string> additionalRoles { get; set; }
    }
}