using System;
using System.Collections.Generic;

namespace VCLWebAPI.Models.BPS
{
    public class BpsProjectApiModel
    {
        public int ProjectId { get; set; }
        public Guid ProjectGuid { get; set; }
        public int UserId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectLocation { get; set; }
        public List<int> ProblemIds { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public bool OrderPlaced { get; set; }
        public Nullable<System.DateTime> OrderPlacedCreatedOn { get; set; }
        public string OrderStatus { get; set; }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }

        public BpsProjectApiModel()
        {
            UserId = 0;
            ProjectName = String.Empty;
            ProjectLocation = String.Empty;
            OrderPlaced = false;
            OrderPlacedCreatedOn = null;
            OrderStatus = "";
        }
    }
}