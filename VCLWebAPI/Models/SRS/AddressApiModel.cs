using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCLWebAPI.Models.SRS
{
    public class AddressApiModel
    {
        public int AddressId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email2 { get; set; }

        public string Address { get; set; }
        public string Apartment { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string AdditionalDetails { get; set; }
        public string AddressType { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
    }
}