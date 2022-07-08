using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCLWebAPI.Models;
using VCLWebAPI.Models.BPS;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Models.SRS;
using VCLWebAPI.Services;

namespace VCLWebAPI.Mappers
{
    public class ProjectMapper
    {
        public Address ApiModelToAddressDb(OrderApiModel orderApi)
        {
            var add = new Address();
            add.ProjectId = orderApi.ProjectId;
            add.Line1 = orderApi.Line1;
            add.Line2 = orderApi.Line2;
            add.State = orderApi.State;
            add.City = orderApi.City;
            add.Country = orderApi.Country;
            add.County = orderApi.County;
            add.PostalCode = orderApi.PostalCode;
            add.Latitude = orderApi.Latitude == null ? 0 : orderApi.Latitude;
            add.Longitude = orderApi.Longitude == null ? 0 : orderApi.Longitude;
            add.AdditionalDetails = orderApi.AdditionalDetails;
            add.AddressType = orderApi.AddressType;
            return add;
        }

        public Order ApiModelToOrderDb(OrderApiModel orderApiList)
        {
            Order order = new Order();
            order.ProjectId = orderApiList.ProjectId;
            order.ParentOrderId = orderApiList.ParentOrderId;
            order.ShippingAddressId = orderApiList.ShippingAddressId;
            order.ShippingMethod = orderApiList.ShippingMethod;
            if (orderApiList.ParentOrderId == null)
            {
                order.Notes = orderApiList.Notes;
                order.ShippingCost = orderApiList.ShippingCost;
                order.Tax = orderApiList.Tax;
                order.Total = orderApiList.Total;
                order.Discount = orderApiList.Discount;
                order.DiscountPercentage = orderApiList.DiscountPercentage;
            }
            return order;
        }

        public OrderDetails ApiModelToOrderDetailsDb(OrderDetailsApiModel orderApi)
        {
            var od = new OrderDetails();
            od.OrderDetailId = orderApi.OrderDetailId;
            od.OrderDetailExternalId = orderApi.OrderDetailExternalId;
            od.OrderId = orderApi.OrderId;
            od.ProductId = orderApi.ProductId;
            od.DesignURL = orderApi.DesignURL;
            od.BomURL = orderApi.BomURL;
            od.JsonURL = orderApi.JsonURL;
            od.ProposalURL = orderApi.ProposalURL;
            od.OrderDetailscol = orderApi.OrderDetailscol;
            od.UnitPrice = orderApi.UnitPrice;
            od.Qty = orderApi.Qty;
            od.SubTotal = orderApi.SubTotal;
            od.AdditionalDetails = orderApi.AdditionalDetails;
            return od;
        }

        public BpsUnifiedProblemApiModel BpsUnifiedProblemDbToApiModel(BpsUnifiedProblem dbModel)
        {
            var model = new BpsUnifiedProblemApiModel(dbModel);
            if (dbModel.OrderDetails.Count > 0)
            {
                model.OrderPlacedCreatedOn = dbModel.OrderDetails.First().Order.CreatedOn;
                model.OrderPlaced = true;
            }
            return model;
        }

        public BpsUnifiedProblemApiModelLite BpsUnifiedProblemDbToApiModelLite(BpsUnifiedProblem dbModel)
        {
            return new BpsUnifiedProblemApiModelLite(dbModel);
        }
        public string GetProcessStatus(string status)
        {
            string results = "";
            switch (status)
            {
                case "Completed":           case "Delivered":
                    results = Utils.ApiEnums.GetStringValue(Utils.ApiEnums.OrderStatus.Completed); break;
                case "Cancelled": results = Utils.ApiEnums.GetStringValue(Utils.ApiEnums.OrderStatus.Cancelled); break;

                case "Shipped":             
                case "Order_Placed":        case "Order Placed":
                case "In_Pre_Production":   case "In Pre Production": 
                case "In_Fabrication":      case "In Fabrication":
                case "In_Assembly":         case "In Assembly":
                    results = Utils.ApiEnums.GetStringValue(Utils.ApiEnums.OrderStatus.In_Process); break;
            }
            return results;
        }
        

        public FinancialApiModel ProjectDbToApiModel(Financial dbModel)
        {
            FinancialApiModel apiModel = new FinancialApiModel
            {
                FinancialId = dbModel.FinancialId,
                FinancialExternalId = dbModel.FinancialExternalId,
                DealerId = dbModel.DealerId,
                DealerName = dbModel.Dealer.Name,
                LineOfCredit = dbModel.LineOfCredit,
                OrdersToDate = dbModel.OrdersToDate,
                PaidToDate = dbModel.PaidToDate,
                CurrentBalance = dbModel.CurrentBalance,

                CreatedByName = dbModel.User.UserName,
                CreatedBy = dbModel.CreatedBy,
                CreatedOn = dbModel.CreatedOn,
                ModifiedByName = dbModel.User1 != null ? dbModel.User1.UserName : null,
                ModifiedBy = dbModel.ModifiedBy,
                ModifiedOn = dbModel.ModifiedOn,

            };
            return apiModel;
        }
        public BpsProjectApiModel ProjectDbToApiModel(BpsProject dbModel)
        {
            BpsProjectService projectService = new BpsProjectService();
            BpsProjectApiModel apiModel = new BpsProjectApiModel
            {
                ProjectId = dbModel.ProjectId,
                ProjectGuid = dbModel.ProjectGuid,
                UserId = dbModel.UserId,
                ProjectName = dbModel.ProjectName,
                ProjectLocation = dbModel.ProjectLocation,
                CreatedOn = dbModel.CreatedOn,
                ProblemIds = projectService.GetProblemIdsForProject(dbModel.ProjectGuid)
            };
            if (dbModel.Address != null)
            {
                apiModel.Line1 = dbModel.Address.Line1;
                apiModel.Line2 = dbModel.Address.Line2;
                apiModel.State = dbModel.Address.State;
                apiModel.City = dbModel.Address.City;
                apiModel.Country = dbModel.Address.Country;
                apiModel.County = dbModel.Address.County;
                apiModel.PostalCode = dbModel.Address.PostalCode;
                apiModel.Latitude = dbModel.Address.Latitude;
                apiModel.Longitude = dbModel.Address.Longitude;
            }

            if (dbModel.Order.Count > 0)
            {
                apiModel.OrderPlacedCreatedOn = dbModel.Order.First().CreatedOn;
                var ordersList = dbModel.Order;
                var completed = "In Process";

                if (ordersList.Count > 1) {
                    ordersList = ordersList.Where(e => e.ParentOrderId != null).ToList();
                }
                
                foreach (var item in ordersList)
                {
                    var sl = item.Order_Status.Where(e => e.StatusId == 6).ToList();
                    if (sl.Count == 1)
                        completed = "Completed";
                    else
                    {
                        completed = "In Process";
                        break;
                    }
                }

                apiModel.OrderStatus = completed;
                apiModel.OrderPlaced = true; 
            }
            return apiModel;
        }

        public DealerApiModel ProjectDbToApiModel(Dealer dbModel)
        {
            DealerApiModel apiModel = new DealerApiModel
            {
                DealerId = dbModel.DealerId,
                DealerGuid = dbModel.DealerExternalId,
                Name = dbModel.Name,
                PrimaryContactName = dbModel.PrimaryContactName,
                PrimaryContactEmail = dbModel.PrimaryContactEmail,
                PrimaryContactPhone = dbModel.PrimaryContactPhone,
                CreditLine = dbModel.CreditLine,
                DefaultSalesTax = dbModel.DefaultSalesTaxRate,

                AWSFabricatorId = dbModel.AWSFabricatorId,
                ADSFabricatorId = dbModel.ADSFabricatorId,
                ASSFabricatorId = dbModel.ASSFabricatorId,

                AWSFabricator = ProjectDbToApiModel(dbModel.Fabricator2),
                ADSFabricator = ProjectDbToApiModel(dbModel.Fabricator),
                ASSFabricator = ProjectDbToApiModel(dbModel.Fabricator1),
            };

            if (dbModel.AddressId != null)
            {
                apiModel.Line1 = dbModel.Address.Line1;
                apiModel.Line2 = dbModel.Address.Line2;
                apiModel.PostalCode = dbModel.Address.PostalCode;
                apiModel.City = dbModel.Address.City;
                apiModel.State = dbModel.Address.State;
                apiModel.Country = dbModel.Address.Country;
                apiModel.County = dbModel.Address.County;
                apiModel.Latitude = dbModel.Address.Latitude;
                apiModel.Longitude = dbModel.Address.Longitude;
            }

            return apiModel;
        }

        public async Task<SRSUserApiModel> ProjectDbToApiModel(Dealer dealer, User dbModel)
        {
            SRSUserApiModel apiModel = new SRSUserApiModel
            {
                UserGuid = dbModel.UserGuid.ToString(),
                UserId = dbModel.UserId,
                NameFirst = dbModel.NameFirst,
                NameLast = dbModel.NameLast,
                Email = dbModel.Email
            };
            apiModel.DealerId = dealer.DealerId;
            apiModel.Dealer = ProjectDbToApiModel(dealer);

            apiModel.AWSFabricatorId = dealer.AWSFabricatorId;
            apiModel.AWSFabricator = ProjectDbToApiModel(dealer.Fabricator2);
            apiModel.ADSFabricatorId = dealer.ADSFabricatorId;
            apiModel.ADSFabricator = ProjectDbToApiModel(dealer.Fabricator);
            apiModel.ASSFabricatorId = dealer.ASSFabricatorId;
            apiModel.ASSFabricator = ProjectDbToApiModel(dealer.Fabricator1);

            AccountService accountService = new AccountService();
            string role = await accountService.GetRoleForSRS(dbModel.Email);
            if (role.Contains("-"))
            {
                var roleTemp = role.Split('-');
                apiModel.UserRole = roleTemp[1];
            }
            else
            {
                apiModel.UserRole = role;
            }

            return apiModel;
        }

        public FabricatorApiModel ProjectDbToApiModel(Fabricator dbModel)
        {
            FabricatorApiModel apiModel = new FabricatorApiModel
            {
                FabricatorId = dbModel.FabricatorId,
                FabricatorGuid = dbModel.FabricatorExternalId,
                Name = dbModel.Name,
                PrimaryContactName = dbModel.PrimaryContactName,
                PrimaryContactEmail = dbModel.PrimaryContactEmail,
                PrimaryContactPhone = dbModel.PrimaryContactPhone,
                SupportsAWS = dbModel.SupportsAWS,
                SupportsADS = dbModel.SupportsADS,
                SupportsASS = dbModel.SupportsASS,
            };

            if (dbModel.AddressId != null)
            {
                apiModel.Line1 = dbModel.Address.Line1;
                apiModel.Line2 = dbModel.Address.Line2;
                apiModel.PostalCode = dbModel.Address.PostalCode;
                apiModel.City = dbModel.Address.City;
                apiModel.State = dbModel.Address.State;
                apiModel.Country = dbModel.Address.Country;
                apiModel.County = dbModel.Address.County;
                apiModel.Latitude = dbModel.Address.Latitude;
                apiModel.Longitude = dbModel.Address.Longitude;
            }

            return apiModel;
        }

        public SRSUserApiModel ProjectDbToApiModel(Fabricator fab, User dbModel)
        {
            SRSUserApiModel apiModel = new SRSUserApiModel
            {
                UserGuid = dbModel.UserGuid.ToString(),
                UserId = dbModel.UserId,
                NameFirst = dbModel.NameFirst,
                NameLast = dbModel.NameLast,
                Email = dbModel.Email
            };
            apiModel.FabricatorId = fab.FabricatorId;
            apiModel.Fabricator = ProjectDbToApiModel(fab);
            return apiModel;
        }
        
        public OrderApiModel ProjectDbToApiModel(Order order)
        {
            if (order == null) { return new OrderApiModel(); }
            else
            {
                var orderApi = new OrderApiModel(order);
                orderApi.Current_Process = GetProcessStatus(orderApi.Current_Status);
                return orderApi;
            }
        }

        public OrderStatusApiModel ProjectDbToApiModel(Order_Status orderStatus)
        {
            return new OrderStatusApiModel(orderStatus);
        }

        public Order_Status ApiModelToOrderStatusDb(int orderId,  OrderApiModel apiModel, List<SLU_Status> sluStatusList) {
            if (apiModel.Current_Status == null) apiModel.Current_Status = Utils.ApiEnums.GetStringValue(Utils.ApiEnums.OrderStatus.Order_Placed);
            var orderStatus = new Order_Status();
            orderStatus.OrderId = orderId;
            orderStatus.StatusModifiedOn = DateTime.Now;
            orderStatus.StatusId = sluStatusList.Where(e => e.Code == apiModel.Current_Status || e.Description == apiModel.Current_Status).FirstOrDefault().StatusId;
            return orderStatus;
        }
        

        //public BpsProblem ProblemDbToDbModel(BpsProblem problem)
        //{
        //    BpsProjectService projectService = new BpsProjectService();
        //    BpsProjectApiModel project = projectService.GetProjectById(problem.ProjectId);
        //    BpsProblem bpsProblem = new BpsProblem
        //    {
        //        ProblemGuid = problem.ProblemGuid,
        //        ProjectId = project.ProjectId,
        //        ProblemName = problem.ProblemName,
        //        SystemModel = problem.SystemModel,
        //        SystemName = problem.SystemName,
        //        SightlineArticleNumber = problem.SightlineArticleNumber,
        //        IntermediateArticleNumber = problem.IntermediateArticleNumber,
        //        WallType = problem.WallType,
        //        WallHeight = problem.WallHeight,
        //        WallWidth = problem.WallWidth,
        //        RoomArea = problem.RoomArea,
        //        WindLoad = problem.WindLoad,
        //        EngineeringStandard = problem.EngineeringStandard,
        //        BuildingLength = problem.BuildingLength,
        //        BuildingWidth = problem.BuildingWidth,
        //        BuildingRiskCategory = problem.BuildingRiskCategory,
        //        WindSpeed = problem.WindSpeed,
        //        ExposureCategory = problem.ExposureCategory,
        //        WindowWidth = problem.WindowWidth,
        //        WindowHeight = problem.WindowHeight,
        //        WindowElevation = problem.WindowElevation,
        //        WindowZone = problem.WindowZone,
        //        RelativeHumidity = problem.RelativeHumidity,
        //        SystemImage = problem.SystemImage,
        //        GlassConfigurations = problem.GlassConfigurations,
        //        CreatedOn = problem.CreatedOn,
        //        ModifiedOn = problem.ModifiedOn,
        //        PhysicsTypeAcoustic = problem.PhysicsTypeAcoustic,
        //        PhysicsTypeStructure = problem.PhysicsTypeStructure,
        //        PhysicsTypeThermal = problem.PhysicsTypeThermal,
        //        ProductTypeId = problem.ProductTypeId,
        //        OperabilityConfigurations = problem.OperabilityConfigurations,
        //        BuildingHeight = problem.BuildingHeight,
        //        TerrainCategory = problem.TerrainCategory,
        //        VentFrameArticleNumber = problem.VentFrameArticleNumber,
        //        CustomArticles = problem.CustomArticles,
        //        WindZone = problem.WindZone,
        //        GlazingGasketCombination = problem.GlazingGasketCombination,
        //        Alloys = problem.Alloys,
        //        InsulatingBar = problem.InsulatingBar,
        //        PermissibleDeflection = problem.PermissibleDeflection,
        //        PermissibleVerticalDeflection = problem.PermissibleVerticalDeflection,
        //        UnifiedObjectModel = problem.UnifiedObjectModel,
        //        AcousticReportUrl = problem.AcousticReportUrl,
        //        StructuralReportUrl = problem.StructuralReportUrl,
        //        ThermalReportUrl = problem.ThermalReportUrl,
        //        AcousticResults = problem.AcousticResults,
        //        StructuralResults = problem.StructuralResults,
        //        ThermalResults = problem.ThermalResults
        //    };

        //    return bpsProblem;
        //}

        //public BpsProblemApiModel ProblemDbToApiModel(BpsProblem problem)
        //{
        //    BpsProjectService projectService = new BpsProjectService();
        //    BpsProjectApiModel project = projectService.GetProjectById(problem.ProjectId);
        //    BpsProblemApiModel bpsProblem = new BpsProblemApiModel
        //    {
        //        ProblemGuid = problem.ProblemGuid,
        //        ProjectGuid = project.ProjectGuid,
        //        ProblemName = problem.ProblemName,
        //        ProjectName = project.ProjectName,
        //        ProjectLocation = project.ProjectLocation,
        //        SystemModel = problem.SystemModel,
        //        SystemName = problem.SystemName,
        //        SightlineArticleNumber = problem.SightlineArticleNumber,
        //        IntermediateArticleNumber = problem.IntermediateArticleNumber,
        //        WallType = problem.WallType,
        //        WallHeight = problem.WallHeight,
        //        WallWidth = problem.WallWidth,
        //        RoomArea = problem.RoomArea,
        //        WindLoad = problem.WindLoad,
        //        EngineeringStandard = problem.EngineeringStandard,
        //        BuildingLength = problem.BuildingLength,
        //        BuildingWidth = problem.BuildingWidth,
        //        BuildingRiskCategory = problem.BuildingRiskCategory,
        //        WindSpeed = problem.WindSpeed,
        //        ExposureCategory = problem.ExposureCategory,
        //        WindowWidth = problem.WindowWidth,
        //        WindowHeight = problem.WindowHeight,
        //        WindowElevation = problem.WindowElevation,
        //        WindowZone = problem.WindowZone,
        //        RelativeHumidity = problem.RelativeHumidity,
        //        SystemImage = problem.SystemImage,
        //        GlassConfigurations = problem.GlassConfigurations,
        //        CreatedOn = problem.CreatedOn,
        //        ModifiedOn = problem.ModifiedOn,
        //        PhysicsTypeAcoustic = problem.PhysicsTypeAcoustic,
        //        PhysicsTypeStructure = problem.PhysicsTypeStructure,
        //        PhysicsTypeThermal = problem.PhysicsTypeThermal,
        //        ProductCode = problem.ProductTypeId != null ? projectService.GetProductCode(problem.ProductTypeId) : null,
        //        OperabilityConfigurations = problem.OperabilityConfigurations,
        //        BuildingHeight = problem.BuildingHeight,
        //        TerrainCategory = problem.TerrainCategory,
        //        VentFrameArticleNumber = problem.VentFrameArticleNumber,
        //        CustomArticles = problem.CustomArticles,
        //        WindZone = problem.WindZone,
        //        GlazingGasketCombination = problem.GlazingGasketCombination,
        //        Alloys = problem.Alloys,
        //        InsulatingBar = problem.InsulatingBar,
        //        PermissibleDeflection = problem.PermissibleDeflection,
        //        PermissibleVerticalDeflection = problem.PermissibleVerticalDeflection,
        //        UnifiedObjectModel = problem.UnifiedObjectModel,
        //        AcousticReportUrl = problem.AcousticReportUrl,
        //        StructuralReportUrl = problem.StructuralReportUrl,
        //        ThermalReportUrl = problem.ThermalReportUrl,
        //        AcousticResults = problem.AcousticResults,
        //        StructuralResults = problem.StructuralResults,
        //        ThermalResults = problem.ThermalResults
        //    };

        //    return bpsProblem;
        //}

        //public BpsProblem ProblemApiToDbModel(BpsProblemApiModel problem)
        //{
        //    BpsProjectService projectService = new BpsProjectService();
        //    BpsProjectApiModel project = projectService.GetProjectByGuid(problem.ProjectGuid);
        //    BpsProblem bpsProblem = new BpsProblem
        //    {
        //        ProblemGuid = problem.ProblemGuid,
        //        ProjectId = project.ProjectId,
        //        ProblemName = problem.ProblemName,
        //        SystemModel = problem.SystemModel,
        //        SystemName = problem.SystemName,
        //        SightlineArticleNumber = problem.SightlineArticleNumber,
        //        IntermediateArticleNumber = problem.IntermediateArticleNumber,
        //        WallType = problem.WallType,
        //        WallHeight = problem.WallHeight,
        //        WallWidth = problem.WallWidth,
        //        RoomArea = problem.RoomArea,
        //        WindLoad = problem.WindLoad,
        //        EngineeringStandard = problem.EngineeringStandard,
        //        BuildingLength = problem.BuildingLength,
        //        BuildingWidth = problem.BuildingWidth,
        //        BuildingRiskCategory = problem.BuildingRiskCategory,
        //        WindSpeed = problem.WindSpeed,
        //        ExposureCategory = problem.ExposureCategory,
        //        WindowWidth = problem.WindowWidth,
        //        WindowHeight = problem.WindowHeight,
        //        WindowElevation = problem.WindowElevation,
        //        WindowZone = problem.WindowZone,
        //        RelativeHumidity = problem.RelativeHumidity,
        //        SystemImage = problem.SystemImage,
        //        GlassConfigurations = problem.GlassConfigurations,
        //        CreatedOn = problem.CreatedOn,
        //        ModifiedOn = problem.ModifiedOn,
        //        PhysicsTypeAcoustic = problem.PhysicsTypeAcoustic,
        //        PhysicsTypeStructure = problem.PhysicsTypeStructure,
        //        PhysicsTypeThermal = problem.PhysicsTypeThermal,
        //        ProductTypeId = projectService.GetProductTypeId(problem.ProductCode),
        //        OperabilityConfigurations = problem.OperabilityConfigurations,
        //        BuildingHeight = problem.BuildingHeight,
        //        TerrainCategory = problem.TerrainCategory,
        //        VentFrameArticleNumber = problem.VentFrameArticleNumber,
        //        CustomArticles = problem.CustomArticles,
        //        WindZone = problem.WindZone,
        //        GlazingGasketCombination = problem.GlazingGasketCombination,
        //        InsulatingBar = problem.InsulatingBar,
        //        PermissibleDeflection = problem.PermissibleDeflection,
        //        UnifiedObjectModel = problem.UnifiedObjectModel,
        //        AcousticReportUrl = problem.AcousticReportUrl,
        //        StructuralReportUrl = problem.StructuralReportUrl,
        //        ThermalReportUrl = problem.ThermalReportUrl,
        //        AcousticResults = problem.AcousticResults,
        //        StructuralResults = problem.StructuralResults,
        //        ThermalResults = problem.ThermalResults
        //    };

        //    return bpsProblem;
        //}
    }
}
