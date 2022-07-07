using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Models.SRS;
using VCLWebAPI.Mappers;

namespace VCLWebAPI.Services
{
    public class FabricatorService
    {
        private readonly VCLDesignDBEntities _db;
        private readonly AccountService _as;

        public FabricatorService()
        {
            _db = new VCLDesignDBEntities();
            _as = new AccountService();
        }

        public List<FabricatorApiModel> GetAll()
        {
            var fabricators =  _db.Fabricator.ToList();
            List<FabricatorApiModel> response = new List<FabricatorApiModel>();
            var projectMApper = new ProjectMapper();
            foreach (var fab in fabricators)
            {
                response.Add(projectMApper.ProjectDbToApiModel(fab));
            }
            return response;
        }
        public void Create(FabricatorApiModel fab)
        {
            var add = new Address();
            add.AddressExternalId = Guid.NewGuid();
            add.Line1 = fab.Line1;
            add.Line2 = fab.Line2;
            add.State = fab.State;
            add.City = fab.City;
            add.Country = fab.Country;
            add.County = fab.County;
            add.PostalCode = fab.PostalCode;
            add.CreatedBy = _as.GetCurrentUser().UserId;
            add.CreatedOn = DateTime.Now;
            add.Longitude = fab.Longitude == null ? 0 : fab.Longitude;
            add.Latitude = fab.Latitude == null ? 0 : fab.Latitude;
            _db.Address.Add(add);
            _db.SaveChanges();


            var fabricator = new Fabricator();
            fabricator.FabricatorExternalId = Guid.NewGuid();
            fabricator.Name = fab.Name;
            fabricator.PrimaryContactEmail = fab.PrimaryContactEmail;
            fabricator.PrimaryContactName = fab.PrimaryContactName;
            fabricator.PrimaryContactPhone = fab.PrimaryContactPhone;

            fabricator.SupportsAWS = fab.SupportsAWS;
            fabricator.SupportsADS = fab.SupportsADS;
            fabricator.SupportsASS = fab.SupportsASS;
            fabricator.AddressId = add.AddressId;
            _db.Fabricator.Add(fabricator);
            _db.SaveChanges();
        }

        public string ValidateUpdate(Guid guid, FabricatorApiModel fab)
        {
            string result = "";
            var fabricator = _db.Fabricator.Where(u => u.FabricatorExternalId == guid).FirstOrDefault();
            var dealers = _db.Dealer.ToList();
            if (fabricator.SupportsAWS == 1 && fabricator.SupportsAWS != fab.SupportsAWS) { 
                var AWS = dealers.Where(e => e.AWSFabricatorId == fabricator.FabricatorId).ToList();
                if(AWS.Count > 0)
                    result += "Support product type 'AWS' is in use by " + AWS.Count + " Dealer. </br>";
            }
            if (fabricator.SupportsADS == 1 && fabricator.SupportsADS != fab.SupportsADS) { 
                var ADS = dealers.Where(e => e.ADSFabricatorId == fabricator.FabricatorId).ToList();
                if (ADS.Count > 0)
                    result += "Support product type 'ADS' is in use by " + ADS.Count + " Dealer. </br>";
            }
            if (fabricator.SupportsASS == 1 && fabricator.SupportsASS != fab.SupportsASS) { 
                var ASS = dealers.Where(e => e.ASSFabricatorId == fabricator.FabricatorId).ToList();
                if (ASS.Count > 0)
                    result += "Support product type 'ASS' is in use by " + ASS.Count + " Dealer. </br>";
            }
            return result;
        }

        public string Update(Guid guid, FabricatorApiModel fab)
        {
            var fabricator = _db.Fabricator.Where(u => u.FabricatorExternalId == guid).FirstOrDefault();
            var isValid = ValidateUpdate(guid, fab);
            if (isValid == "") {
                fabricator.Name = fab.Name;
                fabricator.PrimaryContactEmail = fab.PrimaryContactEmail;
                fabricator.PrimaryContactName = fab.PrimaryContactName;
                fabricator.PrimaryContactPhone = fab.PrimaryContactPhone;
                fabricator.SupportsAWS = fab.SupportsAWS;
                fabricator.SupportsADS = fab.SupportsADS;
                fabricator.SupportsASS = fab.SupportsASS;
                fabricator.Address.Line1 = fab.Line1;
                fabricator.Address.Line2 = fab.Line2;
                fabricator.Address.State = fab.State;
                fabricator.Address.City = fab.City;
                fabricator.Address.Country = fab.Country;
                fabricator.Address.County = fab.County;
                fabricator.Address.PostalCode = fab.PostalCode;
                fabricator.Address.Latitude = fab.Latitude == null ? 0 : fab.Latitude;
                fabricator.Address.Longitude = fab.Longitude == null ? 0 : fab.Longitude;
                fabricator.Address.ModifiedBy = _as.GetCurrentUser().UserId;
                fabricator.Address.ModifiedOn = DateTime.Now;
                _db.SaveChanges();
                return "";
            }
            else {
                return isValid;
            }
        }

        public FabricatorApiModel Get(Guid guid)
        {
            var fabricator = _db.Fabricator.Where(u => u.FabricatorExternalId == guid).FirstOrDefault();
            FabricatorApiModel response = new FabricatorApiModel();
            var projectMApper = new ProjectMapper();
            response = projectMApper.ProjectDbToApiModel(fabricator);
            return response;
        }

        public void Delete(Guid guid)
        {
            var fabricator = _db.Fabricator.Where(u => u.FabricatorExternalId == guid).FirstOrDefault();
            if(fabricator.AddressId != null)
            {
                var add = _db.Address.Where(u => u.AddressId == fabricator.AddressId).FirstOrDefault();
                if(add.Fabricator.Count == 1)
                {
                    _db.Address.Remove(add);
                    _db.SaveChanges();
                }
            }
            _db.Fabricator.Remove(fabricator);
            _db.SaveChanges();
        }

        public bool CanDelete(Guid guid)
        {
            var fabricator = _db.Fabricator.Where(u => u.FabricatorExternalId == guid).FirstOrDefault();
            var dealers = _db.Dealer.Where(e => e.AWSFabricatorId == fabricator.FabricatorId
            || e.ADSFabricatorId == fabricator.FabricatorId || e.ASSFabricatorId == fabricator.FabricatorId).ToList();
            if (fabricator.User.Count > 0 || dealers.Count > 0)
            {
                return false;
            }
            else
                return true;
        }
    }
}