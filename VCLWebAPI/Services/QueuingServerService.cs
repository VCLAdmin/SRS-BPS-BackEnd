using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
//using System.Web;
using VCLWebAPI.Models.Edmx;
using Microsoft.AspNetCore.Http;

namespace VCLWebAPI.Services
{
    public class QueuingServerService
    {
        private readonly VCLDesignDBEntities _db;
        public QueuingServerService()
        {
            _db = new VCLDesignDBEntities();
        }

        public void ConversionCompleted(Guid orderExternalId)
        {
            var orderDetails = _db.OrderDetails.FirstOrDefault(d => d.OrderDetailExternalId == orderExternalId);
            if (orderDetails == null)
            {
                return;
            }
            var jsonFile = "Orders/" + orderExternalId + "/JSON/Download.json";
            var designPdf = "Orders/" + orderExternalId + "/Design.pdf";
            var bomExcel = "Orders/" + orderExternalId + "/FDS_Exel.xlsx";
            orderDetails.DesignURL = designPdf;
            orderDetails.BomURL = bomExcel;
            orderDetails.JsonURL = jsonFile;

            _db.SaveChanges();
        }

        public async Task<string> SnsNewMessage(HttpRequest request, string id = "")
        {
            try
            {
                var jsonData = "";
                Stream req = request.Body;//await request.Content.ReadAsStreamAsync();
                req.Seek(0, System.IO.SeekOrigin.Begin);
                String json = new StreamReader(req).ReadToEnd();
                var sm = Amazon.SimpleNotificationService.Util.Message.ParseMessage(json);
                if (sm.Type.Equals("SubscriptionConfirmation")) //for confirmation
                {
                    Debug.WriteLine("Received Confirm subscription request");
                    if (!string.IsNullOrEmpty(sm.SubscribeURL))
                    {
                        var uri = new Uri(sm.SubscribeURL);
                        Debug.WriteLine("uri:" + uri.ToString());
                        var baseUrl = uri.GetLeftPart(System.UriPartial.Authority);
                        var resource = sm.SubscribeURL.Replace(baseUrl, "");
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(baseUrl);
                            var result = await client.GetAsync(resource);
                        }
                    }
                }
                else // For processing of messages
                {
                    // do whatever it has to do here


                    Debug.WriteLine("Message received from SNS:" + sm.TopicArn);
                }
                //do stuff
                return "Success";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed");
                return "";
            }
        }

    }
}