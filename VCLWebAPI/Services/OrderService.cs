using BpsUnifiedModelLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using System.Web;
using VCLWebAPI.Mappers;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Models.QueuingServer;
using VCLWebAPI.Models.SRS;
using VCLWebAPI.Utils;
using Amazon;
using Amazon.S3;
//using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Text.RegularExpressions;
using SendGrid.Helpers.Mail;

namespace VCLWebAPI.Services
{
    public class OrderService
    {
        private readonly VCLDesignDBEntities _db;
        private readonly ProjectMapper _pm;
        private readonly AccountService _as;
        private readonly DealerService _ds;        
        public readonly string QueuingServerUrl = ConfigurationManager.AppSettings["QS_url"];
        public OrderService()
        {
            _db = new VCLDesignDBEntities();
            _pm = new ProjectMapper();
            _as = new AccountService();
            _ds = new DealerService();
        }

        public OrderApiModel Get(Guid guid)
        {
            var order = _db.Order.Where(u => u.OrderExternalId == guid).FirstOrDefault();
            if(order == null)
                throw new InvalidDataException();
            return _pm.ProjectDbToApiModel(order);
        }

        public List<OrderApiModel> GetAll()
        {
            var currentUser = _as.GetCurrentUser();
            if (currentUser.Dealer.Count > 0)
            {
                var DealerId = currentUser.Dealer.First().DealerId;
                return _db.Order.Where(e => e.CreatedBy == currentUser.UserId && e.OrderDetails.Count > 0 && e.DealerId == DealerId).ToList().Select(s => _pm.ProjectDbToApiModel(s)).ToList();
            }
            else if (currentUser.Fabricator.Count > 0)
            {
                var FabricatorId = currentUser.Fabricator.First().FabricatorId;
                return _db.Order.Where(e => e.OrderDetails.Count > 0 && e.Dealer.Fabricator.FabricatorId == FabricatorId).ToList().Select(s => _pm.ProjectDbToApiModel(s)).ToList();
            } else
            {
                return _db.Order.Where(e => e.OrderDetails.Count > 0).ToList().Select(s => _pm.ProjectDbToApiModel(s)).ToList();
            }
        }
        public List<OrderApiModel> GetCompleteList()
        {
            var currentUser = _as.GetCurrentUser();
            if (currentUser.Dealer.Count > 0)
            {
                var DealerId = currentUser.Dealer.First().DealerId;
                return _db.Order.Where(e => e.DealerId == DealerId).ToList().Select(s => _pm.ProjectDbToApiModel(s)).ToList();
            }
            else if (currentUser.Fabricator.Count > 0)
            {
                var FabricatorId = currentUser.Fabricator.First().FabricatorId;
                return _db.Order.Where(e => e.Dealer.Fabricator.FabricatorId == FabricatorId).ToList().Select(s => _pm.ProjectDbToApiModel(s)).ToList();
            }
            else
            {
                return _db.Order.ToList().Select(s => _pm.ProjectDbToApiModel(s)).ToList();
            }
        }

        public List<int> GetOrderDashboard(string getType = "Count")
        {
            // var orders = _db.Order.Where(e => e.OrderDetails.Count > 0).ToList().Select(s => _pm.ProjectDbToApiModel(s)).ToList();
            var orders = GetAll();
            List<int> response = new List<int>();
            int orderPlaced = GetCount(orders, "Order Placed", getType);
            int prePod = GetCount(orders, "In Pre Production", getType);
            int fabrication = GetCount(orders, "In Fabrication", getType);
            int inAssembly = GetCount(orders, "In Assembly", getType);
            int shipped = GetCount(orders, "Shipped", getType);
            int delivred = GetCount(orders, "Delivered", getType);
            int completed = GetCount(orders, "Completed", getType);
            int cancelled = GetCount(orders, "Cancelled", getType);

            response.Add(orderPlaced);
            response.Add(prePod);
            response.Add(fabrication);
            response.Add(inAssembly);
            response.Add(shipped);
            response.Add(delivred);
            response.Add(completed);
            response.Add(cancelled);
            return response;
        }
        public int GetCount(List<OrderApiModel> orders, string status, string getType = "Count") {
            if (getType == "Count")
            {
                return orders.Where(o => o.Current_Status == status).ToList().Count;
            }
            else
            {
                int total = 0;
                foreach (var ord in orders.Where(o => o.Current_Status == status).ToList())
                {
                    var od = ord.OrderDetails.FirstOrDefault();
                    if (od != null)
                        total += (int)od.SubTotal;
                }
                return total;
            }
        }
        public List<int> GetUserOrderDashboard(string status, string dateValue, string getType = "Count")
        {
            var currentUserId = _as.GetCurrentUser().UserId;
            List<OrderApiModel> orders = _db.Order.Where(e => e.CreatedBy == currentUserId && e.OrderDetails.Count > 0).ToList().Select(s => _pm.ProjectDbToApiModel(s)).ToList();
            if (status != "ALL") {
                orders = orders.Where(e => e.Current_Process == status).ToList();
            } 
            if (dateValue != "ALL")
            {
                DateTime[] dates = AppUtils.GetDateRange(dateValue);
                if (dates[0] != null && dates[1] != null)
                    orders = orders.Where(f => f.CreatedOn >= dates[0] && f.CreatedOn < dates[1]).ToList();
            }

            List<int> response = new List<int>();
            int orderPlaced = GetCount(orders, "Order Placed", getType);
            int prePod = GetCount(orders, "In Pre Production", getType);
            int fabrication = GetCount(orders, "In Fabrication", getType);
            int inAssembly = GetCount(orders, "In Assembly", getType);
            int shipped = GetCount(orders, "Shipped", getType);
            int delivred = GetCount(orders, "Delivered", getType);
            int completed = GetCount(orders, "Completed", getType);
            int cancelled = GetCount(orders, "Cancelled", getType);
            response.Add(orderPlaced);
            response.Add(prePod);
            response.Add(fabrication);
            response.Add(inAssembly);
            response.Add(shipped);
            response.Add(delivred);
            response.Add(completed);
            response.Add(cancelled);
            return response;
        }


        public int GetAmount(OrdersByDealer orders, string getType = "Count")
        {
            if (getType == "Amount")
            {
                int total = 0;
                foreach (var ord in orders.Orders.ToList())
                {
                    var od = ord.OrderDetails.FirstOrDefault();
                    if (od != null)
                        total += (int)od.SubTotal;
                }
                return total;
            }
            else
            {
                return orders.TotalOrders;
            }

        }

        public List<OrdersByDealer> GetUserOrderByDealer(string status, string dateValue, string getType = "Count")
        {
            var currentUserId = _as.GetCurrentUser().UserId;
            var orders = _db.Order.Where(e => e.CreatedBy == currentUserId && e.OrderDetails.Count > 0).ToList().Select(s => _pm.ProjectDbToApiModel(s)).ToList();
            if (status != "ALL")
            {
                orders = orders.Where(e => e.Current_Process == status).ToList();
            }
            if (dateValue != "ALL")
            {
                DateTime[] dates = AppUtils.GetDateRange(dateValue);
                if (dates[0] != null && dates[1] != null)
                    orders = orders.Where(f => f.CreatedOn >= dates[0] && f.CreatedOn < dates[1]).ToList(); //ProjectCreatedOn 
            }
            List<OrdersByDealer> ordersByDealerList = new List<OrdersByDealer>();
            List<OrdersByDealer> ordersByDealer =
                    orders
                    .GroupBy(x => new { x.CreatedBy, x.CreatedOn.Month })
                    .Select(y => new OrdersByDealer
                    {
                        DealerId = y.Key.CreatedBy,
                        DealerName = y.Select(s => s.CreatedByName).FirstOrDefault(),
                        Month = y.Key.Month,
                        TotalOrders = y.Count(),
                        Orders = y.ToList()
                    }).ToList();

            return BuildResult(ordersByDealerList, ordersByDealer, getType);
        }
        public List<OrdersByDealer> GetOrderByDealer(string getType = "Count")
        {
            // var orders = _db.Order.Where(e => e.OrderDetails.Count > 0).ToList().Select(s => _pm.ProjectDbToApiModel(s)).ToList();
            var orders = GetAll();
            List<OrdersByDealer> ordersByDealerList = new List<OrdersByDealer>();
            List <OrdersByDealer> ordersByDealer =
                    orders
                    .GroupBy(x => new { x.DealerId, x.CreatedOn.Month })
                    .Select(y => new OrdersByDealer
                    {
                        DealerId = y.Key.DealerId,
                        DealerName = y.Select(s => s.DealerName).FirstOrDefault(),
                        Month = y.Key.Month,
                        TotalOrders = y.Count(),
                        Orders = y.ToList()
                    }).ToList();

            return BuildResult(ordersByDealerList, ordersByDealer, getType);
        }

        private List<OrdersByDealer> BuildResult(List<OrdersByDealer> ordersByDealerList, List<OrdersByDealer> ordersByDealer, string getType = "Count")
        {
            foreach (var item in ordersByDealer)
            {
                if (!ordersByDealerList.Any(a => a.DealerId == item.DealerId))
                {
                    item.MonthOrders = new List<int>();
                    var jan = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 1).FirstOrDefault();
                    var fab = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 2).FirstOrDefault();
                    var mar = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 3).FirstOrDefault();
                    var apl = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 4).FirstOrDefault();
                    var may = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 5).FirstOrDefault();
                    var jun = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 6).FirstOrDefault();
                    var jul = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 7).FirstOrDefault();
                    var aug = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 8).FirstOrDefault();
                    var sec = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 9).FirstOrDefault();
                    var oct = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 10).FirstOrDefault();
                    var nov = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 11).FirstOrDefault();
                    var dec = ordersByDealer.Where(o => o.DealerId == item.DealerId && o.Month == 12).FirstOrDefault();

                    item.MonthOrders.Add(jan == null ? 0 : GetAmount(jan, getType));
                    item.MonthOrders.Add(fab == null ? 0 : GetAmount(fab, getType));
                    item.MonthOrders.Add(mar == null ? 0 : GetAmount(mar, getType));
                    item.MonthOrders.Add(apl == null ? 0 : GetAmount(apl, getType));
                    item.MonthOrders.Add(may == null ? 0 : GetAmount(may, getType));
                    item.MonthOrders.Add(jun == null ? 0 : GetAmount(jun, getType));
                    item.MonthOrders.Add(jul == null ? 0 : GetAmount(jul, getType));
                    item.MonthOrders.Add(aug == null ? 0 : GetAmount(aug, getType));
                    item.MonthOrders.Add(sec == null ? 0 : GetAmount(sec, getType));
                    item.MonthOrders.Add(oct == null ? 0 : GetAmount(oct, getType));
                    item.MonthOrders.Add(nov == null ? 0 : GetAmount(nov, getType));
                    item.MonthOrders.Add(dec == null ? 0 : GetAmount(dec, getType));

                    item.TotalOrders = item.MonthOrders.Sum();
                    ordersByDealerList.Add(item);
                }
            }

            return ordersByDealerList.ToList();
        }

        public OrderApiModel GetProjectOrders(Guid projectId)
        {
            var dbOrders = _db.Order.Where(e => e.BpsProject.ProjectGuid == projectId).FirstOrDefault();
            return _pm.ProjectDbToApiModel(dbOrders); // Check this if we change to have multiple orders per project
        }

        public async void CreateOrder(OrderApiModel newOrderApi)
        {
            if (newOrderApi.OrderDetails != null && newOrderApi.OrderDetails.Count > 0)
            {
                AppUtils appUtil = new AppUtils();
                //BpsUnifiedProblem bpsProblem = _db.BpsUnifiedProblem.First(x => x.ProblemId == 2570);
                //SqsMessageBodyMessage sqsMsg2 = new SqsMessageBodyMessage();
                //var record2 = new Record();
                //record2.s3 = new S3();
                //record2.s3.@object = new SqsFileObject();
                //record2.s3.@object.key = "XXXX";

                //sqsMsg2.Records = new List<Record>() {
                //            record2
                //        };
                //RhinoQueuingMessage rhinoQueuingMessage2 = new RhinoQueuingMessage();
                //rhinoQueuingMessage2.SqsMessageBodyMessage = sqsMsg2;
                //rhinoQueuingMessage2.unifiedModel = bpsProblem.UnifiedModel;
                ////rhinoQueuingMessage.unifiedModel = JsonConvert.DeserializeObject<BpsUnifiedModel>(bpsProblem.UnifiedModel);
                ////new Task(async () => await appUtil.Post(QueuingServerUrl + "/VCL/Convert", sqsMsg2)).Start();
                //new Task(async () => await appUtil.Post(QueuingServerUrl + "/VCL/RhinoGenerate", rhinoQueuingMessage2)).Start();
                //return;

                #region Address 
                int addId = 0;
                User currentUser = _as.GetCurrentUser();
                //Rsmith @schuco-usa.com
                //+1 646-873-3278
                if (newOrderApi.Line1 != null)
                {
                    var add = _pm.ApiModelToAddressDb(newOrderApi);
                    add.AddressExternalId = Guid.NewGuid();
                    add.CreatedOn = DateTime.Now;
                    add.CreatedBy = currentUser.UserId;

                    _db.Address.Add(add);
                    _db.SaveChanges();
                    addId = add.AddressId;
                }
                newOrderApi.ShippingAddressId = addId;
                #endregion

                var dealer = _db.Dealer.Where(u => u.User.Any(e => e.UserGuid == currentUser.UserGuid)).FirstOrDefault();
                _ds.UpdateDealerOrderFinancial(dealer.DealerId, (double)newOrderApi.Total);

                #region Parent Order / Order
                var mainOrder = _pm.ApiModelToOrderDb(newOrderApi);
                mainOrder.OrderExternalId = Guid.NewGuid();
                mainOrder.DealerId = currentUser.Dealer.FirstOrDefault().DealerId;
                mainOrder.CreatedOn = DateTime.Now;
                mainOrder.CreatedBy = currentUser.UserId;

                _db.Order.Add(mainOrder);
                _db.SaveChanges();
                //if (newOrderApi.OrderDetails.Count() > 1)
                //{
                newOrderApi.ParentOrderId = mainOrder.OrderId;
                //    _db.SaveChanges();
                //}

                #endregion

                #region Create Order based on number of OrderDetails
                var sluStatus = _db.SLU_Status.ToList();
                var proj = _db.BpsProject.Where(p => p.ProjectId == newOrderApi.ProjectId).FirstOrDefault();
               
                List<string> productIds = new List<string>();
                foreach (var orderApi in newOrderApi.OrderDetails)
                {
                    //if (newOrderApi.OrderDetails.Count() == 1) {
                    //    orderApi.OrderId = mainOrder.OrderId;
                    //    Order_Status orderStatus = _pm.ApiModelToOrderStatusDb(mainOrder.OrderId, newOrderApi, sluStatus);
                    //    orderStatus.StatusModifiedOn = DateTime.Now;
                    //    orderStatus.StatusModifiedBy = currentUser.UserId;

                    //    mainOrder.Order_Status.Add(orderStatus);
                    //    _db.Order_Status.Add(orderStatus);
                    //    _db.SaveChanges();

                    //} else {

                    var order = _pm.ApiModelToOrderDb(newOrderApi);
                    order.OrderExternalId = Guid.NewGuid();
                    order.CreatedOn = DateTime.Now;
                    order.CreatedBy = currentUser.UserId;
                    order.DealerId = currentUser.Dealer.FirstOrDefault().DealerId;
                    _db.Order.Add(order);
                    _db.SaveChanges();
                    orderApi.OrderId = order.OrderId;

                    Order_Status orderStatus = _pm.ApiModelToOrderStatusDb(order.OrderId, newOrderApi, sluStatus);
                    order.Order_Status.Add(orderStatus);
                    _db.Order_Status.Add(orderStatus);
                    _db.SaveChanges();
                    //}

                    OrderDetails od = _pm.ApiModelToOrderDetailsDb(orderApi);
                    od.OrderDetailExternalId = Guid.NewGuid();
                    productIds.Add(proj.ProjectId + " - " + od.ProductId.ToString());
                     
                    _db.OrderDetails.Add(od);
                    _db.SaveChanges();

                    // QueuingServer
                    // Send a request to the queuing server for each
                    // Un comment this when code is deployed

                    var unifiedModel = _db.BpsUnifiedProblem.FirstOrDefault(x => x.ProblemId == od.ProductId).UnifiedModel;
                    var umObject = JsonConvert.DeserializeObject<BpsUnifiedModel>(unifiedModel);
                    umObject.SRSProblemSetting.isOrderPlaced = true;
                    _db.BpsUnifiedProblem.FirstOrDefault(x => x.ProblemId == od.ProductId).UnifiedModel = JsonConvert.SerializeObject(umObject);
                    _db.SaveChanges();

                    if (umObject.ModelInput.FrameSystem.SystemType != "ADS 75")
                    {
                        SqsMessageBodyMessage sqsMsg = new SqsMessageBodyMessage();
                        var record = new Record();
                        record.s3 = new S3();
                        record.s3.@object = new SqsFileObject();
                        record.s3.@object.key = od.OrderDetailExternalId.ToString();

                        sqsMsg.Records = new List<Record>() {
                        record
                    };
                        RhinoQueuingMessage rhinoQueuingMessage = new RhinoQueuingMessage();
                        rhinoQueuingMessage.SqsMessageBodyMessage = sqsMsg;
                        rhinoQueuingMessage.unifiedModel = unifiedModel;
                        //rhinoQueuingMessage.unifiedModel = od.BpsUnifiedProblem.UnifiedModel;
                        //new Task(async () => await appUtil.Post(QueuingServerUrl + "/VCL/Convert", sqsMsg)).Start();
                        await appUtil.Post(QueuingServerUrl + "/VCL/RhinoGenerate", rhinoQueuingMessage);
                    }
                }


                // email notification to send to the dealer saying thanks for placing an order

                var subject = "Order Placed";
                var header = "Order Placed";
                var content = "Thanks for your order of project <b>" + proj.ProjectName + "</b>, it will be activated along with the 50% first payment.";
                var orderNumbers = "Order Numbers: " + "<ul>";
                foreach (var item in productIds)
                {
                    orderNumbers += "<li>"+ item + "</li>";
                }
                orderNumbers += "</ul>";

                // the below is the actual mailId which needs to be enable for production
                // var test = await IMailService.SendEmailAsync(dealer.PrimaryContactEmail, subject, content, header);
               
               // string DealerMailId = WebConfigurationManager.AppSettings["Default_MailId"];
                string DealerMailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                if (DealerMailId != null)
                {
                    await IMailService.SendEmailAsync(DealerMailId, subject, content, header, orderNumbers);
                } else
                {
                    await IMailService.SendEmailAsync(dealer.PrimaryContactEmail, subject, content, header);
                }
                

                //email notifiation to send to SRS admin that dealer has placed an order and 50% initial payment funds are pending.
                var receivedSubject = "Order Received";
                var receivedHeader = "Order Received";
                var receivedContent = "Dealer <b>" + dealer.Name + "</b> has placed an order for <b>" + proj.ProjectName + "</b>, 50% initial payment funds pending.";
                var tos = new List<EmailAddress>();                  

                string AKmailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                string JaviermailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];


                // need to add AK's email and Javier email before push it to Production Server 
                if (AKmailId != null)
                {
                    tos.Add(new EmailAddress(AKmailId, "VCL Design"));
                } else
                {
                    // needs to add  AK's mailId for Production server
                    tos.Add(new EmailAddress("akbean@schuco-usa.com", "AK Bean"));
                }
                if(JaviermailId != null)
                {
                    tos.Add(new EmailAddress(JaviermailId, "VCL Design"));
                } else
                {
                    // needs to add  Javier's mailId for Production server
                    tos.Add(new EmailAddress("jdelacalle@schuco-usa.com", "Javier Delacalle"));
                }

                await IMailService.SendMultipleEmailAsync(tos, receivedSubject, receivedContent, receivedHeader, orderNumbers);

                // email notification to SRS admin that the dealer has placed an order and to confirm funds have been wired to Schuco's account
                var confirmFundSubject = "Order Received";
                var confirmFundHeader = "Order Received";
                var confirmFundContent = "Dealer <b>" + dealer.Name + "</b> has placed an order for <b>" + proj.ProjectName + "</b>, please confirm funds have been wired to Schuco’s account.";

                // the below email is for testing in local and change it to Gabriela's email before pushing it to Production Server
               
                string GabrielaMailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                if (GabrielaMailId != null)
                {
                    await IMailService.SendEmailAsync(GabrielaMailId, confirmFundSubject, confirmFundContent, confirmFundHeader, orderNumbers);
                } else
                {
                    //needs to Gabriela's mailId for Production server
                    // tos.Add(new EmailAddress("gprescott@schuco-usa.com", "Gabriela"));
                    tos.Add(new EmailAddress("jlazar@schuco-usa.com", "Jessica Lazar"));
                    tos.Add(new EmailAddress("jdelacalle@schuco-usa.com", "Javier Delacalle"));
                    await IMailService.SendMultipleEmailAsync(tos, confirmFundSubject, confirmFundContent, confirmFundHeader, orderNumbers);
                }
               

                #endregion
            }
            else
            {
                throw new InvalidDataException();
            }
        }

 public async void sendMail(Guid guid, int statusId, int projectId, int productId)
        {
            var dbOrder = _db.Order.Where(u => u.OrderExternalId == guid).FirstOrDefault();
            var sluStatus = _db.SLU_Status.ToList();
          //  Order_Status orderStatus = _pm.ApiModelToOrderStatusDb(orderApi.OrderId, orderApi, sluStatus);

            var dbDealer = _db.Dealer.Where(d => d.DealerId == dbOrder.DealerId).FirstOrDefault();
            var dbProject = _db.BpsProject.Where(p => p.ProjectId == dbOrder.ProjectId).FirstOrDefault();
            List<string> productIds = new List<string>();
            foreach (var o in dbOrder.OrderDetails)
            {
                productIds.Add(dbProject.ProjectId + " - " + o.ProductId.ToString());
            }
            var orderNumbers = "Order Numbers: " + "<ul>";
            foreach (var item in productIds)
            {
                orderNumbers += "<li>" + item + "</li>";
            }
            orderNumbers += "</ul>";

            // the below code is to send the email notification when the status is chnages to Payment Received
            if (statusId == 2)
            {
                var subject = "Thank you for your payment";
                if (dbOrder != null)
                {
                    // email notification to dealer when the initial 50% payment is done and SRS admin is changed Status to Payment Received
                    if (dbDealer != null)
                    {
                        var content = "Project <b>" + dbProject.ProjectName + "</b> funds for the 50% first payment received.";
                        var header = "Thank you for your payment";
                        // await IMailService.SendEmailAsync(dbDealer.PrimaryContactEmail, subject, content, header);
                        string DealerMailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                        if (DealerMailId != null)
                        {
                            await IMailService.SendEmailAsync(DealerMailId, subject, content, header, orderNumbers);
                        }
                        else
                        {
                            // this is actual dealers mailId when it is in Production
                            await IMailService.SendEmailAsync(dbDealer.PrimaryContactEmail, subject, content, header);
                        }
                    }

                    // email notification to SRS Fabricator when the initial 50% payment is done
                    var dbFabricator = _db.Fabricator.Where(f => f.FabricatorId == dbDealer.AWSFabricatorId || f.FabricatorId == dbDealer.ADSFabricatorId || f.FabricatorId == dbDealer.ASSFabricatorId).Distinct().ToList();
                    var fabricatorsListTos = new List<EmailAddress>();
                    var receivedFabricatorSubject = "Order Received";
                    var receivedFabricatorContent = "Please proceed to production of the units for dealer <b>" + dbDealer.Name + "</b> project <b>" + dbProject.ProjectName + "</b>.";
                    var receivedFabricatorHeader = "Order Received";
                    if (dbFabricator.Count > 0)
                    {
                        
                        string fabricatorMailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                        if (fabricatorMailId != null)
                        {
                            fabricatorsListTos.Add(new EmailAddress(fabricatorMailId, "VCL Design"));
                        }
                        else
                        {
                            // MailIds of Actual Fabricators in Production server
                            foreach (var fab in dbFabricator)
                            {
                                fabricatorsListTos.Add(new EmailAddress(fab.PrimaryContactEmail, fab.PrimaryContactName));
                            }
                        }

                        await IMailService.SendMultipleEmailAsync(fabricatorsListTos, receivedFabricatorSubject, receivedFabricatorContent, receivedFabricatorHeader, orderNumbers);

                    }

                }
            }

            // the below code is to send the email notifications when the order has shipped 
            if (statusId == 6)
            {

                var subject1 = "Order Shipped";
                if (dbOrder != null)
                {
                    // email notification to send to dealer when the order has shipped
                    if (dbDealer != null)
                    {
                        var content = "Units for project <b>" + dbProject.ProjectName + "</b> have been sent to the address you indicated when placing the order.";
                        var header = "Order Shipped";
                       
                            string DealerMailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                        if (DealerMailId != null)
                            {
                                await IMailService.SendEmailAsync(DealerMailId, subject1, content, header, orderNumbers);
                            }
                            else
                            {
                                // this is actual dealers mailId when it is in Production
                                await IMailService.SendEmailAsync(dbDealer.PrimaryContactEmail, subject1, content, header);
                            }
                        }
                }

                // email notification to send to SRS Admin when the order has shipped

                var shippedSubject = "Order Shipped";
                var shippedContent = "Order for project <b>" + dbProject.ProjectName + "</b> for dealer has been shipped.";
                var shippedHeader = "Order Shipped";

                var tos = new List<EmailAddress>();              

                // need to add AK's email and Javier email before push it to Production Server 

                string AKmailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                string JaviermailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];


                // need to add AK's email and Javier email before push it to Production Server 
                if (AKmailId != null)
                {
                    tos.Add(new EmailAddress(AKmailId, "VCL Design"));
                }
                else
                {
                    // needs to add  AK's mailId for Production server
                    tos.Add(new EmailAddress("akbean@schuco-usa.com", "AK Bean"));
                }
                if (JaviermailId != null)
                {
                    tos.Add(new EmailAddress(JaviermailId, "VCL Design"));
                }
                else
                {
                    // needs to add  Javier's mailId for Production server
                    tos.Add(new EmailAddress("jdelacalle@schuco-usa.com", "Javier Delacalle"));
                }

                await IMailService.SendMultipleEmailAsync(tos, shippedSubject, shippedContent, shippedHeader, orderNumbers);


                // email notification to send to SRS fabricator when the order has shipped

                var shippedFabricatorSubject = "Order Shipped";
                var shippedFabricatorContent = "Your order <b>" + dbProject.ProjectName + "</b> has been shipped";
                var shippedFabricatorHeader = "Order Shipped";

                var dbFabricatorShipped = _db.Fabricator.Where(f => f.FabricatorId == dbDealer.AWSFabricatorId || f.FabricatorId == dbDealer.ADSFabricatorId || f.FabricatorId == dbDealer.ASSFabricatorId).Distinct().ToList();
                var fabricatorsListShippedTos = new List<EmailAddress>();

                if (dbFabricatorShipped.Count > 0)
                {     
                    string fabricatorMailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                    if (fabricatorMailId != null)
                    {
                        fabricatorsListShippedTos.Add(new EmailAddress(fabricatorMailId, "VCL Design"));
                    }
                    else
                    {
                        // MailIds of Actual Fabricators in Production server
                        foreach (var fab in dbFabricatorShipped)
                        {
                            fabricatorsListShippedTos.Add(new EmailAddress(fab.PrimaryContactEmail, fab.PrimaryContactName));
                        }
                    }
                    await IMailService.SendMultipleEmailAsync(fabricatorsListShippedTos, shippedFabricatorSubject, shippedFabricatorContent, shippedFabricatorHeader, orderNumbers);

                }



            }

            // the below code is to send email notification when the order has delivered
            if (statusId == 7)
            {
                var subjectDelivered = "Order Delivered";
                if (dbOrder != null)
                {
                    // email notification to send to dealer when the order has delivered
                    if (dbDealer != null)
                    {
                        var content = "Units for project <b>" + dbProject.ProjectName + "</b> have been delivered.";
                        var header = "Order Delivered";
                       

                        string DealerMailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                        if (DealerMailId != null)
                        {
                            await IMailService.SendEmailAsync(DealerMailId, subjectDelivered, content, header, orderNumbers);
                        }
                        else
                        {
                            // this is actual dealers mailId when it is in Production
                            await IMailService.SendEmailAsync(dbDealer.PrimaryContactEmail, subjectDelivered, content, header);
                        }
                    }
                }

                // email notification to send to SRS Admin(to Ak, Javier) when the order has delivered
                var deliveredSubject = "Order Delivered";
                var deliveredContent = "Order <b>" + dbDealer.Name + "</b> project <b>" + dbProject.ProjectName + "</b> has been delivered.";
                var deliveredHeader = "Order Delivered";

                var Deliveredtos = new List<EmailAddress>();             

                // need to add AK's email and Javier email before push it to Production Server 
                string AKmailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                string JaviermailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];


                // need to add AK's email and Javier email before push it to Production Server 
                if (AKmailId != null)
                {
                    Deliveredtos.Add(new EmailAddress(AKmailId, "VCL Design"));
                }
                else
                {
                    // needs to add  AK's mailId for Production server
                    Deliveredtos.Add(new EmailAddress("akbean@schuco-usa.com", "AK Bean"));
                }
                if (JaviermailId != null)
                {
                    Deliveredtos.Add(new EmailAddress(JaviermailId, "VCL Design"));
                }
                else
                {
                    // needs to add  Javier's mailId for Production server
                    Deliveredtos.Add(new EmailAddress("jdelacalle@schuco-usa.com", "Javier Delacalle"));
                }

                await IMailService.SendMultipleEmailAsync(Deliveredtos, deliveredSubject, deliveredContent, deliveredHeader, orderNumbers);

                // email notification to send to SRS Admin(to Gabriel) when the order has delivered and request for second Payment
                var secondRequestSubject = "Order Delivered";
                var secondRequestContent = "Order by <b>" + dbDealer.Name + "</b> project <b>" + dbProject.ProjectName + "</b> has been delivered, please send request for second 50 % payment.";
                var secondRequestHeader = "Order Delivered";               

                string GabrielaMailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                if (GabrielaMailId != null)
                {
                    await IMailService.SendEmailAsync(GabrielaMailId, secondRequestSubject, secondRequestContent, secondRequestHeader, orderNumbers);
                }
                else
                {
                    //needs to Gabriela's mailId for Production server
                    await IMailService.SendEmailAsync("gprescott@schuco-usa.com", secondRequestSubject, secondRequestContent, secondRequestHeader, orderNumbers);
                }

                // email notification to send to SRS Fabricator when the order has delivered
                var deliveredFabricatorSubject = "Order Delivered";
                var deliveredFabricatorContent = "Your order <b>" + dbProject.ProjectName + "</b> has been delivered.";
                var deliveredFabricatorHeader = "Order Delivered";

                var dbFabricatorDelivered = _db.Fabricator.Where(f => f.FabricatorId == dbDealer.AWSFabricatorId || f.FabricatorId == dbDealer.ADSFabricatorId || f.FabricatorId == dbDealer.ASSFabricatorId).Distinct().ToList();
                var fabricatorsListDeliveredTos = new List<EmailAddress>();

                if (dbFabricatorDelivered.Count > 0)
                {
                    string fabricatorMailId = System.Configuration.ConfigurationManager.AppSettings["Default_MailId"];
                    if (fabricatorMailId != null)
                    {
                        fabricatorsListDeliveredTos.Add(new EmailAddress(fabricatorMailId, "VCL Design"));
                    }
                    else
                    {
                        // MailIds of Actual Fabricators in Production server
                        foreach (var fab in dbFabricatorDelivered)
                        {
                            fabricatorsListDeliveredTos.Add(new EmailAddress(fab.PrimaryContactEmail, fab.PrimaryContactName));
                        }
                    }
                    await IMailService.SendMultipleEmailAsync(fabricatorsListDeliveredTos, deliveredFabricatorSubject, deliveredFabricatorContent, deliveredFabricatorHeader, orderNumbers);

                }

            }

        }
        public void UpdateOrderStatus(Guid guid, OrderApiModel orderApi)
        {
            var dbOrder = _db.Order.Where(u => u.OrderExternalId == guid).FirstOrDefault();
            var sluStatus = _db.SLU_Status.ToList();

            try
            {
                Order_Status orderStatus = _pm.ApiModelToOrderStatusDb(orderApi.OrderId, orderApi, sluStatus);
                var oss = _db.Order_Status.Where(e => e.OrderId == orderStatus.OrderId).ToList();
                if (!(oss.Any(e => e.StatusId == orderStatus.StatusId)))
                {
                    orderStatus.StatusModifiedBy = _as.GetCurrentUser().UserId;
                    dbOrder.Order_Status.Add(orderStatus);
                    _db.Order_Status.Add(orderStatus);
                    _db.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            dbOrder.Notes = orderApi.Notes;
            dbOrder.ModifiedBy = _as.GetCurrentUser().UserId;
            dbOrder.ModifiedOn = DateTime.Now;
            _db.Entry(dbOrder).State = EntityState.Modified;
            _db.SaveChanges();
        }
            
        public void UpdateOrder(Guid guid, OrderApiModel orderApi)
        {
            var dbOrder = _db.Order.Where(u => u.OrderExternalId == guid).FirstOrDefault();
            var sluStatus = _db.SLU_Status.ToList();
            //var dbAddress = _db.Address.Where(u => u.ProjectId == dbOrder.ProjectId).FirstOrDefault();
            //if (orderApi.Line1 != null)
            //{
            //    dbAddress.Line1 = orderApi.Line1;
            //    dbAddress.Line2 = orderApi.Line2;
            //    dbAddress.State = orderApi.State;
            //    dbAddress.City = orderApi.City;
            //    dbAddress.PostalCode = orderApi.PostalCode;
            //    dbAddress.ModifiedBy = _as.GetCurrentUser().UserId;
            //    dbAddress.ModifiedOn = DateTime.Now;
            //    _db.Entry(dbAddress).State = EntityState.Modified;
            //}
            //foreach (var orderApi in orderApi.OrderDetails)
            //{
            //    order.OrderDetails.Add(_pm.ApiModelToOrderDetailsDb(orderApi));
            //}

            Order_Status orderStatus = _pm.ApiModelToOrderStatusDb(orderApi.OrderId, orderApi, sluStatus);
            dbOrder.Order_Status.Add(orderStatus);
            _db.Order_Status.Add(orderStatus);
            _db.SaveChanges();

            dbOrder.Notes = orderApi.Notes;
            dbOrder.ShippingCost = orderApi.ShippingCost;
            dbOrder.Tax = orderApi.Tax;
            dbOrder.Total = orderApi.Total;
            dbOrder.Discount = orderApi.Discount;
            dbOrder.DiscountPercentage = orderApi.DiscountPercentage;
            dbOrder.ShippingMethod = orderApi.ShippingMethod;
            dbOrder.ModifiedBy = _as.GetCurrentUser().UserId;
            dbOrder.ModifiedOn = DateTime.Now;
            _db.Entry(dbOrder).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void Delete(Guid guid)
        {
            var order = _db.Order.Where(u => u.OrderExternalId == guid).FirstOrDefault();
            if (order == null)
                throw new InvalidDataException();

            _db.Order.Remove(order);
            _db.Entry(order).State = EntityState.Deleted;
            _db.SaveChanges();
        }
        public void DeleteOrderDetails(Order order)
        {
            var orderDetails = _db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();
            foreach (var od in orderDetails)
            {
                _db.OrderDetails.Remove(od);
                _db.Entry(od).State = EntityState.Deleted;
            }
            _db.SaveChanges();
        }

        public void DeleteOrderStatus(Order order)
        {
            var orderStatus = _db.Order_Status.Where(x => x.OrderId == order.OrderId).ToList();
            foreach (var os in orderStatus)
            {
                _db.Order_Status.Remove(os);
                _db.Entry(os).State = EntityState.Deleted;
            }
            _db.SaveChanges();
        }

        public string GeneratePreassignedExcelURL(int id)
        {
            var od = _db.OrderDetails.FirstOrDefault(x => x.OrderId == id);
            if (od == null)
            {
                return "";
            }

            var key = od.BomURL;
            return GetUrl(key);
        }

        public string GeneratePreassignedURL(int id)
        {
            var od = _db.OrderDetails.FirstOrDefault(x => x.OrderId == id);
            if (od == null)
            {
                return "";
            }
            var key = od.DesignURL;
            return GetUrl(key);

        }
        private string GetUrl(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return "";
            }

            string accessKey = Globals.accessKey; // System.Configuration.ConfigurationManager.AppSettings["DE_AWSAccessKey"];
            string secrectKey = Globals.secretKey; // System.Configuration.ConfigurationManager.AppSettings["DE_AWSSecretKey"];
            string service_url = Globals.service_url; // System.Configuration.ConfigurationManager.AppSettings["DES3ServiceUrl"];
            string bucket_name = Globals.bucket_name; // System.Configuration.ConfigurationManager.AppSettings["DEAWSBucket"];

            var _s3client = new AmazonS3Client(
                       accessKey,
                       secrectKey,
                       new AmazonS3Config
                       {
                           ServiceURL = service_url
                       });
            var signedURL = GeneratePreSignedURL(key, -1, bucket_name, _s3client); //"Orders/XXXX/Design.pdf"
            return signedURL;
        }


        public async Task<bool> UploadScreenshotAsync(string problemGuid, string imageData)
        {
            try
            {
                var base64Data = Regex.Match(imageData, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var bytes = Convert.FromBase64String(base64Data);

                string accessKey = Globals.accessKey; //System.Configuration.ConfigurationManager.AppSettings["DE_AWSAccessKey"];
                string secrectKey = Globals.secretKey; //System.Configuration.ConfigurationManager.AppSettings["DE_AWSSecretKey"];
                string service_url = Globals.service_url; //System.Configuration.ConfigurationManager.AppSettings["DES3ServiceUrl"];
                string bucket_name = Globals.bucket_name; //System.Configuration.ConfigurationManager.AppSettings["DEAWSBucket"];
                string localFileFullPath = "screenshots/" + problemGuid + ".png";

                AmazonS3Client client = new AmazonS3Client(
                accessKey,
                secrectKey,
                new AmazonS3Config
                {
                    ServiceURL = service_url
                });

                using (var stream = new MemoryStream(bytes))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = bucket_name,
                        InputStream = stream,
                        ContentType = "image/png",
                        Key = localFileFullPath
                    };

                    var response = await client.PutObjectAsync(request).ConfigureAwait(false);
                }
                return true; //indicate that the file was sent
            }
            catch (AmazonS3Exception e)
            {
                throw e;
            }
        }

        public string GeneratePreassignedScreenshotURL(string id)
        {
            if (id == null)
            {
                return "";
            }
            var key = id;
            return GetScreenshotUrl(key);

        }

        private string GetScreenshotUrl(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return "";
            }

            string localFileFullPath = "screenshots/" + key + ".png";

            var _s3client = new AmazonS3Client(
                       Globals.accessKey,
                       Globals.secretKey,
                       new AmazonS3Config
                       {
                           ServiceURL = Globals.service_url
                       });
            var signedURL = GeneratePreSignedURL(localFileFullPath, -1, Globals.bucket_name, _s3client); //"Orders/XXXX/Design.pdf"
            return signedURL;
        }

        public string GeneratePreSignedURL(string key, int expires, string _bucketName, AmazonS3Client s3Client)
        {
            string urlString = "";

            expires = expires > 0 ? expires : 5;

            try
            {
                GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    Expires = DateTime.Now.AddMinutes(expires)
                };
                urlString = s3Client.GetPreSignedURL(request1);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return urlString;
        }

    }
}