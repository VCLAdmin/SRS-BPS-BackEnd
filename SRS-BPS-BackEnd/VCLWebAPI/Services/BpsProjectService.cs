using BpsUnifiedModelLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using VCLWebAPI.Mappers;
using VCLWebAPI.Models.BPS;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Models.SRS;
using VCLWebAPI.Utils;
using ViewResources;
using VCLMigration;
using VCLWebAPI.Models;
using Amazon.S3.Model;
using System.Threading.Tasks;
using Amazon.S3;

namespace VCLWebAPI.Services
{
    public class BpsProjectService
    {
        private readonly VCLDesignDBEntities _db;
        private readonly ProjectMapper _projectMapper;
        private readonly AccountService _accountService;
        private readonly DealerService _ds;
        private readonly OrderService _os;

        public BpsProjectService()
        {
            _db = new VCLDesignDBEntities();
            _projectMapper = new ProjectMapper();
            _accountService = new AccountService(_db);
            _ds = new DealerService();
            _os = new OrderService();
        }

        // **********************************
        // project-related API calls services
        // **********************************
        public BpsProjectApiModel CreateProject(BpsProjectApiModel project)
        {
            project.UserId = _accountService.GetCurrentUser().UserId;
            var add = new Address();
            if (project.Line1 != "")
            {
                add.AddressExternalId = Guid.NewGuid();
                add.Active = true;
                add.AdditionalDetails = "";
                add.Line1 = project.Line1 == null ? "." : project.Line1;
                add.Line2 = project.Line2;
                add.State = project.State;
                add.City = project.City == null ? "." : project.City;
                add.Country = project.Country;
                add.County = project.County;
                add.PostalCode = project.PostalCode == null ? "." : project.PostalCode;
                add.CreatedBy = _accountService.GetCurrentUser().UserId;
                add.CreatedOn = DateTime.Now;
                add.Latitude = project.Latitude == null ? 0 : project.Latitude;
                add.Longitude = project.Longitude == null ? 0 : project.Longitude;
                _db.Address.Add(add);
                _db.SaveChanges();
            }

            var bpsProject = new BpsProject
            {
                ProjectName = project.ProjectName,
                ProjectLocation = project.ProjectLocation,
                ProjectGuid = Guid.NewGuid(),
                AddressId = add.AddressId,
                UserId = project.UserId
            };

            _db.BpsProject.Add(bpsProject);
            _db.SaveChanges();

            bpsProject = _db.BpsProject.Single(x => x.ProjectGuid == bpsProject.ProjectGuid);
            project.ProjectGuid = bpsProject.ProjectGuid;
            project.ProjectId = bpsProject.ProjectId;
            project.CreatedOn = bpsProject.CreatedOn;

            return project;
        }

        public List<BpsProjectApiModel> GetProjectsForCurrentUser()
        {
            User currentUser = _accountService.GetCurrentUser();
            List<BpsProject> projects = _db.BpsProject.Where(x => x.UserId == currentUser.UserId).ToList();
            List<BpsUnifiedProblem> bpsUnifiedProblem = _db.BpsUnifiedProblem.ToList();
            List<BpsProjectApiModel> response = new List<BpsProjectApiModel>();
            List<int> problemIds;
            BpsUnifiedProblem problem;
            var projectMApper = new ProjectMapper();

            foreach (var project in projects)
            {
                if (project.Order.Count == 0)
                    project.Order = _db.Order.Where(e => e.ProjectId == project.ProjectId).ToList();
                DateTime projectModifiedOn = (DateTime)project.CreatedOn;
                problemIds = GetProblemIdsForProject(project.ProjectGuid, projects, bpsUnifiedProblem);
                foreach (int problemId in problemIds)
                {
                    problem = bpsUnifiedProblem.Single(x => x.ProblemId == problemId);
                    if (projectModifiedOn.CompareTo(problem.ModifiedOn) < 0)
                        projectModifiedOn = problem.ModifiedOn;
                }
                BpsProjectApiModel projectApiModel = projectMApper.ProjectDbToApiModel(project);
                projectApiModel.ModifiedOn = projectModifiedOn;
                projectApiModel.ProblemIds = problemIds;
                response.Add(projectApiModel);
            }
            return response;
        }

        public List<BpsProjectApiModel> GetSRSProjectsForCurrentUser(string status, string dateValue)
        {
            List<BpsProject> projects = new List<BpsProject>();
            User currentUser = _accountService.GetCurrentUser();
            string role = _accountService.GetUserRole(currentUser.Email);
            var dealer = currentUser.Dealer.FirstOrDefault();
            if (dealer != null)
            {
                var dealerInfo = _db.Dealer.Where(e => e.DealerId == dealer.DealerId).FirstOrDefault();
                foreach (var user in dealer.User)
                {
                    projects.AddRange(_db.BpsProject.Where(x => x.UserId == user.UserId).ToList());
                }
            }

            List<BpsUnifiedProblem> bpsUnifiedProblem = _db.BpsUnifiedProblem.ToList();
            List<BpsProjectApiModel> response = new List<BpsProjectApiModel>();
            List<int> problemIds;
            BpsUnifiedProblem problem;
            var projectMApper = new ProjectMapper();

            List<BpsProject> filterProjects = new List<BpsProject>();
            if (status != "ALL")
            {
                foreach (var item in projects.ToList())
                {
                    if (status == "In Process")
                    {
                        if (item.Order.Where(e => e.Order_Status.Any(c => c.StatusId == 6)).ToList().Count == 0)
                        {
                            filterProjects.Add(item);
                        }
                    }
                    else
                    {
                        if (item.Order.Where(e => e.Order_Status.Any(c => c.StatusId == 6)).ToList().Count > 0)
                        {
                            filterProjects.Add(item);
                        }
                    }
                }
                projects = filterProjects;
            }
            if (dateValue != "ALL")
            {
                DateTime[] dates = AppUtils.GetDateRange(dateValue);
                if (dates[0] != null && dates[1] != null)
                    projects = projects.Where(e => e.Order.Any(a => a.CreatedOn >= dates[0] && a.CreatedOn < dates[1])).ToList();
            }

            foreach (var project in projects)
            {
                if (project.Order.Count == 0)
                    project.Order = _db.Order.Where(e => e.ProjectId == project.ProjectId).ToList();
                DateTime projectModifiedOn = (DateTime)project.CreatedOn;
                problemIds = GetProblemIdsForProject(project.ProjectGuid, projects, bpsUnifiedProblem);
                foreach (int problemId in problemIds)
                {
                    problem = bpsUnifiedProblem.Single(x => x.ProblemId == problemId);
                    if (projectModifiedOn.CompareTo(problem.ModifiedOn) < 0)
                        projectModifiedOn = problem.ModifiedOn;
                }
                BpsProjectApiModel projectApiModel = projectMApper.ProjectDbToApiModel(project);
                projectApiModel.ModifiedOn = projectModifiedOn;
                projectApiModel.ProblemIds = problemIds;
                response.Add(projectApiModel);
            }

            return response;
        }

        // update Project info, including project name and project location
        public List<BpsSimplifiedProblemApiModel> UpdateProjectInfo(ProjectInfo projectInfo)
        {
            List<BpsSimplifiedProblemApiModel> responses = new List<BpsSimplifiedProblemApiModel>();
            BpsProject project = _db.BpsProject.Where(x => x.ProjectGuid == projectInfo.ProjectGuid).SingleOrDefault();
            if (project == null)
            {
                throw new InvalidDataException();
            }
            if (project is null) return null;
            if (!String.IsNullOrEmpty(projectInfo.ProjectName))
            {
                project.ProjectName = projectInfo.ProjectName;
            }
            if (!String.IsNullOrEmpty(projectInfo.Location))
            {
                project.ProjectLocation = projectInfo.Location;
                project.Address.Active = true;
                project.Address.AdditionalDetails = "";
                project.Address.Line1 = projectInfo.Line1;
                project.Address.Line2 = projectInfo.Line2;
                project.Address.State = projectInfo.State;
                project.Address.City = projectInfo.City;
                project.Address.Country = projectInfo.Country;
                project.Address.County = projectInfo.County;
                project.Address.PostalCode = projectInfo.PostalCode;
                project.Address.CreatedBy = _accountService.GetCurrentUser().UserId;
                project.Address.CreatedOn = DateTime.Now;
                project.Address.Latitude = projectInfo.Latitude == null ? 0 : projectInfo.Latitude;
                project.Address.Longitude = projectInfo.Longitude == null ? 0 : projectInfo.Longitude;
            }

            if (!String.IsNullOrEmpty(projectInfo.ProjectName) || !String.IsNullOrEmpty(projectInfo.Location))
            {
                _db.Entry(project).State = EntityState.Modified;
                _db.SaveChanges();
            }

            // find all problem guid in the project
            int projectId = project.ProjectId;
            List<BpsUnifiedProblem> problems = _db.BpsUnifiedProblem.Where(x => x.ProjectId == projectId).ToList();

            foreach (BpsUnifiedProblem problem in problems)
            {
                // update project name and location in unifiedModel
                BpsUnifiedModel unifiedModel = JsonConvert.DeserializeObject<BpsUnifiedModel>(problem.UnifiedModel);
                if (!String.IsNullOrEmpty(projectInfo.ProjectName)) unifiedModel.ProblemSetting.ProjectName = projectInfo.ProjectName;
                if (!String.IsNullOrEmpty(projectInfo.Location)) unifiedModel.ProblemSetting.Location = projectInfo.Location;
                string unifiedModelString = JsonConvert.SerializeObject(unifiedModel);
                problem.UnifiedModel = unifiedModelString;
                _db.Entry(problem).State = EntityState.Modified;
                _db.SaveChanges();

                //// update  project name and location in report
                //string reportFileUrl, summaryFileUrl, filePath, summaryFilePath;
                //bool updateStructuralReport = false, updateStructuralSummaryReport = false, updateAcousticReport = false, updateThermalReport = false;
                //if (!(unifiedModel.AnalysisResult is null))
                //{
                //    var pdfservice = new SBA.Services.PdfService();
                //    if (!(unifiedModel.AnalysisResult.StructuralResult is null) && !String.IsNullOrEmpty(unifiedModel.AnalysisResult.StructuralResult.reportFileUrl))
                //    {
                //        reportFileUrl = unifiedModel.AnalysisResult.StructuralResult.reportFileUrl.Substring(1);
                //        summaryFileUrl = unifiedModel.AnalysisResult.StructuralResult.summaryFileUrl.Substring(1);
                //        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportFileUrl);
                //        summaryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, summaryFileUrl);
                //        updateStructuralReport = pdfservice.UpdateProjectInfo(filePath, projectInfo.ProjectName, projectInfo.Location);
                //        updateStructuralSummaryReport = pdfservice.UpdateProjectInfo(summaryFilePath, projectInfo.ProjectName, projectInfo.Location);
                //    }

                //    // to update configuration name to acoustic report
                //    if (!(unifiedModel.AnalysisResult.AcousticResult is null) && !String.IsNullOrEmpty(unifiedModel.AnalysisResult.AcousticResult.reportFileUrl))
                //    {
                //        reportFileUrl = unifiedModel.AnalysisResult.AcousticResult.reportFileUrl.Substring(1);
                //        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportFileUrl);
                //        updateAcousticReport = pdfservice.UpdateProjectInfo(filePath, projectInfo.ProjectName, projectInfo.Location);
                //    }

                //    // to update configuration name to thermal report
                //    if (!(unifiedModel.AnalysisResult.ThermalResult is null) && !String.IsNullOrEmpty(unifiedModel.AnalysisResult.ThermalResult.reportFileUrl))
                //    {
                //        reportFileUrl = unifiedModel.AnalysisResult.ThermalResult.reportFileUrl.Substring(1);
                //        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportFileUrl);
                //        updateThermalReport = pdfservice.UpdateProjectInfo(filePath, projectInfo.ProjectName, projectInfo.Location);
                //    }
                //}

                BpsSimplifiedProblemApiModel response = new BpsSimplifiedProblemApiModel
                {
                    ProblemGuid = problem.ProblemGuid,
                    ProblemName = problem.ProblemName,
                    //updateStructuralReport = updateStructuralReport,
                    //updateAcousticReport = updateAcousticReport,
                    //updateThermalReport = updateThermalReport
                };
                responses.Add(response);
            }

            return responses;
        }

        // auxililar function for GetProjectsForCurrentUser
        public List<int> GetProblemIdsForProject(Guid projectGuid)
        {
            BpsProject project = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (project == null)
            {
                throw new InvalidDataException();
            }
            int projectId = project.ProjectId;
            List<int> problemIds = _db.BpsUnifiedProblem.Where(x => x.ProjectId == projectId).Select(x => x.ProblemId).ToList();
            return problemIds;
        }

        public List<string> GetProblemNamesForProject(Guid projectGuid)
        {
            BpsProject project = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (project == null)
            {
                throw new InvalidDataException();
            }
            int projectId = project.ProjectId;
            List<string> problemNames = _db.BpsUnifiedProblem.Where(x => x.ProjectId == projectId).Select(x => x.ProblemName).ToList();
            return problemNames;
        }
        public List<int> GetProblemIdsForProject(Guid projectGuid, List<BpsProject> projects, List<BpsUnifiedProblem> bpsUnifiedProblem)
        {
            BpsProject project = projects.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (project == null)
            {
                throw new InvalidDataException();
            }
            int projectId = project.ProjectId;
            List<int> problemIds = bpsUnifiedProblem.Where(x => x.ProjectId == projectId).Select(x => x.ProblemId).ToList();
            return problemIds;
        }

        public BpsProjectApiModel GetProjectByGuid(Guid projectGuid)
        {
            BpsProject project = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (project == null)
            {
                throw new InvalidDataException();
            }
            BpsProjectApiModel response = _projectMapper.ProjectDbToApiModel(project);

            return response;
        }

        //public bool PlaceOrder(Guid projectGuid)
        //{
        //    BpsProject project = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
        //    if (project == null)
        //    {
        //        throw new InvalidDataException();
        //    }
        //    List<BpsUnifiedProblem> problems = _db.BpsUnifiedProblem.Where(x => x.ProjectId == project.ProjectId).ToList();
        //    foreach (var problem in problems)
        //    {
        //        var um = JsonConvert.DeserializeObject<BpsUnifiedModel>(problem.UnifiedModel);
        //        um.ProblemSetting.OrderPlaced = true;
        //        um.ProblemSetting.OrderPlacedOn = DateTime.Now;
        //        problem.UnifiedModel = JsonConvert.SerializeObject(um);
        //        _db.Entry(problem).State = EntityState.Modified;
        //    }
        //    _db.SaveChanges();
        //    return true;
        //}
        public List<BpsUnifiedProblem> GetProblemsForProject(Guid projectGuid)
        {
            BpsProject project = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (project == null)
            {
                throw new InvalidDataException();
            }
            SetCurrentCulture(project);
            int projectId = project.ProjectId;
            List<BpsUnifiedProblem> problems = _db.BpsUnifiedProblem.Where(x => x.ProjectId == projectId).ToList();
            return problems;
        }
        public void DeleteAllProjects()
        {
            User currentUser = _accountService.GetCurrentUser();
            if (currentUser.Email.ToLower() == "Administrator@vcldesign.com".ToLower()) {
                List<BpsProject> projectList = _db.BpsProject.ToList();
                foreach (var item in projectList)
                {
                    DeleteProject(item.ProjectGuid);
                }
                //DeleteAllUsers();
            }
        }

        //public void DeleteAllUsers()
        //{
        //    SRSUserService srsUS = new SRSUserService();
        //    var userList = _db.User.ToList();
        //    foreach (var user in userList)
        //    {
        //        if (AllowedToChange(user.Email))
        //        {
        //            if (srsUS.CanDelete((Guid)user.UserGuid))
        //            {
        //                srsUS.Delete((Guid)user.UserGuid);
        //            }
        //        }
        //    }
        //}
        //public bool AllowedToChange(string email)
        //{
        //    var allowed = false;
        //    switch (email.ToLower())
        //    {
        //        case "srsadministrator@vcldesign.com":
        //            allowed = false;
        //            break;
        //        case "administrator@vcldesign.com":
        //            allowed = false;
        //            break;
        //        case "internal@vcldesign.com":
        //            allowed = false;
        //            break;
        //        case "designer@vcldesign.com":
        //            allowed = false;
        //            break;
        //        case "digitalproposal@vcldesign.com":
        //            allowed = false;
        //            break;
        //        case "acoustics@vcldesign.com":
        //            allowed = false;
        //            break;
        //        case "productconfigurator@vcldesign.com":
        //            allowed = false;
        //            break;
        //        default:
        //            allowed = true;
        //            break;
        //    }
        //    return allowed;
        //}

        public Guid? DeleteProject(Guid projectGuid)
        {
            BpsProject project = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (project == null)
            {
                throw new InvalidDataException();
            }
            User user = _db.User.Where(x => x.UserId == project.UserId).SingleOrDefault();
            if (user is null) return null;
            List<int> problemIds = GetProblemIdsForProject(projectGuid);

            foreach (int problemId in problemIds)
            {
                BpsUnifiedProblem problem = GetProblemById(problemId);
                DeleteOrders(problem, "project");
            }
            DeleteProjectOrders(project.ProjectId);

            Guid? userGuid = user.UserGuid;
            string pdfFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Content\\structural-result\\{userGuid}\\{projectGuid}\\");
            if (Directory.Exists(pdfFolderPath)) Directory.Delete(pdfFolderPath, true);
            if (project.AddressId != null)
                DeleteAddress((int)project.AddressId);
            _db.BpsProject.Remove(project);
            _db.Entry(project).State = EntityState.Deleted;
            _db.SaveChanges();
            return projectGuid;
        }
        public Guid? CancelAllProjectOrders(int projectId)
        {
            BpsProject project = _db.BpsProject.Where(x => x.ProjectId == projectId).SingleOrDefault();
            if (project == null)
            {
                throw new InvalidDataException();
            }
            return CancelAllOrders(project.ProjectGuid);
        }

        public Guid? CancelAllOrders(Guid projectGuid)
        {
            BpsProject project = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (project == null)
            {
                throw new InvalidDataException();
            }
            User user = _db.User.Where(x => x.UserId == project.UserId).SingleOrDefault();
            if (user is null) return null;
            List<int> problemIds = GetProblemIdsForProject(projectGuid);

            foreach (int problemId in problemIds)
            {
                BpsUnifiedProblem problem = GetProblemById(problemId);
                var probUnifModel = JsonConvert.DeserializeObject<BpsUnifiedModel>(problem.UnifiedModel);
                probUnifModel.AnalysisResult = null;
                probUnifModel.SRSProblemSetting.isOrderPlaced = false;
                problem.UnifiedModel = JsonConvert.SerializeObject(probUnifModel);
                DeleteOrders(problem, "DeleteAllOrders");
            }
            DeleteProjectOrders(project.ProjectId);
            _db.SaveChanges();

            return projectGuid;
        }
        
        // **********************************
        // problem-related API calls services
        // **********************************
        private BpsUnifiedProblem BuildDefaultWindowProblem(BpsProject dbProject, Guid projectGuid, Guid problemGuid, string problemName = null, bool addProductType = false, BpsUnifiedProblem oldProblem = null)
        {
            if (dbProject == null)
            {
                throw new InvalidDataException();
            }

            User user = _accountService.GetUser(dbProject.UserId);
            string userLanguage = String.Empty;
            var cultureInfo = new CultureInfo("en-US");
            if (user != null)
            {
                userLanguage = user.Language;
                cultureInfo = !String.IsNullOrEmpty(userLanguage) ? new CultureInfo(userLanguage) : cultureInfo;
            }
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            var ProductType = "";
            if (addProductType)
            {
                ProductType = "Window";
            }
            BpsUnifiedModel copyProblem = new BpsUnifiedModel();
            if (oldProblem != null)
                copyProblem = JsonConvert.DeserializeObject<BpsUnifiedModel>(oldProblem.UnifiedModel);
            BpsUnifiedProblem bpsUnifiedProblem = new BpsUnifiedProblem
            {
                ProblemGuid = problemGuid,
                ProblemName = problemName,
                ProjectId = dbProject.ProjectId,
                UnifiedModel = null
            };
            BpsUnifiedModel bpsUnifiedModel = new BpsUnifiedModel
            {
                UnifiedModelVersion = "V2",
                UserSetting = new UserSetting
                {
                    Language = userLanguage,
                    UserName = user.UserName,
                },
                ProblemSetting = new ProblemSetting
                {
                    EnableAcoustic = oldProblem != null ? copyProblem.ProblemSetting.EnableAcoustic : false,
                    EnableStructural = oldProblem != null ? copyProblem.ProblemSetting.EnableStructural : false,
                    EnableThermal = oldProblem != null ? copyProblem.ProblemSetting.EnableThermal : false,
                    UserGuid = (Guid)user.UserGuid,
                    ProjectGuid = projectGuid,
                    ProblemGuid = problemGuid,
                    ProductType = ProductType,
                    ProjectName = dbProject.ProjectName,
                    Location = dbProject.ProjectLocation,
                    ConfigurationName = problemName,
                    UserNotes = "",
                    //OrderPlaced = false,
                },
                ModelInput = new ModelInput
                {
                    FrameSystem = new FrameSystem
                    {
                        SystemType = "AWS 75.SI+",
                        UvalueType = "AIF",
                        InsulationType = "Polyamide Anodized After",
                        InsulationMaterial = "8",
                        Alloys = "6060-T66 (150MPa)"
                    },
                    Geometry = new Geometry
                    {
                        Points = new List<Point>(),
                        Members = new List<Member>(),
                        Sections = new List<Section>(),
                        Infills = new List<Infill>(),
                        GlazingSystems = new List<GlazingSystem>()
                    },
                    Acoustic = null,
                    Structural = new Structural
                    {
                        DispIndexType = 1,
                        DispHorizontalIndex = 0,
                        DispVerticalIndex = 0,
                        WindLoadInputType = 1,
                        dinWindLoadInput = null,
                        WindLoad = 0.96,
                        HorizontalLiveLoad = 0.0,
                        ShowBoundaryCondition = false,
                        Cpp = 1,
                        Cpn = -1,
                    },
                    Thermal = null,
                },
                CollapsedPanels = new CollapsedPanelStatus
                {
                    Panel_Configure = true,
                    Panel_Operability = false,
                    Panel_Framing = false,
                    Panel_Glass = false,
                    Panel_Acoustic = false,
                    Panel_Structural = false,
                    Panel_Thermal = false
                },
            };
            bpsUnifiedModel.ModelInput.Geometry.Points.Add(new Point { PointID = 1, X = 0, Y = 0 });
            bpsUnifiedModel.ModelInput.Geometry.Points.Add(new Point { PointID = 2, X = 0, Y = 1480 });
            bpsUnifiedModel.ModelInput.Geometry.Points.Add(new Point { PointID = 3, X = 1230, Y = 0 });
            bpsUnifiedModel.ModelInput.Geometry.Points.Add(new Point { PointID = 4, X = 1230, Y = 1480 });
            bpsUnifiedModel.ModelInput.Geometry.Members.Add(new Member { MemberID = 1, PointA = 1, PointB = 2, SectionID = 1, MemberType = 1 });
            bpsUnifiedModel.ModelInput.Geometry.Members.Add(new Member { MemberID = 2, PointA = 3, PointB = 4, SectionID = 1, MemberType = 1 });
            bpsUnifiedModel.ModelInput.Geometry.Members.Add(new Member { MemberID = 3, PointA = 1, PointB = 3, SectionID = 1, MemberType = 1 });
            bpsUnifiedModel.ModelInput.Geometry.Members.Add(new Member { MemberID = 4, PointA = 2, PointB = 4, SectionID = 1, MemberType = 1 });
            bpsUnifiedModel.ModelInput.Geometry.Sections.Add(new Section
            {
                SectionID = 1,
                SectionType = 1,
                ArticleName = "382110",
                InsideW = 26.0,
                OutsideW = 51.0,
                LeftRebate = -1.0,
                RightRebate = -1.0,
                DistBetweenIsoBars = 20.8,
                d = 75.0,
                Weight = 14.474686776721264,
                Ao = 188.51673928444947,
                Au = 192.49241124510917,
                Io = 12839.933348767925,
                Iu = 5674.8238143110484,
                Ioyy = 11967.261839460531,
                Iuyy = 40058.498648296067,
                Zoo = 11.888701242897334,
                Zuo = 10.425759612750095,
                Zou = 11.910278918890761,
                Zuu = 5.873210480321716,
                RSn20 = 0.0,
                RSp80 = 0.0,
                RTn20 = 0.0,
                RTp80 = 0.0,
                Cn20 = 0.0,
                Cp20 = 0.0,
                Cp80 = 0.0,
                beta = 0.0,
                A2 = 1.2,
                E = 70000.0,
                alpha = 0.0
            });
            bpsUnifiedModel.ModelInput.Geometry.Sections.Add(new Section
            {
                SectionID = 2,
                SectionType = 2,
                ArticleName = "382270",
                InsideW = 26.0,
                OutsideW = 76.0,
                LeftRebate = -1.0,
                RightRebate = -1.0,
                DistBetweenIsoBars = 16.0,
                d = 75.0,
                Weight = 15.434113076966556,
                Ao = 183.70289311685377,
                Au = 223.99917020522358,
                Io = 12452.370247852989,
                Iu = 5668.2073847122683,
                Ioyy = 11346.007399828639,
                Iuyy = 84807.002208860184,
                Zoo = 11.553686160262302,
                Zuo = 11.805384503601658,
                Zou = 12.242390580474101,
                Zuu = 4.4946360317557508,
                RSn20 = 87.3,
                RSp80 = 51.3,
                RTn20 = 64.4,
                RTp80 = 51.0,
                Cn20 = 78.0,
                Cp20 = 29.0,
                Cp80 = 46.0,
                beta = 150.0,
                A2 = 1.2,
                E = 70000.0,
                alpha = 0.0
            });
            bpsUnifiedModel.ModelInput.Geometry.Sections.Add(new Section
            {
                SectionID = 3,
                SectionType = 3,
                ArticleName = "382270",
                InsideW = 26.0,
                OutsideW = 76.0,
                LeftRebate = -1.0,
                RightRebate = -1.0,
                DistBetweenIsoBars = 16.0,
                d = 75.0,
                Weight = 15.434113076966556,
                Ao = 183.70289311685377,
                Au = 223.99917020522358,
                Io = 12452.370247852989,
                Iu = 5668.2073847122683,
                Ioyy = 11346.007399828639,
                Iuyy = 84807.002208860184,
                Zoo = 11.553686160262302,
                Zuo = 11.805384503601658,
                Zou = 12.242390580474101,
                Zuu = 4.4946360317557508,
                RSn20 = 87.3,
                RSp80 = 51.3,
                RTn20 = 64.4,
                RTp80 = 51.0,
                Cn20 = 78.0,
                Cp20 = 29.0,
                Cp80 = 46.0,
                beta = 150.0,
                A2 = 1.2,
                E = 70000.0,
                alpha = 0.0
            });
            bpsUnifiedModel.ModelInput.Geometry.Infills.Add(new Infill
            {
                InfillID = 1,
                BoundingMembers = new List<int> { 1, 4, 2, 3 },
                GlazingSystemID = 1,
                PanelSystemID = -1,
                OperabilitySystemID = -1
            });
            bpsUnifiedModel.ModelInput.Geometry.GlazingSystems.Add(new GlazingSystem
            {
                GlazingSystemID = 1,
                Rw = 31,
                UValue = 1.1,
                SpacerType = 1,
                Description = "4/16/4 (24 mm)",
                Plates = new List<Plate>(),
                Cavities = new List<Cavity>()
            });
            bpsUnifiedModel.ModelInput.Geometry.GlazingSystems[0].Plates.Add(new Plate { Material = "glass", H = 4, InterH = 0 });
            bpsUnifiedModel.ModelInput.Geometry.GlazingSystems[0].Plates.Add(new Plate { Material = "glass", H = 4, InterH = 0 });
            bpsUnifiedModel.ModelInput.Geometry.GlazingSystems[0].Cavities.Add(new Cavity { CavityType = "Air", Lz = 16 });
            bpsUnifiedProblem.UnifiedModel = JsonConvert.SerializeObject(bpsUnifiedModel);
            return bpsUnifiedProblem;
        }

        public BpsUnifiedProblem CreateDefaultProblemForProject(Guid projectGuid, string problemName = null)
        {
            BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (dbProject == null)
            {
                throw new InvalidDataException();
            }
            SetCurrentCulture(dbProject);
            // get new default problemName
            int problemCount = GetProblemIdsForProject(projectGuid).Count();
            List<BpsUnifiedProblem> allProblems = GetProblemsByName(dbProject.ProjectId, "");
            String[] allProblemNames = allProblems.Select(problem => problem.ProblemName).ToArray();
            string cteName = "";
            cteName = Resource.Configuration + " " + problemCount;
            while (allProblemNames.Contains(cteName))
            {
                problemCount += 1;
                cteName = Resource.Configuration + " " + problemCount;
            }
            Guid problemGuid = Guid.NewGuid();
            BpsUnifiedProblem bpsUnifiedProblem = BuildDefaultWindowProblem(dbProject, projectGuid, problemGuid, cteName, false);
            _db.BpsUnifiedProblem.Add(bpsUnifiedProblem);
            _db.SaveChanges();
            return bpsUnifiedProblem;
        }

        private void SetCurrentCulture(BpsProject dbProject)
        {
            User user = _accountService.GetUser(dbProject.UserId);
            var cultureInfo = new CultureInfo("en-US");
            if (user != null)
                cultureInfo = !String.IsNullOrEmpty(user.Language) ? new CultureInfo(user.Language) : cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        public BpsUnifiedProblem GetDefaultWindowProblem(Guid projectGuid, Guid problemGuid)
        {
            BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (dbProject == null)
            {
                throw new InvalidDataException();
            }
            BpsUnifiedProblem dbProblem = _db.BpsUnifiedProblem.Where(x => x.ProblemGuid == problemGuid).SingleOrDefault();
            string problemName = dbProblem.ProblemName;
            return BuildDefaultWindowProblem(dbProject, projectGuid, problemGuid, problemName, true);
        }

        // setup default problem for Facade stick-build
        private BpsUnifiedProblem BuildDefaultProblemForFacadeProject(BpsProject dbProject, Guid projectGuid, Guid problemGuid, string problemName, int xPanelNo, int yPanelNo, double xInterval, double yInterval, BpsUnifiedProblem oldProblem = null)
        {
            BpsUnifiedModel copyProblem = new BpsUnifiedModel();
            if (oldProblem != null)
                copyProblem = JsonConvert.DeserializeObject<BpsUnifiedModel>(oldProblem.UnifiedModel);

            if (dbProject == null)
            {
                throw new InvalidDataException();
            }

            User user = _accountService.GetUser(dbProject.UserId);
            string userLanguage = String.Empty;
            var cultureInfo = new CultureInfo("en-US");
            if (user != null)
            {
                userLanguage = user.Language;
                cultureInfo = !String.IsNullOrEmpty(userLanguage) ? new CultureInfo(userLanguage) : cultureInfo;
            }
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            xPanelNo = xPanelNo == 0 ? 3 : xPanelNo;
            yPanelNo = yPanelNo == 0 ? 2 : yPanelNo;

            // set default Unified Model
            BpsUnifiedModel bpsUnifiedModel = new BpsUnifiedModel
            {
                UnifiedModelVersion = "V2",
                UserSetting = new UserSetting
                {
                    Language = userLanguage,
                    UserName = user.UserName,
                },
                ProblemSetting = new ProblemSetting
                {
                    EnableAcoustic = oldProblem == null ? false : copyProblem.ProblemSetting.EnableAcoustic,
                    EnableStructural = oldProblem == null ? false : copyProblem.ProblemSetting.EnableStructural,
                    EnableThermal = oldProblem == null ? false : copyProblem.ProblemSetting.EnableThermal,
                    UserGuid = (Guid)user.UserGuid,
                    ProjectGuid = projectGuid,
                    ProblemGuid = problemGuid,
                    ProjectName = dbProject.ProjectName,
                    Location = dbProject.ProjectLocation,
                    ConfigurationName = problemName,
                    ProductType = "Facade",
                    FacadeType = "mullion-transom",
                    //OrderPlaced = false,
                },
                ModelInput = new ModelInput
                {
                    FrameSystem = new FrameSystem
                    {
                        SystemType = "FWS 50",
                        InsulationZone = "",
                        UvalueType = "AIF",
                        InsulationType = "PT",
                        Alloys = "6060-T66 (150MPa)",
                        xNumberOfPanels = xPanelNo,
                        yNumberOfPanels = yPanelNo,
                        MajorMullionTopRecess = 100,
                        MajorMullionBottomRecess = 150
                    },
                    Geometry = new Geometry
                    {
                        Points = new List<Point>(),
                        Members = new List<Member>(),
                        FacadeSections = new List<FacadeSection>(),
                        Infills = new List<Infill>(),
                        GlazingSystems = new List<GlazingSystem>(),
                        SlabAnchors = new List<SlabAnchor>(),
                    },
                    Structural = new Structural
                    {
                        DispIndexType = 0,
                        DispHorizontalIndex = 0,
                        DispVerticalIndex = 0,
                        WindLoadInputType = 1,
                        dinWindLoadInput = null,
                        WindLoad = 0.96,
                        Cpp = 1,
                        Cpn = -1,
                        HorizontalLiveLoad = 0,
                        HorizontalLiveLoadHeight = 0,
                        LoadFactor = null,
                        SeasonFactor = null,
                        TemperatureChange = null,
                        ShowBoundaryCondition = false
                    }
                },
                CollapsedPanels = new CollapsedPanelStatus
                {
                    Panel_Configure = true,
                    Panel_Operability = false,
                    Panel_Framing = false,
                    Panel_Glass = false,
                    Panel_Acoustic = false,
                    Panel_Structural = false,
                    Panel_Thermal = false
                },
            };

            // initialize Geometry
            Geometry geo = bpsUnifiedModel.ModelInput.Geometry;
            FacadeSection facadeMajorMullionSection = new FacadeSection
            {
                SectionID = 1,
                SectionType = 4,
                ArticleName = "536840",
                Depth = 125,
            };
            geo.FacadeSections.Add(facadeMajorMullionSection);
            FacadeSection transomSection = new FacadeSection
            {
                SectionID = 2,
                SectionType = 5,
                ArticleName = "322420",
                Depth = 110,
            };
            geo.FacadeSections.Add(transomSection);
            FacadeSection L2transomSection = new FacadeSection
            {
                SectionID = 5,
                SectionType = 5,
                ArticleName = "322340",
                Depth = 104,
            };
            geo.FacadeSections.Add(L2transomSection);
            FacadeSection facadeMinorMullionSection = new FacadeSection
            {
                SectionID = 3,
                SectionType = 6,
                ArticleName = "322340",
                Depth = 104,
            };
            geo.FacadeSections.Add(facadeMinorMullionSection);
            FacadeSection facadeReinforcementSection = new FacadeSection
            {
                SectionID = 4,
                SectionType = 7,
                ArticleName = "536840",
                Depth = 125,
            };
            geo.FacadeSections.Add(facadeReinforcementSection);

            // add Points
            int count = 0;
            double majorMullionBottomRecess = bpsUnifiedModel.ModelInput.FrameSystem.MajorMullionBottomRecess;
            double majorMullionTopRecess = bpsUnifiedModel.ModelInput.FrameSystem.MajorMullionTopRecess;
            for (int i = 1; i <= xPanelNo + 1; i++)
            {
                count = count + 2;
                Point point1 = new Point
                {
                    PointID = 2 * i - 1,
                    X = (i - 1) * xInterval,
                    Y = 0,
                };
                geo.Points.Add(point1);
                Point point2 = new Point
                {
                    PointID = 2 * i,
                    X = (i - 1) * xInterval,
                    Y = yPanelNo * yInterval,
                };
                geo.Points.Add(point2);
            }

            for (int j = 1; j <= yPanelNo + 1; j++)
            {
                for (int i = 1; i <= xPanelNo; i++)
                {
                    count = count + 2;
                    Point point1 = new Point
                    {
                        PointID = 2 * (xPanelNo + 1) + 2 * (j - 1) * xPanelNo + (2 * i - 1),
                        X = (i - 1) * xInterval,
                        Y = (j - 1) * yInterval,
                    };
                    geo.Points.Add(point1);
                    Point point2 = new Point
                    {
                        PointID = 2 * (xPanelNo + 1) + 2 * (j - 1) * xPanelNo + 2 * i,
                        X = i * xInterval,
                        Y = (j - 1) * yInterval,
                    };
                    geo.Points.Add(point2);
                }
            }

            // add mullion
            for (int i = 1; i <= xPanelNo + 1; i++)
            {
                Member member = new Member
                {
                    MemberID = i,
                    MemberType = 4,
                    SectionID = 1,
                    PointA = 2 * i - 1,
                    PointB = 2 * i,
                };
                geo.Members.Add(member);
            }

            // add slab anchor
            for (int i = 1; i <= xPanelNo + 1; i++)
            {
                SlabAnchor saBot = new SlabAnchor
                {
                    SlabAnchorID = 2 * i - 1,
                    MemberID = i,
                    AnchorType = "Fixed",
                    Y = -majorMullionBottomRecess,
                };
                geo.SlabAnchors.Add(saBot);
                SlabAnchor saTop = new SlabAnchor
                {
                    SlabAnchorID = 2 * i,
                    MemberID = i,
                    AnchorType = "Sliding",
                    Y = yPanelNo * yInterval + majorMullionTopRecess,
                };
                geo.SlabAnchors.Add(saTop);
            }

            // add transom
            count = xPanelNo + 1;
            for (int j = 1; j <= yPanelNo + 1; j++)
            {
                for (int i = 1; i <= xPanelNo; i++)
                {
                    count = count + 1;
                    Member member = new Member
                    {
                        MemberID = count,
                        MemberType = 5,
                        SectionID = 2,
                        PointA = 2 * (xPanelNo + 1) + 2 * (j - 1) * xPanelNo + (2 * i - 1),
                        PointB = 2 * (xPanelNo + 1) + 2 * (j - 1) * xPanelNo + 2 * i,
                    };
                    geo.Members.Add(member);
                }
            }

            //add SlabAnchor
            geo.Reinforcements = new List<Reinforcement>();
            geo.SpliceJoints = new List<SpliceJoint>();

            // add glass
            count = 0;
            for (int j = 1; j <= yPanelNo; j++)
            {
                for (int i = 1; i <= xPanelNo; i++)
                {
                    count = count + 1;
                    Infill glass = new Infill
                    {
                        InfillID = count,
                        BoundingMembers = new List<int> { i, (j + 1) * xPanelNo + 1 + i, i + 1, j * xPanelNo + 1 + i },
                        GlazingSystemID = 1,
                        PanelSystemID = -1,
                        OperabilitySystemID = -1
                    };
                    geo.Infills.Add(glass);
                }
            }

            // add glazing system
            GlazingSystem gs = new GlazingSystem
            {
                GlazingSystemID = 1,
                Rw = 37.0,
                UValue = 1.1,
                SpacerType = 1,
                Description = "8/16/4 (28 mm)",
                Plates = new List<Plate>(),
                Cavities = new List<Cavity>(),
            };
            // add plates and cavities
            Plate plate = new Plate
            {
                Material = "glass",
                H = 8,
                InterMaterial = "",
                InterH = 0
            };
            Plate plate2 = new Plate
            {
                Material = "glass",
                H = 4,
                InterMaterial = "",
                InterH = 0
            };
            gs.Plates.Add(plate);
            gs.Plates.Add(plate2);
            Cavity cavity = new Cavity
            {
                CavityType = "Argon",
                Lz = 16.0,
            };
            gs.Cavities.Add(cavity);
            geo.GlazingSystems.Add(gs);

            string strUnifiedModel = JsonConvert.SerializeObject(bpsUnifiedModel);

            // initialize bpsUnifiedProblem
            BpsUnifiedProblem bpsUnifiedProblem = new BpsUnifiedProblem
            {
                ProblemGuid = problemGuid,
                ProblemName = problemName,
                ProjectId = dbProject.ProjectId,
                UnifiedModel = strUnifiedModel
            };
            return bpsUnifiedProblem;
        }

        public BpsUnifiedProblem CreateDefaultProblemForFacadeProject(Guid projectGuid, int xPanelNo, int yPanelNo, double xInterval, double yInterval, string problemName = null)
        {
            BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            // get new default problemName
            int problemCount = GetProblemIdsForProject(projectGuid).Count();
            SetCurrentCulture(dbProject);
            if (String.IsNullOrEmpty(problemName)) problemName = Resource.Configuration + " " + (problemCount + 1);
            Guid problemGuid = Guid.NewGuid();
            var bpsUnifiedProblem = BuildDefaultProblemForFacadeProject(dbProject, projectGuid, problemGuid, problemName, xPanelNo, yPanelNo, xInterval, yInterval);
            _db.BpsUnifiedProblem.Add(bpsUnifiedProblem);
            _db.SaveChanges();
            return bpsUnifiedProblem;
        }

        // setup default problem for UDC
        private BpsUnifiedProblem BuildDefaultProblemForFacadeUDCProject(BpsProject dbProject, Guid projectGuid, Guid problemGuid, string problemName, BpsUnifiedProblem oldProblem = null)
        {
            BpsUnifiedModel copyProblem = new BpsUnifiedModel();
            if (oldProblem != null)
                copyProblem = JsonConvert.DeserializeObject<BpsUnifiedModel>(oldProblem.UnifiedModel);

            if (dbProject == null)
            {
                throw new InvalidDataException();
            }

            User user = _accountService.GetUser(dbProject.UserId);
            string userLanguage = String.Empty;
            var cultureInfo = new CultureInfo("en-US");
            if (user != null)
            {
                userLanguage = user.Language;
                cultureInfo = !String.IsNullOrEmpty(userLanguage) ? new CultureInfo(userLanguage) : cultureInfo;
            }
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            // set default Unified Model
            BpsUnifiedModel bpsUnifiedModel = new BpsUnifiedModel
            {
                UnifiedModelVersion = "V2",
                UserSetting = new UserSetting
                {
                    Language = userLanguage,
                    UserName = user.UserName,
                },
                ProblemSetting = new ProblemSetting
                {
                    EnableAcoustic = oldProblem == null ? false : copyProblem.ProblemSetting.EnableAcoustic,
                    EnableStructural = oldProblem == null ? false : copyProblem.ProblemSetting.EnableStructural,
                    EnableThermal = oldProblem == null ? false : copyProblem.ProblemSetting.EnableThermal,
                    UserGuid = (Guid)user.UserGuid,
                    ProjectGuid = projectGuid,
                    ProblemGuid = problemGuid,
                    ProjectName = dbProject.ProjectName,
                    Location = dbProject.ProjectLocation,
                    ConfigurationName = problemName,
                    ProductType = "Facade",
                    FacadeType = "UDC",
                    //OrderPlaced = false,
                },
                ModelInput = new ModelInput
                {
                    FrameSystem = new FrameSystem
                    {
                        SystemType = "UDC 80",
                        InsulationZone = "",
                        UvalueType = "AIF",
                        InsulationType = "PT",
                        Alloys = "6060-T66 (150MPa)",
                        VerticalJointWidth = 10,
                        HorizontalJointWidth = 10,
                    },
                    Geometry = new Geometry
                    {
                        Points = new List<Point>(),
                        Members = new List<Member>(),
                        FacadeSections = new List<FacadeSection>(),
                        Infills = new List<Infill>(),
                        GlazingSystems = new List<GlazingSystem>(),
                        SlabAnchors = new List<SlabAnchor>(),
                    },
                    Structural = new Structural
                    {
                        DispIndexType = 0,
                        DispHorizontalIndex = 0,
                        DispVerticalIndex = 0,
                        WindLoadInputType = 1,
                        dinWindLoadInput = null,
                        WindLoad = 0.96,
                        Cpp = 1,
                        Cpn = -1,
                        HorizontalLiveLoad = 0,
                        HorizontalLiveLoadHeight = 0,
                        LoadFactor = null,
                        SeasonFactor = null,
                        TemperatureChange = null,
                        ShowBoundaryCondition = false
                    }
                },
                CollapsedPanels = new CollapsedPanelStatus
                {
                    Panel_Configure = true,
                    Panel_Operability = false,
                    Panel_Framing = false,
                    Panel_Glass = false,
                    Panel_Acoustic = false,
                    Panel_Structural = false,
                    Panel_Thermal = false
                },
            };

            // initialize Geometry
            Geometry geo = bpsUnifiedModel.ModelInput.Geometry;
            double width = 1230, height = 1480;
            geo.Points.Add(new Point { PointID = 1, X = 0, Y = 0 });
            geo.Points.Add(new Point { PointID = 2, X = 0, Y = height });
            geo.Points.Add(new Point { PointID = 3, X = width, Y = 0 });
            geo.Points.Add(new Point { PointID = 4, X = width, Y = height });
            geo.Members.Add(new Member { MemberID = 1, PointA = 1, PointB = 2, SectionID = 2, MemberType = 22 });
            geo.Members.Add(new Member { MemberID = 2, PointA = 3, PointB = 4, SectionID = 2, MemberType = 22 });
            geo.Members.Add(new Member { MemberID = 3, PointA = 1, PointB = 3, SectionID = 3, MemberType = 23 });
            geo.Members.Add(new Member { MemberID = 4, PointA = 2, PointB = 4, SectionID = 1, MemberType = 21 });
            FacadeSection udcTopFrameSection = new FacadeSection
            {
                SectionID = 1,
                SectionType = 21,
                ArticleName = "505200",
                Depth = 135
            };
            geo.FacadeSections.Add(udcTopFrameSection);
            FacadeSection udcVerticalFrameSection = new FacadeSection
            {
                SectionID = 2,
                SectionType = 22,
                ArticleName = "505200",
                Depth = 135
            };
            geo.FacadeSections.Add(udcVerticalFrameSection);
            FacadeSection udcBottomFrameSection = new FacadeSection
            {
                SectionID = 3,
                SectionType = 23,
                ArticleName = "505200",
                Depth = 135
            };
            geo.FacadeSections.Add(udcBottomFrameSection);
            FacadeSection udcVerticalGlazingBarSection = new FacadeSection
            {
                SectionID = 4,
                SectionType = 24,
                ArticleName = "505420",
                Width = 80,
                Depth = 129
            };
            geo.FacadeSections.Add(udcVerticalGlazingBarSection);
            FacadeSection udcHorizontalGlazingBarSection = new FacadeSection
            {
                SectionID = 5,
                SectionType = 25,
                ArticleName = "505420",
                Width = 80,
                Depth = 129
            };
            geo.FacadeSections.Add(udcHorizontalGlazingBarSection);


            // add slab anchor
            for (int i = 1; i <= 2; i++)
            {
                SlabAnchor saBot = new SlabAnchor
                {
                    SlabAnchorID = 2 * i - 1,
                    MemberID = i,
                    AnchorType = "Fixed",
                    Y = 0,
                };
                geo.SlabAnchors.Add(saBot);
                SlabAnchor saTop = new SlabAnchor
                {
                    SlabAnchorID = 2 * i,
                    MemberID = i,
                    AnchorType = "Sliding",
                    Y = height,
                };
                geo.SlabAnchors.Add(saTop);
            }

            geo.Infills.Add(new Infill
            {
                InfillID = 1,
                BoundingMembers = new List<int> { 1, 4, 2, 3 },
                GlazingSystemID = 1,
                PanelSystemID = -1,
                OperabilitySystemID = -1
            });
            geo.GlazingSystems.Add(new GlazingSystem
            {
                GlazingSystemID = 1,
                Rw = 31,
                UValue = 1.1,
                SpacerType = 1,
                Description = "4/16/4 (24 mm)",
                Plates = new List<Plate>(),
                Cavities = new List<Cavity>()
            });
            geo.GlazingSystems[0].Plates.Add(new Plate { Material = "glass", H = 4, InterH = 0 });
            geo.GlazingSystems[0].Plates.Add(new Plate { Material = "glass", H = 4, InterH = 0 });
            geo.GlazingSystems[0].Cavities.Add(new Cavity { CavityType = "Air", Lz = 16 });

            string strUnifiedModel = JsonConvert.SerializeObject(bpsUnifiedModel);

            // initialize bpsUnifiedProblem
            BpsUnifiedProblem bpsUnifiedProblem = new BpsUnifiedProblem
            {
                ProblemGuid = problemGuid,
                ProblemName = problemName,
                ProjectId = dbProject.ProjectId,
                UnifiedModel = strUnifiedModel
            };
            return bpsUnifiedProblem;
        }

        public BpsUnifiedProblem CreateDefaultProblemForFacadeUDCProject(Guid projectGuid, string problemName = null)
        {
            BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            // get new default problemName
            int problemCount = GetProblemIdsForProject(projectGuid).Count();
            SetCurrentCulture(dbProject);
            if (String.IsNullOrEmpty(problemName)) problemName = Resource.Configuration + " " + (problemCount + 1);
            Guid problemGuid = Guid.NewGuid();
            var bpsUnifiedProblem = BuildDefaultProblemForFacadeUDCProject(dbProject, projectGuid, problemGuid, problemName);
            _db.BpsUnifiedProblem.Add(bpsUnifiedProblem);
            _db.SaveChanges();
            return bpsUnifiedProblem;
        }

        public BpsUnifiedProblem GetDefaultFacadeProblem(Guid projectGuid, Guid problemGuid, int xPanelNo, int yPanelNo, double xInterval, double yInterval)
        {
            BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (dbProject == null)
            {
                throw new InvalidDataException();
            }
            BpsUnifiedProblem dbProblem = _db.BpsUnifiedProblem.Where(x => x.ProblemGuid == problemGuid).SingleOrDefault();
            string problemName = dbProblem.ProblemName;
            return BuildDefaultProblemForFacadeProject(dbProject, projectGuid, problemGuid, problemName, xPanelNo, yPanelNo, xInterval, yInterval);
        }
        public BpsUnifiedProblem GetDefaultFacadeUDCProblem(Guid projectGuid, Guid problemGuid)
        {
            BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (dbProject == null)
            {
                throw new InvalidDataException();
            }
            BpsUnifiedProblem dbProblem = _db.BpsUnifiedProblem.Where(x => x.ProblemGuid == problemGuid).SingleOrDefault();
            string problemName = dbProblem.ProblemName;
            return BuildDefaultProblemForFacadeUDCProject(dbProject, projectGuid, problemGuid, problemName);
        }

        public BpsUnifiedProblem CreateDefaultProblemForASEProject(Guid projectGuid, string ASEtype, string problemName = null)
        {
            BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            // get new default problemName
            int problemCount = GetProblemIdsForProject(projectGuid).Count();
            SetCurrentCulture(dbProject);
            if (String.IsNullOrEmpty(problemName)) problemName = Resource.Configuration + " " + (problemCount + 1);
            Guid problemGuid = Guid.NewGuid();
            var bpsUnifiedProblem = BuildDefaultProblemForASEProject(dbProject, projectGuid, problemGuid, problemName, ASEtype);
            _db.BpsUnifiedProblem.Add(bpsUnifiedProblem);
            _db.SaveChanges();
            return bpsUnifiedProblem;
        }

        public BpsUnifiedProblem GetDefaultASEProblem(Guid projectGuid, Guid problemGuid, string ASEtype)
        {
            BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (dbProject == null)
            {
                throw new InvalidDataException();
            }
            BpsUnifiedProblem dbProblem = _db.BpsUnifiedProblem.Where(x => x.ProblemGuid == problemGuid).SingleOrDefault();
            string problemName = dbProblem.ProblemName;
            return BuildDefaultProblemForASEProject(dbProject, projectGuid, problemGuid, problemName, ASEtype);
        }

        private BpsUnifiedProblem BuildDefaultProblemForASEProject(BpsProject dbProject, Guid projectGuid, Guid problemGuid, string problemName, string ASEtype, BpsUnifiedProblem oldProblem = null)
        {
            BpsUnifiedModel copyProblem = new BpsUnifiedModel();
            if (oldProblem != null)
                copyProblem = JsonConvert.DeserializeObject<BpsUnifiedModel>(oldProblem.UnifiedModel);

            if (dbProject == null)
            {
                throw new InvalidDataException();
            }

            User user = _accountService.GetUser(dbProject.UserId);
            string userLanguage = String.Empty;
            var cultureInfo = new CultureInfo("en-US");
            if (user != null)
            {
                userLanguage = user.Language;
                cultureInfo = !String.IsNullOrEmpty(userLanguage) ? new CultureInfo(userLanguage) : cultureInfo;
            }
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;


            // read default model from default model files
            string defaultModelFilepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Resources/DefaultModels/ASE/{ASEtype}_DefaultModel.json");
            string strInput = "";
            using (StreamReader inputFile = new StreamReader(defaultModelFilepath))
            {
                strInput = inputFile.ReadToEnd();
            }
            BpsUnifiedModel bpsUnifiedModel = JsonConvert.DeserializeObject<BpsUnifiedModel>(strInput);

            // set default Unified Model
            bpsUnifiedModel.ProblemSetting.UserGuid = (Guid)user.UserGuid;
            bpsUnifiedModel.ProblemSetting.ProjectGuid = projectGuid;
            bpsUnifiedModel.ProblemSetting.ProblemGuid = problemGuid;

            string strUnifiedModel = JsonConvert.SerializeObject(bpsUnifiedModel);

            // initialize bpsUnifiedProblem
            BpsUnifiedProblem bpsUnifiedProblem = new BpsUnifiedProblem
            {
                ProblemGuid = problemGuid,
                ProblemName = problemName,
                ProjectId = dbProject.ProjectId,
                UnifiedModel = strUnifiedModel
            };
            return bpsUnifiedProblem;
        }



        // public BpsUnifiedProblem CloneProblemForProject(Guid projectGuid, Guid problemGuid)
        // {
        //     BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
        //     BpsUnifiedProblem oldProblem = GetProblemByGuid(problemGuid);

        //     // get new default problemName
        //     int problemCount = GetProblemIdsForProject(projectGuid).Count();
        //     var problemName = Resource.Configuration + " " + (problemCount + 1);
        //     problemGuid = Guid.NewGuid();
        //     var bpsUnifiedProblem = BuildDefaultWindowProblem(dbProject, projectGuid, problemGuid, problemName, true, oldProblem);
        //     _db.BpsUnifiedProblem.Add(bpsUnifiedProblem);
        //     _db.SaveChanges();
        //     return bpsUnifiedProblem;
        // }
        // public BpsUnifiedProblem CloneProblemForFacadeProject(Guid projectGuid, Guid problemGuid)
        // {
        //     BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
        //     BpsUnifiedProblem oldProblem = GetProblemByGuid(problemGuid);
        //     BpsUnifiedModel copyProblem = JsonConvert.DeserializeObject<BpsUnifiedModel>(oldProblem.UnifiedModel);
        //     var xPanelNo = copyProblem.ModelInput.FrameSystem.xNumberOfPanels;
        //     var yPanelNo = copyProblem.ModelInput.FrameSystem.yNumberOfPanels;
        //     var xInterval = 2000;
        //     var yInterval = 2000;
        //     // get new default problemName
        //     int problemCount = GetProblemIdsForProject(projectGuid).Count();
        //     var problemName = Resource.Configuration + " " + (problemCount + 1);
        //     problemGuid = Guid.NewGuid();
        //     var bpsUnifiedProblem = BuildDefaultProblemForFacadeProject(dbProject, projectGuid, problemGuid, problemName, xPanelNo, yPanelNo, xInterval, yInterval, oldProblem);
        //     _db.BpsUnifiedProblem.Add(bpsUnifiedProblem);
        //     _db.SaveChanges();
        //     return bpsUnifiedProblem;
        // }

        public BpsUnifiedProblem GetProblemByGuid(Guid problemGuid)
        {
            BpsUnifiedProblem problem = _db.BpsUnifiedProblem.Where(x => x.ProblemGuid == problemGuid).SingleOrDefault();
            if (problem == null)
            {
                throw new InvalidDataException();
            }
            return problem;
        }

        public BpsUnifiedProblem GetProblemById(int problemId)
        {
            BpsUnifiedProblem problem = _db.BpsUnifiedProblem.Where(x => x.ProblemId == problemId).SingleOrDefault();
            if (problem == null)
            {
                throw new InvalidDataException();
            }
            return problem;
        }

        public List<BpsUnifiedProblem> GetProblemsByName(int projectId, string problemName)
        {
            var configs = _db.BpsUnifiedProblem.Where(x => x.ProjectId == projectId && x.ProblemName.Contains(problemName)).ToList();
            return configs;
        }

        public void UpdateOrderNumbers()
        {
            List<BpsUnifiedProblem> dbProblems = _db.BpsUnifiedProblem.ToList();
            foreach (var prob in dbProblems)
            {
                BpsUnifiedModel unifiedModel = JsonConvert.DeserializeObject<BpsUnifiedModel>(prob.UnifiedModel);
                unifiedModel.SRSProblemSetting.OrderNumber = prob.ProjectId + "-" + prob.ProblemId;
                _db.Entry(dbProblems).State = EntityState.Modified;
            }
            _db.SaveChanges();
        }


        public Guid? UpdateProblem(BpsUnifiedModel unifiedModel)
        {
            if (unifiedModel == null)
            {
                throw new InvalidDataException();
            }
            Guid problemGuid = unifiedModel.ProblemSetting.ProblemGuid;
            if (unifiedModel.SRSProblemSetting != null) {
                var problem = GetProblemByGuid(problemGuid);
                unifiedModel.SRSProblemSetting.OrderNumber = problem.ProjectId + "-" + problem.ProblemId;
            }
            string unifiedModelString = JsonConvert.SerializeObject(unifiedModel);
            BpsUnifiedProblem bpsUnifiedProblem = _db.BpsUnifiedProblem.Where(x => x.ProblemGuid == problemGuid).SingleOrDefault();
            if (bpsUnifiedProblem is null) return null;

            bpsUnifiedProblem.UnifiedModel = unifiedModelString;

            _db.Entry(bpsUnifiedProblem).State = EntityState.Modified;
            _db.SaveChanges();

            return problemGuid;
        }
        public List<OrderApiModel> DeleteOrderById(int problemId)
        {
            BpsUnifiedProblem project = _db.BpsUnifiedProblem.Where(x => x.ProblemId == problemId).SingleOrDefault();
            DeleteOrderByGuid(project.ProblemGuid);
            return _db.Order.Where(e => e.OrderDetails.Count > 0).ToList().Select(s => _projectMapper.ProjectDbToApiModel(s)).ToList();
        }
        public List<OrderApiModel> DeleteProblemById(int problemId)
        {
            BpsUnifiedProblem project = _db.BpsUnifiedProblem.Where(x => x.ProblemId == problemId).SingleOrDefault();
            DeleteProblemByGuid(project.ProblemGuid);
            return _db.Order.Where(e => e.OrderDetails.Count > 0).ToList().Select(s => _projectMapper.ProjectDbToApiModel(s)).ToList();
        }
        public Guid? DeleteProblemByGuid(Guid problemGuid)
        {
            BpsUnifiedProblem problem = GetProblemByGuid(problemGuid);
            if (problem is null)
                throw new InvalidDataException();

            BpsProject project = _db.BpsProject.Where(x => x.ProjectId == problem.ProjectId).SingleOrDefault();
            if (project is null) return null;
            User user = _db.User.Where(x => x.UserId == project.UserId).SingleOrDefault();
            if (user is null) return null;
            Guid? projectGuid = project.ProjectGuid;
            Guid? userGuid = user.UserGuid;
            string pdfFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Content\\structural-result\\{userGuid}\\{projectGuid}\\{problemGuid}\\");
            if (Directory.Exists(pdfFolderPath)) Directory.Delete(pdfFolderPath, true);

            DeleteFile(problemGuid.ToString());

            DeleteProblem(problem, "child");
            return problemGuid;
        }


        public bool CheckFileExist(string key, string bucketName, AmazonS3Client s3Client)
        {
            try
            {
                var fileObject = s3Client.GetObject(bucketName, key);
                if (fileObject != null)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
                //throw new NullReferenceException("File not present on s3.");
            }
        }
        public bool DeleteFile(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            string accessKey = System.Configuration.ConfigurationManager.AppSettings["DE_AWSAccessKey"];
            string secretKey = System.Configuration.ConfigurationManager.AppSettings["DE_AWSSecretKey"];
            string service_url = System.Configuration.ConfigurationManager.AppSettings["DES3ServiceUrl"];
            string bucket_name = System.Configuration.ConfigurationManager.AppSettings["DEAWSBucket"];
            string localFileFullPath = "screenshots/" + key + ".png";

            AmazonS3Client s3Client = new AmazonS3Client(
            accessKey,
            secretKey,
            new AmazonS3Config
            {
                ServiceURL = service_url
            });
            if (CheckFileExist(localFileFullPath, bucket_name, s3Client))
            {
                s3Client.DeleteObject(bucket_name, localFileFullPath);
            }
            return true;
        }

        public Guid? DeleteOrderByGuid(Guid problemGuid)
        {
            BpsUnifiedProblem problem = GetProblemByGuid(problemGuid);
            if (problem is null)
                throw new InvalidDataException();
            DeleteOrders(problem, "child");
            return problemGuid;
        }
        public void DeleteProblem(BpsUnifiedProblem problem, string deleteType)
        {
            if (problem is null)
                throw new InvalidDataException();

            BpsProject project = _db.BpsProject.Where(e => e.ProjectId == problem.ProjectId).FirstOrDefault();
            List<int> problemIds = GetProblemIdsForProject(project.ProjectGuid);
            if (problemIds.Count == 1 && deleteType == "child")
            {

                User user = _db.User.Where(x => x.UserId == project.UserId).SingleOrDefault();
                DeleteProjectOrders(project.ProjectId);

                Guid? userGuid = user.UserGuid;
                string pdfFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Content\\structural-result\\{userGuid}\\{project.ProjectGuid}\\");
                if (Directory.Exists(pdfFolderPath)) Directory.Delete(pdfFolderPath, true);

                _db.BpsProject.Remove(project);
                _db.Entry(project).State = EntityState.Deleted;
                _db.SaveChanges();

            }
            else
            {
                List<Order> order = _db.Order.Where(e => e.OrderDetails.Any(a => a.ProductId == problem.ProblemId)).ToList();
                foreach (var item in order)
                {
                    var amount = DeleteOrderDetails(item.OrderId);
                    DeleteOrderStatus(item.OrderId);
                    DeleteOrder(item.OrderId, amount);
                }
                _db.SaveChanges();
            }
            _db.BpsUnifiedProblem.Remove(problem);
            _db.Entry(problem).State = EntityState.Deleted;
            _db.SaveChanges();
            return;
        }

        public void DeleteOrders(BpsUnifiedProblem problem, string deleteType)
        {
            if (problem is null)
                throw new InvalidDataException();

            BpsProject project = _db.BpsProject.Where(e => e.ProjectId == problem.ProjectId).FirstOrDefault();
            int ordersLeft = 0;
            List<Order> orderList = _db.Order.Where(e => e.ProjectId == problem.ProjectId).ToList();
            if (orderList.Count > 0) {
                if (orderList.Count == 1)
                {
                    ordersLeft = 1;
                }
                else
                {
                    var parentOrder = orderList.Where(e => e.ParentOrderId == null).FirstOrDefault();
                    ordersLeft = orderList.Where(e => e.ParentOrderId != null).ToList().Count;
                }
            }

            List<int> problemIds = GetProblemIdsForProject(project.ProjectGuid);
            if (ordersLeft == 1 && deleteType == "child") {

                User user = _db.User.Where(x => x.UserId == project.UserId).SingleOrDefault();
                DeleteProjectOrders(project.ProjectId);
                _db.SaveChanges();
            }
            else
            {
                List<Order> order = _db.Order.Where(e => e.OrderDetails.Any(a => a.ProductId == problem.ProblemId)).ToList();
                foreach (var item in order)
                {
                    var amount = DeleteOrderDetails(item.OrderId);
                    if (order.Count == 1) {
                        amount += item.ShippingCost + item.Tax - item.Discount;
                    }
                    DeleteOrderStatus(item.OrderId);
                    DeleteOrder(item.OrderId, amount);
                    var parentOrder = orderList.Where(e => e.ParentOrderId == null).FirstOrDefault();
                    if(parentOrder != null)
                        parentOrder.Total -= amount;
                }
                _db.SaveChanges();
            }
            if (deleteType == "project") {
                _db.BpsUnifiedProblem.Remove(problem);
                _db.Entry(problem).State = EntityState.Deleted;
            }
            _db.SaveChanges();
            return;
        }
        public void DeleteAddress(int projectId)
        {
            var collectionAddress = _db.Address.Where(x => x.ProjectId == projectId).ToList();
            foreach (var address in collectionAddress)
            {
                _db.Address.Remove(address);
                _db.Entry(address).State = EntityState.Deleted;
            }
            _db.SaveChanges();
        }
        public void DeleteAddressById(int addressId)
        {
            var collectionAddress = _db.Address.Where(x => x.AddressId == addressId).ToList();
            foreach (var address in collectionAddress)
            {
                _db.Address.Remove(address);
                _db.Entry(address).State = EntityState.Deleted;
            }
            _db.SaveChanges();
        }
        public void DeleteProjectOrders(int projectId)
        {
            User currentUser = _accountService.GetCurrentUser();
            var collectionOrder = _db.Order.Where(x => x.ProjectId == projectId).ToList();
            var dealerList = _db.Dealer.ToList();
            if (collectionOrder.Count > 0) { 
                var dealerId = collectionOrder[0].DealerId;
                var dealer = dealerList.Where(u => u.DealerId == dealerId).FirstOrDefault(); 

                foreach (var order in collectionOrder)
                {
                    DeleteOrderStatus(order.OrderId);
                    var amount = DeleteOrderDetails(order.OrderId);
                    _db.Order.Remove(order);
                    _ds.UpdateDealerOrderFinancial(dealer.DealerId, -(double)amount);
                    if (order.ParentOrderId == null)
                    {
                        _ds.UpdateDealerOrderFinancial(dealer.DealerId, -(double)(order.ShippingCost + order.Tax - order.Discount));
                    }
                    _db.Entry(order).State = EntityState.Deleted;
                }
                _db.Entry(dealer).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }
        public void DeleteOrder(int orderId, double amount)
        {
            User currentUser = _accountService.GetCurrentUser();
            var order = _db.Order.Where(x => x.OrderId == orderId).FirstOrDefault();
            var dealerList = _db.Dealer.ToList();
            var dealerId = order.DealerId;
            var dealer = dealerList.Where(u => u.DealerId == dealerId).FirstOrDefault();

            if (order != null)
            {
                _db.Order.Remove(order);
                _db.Entry(order).State = EntityState.Deleted;
                _ds.UpdateDealerOrderFinancial(dealer.DealerId, -(double)amount);
            }
            _db.SaveChanges();
        }
        public double DeleteOrderDetails(int orderId)
        {
            double amount = 0;
            var orderDetails = _db.OrderDetails.Where(x => x.OrderId == orderId).ToList();
            foreach (var details in orderDetails)
            {
                amount = (double)details.SubTotal; // * (double)details.Qty * 
                _db.OrderDetails.Remove(details);
                _db.Entry(details).State = EntityState.Deleted;
            }
            _db.SaveChanges();
            return amount;
        }
        public void DeleteOrderStatus(int orderId)
        {
            var orderStatus = _db.Order_Status.Where(x => x.OrderId == orderId).ToList();
            foreach (var status in orderStatus)
            {
                _db.Order_Status.Remove(status);
                _db.Entry(status).State = EntityState.Deleted;
            }
            _db.SaveChanges();
        }

        public Guid? CopyProblemByGuid(Guid problemGuid)
        {
            BpsUnifiedProblem oldProblem = GetProblemByGuid(problemGuid);
            if (oldProblem is null)
                throw new InvalidDataException();
            BpsUnifiedModel unifiedModel = JsonConvert.DeserializeObject<BpsUnifiedModel>(oldProblem.UnifiedModel);
            unifiedModel.AnalysisResult = null;
            unifiedModel.ProblemSetting.ProblemGuid = Guid.NewGuid();
            int cpyNumber = GetProblemsByName(oldProblem.ProjectId, oldProblem.ProblemName).Count();
            List<BpsUnifiedProblem> allProblems = GetProblemsByName(oldProblem.ProjectId, "");
            String[] allProblemNames = allProblems.Select(problem => problem.ProblemName).ToArray();
            string cpyName = "";
            //if (oldProblem.ProblemName.IndexOf(" Copy_") != -1)
            //    cpyName = oldProblem.ProblemName.Substring(0, oldProblem.ProblemName.IndexOf(" Copy_"));
            //else
            cpyName = oldProblem.ProblemName;
            cpyName = cpyName + " " + Resource.Copy + "_" + cpyNumber;
            while (allProblemNames.Contains(cpyName))
            {
                cpyNumber += 1;
                cpyName = oldProblem.ProblemName;
                cpyName = cpyName + " " + Resource.Copy + "_" + cpyNumber;
            }
            unifiedModel.ProblemSetting.ConfigurationName = cpyName;
            string unifiedModelString = JsonConvert.SerializeObject(unifiedModel);
            BpsUnifiedProblem newProblem = new BpsUnifiedProblem
            {
                ProblemGuid = unifiedModel.ProblemSetting.ProblemGuid,
                ProblemName = cpyName,
                ProjectId = oldProblem.ProjectId,
                UnifiedModel = unifiedModelString
            };

            _db.BpsUnifiedProblem.Add(newProblem);
            _db.SaveChanges();
            UpdateOrderNumber(unifiedModel.ProblemSetting.ProblemGuid);

            return unifiedModel.ProblemSetting.ProblemGuid;
        }

        private void UpdateOrderNumber(Guid problemGuid)
        {
            var problem = GetProblemByGuid(problemGuid);
            if (problem is null)
                throw new InvalidDataException();
            BpsUnifiedModel newUnifiedModel = JsonConvert.DeserializeObject<BpsUnifiedModel>(problem.UnifiedModel);
            if (newUnifiedModel.SRSProblemSetting != null)
                newUnifiedModel.SRSProblemSetting.OrderNumber = problem.ProjectId + "-" + problem.ProblemId;
            problem.UnifiedModel = JsonConvert.SerializeObject(newUnifiedModel);
            _db.Entry(problem).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public string SaveProblemScreenShot(string problemGuid, string imageData)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Content\\screenshots\\{problemGuid}.png");
            var base64Data = Regex.Match(imageData, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var bytes = Convert.FromBase64String(base64Data);


            using (var img = new FileStream(filePath, FileMode.Create))
            {
                img.Write(bytes, 0, bytes.Length);
                img.Flush();
            }
            return filePath;
        }

        public async Task<bool> SaveProblemScreenShotS3(string problemGuid, string imageData)
        {
            return await _os.UploadScreenshotAsync(problemGuid, imageData);
        }
        // Rename Configuration
        public BpsSimplifiedProblemApiModel RenameProblem(BpsUnifiedModel unifiedModel)
        {
            BpsUnifiedProblem problem = _db.BpsUnifiedProblem.Where(x => x.ProblemGuid == unifiedModel.ProblemSetting.ProblemGuid).SingleOrDefault();
            //String oldProblemName = problem.ProblemName;
            //String newProblemName = unifiedModel.ProblemSetting.ConfigurationName;
            //List<BpsUnifiedProblem> allOtherProblems = _db.BpsUnifiedProblem.Where(x => x.ProblemGuid == unifiedModel.ProblemSetting.ProjectGuid && x.ProblemGuid != unifiedModel.ProblemSetting.ProblemGuid).ToList();
            //String[] allProblemNames = allOtherProblems.Select(myProblem => myProblem.ProblemName).ToArray();
            //if (allProblemNames.Contains(newProblemName))
            //{
            unifiedModel = GiveNewName(unifiedModel, problem);
            //}
            BpsUnifiedModel unifiedModelToUpdate = JsonConvert.DeserializeObject<BpsUnifiedModel>(problem.UnifiedModel);

            if (problem is null)
                throw new InvalidDataException();
            if (!String.IsNullOrEmpty(unifiedModel.ProblemSetting.ConfigurationName))
            {
                unifiedModelToUpdate.ProblemSetting.ConfigurationName = unifiedModel.ProblemSetting.ConfigurationName.Trim();
                problem.UnifiedModel = JsonConvert.SerializeObject(unifiedModelToUpdate);
                problem.ProblemName = unifiedModel.ProblemSetting.ConfigurationName.Trim();
                _db.Entry(problem).State = EntityState.Modified;
                _db.SaveChanges();
            }

            //// to update configuration name to structural report
            //string reportFileUrl, summaryFileUrl, filePath, summaryFilePath;
            //bool updateStructuralReport = false, updateAcousticReport = false, updateThermalReport = false;
            //if (!(unifiedModel.AnalysisResult is null))
            //{
            //    var pdfservice = new SBA.Services.PdfService();
            //    // window report
            //    if ((!(unifiedModel.AnalysisResult.StructuralResult is null) && !String.IsNullOrEmpty(unifiedModel.AnalysisResult.StructuralResult.reportFileUrl)))
            //    {
            //        reportFileUrl = unifiedModel.AnalysisResult.StructuralResult.reportFileUrl.Substring(1);
            //        summaryFileUrl = unifiedModel.AnalysisResult.StructuralResult.summaryFileUrl.Substring(1);
            //        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportFileUrl);
            //        summaryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, summaryFileUrl);
            //        updateStructuralReport = pdfservice.UpdateConfigurationName(filePath, unifiedModel.ProblemSetting.ConfigurationName);
            //        updateStructuralReport = pdfservice.UpdateConfigurationName(summaryFilePath, unifiedModel.ProblemSetting.ConfigurationName);
            //    }

            //    // facade report
            //    if (!(unifiedModel.AnalysisResult.FacadeStructuralResult is null) && !String.IsNullOrEmpty(unifiedModel.AnalysisResult.FacadeStructuralResult.reportFileUrl))
            //    {
            //        reportFileUrl = unifiedModel.AnalysisResult.FacadeStructuralResult.reportFileUrl.Substring(1);
            //        summaryFileUrl = unifiedModel.AnalysisResult.FacadeStructuralResult.summaryFileUrl.Substring(1);
            //        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportFileUrl);
            //        summaryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, summaryFileUrl);
            //        updateStructuralReport = pdfservice.UpdateConfigurationName(filePath, unifiedModel.ProblemSetting.ConfigurationName);
            //        updateStructuralReport = pdfservice.UpdateConfigurationName(summaryFilePath, unifiedModel.ProblemSetting.ConfigurationName);
            //    }

            //    // to update configuration name to acoustic report
            //    if (!(unifiedModel.AnalysisResult.AcousticResult is null) && !String.IsNullOrEmpty(unifiedModel.AnalysisResult.AcousticResult.reportFileUrl))
            //    {
            //        reportFileUrl = unifiedModel.AnalysisResult.AcousticResult.reportFileUrl.Substring(1);
            //        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportFileUrl);
            //        updateAcousticReport = pdfservice.UpdateConfigurationName(filePath, unifiedModel.ProblemSetting.ConfigurationName);
            //    }

            //    // to update configuration name to thermal report
            //    if (!(unifiedModel.AnalysisResult.ThermalResult is null) && !String.IsNullOrEmpty(unifiedModel.AnalysisResult.ThermalResult.reportFileUrl))
            //    {
            //        reportFileUrl = unifiedModel.AnalysisResult.ThermalResult.reportFileUrl.Substring(1);
            //        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportFileUrl);
            //        updateThermalReport = pdfservice.UpdateConfigurationName(filePath, unifiedModel.ProblemSetting.ConfigurationName);
            //    }
            //}

            UpdateProblem(unifiedModel);

            BpsSimplifiedProblemApiModel response = new BpsSimplifiedProblemApiModel
            {
                ProblemGuid = problem.ProblemGuid,
                ProblemName = problem.ProblemName,
                //updateStructuralReport = updateStructuralReport,
                //updateAcousticReport = updateAcousticReport,
                //updateThermalReport = updateThermalReport
            };

            return response;
        }

        private BpsUnifiedModel GiveNewName(BpsUnifiedModel unifiedModel, BpsUnifiedProblem problem)
        {
            //int cpy = GetProblemsByName(problem.ProjectId, unifiedModel.ProblemSetting.ConfigurationName.Trim()).Count();
            //if (cpy > 1)
            //    unifiedModel.ProblemSetting.ConfigurationName = unifiedModel.ProblemSetting.ConfigurationName.Trim() + ' ' + cpy;

            //cpy = GetProblemsByName(problem.ProjectId, unifiedModel.ProblemSetting.ConfigurationName.Trim()).Count();
            //if (cpy > 1)
            //    return GiveNewName(unifiedModel, problem);
            //else
            return unifiedModel;
        }

        // **********************************
        // report-related API calls services
        // **********************************
        public MemoryStream GetReport(string reportURL)
        {
            MemoryStream dataStream = new MemoryStream();
            if (File.Exists(reportURL))
            {
                var dataBytes = File.ReadAllBytes(reportURL);
                dataStream = new MemoryStream(dataBytes);
            }
            return dataStream;
        }

        public BpsSimplifiedProblemApiModel UpdateUserNotes(BpsUnifiedModel unifiedModel)
        {
            // to add usernotes to structural report
            //string reportFileUrl, filePath;
            //bool updateStructuralReport = false, updateAcousticReport = false, updateThermalReport = false;
            //if (unifiedModel.AnalysisResult is null) return null;
            //var pdfservice = new SBA.Services.PdfService();
            //if (!(unifiedModel.AnalysisResult.StructuralResult is null) && !String.IsNullOrEmpty(unifiedModel.AnalysisResult.StructuralResult.reportFileUrl))
            //{
            //    reportFileUrl = unifiedModel.AnalysisResult.StructuralResult.reportFileUrl.Substring(1);
            //    filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportFileUrl);
            //    updateStructuralReport = pdfservice.UpdateUserNote(filePath, unifiedModel.ProblemSetting.UserNotes);
            //}

            //if (!(unifiedModel.AnalysisResult.FacadeStructuralResult is null) && !String.IsNullOrEmpty(unifiedModel.AnalysisResult.FacadeStructuralResult.reportFileUrl))
            //{
            //    reportFileUrl = unifiedModel.AnalysisResult.FacadeStructuralResult.reportFileUrl.Substring(1);
            //    filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportFileUrl);
            //    updateStructuralReport = pdfservice.UpdateUserNote(filePath, unifiedModel.ProblemSetting.UserNotes);
            //}

            //// to add usernotes to acoustic report
            //if (!(unifiedModel.AnalysisResult.AcousticResult is null) && !String.IsNullOrEmpty(unifiedModel.AnalysisResult.AcousticResult.reportFileUrl))
            //{
            //    reportFileUrl = unifiedModel.AnalysisResult.AcousticResult.reportFileUrl.Substring(1);
            //    filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportFileUrl);
            //    updateAcousticReport = pdfservice.UpdateUserNote(filePath, unifiedModel.ProblemSetting.UserNotes);
            //}

            //// to add usernotes to thermal report
            //if (!(unifiedModel.AnalysisResult.ThermalResult is null) && !String.IsNullOrEmpty(unifiedModel.AnalysisResult.ThermalResult.reportFileUrl))
            //{
            //    reportFileUrl = unifiedModel.AnalysisResult.ThermalResult.reportFileUrl.Substring(1);
            //    filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportFileUrl);
            //    updateThermalReport = pdfservice.UpdateUserNote(filePath, unifiedModel.ProblemSetting.UserNotes);
            //}

            UpdateProblem(unifiedModel);

            BpsSimplifiedProblemApiModel response = new BpsSimplifiedProblemApiModel
            {
                ProblemGuid = unifiedModel.ProblemSetting.ProblemGuid,
                ProblemName = unifiedModel.ProblemSetting.ConfigurationName,
                //updateStructuralReport = updateStructuralReport,
                //updateAcousticReport = updateAcousticReport,
                //updateThermalReport = updateThermalReport
            };

            return response;
        }

        public BpsUnifiedModel UploadResults(string strUnifiedModel, HttpFileCollection hfc)
        {
            BpsUnifiedModel unifiedModel = JsonConvert.DeserializeObject<BpsUnifiedModel>(strUnifiedModel);

            string StructuralFullReportFileName = "", StructuralSummaryReportFileName = "", AcousticReportFileName = "", ThermalReportFileName = "";
            if (!(unifiedModel.AnalysisResult is null))
            {
                if (!(unifiedModel.AnalysisResult.StructuralResult is null))
                {
                    StructuralFullReportFileName = Path.GetFileName(unifiedModel.AnalysisResult.StructuralResult.reportFileUrl);
                    StructuralSummaryReportFileName = Path.GetFileName(unifiedModel.AnalysisResult.StructuralResult.summaryFileUrl);
                }
                if (!(unifiedModel.AnalysisResult.FacadeStructuralResult is null))
                {
                    StructuralFullReportFileName = Path.GetFileName(unifiedModel.AnalysisResult.FacadeStructuralResult.reportFileUrl);
                    StructuralSummaryReportFileName = Path.GetFileName(unifiedModel.AnalysisResult.FacadeStructuralResult.summaryFileUrl);
                }
                if (!(unifiedModel.AnalysisResult.UDCStructuralResult is null))
                {
                    StructuralFullReportFileName = Path.GetFileName(unifiedModel.AnalysisResult.UDCStructuralResult.reportFileUrl);
                    StructuralSummaryReportFileName = Path.GetFileName(unifiedModel.AnalysisResult.UDCStructuralResult.summaryFileUrl);
                }
                if (!(unifiedModel.AnalysisResult.AcousticResult is null))
                {
                    AcousticReportFileName = Path.GetFileName(unifiedModel.AnalysisResult.AcousticResult.reportFileUrl);
                }
                if (!(unifiedModel.AnalysisResult.ThermalResult is null))
                {
                    ThermalReportFileName = Path.GetFileName(unifiedModel.AnalysisResult.ThermalResult.reportFileUrl);
                }

                for (int i = 0; i <= hfc.Count - 1; i++)
                {
                    HttpPostedFile hpf = hfc[i];
                    if (hpf.ContentLength > 0)
                    {
                        if (hpf.FileName == StructuralFullReportFileName)
                        {
                            StructuralFullReportFileName = unifiedModel.ProblemSetting.ProjectName + " Structural_Report.pdf";
                            string StructuralFullReportURL = $"/Content/structural-result/{ unifiedModel.ProblemSetting.UserGuid}/{ unifiedModel.ProblemSetting.ProjectGuid}/{ unifiedModel.ProblemSetting.ProblemGuid}/{StructuralFullReportFileName}";
                            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + Path.GetDirectoryName(StructuralFullReportURL));
                            hpf.SaveAs(AppDomain.CurrentDomain.BaseDirectory + StructuralFullReportURL);
                            if (unifiedModel.ProblemSetting.ProductType == "Window")
                                unifiedModel.AnalysisResult.StructuralResult.reportFileUrl = StructuralFullReportURL;
                            else if(!(unifiedModel.AnalysisResult.FacadeStructuralResult is null))
                                unifiedModel.AnalysisResult.FacadeStructuralResult.reportFileUrl = StructuralFullReportURL;
                            else if (!(unifiedModel.AnalysisResult.UDCStructuralResult is null))
                                unifiedModel.AnalysisResult.UDCStructuralResult.reportFileUrl = StructuralFullReportURL;
                        }
                        else if (hpf.FileName == StructuralSummaryReportFileName)
                        {
                            StructuralSummaryReportFileName = unifiedModel.ProblemSetting.ProjectName + " Structural_SummaryReport.pdf";
                            string StructuralSummaryReportURL = $"/Content/structural-result/{ unifiedModel.ProblemSetting.UserGuid}/{ unifiedModel.ProblemSetting.ProjectGuid}/{ unifiedModel.ProblemSetting.ProblemGuid}/{StructuralSummaryReportFileName}";
                            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + Path.GetDirectoryName(StructuralSummaryReportURL));
                            hpf.SaveAs(AppDomain.CurrentDomain.BaseDirectory + StructuralSummaryReportURL);
                            if (unifiedModel.ProblemSetting.ProductType == "Window")
                                unifiedModel.AnalysisResult.StructuralResult.summaryFileUrl = StructuralSummaryReportURL;
                            else if(!(unifiedModel.AnalysisResult.FacadeStructuralResult is null))
                                unifiedModel.AnalysisResult.FacadeStructuralResult.summaryFileUrl = StructuralSummaryReportURL;
                            else if (!(unifiedModel.AnalysisResult.UDCStructuralResult is null))
                                unifiedModel.AnalysisResult.UDCStructuralResult.summaryFileUrl = StructuralSummaryReportURL;
                        }
                        else if (hpf.FileName == AcousticReportFileName)
                        {
                            AcousticReportFileName = unifiedModel.ProblemSetting.ProjectName + " Acoustic_Report.pdf";
                            string AcousticReportURL = $"/Content/structural-result/{ unifiedModel.ProblemSetting.UserGuid}/{ unifiedModel.ProblemSetting.ProjectGuid}/{ unifiedModel.ProblemSetting.ProblemGuid}/{AcousticReportFileName}";
                            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + Path.GetDirectoryName(AcousticReportURL));
                            hpf.SaveAs(AppDomain.CurrentDomain.BaseDirectory + AcousticReportURL);
                            unifiedModel.AnalysisResult.AcousticResult.reportFileUrl = AcousticReportURL;
                        }
                        else if (hpf.FileName == ThermalReportFileName)
                        {
                            ThermalReportFileName = unifiedModel.ProblemSetting.ProjectName + " Thermal_Report.pdf";
                            string ThermalReportURL = $"/Content/structural-result/{ unifiedModel.ProblemSetting.UserGuid}/{ unifiedModel.ProblemSetting.ProjectGuid}/{ unifiedModel.ProblemSetting.ProblemGuid}/{ThermalReportFileName}";
                            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + Path.GetDirectoryName(ThermalReportURL));
                            hpf.SaveAs(AppDomain.CurrentDomain.BaseDirectory + ThermalReportURL);
                            unifiedModel.AnalysisResult.ThermalResult.reportFileUrl = ThermalReportURL;
                        }
                    }
                }
            };

            UpdateProblem(unifiedModel);

            return unifiedModel;
        }

        // **********************************
        // SRS-specific API calls services
        // **********************************
        public BpsUnifiedProblem CreateDefaultWindowProblemForSRSProject(Guid projectGuid, string problemName = null)
        {
            BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (dbProject == null)
            {
                throw new InvalidDataException();
            }
            SetCurrentCulture(dbProject);
            // get new default problemName
            int problemCount = GetProblemIdsForProject(projectGuid).Count() + 1;
            List<BpsUnifiedProblem> allProblems = GetProblemsByName(dbProject.ProjectId, "");
            String[] allProblemNames = allProblems.Select(problem => problem.ProblemName).ToArray();
            string cteName = "";
            cteName = "Product " + problemCount;
            while (allProblemNames.Contains(cteName))
            {
                problemCount += 1;
                cteName = "Product " + problemCount;
            }
            Guid problemGuid = Guid.NewGuid();
            BpsUnifiedProblem bpsUnifiedProblem = BuildDefaultWindowProblemForSRS(dbProject, projectGuid, problemGuid, cteName, false);
            _db.BpsUnifiedProblem.Add(bpsUnifiedProblem);
            _db.SaveChanges();
            return bpsUnifiedProblem;
        }

        private BpsUnifiedProblem BuildDefaultWindowProblemForSRS(BpsProject dbProject, Guid projectGuid, Guid problemGuid, string problemName = null, bool addProductType = false, BpsUnifiedProblem oldProblem = null)
        {
            if (dbProject == null)
            {
                throw new InvalidDataException();
            }

            User user = _accountService.GetUser(dbProject.UserId);
            string userLanguage = String.Empty;
            var cultureInfo = new CultureInfo("en-US");
            if (user != null)
            {
                userLanguage = user.Language;
                cultureInfo = !String.IsNullOrEmpty(userLanguage) ? new CultureInfo(userLanguage) : cultureInfo;
            }
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            var ProductType = "";
            if (addProductType)
            {
                ProductType = "Window";
            }
            BpsUnifiedModel copyProblem = new BpsUnifiedModel();
            if (oldProblem != null) copyProblem = JsonConvert.DeserializeObject<BpsUnifiedModel>(oldProblem.UnifiedModel);
            BpsUnifiedProblem bpsUnifiedProblem = new BpsUnifiedProblem
            {
                ProblemGuid = problemGuid,
                ProblemName = problemName,
                ProjectId = dbProject.ProjectId,
                UnifiedModel = null
            };
            Hardware hardWare = new Hardware
            {
                HardwareID = 1,
                HardwareAlloy = "7075-T6",
                HardwareFy = 180.0,
                HardwareFu = 29.0,
                HardwareFinishes = "Silver"
            };
            ModelInput modelInput = new ModelInput
            {
                FrameSystem = new FrameSystem
                {
                    SystemType = "AWS 75.SI+",
                    UvalueType = "AIF",
                    InsulationType = "Polyamide Anodized After",
                    InsulationMaterial = "8",
                    Alloys = "6060-T66 (150MPa)",
                    AluminumFinish = "Anodized",
                    AluminumColor = "Silver grey - RAL 7001"
                },
                Geometry = new Geometry
                {
                    Points = new List<Point>(),
                    Members = new List<Member>(),
                    Sections = new List<Section>(),
                    Infills = new List<Infill>(),
                    GlazingSystems = new List<GlazingSystem>()
                },
                Acoustic = null,
                Structural = new Structural
                {
                    DispIndexType = 6,
                    DispHorizontalIndex = 0,
                    DispVerticalIndex = 0,
                    WindLoadInputType = 1,
                    dinWindLoadInput = null,
                    WindLoad = 1.68,
                    HorizontalLiveLoad = 0.5,
                    HorizontalLiveLoadHeight = 900,
                    ShowBoundaryCondition = false,
                    Cpp = 1,
                    Cpn = -1,
                },
                Thermal = null,
                SRSExtendedData = new SRSExtendedData
                {
                    Hardwares = new List<Hardware>(),
                    MachiningInfo = new Machining
                    {
                        GlueHoleOffsetsfromLeftTopCorner = 22.0,
                        NailHoleOffsetsfromLeftTopCorner = 44.0
                    }
                }
            };
            modelInput.SRSExtendedData.Hardwares.Add(hardWare);
            BpsUnifiedModel bpsUnifiedModel = new BpsUnifiedModel
            {
                UnifiedModelVersion = "V2",
                UserSetting = new UserSetting
                {
                    Language = userLanguage,
                    UserName = user.UserName,
                },
                ProblemSetting = new ProblemSetting
                {
                    EnableAcoustic = oldProblem != null ? copyProblem.ProblemSetting.EnableAcoustic : false,
                    EnableStructural = oldProblem != null ? copyProblem.ProblemSetting.EnableStructural : false,
                    EnableThermal = oldProblem != null ? copyProblem.ProblemSetting.EnableThermal : false,
                    UserGuid = (Guid)user.UserGuid,
                    ProjectGuid = projectGuid,
                    ProblemGuid = problemGuid,
                    ProductType = ProductType,
                    ProjectName = dbProject.ProjectName,
                    Location = dbProject.ProjectLocation,
                    ConfigurationName = problemName,
                    UserNotes = ""
                },
                SRSProblemSetting = new SRSProblemSetting {
                    SubTotal = 863.0
                },
                ModelInput = modelInput,
                CollapsedPanels = new CollapsedPanelStatus
                {
                    Panel_Configure = true,
                    Panel_Operability = false,
                    Panel_Framing = false,
                    Panel_Glass = false,
                    Panel_Acoustic = false,
                    Panel_Structural = false,
                    Panel_Thermal = false
                },
            };
            bpsUnifiedModel.ModelInput.Geometry.Points.Add(new Point { PointID = 1, X = 0, Y = 0 });
            bpsUnifiedModel.ModelInput.Geometry.Points.Add(new Point { PointID = 2, X = 0, Y = 1480 });
            bpsUnifiedModel.ModelInput.Geometry.Points.Add(new Point { PointID = 3, X = 1230, Y = 0 });
            bpsUnifiedModel.ModelInput.Geometry.Points.Add(new Point { PointID = 4, X = 1230, Y = 1480 });
            bpsUnifiedModel.ModelInput.Geometry.Members.Add(new Member { MemberID = 1, PointA = 1, PointB = 2, SectionID = 1, MemberType = 1 });
            bpsUnifiedModel.ModelInput.Geometry.Members.Add(new Member { MemberID = 2, PointA = 3, PointB = 4, SectionID = 1, MemberType = 1 });
            bpsUnifiedModel.ModelInput.Geometry.Members.Add(new Member { MemberID = 3, PointA = 1, PointB = 3, SectionID = 1, MemberType = 1 });
            bpsUnifiedModel.ModelInput.Geometry.Members.Add(new Member { MemberID = 4, PointA = 2, PointB = 4, SectionID = 1, MemberType = 1 });
            bpsUnifiedModel.ModelInput.Geometry.Sections.Add(new Section
            {
                SectionID = 1,
                SectionType = 1,
                ArticleName = "486890",
                InsideW = 34,
                OutsideW = 59,
            });
            bpsUnifiedModel.ModelInput.Geometry.Sections.Add(new Section
            {
                SectionID = 2,
                SectionType = 2,
                ArticleName = "382280",
                InsideW = 34,
                OutsideW = 84,
            });
            bpsUnifiedModel.ModelInput.Geometry.Sections.Add(new Section
            {
                SectionID = 3,
                SectionType = 3,
                ArticleName = "382280",
                InsideW = 34,
                OutsideW = 84,
            });
            bpsUnifiedModel.ModelInput.Geometry.Infills.Add(new Infill
            {
                InfillID = 1,
                BoundingMembers = new List<int> { 1, 4, 2, 3 },
                GlazingSystemID = 1,
                PanelSystemID = -1,
                OperabilitySystemID = -1,
                GlazingBeadProfileArticleName= "184090",
                GlazingGasketArticleName= "284835",
                GlazingRebateGasketArticleName= "284321",
                GlazingRebateInsulationArticleName= "288429",
            });
            bpsUnifiedModel.ModelInput.Geometry.GlazingSystems.Add(new GlazingSystem
            {
                Manufacturer = "Vitro Architectural Glass",
                GlazingSystemID = 1,
                Rw= 35,
                RwC= -1,
                RwCtr= -4,
                STC= 35,
                OITC= 30,
                SHGC= 0.39,
                VT= 0.7023,
                UValue= 1.358,
                SpacerType= 1,
                Description= "1/4 Clear SB 60+1/2 ARGON+1/4 Clear (1 in)",
                Plates = new List<Plate>(),
                Cavities = new List<Cavity>()
            });
            bpsUnifiedModel.ModelInput.Geometry.GlazingSystems[0].Plates.Add(new Plate { Material = "glass", H = 6, InterH = 0 });
            bpsUnifiedModel.ModelInput.Geometry.GlazingSystems[0].Plates.Add(new Plate { Material = "glass", H = 6, InterH = 0 });
            bpsUnifiedModel.ModelInput.Geometry.GlazingSystems[0].Cavities.Add(new Cavity { CavityType = "Argon", Lz = 12 });
            bpsUnifiedProblem.UnifiedModel = JsonConvert.SerializeObject(bpsUnifiedModel);
            return bpsUnifiedProblem;
        }

        public BpsUnifiedProblem GetDefaultWindowProblemForSRS(Guid projectGuid, Guid problemGuid)
        {
            BpsProject dbProject = _db.BpsProject.Where(x => x.ProjectGuid == projectGuid).SingleOrDefault();
            if (dbProject == null)
            {
                throw new InvalidDataException();
            }
            BpsUnifiedProblem dbProblem = _db.BpsUnifiedProblem.Where(x => x.ProblemGuid == problemGuid).SingleOrDefault();
            string problemName = dbProblem.ProblemName;
            return BuildDefaultWindowProblemForSRS(dbProject, projectGuid, problemGuid, problemName, true);
        }

        public List<JsonResponse> MigrateUnifiedModelToV2() {
            var dbProblemsList = _db.BpsUnifiedProblem.OrderBy(o => o.ProblemId).ToList();
            List<JsonResponse> updateStatusList = new List<JsonResponse>();
            foreach (var problem in dbProblemsList)
            {
                JsonResponse urs = new JsonResponse();
                //if (problem.ProjectId == 1163)
                {
                    try
                    {
                        VCLMigrationService vclms = new VCLMigrationService();
                        problem.UnifiedModel = vclms.getV2UnifiedModel(problem.UnifiedModel);
                        _db.Entry(problem).State = EntityState.Modified;

                        urs.Data = new
                        {
                            ProblemId = problem.ProblemId,
                            JsonString = problem.UnifiedModel
                        };
                        urs.Message = "Updated";
                        urs.Success = true;
                    }
                    catch (Exception ex)
                    {
                        urs.Data = new
                        {
                            ProblemId = problem.ProblemId,
                            JsonString = problem.UnifiedModel
                        };
                        urs.Message = ex.Message;
                        urs.Success = false;
                    }
                    updateStatusList.Add(urs);
                }
            }
            _db.SaveChanges();
            return updateStatusList;
        }
        public List<StateTax> GetStateTaxList()
        {
            return _db.StateTax.ToList();
        }
        public StateTax GetStateTax(string zipcode)
        {
            return _db.StateTax.Where(e => e.ZipCode == zipcode).FirstOrDefault();
        }
    }

}