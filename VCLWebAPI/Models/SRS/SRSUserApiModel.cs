using System.Collections.Generic;
using VCLWebAPI.Models.SRS;

namespace VCLWebAPI.Models
{
    public class SRSUserApiModel
    {
        public int UserId { get; set; }
        public string UserGuid { get; set; }
        public string NameFirst { get; set; }
        public string NameLast { get; set; }
        public string Email { get; set; }

        public int FabricatorId { get; set; }
        public int AWSFabricatorId { get; set; }
        public int ADSFabricatorId { get; set; }
        public int ASSFabricatorId { get; set; }
        public int DealerId { get; set; }
        public string UserRole { get; set; }
        public FabricatorApiModel Fabricator { get; set; }
        public FabricatorApiModel AWSFabricator { get; set; }
        public FabricatorApiModel ADSFabricator { get; set; }
        public FabricatorApiModel ASSFabricator { get; set; }
        public DealerApiModel Dealer { get; set; }
        public string Password { get; set; }
    }
}