using System.Collections.Generic;

namespace VCLWebAPI.Models
{
    public class UserApiModel
    {
        public int UserId { get; set; }
        public string UserGuid { get; set; }
        public string UserName { get; set; }
        public string NameFirst { get; set; }
        public string NameLast { get; set; }
        public string Email { get; set; }
        public string Language { get; set; }
        public string Company { get; set; }
        public string Hash { get; set; }
        public byte[] Salt { get; set; }
        public List<string> AccessRoles { get; set; }

        public List<AccessRoleApiModel> AccessRole { get; set; }

        public List<FeatureApiModel> Features { get; set; }

        public UserApiModel()
        {
            AccessRole = new List<AccessRoleApiModel>();
            AccessRoles = new List<string>();
        }
    }
}