using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.SRS
{
    public class OrderApiModel
    {
        public int OrderId { get; set; }
        public System.Guid OrderExternalId { get; set; }
        public int ProjectId { get; set; }
        public int DealerId { get; set; }
        public string DealerName { get; set; }
        public int FabricatorId { get; set; }
        public string FabricatorName { get; set; }
        public Nullable<int> ParentOrderId { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public System.DateTime? ProjectCreatedOn { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string Notes { get; set; }
        public int ShippingAddressId { get; set; }
        public double ShippingCost { get; set; }
        public double Tax { get; set; }
        public double Total { get; set; }
        public double Discount { get; set; }
        public Nullable<int> DiscountPercentage { get; set; }
        public Nullable<sbyte> ShippingMethod { get; set; }


        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public string PostalCode { get; set; }
        public string AdditionalDetails { get; set; }
        public string AddressType { get; set; }
        public string FromAddress { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }

        public string Current_Status { get; set; }
        public string Current_Process { get; set; }
        public List<OrderStatusApiModel> OrderStatus { get; set; }
        public List<OrderDetailsApiModel> OrderDetails { get; set; }
        public List<OrderApiModel> SubOrder { get; set; }
        public OrderApiModel(Order dbModel)
        {
            if (dbModel != null)
            {
                OrderId = dbModel.OrderId;
                OrderExternalId = dbModel.OrderExternalId;
                ProjectId = dbModel.ProjectId;
                DealerId = dbModel.DealerId;
                DealerName = dbModel.Dealer == null ? "" : dbModel.Dealer.Name;
                FabricatorId = dbModel.Dealer.Fabricator.FabricatorId;
                FabricatorName = dbModel.Dealer.Fabricator == null ? "" : dbModel.Dealer.Fabricator.Name;
                
                ParentOrderId = dbModel.ParentOrderId;
                ProjectCreatedOn = dbModel.BpsProject == null ? null : dbModel.BpsProject.CreatedOn;
                CreatedBy = dbModel.CreatedBy;
                CreatedByName = dbModel.User != null ? dbModel.User.NameFirst + " " + dbModel.User.NameLast : "";
                CreatedOn = dbModel.CreatedOn;
                ModifiedBy = dbModel.ModifiedBy;
                ModifiedByName = dbModel.User1 != null ? dbModel.User1.NameFirst + " " + dbModel.User1.NameLast : "";
                ModifiedOn = dbModel.ModifiedOn;

                Notes = dbModel.Notes;
                ShippingAddressId = dbModel.ShippingAddressId;
                ShippingCost = dbModel.ShippingCost;
                Tax = dbModel.Tax;
                Total = dbModel.Total;
                Discount = dbModel.Discount;
                DiscountPercentage = dbModel.DiscountPercentage;
                ShippingMethod = dbModel.ShippingMethod;

                Line1 = dbModel.Address.Line1;
                Line2 = dbModel.Address.Line2;
                State = dbModel.Address.State;
                City = dbModel.Address.City;
                PostalCode = dbModel.Address.PostalCode;
                Country = dbModel.Address.Country;
                County = dbModel.Address.County;
                Latitude = dbModel.Address.Latitude;
                Longitude = dbModel.Address.Longitude;
                AdditionalDetails = dbModel.Address.AdditionalDetails;
                AddressType = dbModel.Address.AddressType;
                FromAddress = buildAddress(dbModel.Dealer.Fabricator.Address);
                Current_Process = dbModel.Order_Status.Count == 0 ? Utils.ApiEnums.GetStringValue(Utils.ApiEnums.OrderStatus.Order_Placed) : dbModel.Order_Status.OrderByDescending(o => o.StatusModifiedOn).FirstOrDefault().SLU_Status.Description;
                Current_Status = dbModel.Order_Status.Count == 0 ? Utils.ApiEnums.GetStringValue(Utils.ApiEnums.OrderStatus.Order_Placed) : dbModel.Order_Status.OrderByDescending(o => o.StatusModifiedOn).FirstOrDefault().SLU_Status.Description;

                OrderStatus = dbModel.Order_Status.OrderByDescending(o => o.StatusModifiedOn).Select(s => new OrderStatusApiModel(s)).ToList();
                OrderDetails = dbModel.OrderDetails.Select(s => new OrderDetailsApiModel(s)).ToList();
                SubOrder = new List<OrderApiModel>();
                foreach (var suborderOrder in dbModel.Order1)
                {
                    SubOrder.Add(new OrderApiModel(suborderOrder));
                }
            }
        }
        public OrderApiModel()
        {
        }
        public string buildAddress(Address add) {

            string address = "";
            address += add.Line1 == null ? "" : add.Line1 + ", ";
            address += add.Line2 == null ? "" : add.Line2 + ", ";
            address += add.State == null ? "" : add.State + " - ";
            address += add.PostalCode == null ? "" : add.PostalCode + ", ";
            address += add.City == null ? "" : add.City + ", ";
            address += add.County == null ? "" : add.County + ", ";
            address += add.Country == null ? "" : add.Country + ".";
            return address;
        }
    }

    public class OrderDetailsApiModel
    {
        public int OrderDetailId { get; set; }
        public System.Guid OrderDetailExternalId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public Nullable<System.Guid> ProductGuid { get; set; }
        public string DesignURL { get; set; }
        public string JsonURL { get; set; }
        public string ProposalURL { get; set; }
        public string BomURL { get; set; }        
        public string OrderDetailscol { get; set; }
        public string UnitPrice { get; set; }
        public Nullable<double> Qty { get; set; }
        public Nullable<double> SubTotal { get; set; }
        public string AdditionalDetails { get; set; }

        public OrderDetailsApiModel(OrderDetails dbModel)
        {
            if (dbModel != null)
            {
                OrderDetailId = dbModel.OrderDetailId;
                OrderDetailExternalId = dbModel.OrderDetailExternalId;
                OrderId = dbModel.OrderId;
                ProductId = dbModel.ProductId;
                ProductGuid = dbModel.BpsUnifiedProblem.ProblemGuid;
                DesignURL = dbModel.DesignURL;
                JsonURL = dbModel.JsonURL;
                ProposalURL = dbModel.ProposalURL;
                BomURL = dbModel.BomURL;
                OrderDetailscol = dbModel.OrderDetailscol;
                UnitPrice = dbModel.UnitPrice;
                Qty = dbModel.Qty;
                SubTotal = dbModel.SubTotal;
                AdditionalDetails = dbModel.AdditionalDetails;
            }
        }
    }

    public partial class OrderStatusApiModel
    {
        public int OrderId { get; set; }
        public int StatusId { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public System.DateTime StatusModifiedOn { get; set; }
        public Nullable<int> StatusModifiedBy { get; set; }
        public OrderStatusApiModel(Order_Status dbModel)
        {
            if (dbModel != null)
            {
                OrderId = dbModel.OrderId;
                StatusId = dbModel.StatusId;
                StatusCode = dbModel.SLU_Status.Code;
                StatusDescription = dbModel.SLU_Status.Description;
                StatusModifiedOn = dbModel.StatusModifiedOn;
                StatusModifiedBy = dbModel.StatusModifiedBy;
            }
        }
    }

    public class OrdersByDealer {
        public int DealerId { get; set; }
        public string DealerName { get; set; }
        public int Month { get; set; }
        public int TotalOrders { get; set; }
        public List<int> MonthOrders { get; set; }
        public List<OrderApiModel> Orders { get; set; }
    }
}