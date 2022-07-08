using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using VCLWebAPI.Mappers;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Models.SRS;

namespace VCLWebAPI.Services
{
    public class DealerService
    {
        private readonly VCLDesignDBEntities _db;
        private readonly AccountService _as;

        public DealerService()
        {
            _db = new VCLDesignDBEntities();
            _as = new AccountService();
        }

        public List<DealerApiModel> GetAll()
        {
            var projectMApper = new ProjectMapper();
            List<DealerApiModel> response = _db.Dealer.ToList().Select(s => projectMApper.ProjectDbToApiModel(s)).ToList();
            return response;
        }

        public void oldDealersFinancials()
        {
            List<Dealer> allDealer = _db.Dealer.ToList();
            List<Financial> allFinancial = _db.Financial.ToList();
            var updateDealerInfoList = allDealer.Where(w => !allFinancial.Any(e => e.DealerId == w.DealerId)).ToList();
            foreach (var item in updateDealerInfoList)
            {
                CreateFinancial(item);
            }
        }

        public List<FinancialApiModel> GetFinancials()
        {
            var projectMApper = new ProjectMapper();
            return _db.Financial.ToList().Select(s => projectMApper.ProjectDbToApiModel(s)).ToList();
        }
        public FinancialApiModel GetFinance(Guid id)
        {
            var projectMApper = new ProjectMapper();
            var financial = _db.Financial.FirstOrDefault(e => e.FinancialExternalId == id);
            return projectMApper.ProjectDbToApiModel(financial);
        }

        public void CreateFinancial(Dealer dealer)
        {
            if (dealer != null)
            {
                var fin = new Financial();
                fin.FinancialExternalId = Guid.NewGuid();

                fin.DealerId = dealer.DealerId;
                fin.LineOfCredit = dealer.CreditLine;
                fin.OrdersToDate = 0;
                fin.PaidToDate = 0;
                fin.CurrentBalance = dealer.CreditLine;

                fin.CreatedBy = _as.GetCurrentUser().UserId;
                fin.CreatedOn = DateTime.Now;

                _db.Financial.Add(fin);
                _db.SaveChanges();
            }
        }

        public void UpdateFinancial(Guid id, FinancialApiModel financial)
        {
            var fin = _db.Financial.Where(e => e.FinancialExternalId == id).FirstOrDefault();
            if(fin != null)
            {
                fin.LineOfCredit = financial.LineOfCredit;
                fin.OrdersToDate = financial.OrdersToDate;
                fin.PaidToDate = financial.PaidToDate;
                fin.CurrentBalance = financial.LineOfCredit - financial.OrdersToDate + financial.PaidToDate;

                fin.ModifiedBy = _as.GetCurrentUser().UserId;
                fin.ModifiedOn = DateTime.Now;
                _db.SaveChanges();
            }
        }
        public void UpdateDealerFinancial(int dealerId, double creditLine)
        {
            var fin = _db.Financial.Where(e => e.DealerId == dealerId).FirstOrDefault();
            if (fin != null)
            {
                fin.LineOfCredit = creditLine;
                fin.CurrentBalance = fin.LineOfCredit - fin.OrdersToDate + fin.PaidToDate;
                fin.ModifiedBy = _as.GetCurrentUser().UserId;
                fin.ModifiedOn = DateTime.Now;
                _db.Entry(fin).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }
        public void UpdateDealerOrderFinancial(int dealerId, double OrdersToDate)
        {
            var fin = _db.Financial.Where(e => e.DealerId == dealerId).FirstOrDefault();
            if (fin != null)
            {
                fin.OrdersToDate += OrdersToDate;
                fin.CurrentBalance = fin.LineOfCredit - fin.OrdersToDate + fin.PaidToDate;
                fin.ModifiedBy = _as.GetCurrentUser().UserId;
                fin.ModifiedOn = DateTime.Now;
                _db.Entry(fin).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        public void Create(DealerApiModel dealer)
        {
            var add = new Address();
            add.AddressExternalId = Guid.NewGuid();
            add.Line1 = dealer.Line1;
            add.Line2 = dealer.Line2;
            add.State = dealer.State;
            add.City = dealer.City;
            add.Country = dealer.Country;
            add.County = dealer.County;
            add.PostalCode = dealer.PostalCode;
            add.CreatedBy = _as.GetCurrentUser().UserId;
            add.CreatedOn = DateTime.Now;
            add.Latitude = dealer.Latitude == null ? 0 : dealer.Latitude;
            add.Longitude = dealer.Longitude == null ? 0 : dealer.Longitude;
            _db.Address.Add(add);
            _db.SaveChanges();

            var fabricators = _db.Fabricator.ToList();
            try
            {
                var newDealer = new Dealer();
                newDealer.DealerExternalId = Guid.NewGuid();
                newDealer.Name = dealer.Name;
                newDealer.PrimaryContactEmail = dealer.PrimaryContactEmail;
                newDealer.PrimaryContactName = dealer.PrimaryContactName;
                newDealer.PrimaryContactPhone = dealer.PrimaryContactPhone;
                newDealer.AddressId = add.AddressId;
                newDealer.ADSFabricatorId = dealer.ADSFabricatorId;
                newDealer.AWSFabricatorId = dealer.AWSFabricatorId;
                newDealer.ASSFabricatorId = dealer.ASSFabricatorId;
                newDealer.Fabricator = fabricators.FirstOrDefault(e => e.FabricatorId == dealer.AWSFabricatorId);
                newDealer.Fabricator2 = fabricators.FirstOrDefault(e => e.FabricatorId == dealer.ADSFabricatorId);
                newDealer.Fabricator1 = fabricators.FirstOrDefault(e => e.FabricatorId == dealer.ASSFabricatorId);
                newDealer.CreditLine = dealer.CreditLine;
                newDealer.DefaultSalesTaxRate = dealer.DefaultSalesTax;
                _db.Dealer.Add(newDealer);
                CreateFinancial(newDealer);
                _db.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }

        public void Update(Guid guid, DealerApiModel delearApi)
        {
            var dealer = _db.Dealer.Where(u => u.DealerExternalId == guid).FirstOrDefault();
            dealer.Name = delearApi.Name;
            dealer.PrimaryContactEmail = delearApi.PrimaryContactEmail;
            dealer.PrimaryContactName = delearApi.PrimaryContactName;
            dealer.PrimaryContactPhone = delearApi.PrimaryContactPhone;
            dealer.CreditLine = delearApi.CreditLine;
            dealer.DefaultSalesTaxRate = delearApi.DefaultSalesTax;

            dealer.ADSFabricatorId = delearApi.ADSFabricatorId;
            dealer.AWSFabricatorId = delearApi.AWSFabricatorId;
            dealer.ASSFabricatorId = delearApi.ASSFabricatorId;

            dealer.Address.Line1 = delearApi.Line1;
            dealer.Address.Line2 = delearApi.Line2;
            dealer.Address.State = delearApi.State;
            dealer.Address.City = delearApi.City;
            dealer.Address.Country = delearApi.Country;
            dealer.Address.County = delearApi.County;
            dealer.Address.PostalCode = delearApi.PostalCode;
            dealer.Address.Latitude = delearApi.Latitude == null ? 0 : delearApi.Latitude;
            dealer.Address.Longitude = delearApi.Longitude == null ? 0 : delearApi.Longitude;

            dealer.Address.ModifiedBy = _as.GetCurrentUser().UserId;
            dealer.Address.ModifiedOn = DateTime.Now;
            _db.SaveChanges();

            UpdateDealerFinancial(dealer.DealerId, dealer.CreditLine);
            oldDealersFinancials();
        }

        public DealerApiModel Get(Guid guid)
        {
            var dealer = _db.Dealer.Where(u => u.DealerExternalId == guid).FirstOrDefault();
            var projectMApper = new ProjectMapper();
            return projectMApper.ProjectDbToApiModel(dealer);
        }
        public DealerApiModel GetUserDealer()
        {
            try
            {
                User currentUser = _as.GetCurrentUser();
                DealerApiModel response = new DealerApiModel();
                var user = _db.User.Where(u => u.UserGuid == currentUser.UserGuid).FirstOrDefault();
                var dealerList = _db.Dealer.ToList();
                var dealer = dealerList.Where(u => u.User.Any(a => a.UserId == user.UserId)).FirstOrDefault();
                if (dealer == null)
                {
                    return response;
                } else
                {
                    var projectMApper = new ProjectMapper();
                    response = projectMApper.ProjectDbToApiModel(dealer);
                    response.CreditUsed = GetDealerCreditBalance(dealer);
                    response.CreditLine = response.CreditLine;
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private double GetDealerCreditBalance(Dealer dealer)
        {
            var financial = dealer.Financial.Where(e => e.DealerId == dealer.DealerId).FirstOrDefault();
            if (financial == null) return 0;
            else
                return financial.CurrentBalance;
        }

        public void Delete(Guid guid)
        {
            var dealer = _db.Dealer.Where(u => u.DealerExternalId == guid).FirstOrDefault();
            if(dealer.AddressId != null)
            {
                var add = _db.Address.Where(u => u.AddressId == dealer.AddressId).FirstOrDefault();
                _db.Address.Remove(add);
                _db.SaveChanges();
            }

            var financial = _db.Financial.Where(u => u.DealerId == dealer.DealerId).FirstOrDefault();
            _db.Financial.Remove(financial);

            _db.Dealer.Remove(dealer);
            _db.SaveChanges();
        }

        public bool CanDelete(Guid guid)
        {
            var dealer = _db.Dealer.Where(u => u.DealerExternalId == guid).FirstOrDefault();
            if (dealer.User.Count > 0 || dealer.Order.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}