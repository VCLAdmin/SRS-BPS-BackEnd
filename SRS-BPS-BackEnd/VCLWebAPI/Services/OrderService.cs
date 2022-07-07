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
                #endregion
            }
            else
            {
                throw new InvalidDataException();
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