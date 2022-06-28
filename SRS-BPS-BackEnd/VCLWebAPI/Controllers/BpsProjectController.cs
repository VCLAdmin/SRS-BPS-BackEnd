using BpsUnifiedModelLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http;
//using System.Web.Http.Description;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using VCLWebAPI.Mappers;
using VCLWebAPI.Models;
using VCLWebAPI.Models.BPS;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Models.SRS;
using VCLWebAPI.Services;
using VCLWebAPI;

namespace VCLWebAPI.Controllers
{
    /// <summary>
    /// Defines the <see cref="BpsProjectController" />.
    /// </summary>
    [Authorize]
    public class BpsProjectController : BaseController
    {
        /// <summary>
        /// Defines the _bpsProjectService.
        /// </summary>
        private readonly BpsProjectService _bpsProjectService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BpsProjectController"/> class.
        /// </summary>
        public BpsProjectController()
        {
            _bpsProjectService = new BpsProjectService();
        }

        /// <summary>
        /// The DeleteOrders.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid?))]
        [Route("api/BpsProject/CancelAllOrders/{projectGuid}")]
        public IActionResult CancelAllOrders(string projectGuid)
        {
            try
            {
                return Ok(_bpsProjectService.CancelAllOrders(Guid.Parse(projectGuid)));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// The DeleteOrders.
        /// </summary>
        /// <param name="projectId">The projectId<see cref="int"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid?))]
        [Route("api/BpsProject/CancelAllProjectOrders/{projectId}")]
        public IActionResult CancelAllProjectOrders(int projectId)
        {
            try
            {
                return Ok(_bpsProjectService.CancelAllProjectOrders(projectId));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// The CopyProblemByGuid.
        /// </summary>
        /// <param name="problemGuid">The problemGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid?))]
        [Route("api/BpsProject/CopyProblemByGuid/{problemGuid}")]
        public IActionResult CopyProblemByGuid(string problemGuid)
        {
            try
            {
                return Ok(_bpsProjectService.CopyProblemByGuid(Guid.Parse(problemGuid)));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The CreateDefaultProblemForASEProject.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <param name="ASEType">The fl<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsUnifiedProblem))]
        [Route("api/BpsProject/CreateDefaultProblemForASEProject/{projectGuid}/{ASEType}")]
        public IActionResult CreateDefaultProblemForASEProject(string projectGuid, string ASEType)
        {
            try
            {
                return Ok(_bpsProjectService.CreateDefaultProblemForASEProject(Guid.Parse(projectGuid), ASEType));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The CreateDefaultProblemForFacadeProject.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <param name="fl">The fl<see cref="FacadeLayout"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsUnifiedProblem))]
        [Route("api/BpsProject/CreateDefaultProblemForFacadeProject/{projectGuid}")]
        public IActionResult CreateDefaultProblemForFacadeProject([FromRoute] string projectGuid, [FromBody] FacadeLayout fl)
        {
            try
            {
                return Ok(_bpsProjectService.CreateDefaultProblemForFacadeProject(Guid.Parse(projectGuid), fl.xPanelNo, fl.yPanelNo, fl.xInterval, fl.yInterval));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The CreateDefaultProblemForFacadeUDCProject.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsUnifiedProblem))]
        [Route("api/BpsProject/CreateDefaultProblemForFacadeUDCProject/{projectGuid}")]
        public IActionResult CreateDefaultProblemForFacadeUDCProject([FromRoute] string projectGuid)
        {
            try
            {
                return Ok(_bpsProjectService.CreateDefaultProblemForFacadeUDCProject(Guid.Parse(projectGuid)));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The CreateDefaultProblemForProject.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <param name="applicationType">The applicationType<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsUnifiedProblem))]
        [Route("api/BpsProject/CreateDefaultProblemForProject/{projectGuid}/{applicationType}")]
        public IActionResult CreateDefaultProblemForProject(string projectGuid, string applicationType)
        {
            try
            {
                if (applicationType.ToUpper() == "BPS")
                {
                    return Ok(_bpsProjectService.CreateDefaultProblemForProject(Guid.Parse(projectGuid)));
                }
                else if (applicationType.ToUpper() == "SRS")
                {
                    return Ok(_bpsProjectService.CreateDefaultWindowProblemForSRSProject(Guid.Parse(projectGuid)));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The CreateProject.
        /// </summary>
        /// <param name="project">The project<see cref="BpsProjectApiModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsProjectApiModel))]
        [Route("api/BpsProject/CreateProject/")]
        public IActionResult CreateProject([FromBody]BpsProjectApiModel project)
        {
            try
            {
                return Ok(_bpsProjectService.CreateProject(project)); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The DeleteOrderByGuid.
        /// </summary>
        /// <param name="problemGuid">The problemGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid?))]
        [Route("api/BpsProject/DeleteOrderByGuid/{problemGuid}")]
        public IActionResult DeleteOrderByGuid(string problemGuid)
        {
            try
            {
                return Ok(_bpsProjectService.DeleteOrderByGuid(Guid.Parse(problemGuid)));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The DeleteOrderById.
        /// </summary>
        /// <param name="problemId">The problemId<see cref="int"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpDelete]
        [Route("api/BpsProject/DeleteOrderById/{problemId}")]
        public List<OrderApiModel> DeleteOrderById(int problemId)
        {
            try
            {
                return _bpsProjectService.DeleteOrderById(problemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The DeleteProblemByGuid.
        /// </summary>
        /// <param name="problemGuid">The problemGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid?))]
        [Route("api/BpsProject/DeleteProblemByGuid/{problemGuid}")]
        public IActionResult DeleteProblemByGuid(string problemGuid)
        {
            try
            {
                return Ok(_bpsProjectService.DeleteProblemByGuid(Guid.Parse(problemGuid)));
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// The DeleteProblemById.
        /// </summary>
        /// <param name="problemId">The problemId<see cref="int"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpDelete]
        [Route("api/BpsProject/DeleteProblemById/{problemId}")]
        public async Task<List<OrderApiModel>> DeleteProblemById(int problemId)
        {
            try
            {
                return await _bpsProjectService.DeleteProblemById(problemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The DeleteProject.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid?))]
        [Route("api/BpsProject/DeleteAllProjects/{projectGuid}")]
        public IActionResult DeleteAllProjects(string projectGuid)
        {
           try
           {
               _bpsProjectService.DeleteAllProjects();
               return Ok(projectGuid);
           }
           catch (Exception ex)
           {
               throw;
           }
        }

        /// <summary>
        /// The DeleteProject.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid?))]
        [Route("api/BpsProject/DeleteProject/{projectGuid}")]
        public IActionResult DeleteProject(string projectGuid)
        {
            try
            {
                return Ok(_bpsProjectService.DeleteProject(Guid.Parse(projectGuid)));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetDefaultFacadeProblem.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <param name="problemGuid">The problemGuid<see cref="string"/>.</param>
        /// <param name="ASEType">The problemGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsUnifiedProblem))]
        [Route("api/BpsProject/GetDefaultASEProblem/{projectGuid}/{problemGuid}/{Type}")]
        public IActionResult GetDefaultASEProblem([FromRoute] string projectGuid, [FromRoute] string problemGuid, [FromRoute] string Type)
        {
            try
            {

                return Ok(_bpsProjectService.GetDefaultASEProblem(Guid.Parse(projectGuid), Guid.Parse(problemGuid), Type));
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetDefaultFacadeProblem.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <param name="problemGuid">The problemGuid<see cref="string"/>.</param>
        /// <param name="fl">The fl<see cref="FacadeLayout"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsUnifiedProblem))]
        [Route("api/BpsProject/GetDefaultFacadeProblem/{projectGuid}/{problemGuid}")]
        public IActionResult GetDefaultFacadeProblem([FromRoute] string projectGuid, [FromRoute] string problemGuid, [FromBody] FacadeLayout fl)
        {
            try
            {
                return Ok(_bpsProjectService.GetDefaultFacadeProblem(Guid.Parse(projectGuid), Guid.Parse(problemGuid), fl.xPanelNo, fl.yPanelNo, fl.xInterval, fl.yInterval));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetDefaultFacadeProblem.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <param name="problemGuid">The problemGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsUnifiedProblem))]
        [Route("api/BpsProject/GetDefaultFacadeUDCProblem/{projectGuid}/{problemGuid}")]
        public IActionResult GetDefaultFacadeUDCProblem([FromRoute] string projectGuid, [FromRoute] string problemGuid)
        {
            try
            {
                return Ok(_bpsProjectService.GetDefaultFacadeUDCProblem(Guid.Parse(projectGuid), Guid.Parse(problemGuid)));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetDefaultWindowProblem.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <param name="problemGuid">The problemGuid<see cref="string"/>.</param>
        /// <param name="applicationType">The problemGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsUnifiedProblem))]
        [Route("api/BpsProject/GetDefaultWindowProblem/{projectGuid}/{problemGuid}/{applicationType}")]
        public IActionResult GetDefaultWindowProblem(string projectGuid, string problemGuid, string applicationType)
        {
            try
            {
                if (applicationType.ToUpper() == "BPS")
                {
                    return Ok(_bpsProjectService.GetDefaultWindowProblem(Guid.Parse(projectGuid), Guid.Parse(problemGuid)));
                }
                else if (applicationType.ToUpper() == "SRS")
                {
                    return Ok(_bpsProjectService.GetDefaultWindowProblemForSRS(Guid.Parse(projectGuid), Guid.Parse(problemGuid)));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetProblemByGuid.
        /// </summary>
        /// <param name="problemGuid">The problemGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsUnifiedProblemApiModel))]
        [Route("api/BpsProject/GetProblemByGuid/{problemGuid}")]
        public IActionResult GetProblemByGuid(string problemGuid)
        {
            try
            {
                BpsUnifiedProblem unifiedProblem = _bpsProjectService.GetProblemByGuid(Guid.Parse(problemGuid));
                var projectMApper = new ProjectMapper();
                return Ok(projectMApper.BpsUnifiedProblemDbToApiModel(unifiedProblem));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetProblemsForProject.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="List{BpsUnifiedProblem}"/>.</returns>
        [HttpGet]
        [Route("api/BpsProject/GetProblemsForProject/{projectGuid}")]
        public List<BpsUnifiedProblemApiModel> GetProblemsForProject(string projectGuid)
        {
            try
            {
                var projectMApper = new ProjectMapper();
                var allProblems = _bpsProjectService.GetProblemsForProject(Guid.Parse(projectGuid));
                List<BpsUnifiedProblemApiModel> bpsUnifiedProblemList = new List<BpsUnifiedProblemApiModel>();
                foreach (var problem in allProblems)
                {
                    bpsUnifiedProblemList.Add(projectMApper.BpsUnifiedProblemDbToApiModel(problem));
                }
                return bpsUnifiedProblemList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// The GetProblemsForProjectLite.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="List{BpsUnifiedProblem}"/>.</returns>
        [HttpGet]
        [Route("api/BpsProject/GetProblemsForProjectLite/{projectGuid}")]
        public List<BpsUnifiedProblemApiModelLite> GetProblemsForProjectLite(string projectGuid)
        {
            try
            {
                var projectMApper = new ProjectMapper();
                var allProblems = _bpsProjectService.GetProblemsForProject(Guid.Parse(projectGuid));
                List<BpsUnifiedProblemApiModelLite> bpsUnifiedProblemList = new List<BpsUnifiedProblemApiModelLite>();
                foreach (var problem in allProblems)
                {
                    bpsUnifiedProblemList.Add(projectMApper.BpsUnifiedProblemDbToApiModelLite(problem));
                }
                return bpsUnifiedProblemList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetProjectByGuid.
        /// </summary>
        /// <param name="projectGuid">The projectGuid<see cref="string"/>.</param>
        /// <returns>The <see cref="BpsProjectApiModel"/>.</returns>
        [HttpGet]
        [Route("api/BpsProject/GetProjectByGuid/{projectGuid}")]
        public BpsProjectApiModel GetProjectByGuid(string projectGuid)
        {
            try
            {
                BpsProjectApiModel project = _bpsProjectService.GetProjectByGuid(Guid.Parse(projectGuid));
                return project;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetProjectsForCurrentUser.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BpsProjectApiModel>))]
        [Route("api/BpsProject/GetProjectsForCurrentUser")]
        public IActionResult GetProjectsForCurrentUser()
        {
            try
            {
                return Ok(_bpsProjectService.GetProjectsForCurrentUser());
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetReport.
        /// </summary>
        /// <param name="reportRequest">The reportRequest<see cref="ReportRequest"/>.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemoryStream))]
        [Route("api/BpsProject/GetReport/")]
        public HttpResponseMessage GetReport(ReportRequest reportRequest)
        {
            try
            {
                string reportURL = AppDomain.CurrentDomain.BaseDirectory + reportRequest.FolderPath + reportRequest.ReportFileName;
                MemoryStream reportStream = _bpsProjectService.GetReport(reportURL);
                HttpResponseMessage httpResponseMessage;
                httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                httpResponseMessage.Content = new StreamContent(reportStream);
                //
                httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                httpResponseMessage.Content.Headers.ContentDisposition.FileName = reportRequest.ReportFileName;
                httpResponseMessage.Content.Headers.ContentDisposition.Name = reportRequest.ProblemName;
                httpResponseMessage.Content.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                //
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                return httpResponseMessage;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetSRSProjectsForCurrentUser.
        /// </summary>
        /// <param name="status">The userId<see cref="string"/>.</param>
        /// <param name="dateValue">The userId<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BpsProjectApiModel>))]
        [Route("api/BpsProject/GetSRSProjectsForCurrentUser/{status}/{dateValue}")]
        public IActionResult GetSRSProjectsForCurrentUser(string status, string dateValue)
        {
            try
            {
                return Ok(_bpsProjectService.GetSRSProjectsForCurrentUser(status, dateValue));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The Update UnifiedModel to newer Version.
        /// </summary>
        /// <returns>The <see cref="List{JsonResponse}"/>.</returns>
        [HttpGet]
        [Route("api/BpsProject/MigrateUnifiedModelToV2")]
        public List<JsonResponse> MigrateUnifiedModelToV2()
        {
            try
            {
                return _bpsProjectService.MigrateUnifiedModelToV2();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/BpsProject/GetStateTax/{zipcode}")]
        public StateTax GetStateTax(string zipcode)
        {
            try
            {
                return _bpsProjectService.GetStateTax(zipcode);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        /// <summary>
        /// The RenameProblem.
        /// </summary>
        /// <param name="unifiedModel">The unifiedModel<see cref="BpsUnifiedModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsSimplifiedProblemApiModel))]
        [Route("api/BpsProject/RenameProblem/")]
        public IActionResult RenameProblem([FromBody]BpsUnifiedModel unifiedModel)
        {
            try
            {
                return Ok(_bpsProjectService.RenameProblem(unifiedModel));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The SaveProblemScreenShot.
        /// </summary>
        /// <param name="request">The request<see cref="SaveScreenShotRequest"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [Route("api/BpsProject/SaveProblemScreenShot/")]
        public IActionResult SaveProblemScreenShot([FromBody]SaveScreenShotRequest request)
        {
            try
            {
                return Ok(_bpsProjectService.SaveProblemScreenShot(request.problemGuid, request.imageData));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [Route("api/BpsProject/SaveProblemScreenShotS3/")]
        public async Task<bool> SaveProblemScreenShotS3([FromBody]SaveScreenShotRequest request)
        {
            try
            {
                return await _bpsProjectService.SaveProblemScreenShotS3(request.problemGuid, request.imageData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// The UpdateProblem.
        /// </summary>
        /// <param name="unifiedModel">The unifiedModel<see cref="BpsUnifiedModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid?))]
        [Route("api/BpsProject/UpdateProblem/")]
        public IActionResult UpdateProblem([FromBody]BpsUnifiedModel unifiedModel)
        {
            try
            {
                return Ok(_bpsProjectService.UpdateProblem(unifiedModel));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The UpdateProjectInfo.
        /// </summary>
        /// <param name="projectInfo">The projectInfo<see cref="ProjectInfo"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BpsSimplifiedProblemApiModel>))]
        [Route("api/BpsProject/UpdateProjectInfo/")]
        public IActionResult UpdateProjectInfo(ProjectInfo projectInfo)
        {
            try
            {
                return Ok(_bpsProjectService.UpdateProjectInfo(projectInfo));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The UpdateUserNotes.
        /// </summary>
        /// <param name="unifiedModel">The unifiedModel<see cref="BpsUnifiedModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsSimplifiedProblemApiModel))]
        [Route("api/BpsProject/UpdateUserNotes/")]
        public IActionResult UpdateUserNotes(BpsUnifiedModel unifiedModel)
        {
            try
            {
                return Ok(_bpsProjectService.UpdateUserNotes(unifiedModel));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The UploadResults.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BpsUnifiedModel))]
        [Route("api/BpsProject/UploadResults/")]
        public IActionResult UploadResults()
        {
            try
            {
                string strUnifiedModel = HttpContext.Request.Form["unifiedModel"];
                IFormFileCollection hfc = HttpContext.Request.Form.Files;
                BpsUnifiedModel unifiedModel = _bpsProjectService.UploadResults(strUnifiedModel, hfc);
                return Ok(unifiedModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Defines the <see cref="FacadeLayout" />.
    /// </summary>
    public class FacadeLayout
    {
        /// <summary>
        /// Defines the xInterval.
        /// </summary>
        public double xInterval = 2000;

        /// <summary>
        /// Defines the xPanelNo.
        /// </summary>
        public int xPanelNo;

        /// <summary>
        /// Defines the yInterval.
        /// </summary>
        public double yInterval = 2000;

        /// <summary>
        /// Defines the yPanelNo.
        /// </summary>
        public int yPanelNo;
    }

    /// <summary>
    /// Defines the <see cref="ReportRequest" />.
    /// </summary>
    public class ReportRequest
    {
        /// <summary>
        /// Defines the FolderPath.
        /// </summary>
        public string FolderPath;

        /// <summary>
        /// Defines the ProblemName.
        /// </summary>
        public string ProblemName;

        /// <summary>
        /// Defines the ReportFileName.
        /// </summary>
        public string ReportFileName;
    }

    /// <summary>
    /// Defines the <see cref="SaveScreenShotRequest" />.
    /// </summary>
    public class SaveScreenShotRequest
    {
        /// <summary>
        /// Defines the imageData.
        /// </summary>
        public string imageData;

        /// <summary>
        /// Defines the problemGuid.
        /// </summary>
        public string problemGuid;
    }
}
