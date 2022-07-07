using BpsUnifiedModelLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
//using System.Web.Http;
//using System.Web.Script.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Models.SRS;
using VCLWebAPI.Services;
using System.Text.Json;

namespace VCLWebAPI.Controllers
{
    /// <summary>
    /// Defines the <see cref="OrderController" />.
    /// </summary>
    [Authorize]
    [Route("api/Order")]
    public class OrderController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        /// <summary>
        /// Defines the _bpsProjectService.
        /// </summary>
        private readonly BpsProjectService _bpsProjectService;

        /// <summary>
        /// Defines the _orderService.
        /// </summary>
        private readonly OrderService _orderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/> class.
        /// </summary>
        public OrderController()
        {
            _orderService = new OrderService();
            _bpsProjectService = new BpsProjectService();
        }

        // DELETE: api/Fabricator/5
        /// <summary>
        /// The Delete.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="List{OrderApiModel}"/>.</returns>
        [HttpDelete]
        public List<OrderApiModel> Delete(Guid id)
        {
            //Guid extId = Guid.Parse(guid);
            _orderService.Delete(id);
            return _orderService.GetAll();
        }

        /// <summary>
        /// The GetProblemByGuid.
        /// </summary>
        /// <param name="problemGuid">The problemGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GenerateProposal/{problemGuid}/{ProposalOrBOM}")]
        // Use this to generate proposal using problemGuid.
        // Else just call this method to use the download.json located in "...\VCLDesign\VCLWebAPI\Resources\srs-templates\download.json"
        public IActionResult GenerateProposal(string problemGuid, string ProposalOrBOM)
        {
            try
            {
                BpsUnifiedProblem unifiedProblem = _bpsProjectService.GetProblemByGuid(Guid.Parse(problemGuid));
                SRS_Solver.SRSAnalysis srsAnalysis = new SRS_Solver.SRSAnalysis();
                srsAnalysis.GenerateProposal(unifiedProblem.ProblemGuid.ToString(), unifiedProblem.UnifiedModel, ProposalOrBOM);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Delete this once done testing
        /// <summary>
        /// The GenerateProposal_FromJsonFile.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GenerateProposal_FromJsonFile/{ProposalOrBOM}")]
        public IActionResult GenerateProposal_FromJsonFile(string ProposalOrBOM)
        {
            try
            {
                SRS_Solver.SRSAnalysis srsAnalysis = new SRS_Solver.SRSAnalysis();
                srsAnalysis.GenerateProposal_FromJsonFile(ProposalOrBOM);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Delete this once done testing
        /// <summary>
        /// The GenerateProposal_FromJsonModel.
        /// </summary>
        /// <param name="jsonModel">The jsonModel<see cref="BpsUnifiedModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GenerateProposal_FromJsonModel/")]
        public IActionResult GenerateProposal_FromJsonModel(BpsUnifiedModel jsonModel)
        {
            try
            {
                SRS_Solver.SRSAnalysis srsAnalysis = new SRS_Solver.SRSAnalysis();

                //var jsonStringName = new JavaScriptSerializer();
                //var jsonStringResult = jsonStringName.Serialize(jsonModel);
                var jsonStringResult = JsonSerializer.Serialize(jsonModel);

                srsAnalysis.GenerateProposal_FromJsonString(jsonStringResult);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Delete this once done testing
        /// <summary>
        /// The GenerateProposal_FromJsonTemplateModel.
        /// </summary>
        /// <param name="jsonModel">The jsonModel<see cref="SRSUnifiedApiModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("GenerateProposal_FromJsonTemplateModel/")]
        public IActionResult GenerateProposal_FromJsonTemplateModel(SRSUnifiedApiModel jsonModel)
        {
            try
            {
                SRS_Solver.SRSAnalysis srsAnalysis = new SRS_Solver.SRSAnalysis();
                //var jsonStringName = new JavaScriptSerializer();
                //var jsonStringResult = jsonStringName.Serialize(jsonModel.BPSUnifiedModel);

                var jsonStringResult = JsonSerializer.Serialize(jsonModel.BPSUnifiedModel);

                srsAnalysis.GenerateProposal_FromJsonString(jsonStringResult, jsonModel.TemplateFileName);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// The GenerateProposalFile.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        [HttpGet]
        [Route("GenerateProposalZipFile/{projectGuid}/{ProposalOrBOM}")]
        public HttpResponseMessage GenerateProposalZipFile(string projectGuid, string ProposalOrBOM)
        {
            List<BpsUnifiedProblem> problems = _bpsProjectService.GetProblemsForProject(Guid.Parse(projectGuid));
            string zipFileName = "";
            byte[] zipFileBytes = null;
            using (MemoryStream zipStream = new MemoryStream())
            {
                using (System.IO.Compression.ZipArchive zip = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    foreach (var unifiedProblem in problems)
                    {
                        SRS_Solver.SRSAnalysis srsAnalysis = new SRS_Solver.SRSAnalysis();
                        if(zipFileName == "") zipFileName = unifiedProblem.ProblemName + "_" + unifiedProblem.ProjectId.ToString();
                        System.IO.Compression.ZipArchiveEntry zipItem = zip.CreateEntry(unifiedProblem.ProjectId.ToString() + "-" + unifiedProblem.ProblemId.ToString() + "_" + unifiedProblem.ProblemName + ".pdf");
                        using (MemoryStream originalFileMemoryStream = srsAnalysis.GenerateProposal(unifiedProblem.ProblemGuid.ToString(), unifiedProblem.UnifiedModel, ProposalOrBOM))
                        {
                            using (Stream entryStream = zipItem.Open())
                            {
                                originalFileMemoryStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
                zipFileBytes = zipStream.ToArray();
            }

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(zipFileBytes)
            };
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = zipFileName.TrimStart('\"').TrimEnd('\"') + ".zip"
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }


        /// <summary>
        /// The GenerateProposalFile.
        /// </summary>
        /// <param name="problemGuid">The problemGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        [HttpGet]
        [Route("GenerateProposalFile/{problemGuid}/{ProposalOrBOM}")]
        public HttpResponseMessage GenerateProposalFile(string problemGuid, string ProposalOrBOM)
        {
            BpsUnifiedProblem unifiedProblem = _bpsProjectService.GetProblemByGuid(Guid.Parse(problemGuid));
            SRS_Solver.SRSAnalysis srsAnalysis = new SRS_Solver.SRSAnalysis();
            MemoryStream stream = srsAnalysis.GenerateProposal(unifiedProblem.ProblemGuid.ToString(), unifiedProblem.UnifiedModel, ProposalOrBOM);

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                   
                    FileName = ProposalOrBOM == "BOM" ?
                    "CertificationCard.xlsx" : "CertificationCard.pdf"
        };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }

        /// <summary>
        /// The Get.
        /// </summary>
        /// <returns>The <see cref="List{OrderApiModel}"/>.</returns>
        [HttpGet]
        public List<OrderApiModel> Get()
        {
            return _orderService.GetAll();
        }

        // GET: api/Fabricator/5
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="OrderApiModel"/>.</returns>
        [HttpGet]
        public OrderApiModel Get(Guid id)
        {
            //Guid extId = Guid.Parse(guid);
            return _orderService.Get(id);
        }

        /// <summary>
        /// The GetCompleteList.
        /// </summary>
        /// <returns>The <see cref="List{OrderApiModel}"/>.</returns>
        [HttpGet]
        public List<OrderApiModel> GetCompleteList()
        {
            return _orderService.GetCompleteList();
        }

        /// <summary>
        /// The GetOrderByDealer.
        /// </summary>
        /// <param name="type">The type<see cref="string"/>.</param>
        /// <returns>The <see cref="List{OrdersByDealer}"/>.</returns>
        [Route("GetOrderByDealer/{type}")]
        [HttpGet]
        public List<OrdersByDealer> GetOrderByDealer(string type)
        {
            return _orderService.GetOrderByDealer(type);
        }

        /// <summary>
        /// The GetOrderDashboard.
        /// </summary>
        /// <param name="type">The type<see cref="string"/>.</param>
        /// <returns>The <see cref="List{int}"/>.</returns>
        [Route("GetOrderDashboard/{type}")]
        [HttpGet]
        public List<int> GetOrderDashboard(string type)
        {
            return _orderService.GetOrderDashboard(type);
        }

        /// <summary>
        /// The GetPresignedExcelURL.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        public string GetPresignedExcelURL(int id)
        {
            return _orderService.GeneratePreassignedExcelURL(id);
        }

        /// <summary>
        /// The GetPresignedURL.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        public string GetPresignedURL(int id)
        {
            return _orderService.GeneratePreassignedURL(id);
        }

        /// <summary>
        /// The GetPresignedScreenshotURL.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        [Route("GetPresignedScreenshotURL/{id}")]
        public string GetPresignedScreenshotURL(string id)
        {
            return _orderService.GeneratePreassignedScreenshotURL(id);
        }

        /// <summary>
        /// The GetProjectOrders.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="OrderApiModel"/>.</returns>
        [HttpGet]
        [Route("GetProjectOrders/{id}")]
        public OrderApiModel GetProjectOrders(Guid id)
        {
            return _orderService.GetProjectOrders(id);
        }

        /// <summary>
        /// The GetUserOrderByDealer.
        /// </summary>
        /// <param name="status">The status<see cref="string"/>.</param>
        /// <param name="date">The date<see cref="string"/>.</param>
        /// <param name="type">The type<see cref="string"/>.</param>
        /// <returns>The <see cref="List{OrdersByDealer}"/>.</returns>
        [Route("GetUserOrderByDealer/{status}/{date}/{type}")]
        [HttpGet]
        public List<OrdersByDealer> GetUserOrderByDealer(string status, string date, string type)
        {
            return _orderService.GetUserOrderByDealer(status, date, type);
        }

        /// <summary>
        /// The GetUserOrderDashboard.
        /// </summary>
        /// <param name="status">The status<see cref="string"/>.</param>
        /// <param name="date">The date<see cref="string"/>.</param>
        /// <param name="type">The type<see cref="string"/>.</param>
        /// <returns>The <see cref="List{int}"/>.</returns>
        [Route("GetUserOrderDashboard/{status}/{date}/{type}")]
        [HttpGet]
        public List<int> GetUserOrderDashboard(string status, string date, string type)
        {
            return _orderService.GetUserOrderDashboard(status, date, type);
        }

        // POST: api/Fabricator
        /// <summary>
        /// The PostOrders.
        /// </summary>
        /// <param name="newOrder">The newOrder<see cref="OrderApiModel"/>.</param>
        /// <returns>The <see cref="List{OrderApiModel}"/>.</returns>
        [HttpPost]
        [Route("PostOrders")]
        public List<OrderApiModel> PostOrders([FromBody] OrderApiModel newOrder)
        {
            _orderService.CreateOrder(newOrder);
            return _orderService.GetAll();
        }

        // POST: api/Fabricator
        //[HttpPost]
        //public List<OrderApiModel> Post([FromBody] OrderApiModel fab)
        //{
        //    _orderService.Create(fab);
        //    return _orderService.GetAll();
        //}

        // PUT: api/Fabricator/5
        /// <summary>
        /// The Put.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <param name="order">The order<see cref="OrderApiModel"/>.</param>
        /// <returns>The <see cref="List{OrderApiModel}"/>.</returns>
        [HttpPut]
        public List<OrderApiModel> Put(Guid id, [FromBody] OrderApiModel order)
        {
            _orderService.UpdateOrderStatus(id, order);
            return _orderService.GetAll();
        }

        // PUT: api/Fabricator/5
        /// <summary>
        /// The UpdateOrder.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <param name="order">The order<see cref="OrderApiModel"/>.</param>
        /// <returns>The <see cref="List{OrderApiModel}"/>.</returns>
        [HttpPut]
        public List<OrderApiModel> UpdateOrder(Guid id, [FromBody] OrderApiModel order)
        {
            _orderService.UpdateOrder(id, order);
            return _orderService.GetAll();
        }
    }
}
