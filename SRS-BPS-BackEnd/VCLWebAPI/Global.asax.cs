using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using VCLWebAPI.Services;

namespace VCLWebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SetConfiguration();
        }

        public void SetConfiguration()
        {
            //Can be removed later once the UI is ready with adding of new user.
            AccountService accountService = new AccountService();
            accountService.AddAspNetUsers();
            //accountService.AddNewUsers();
            //accountService.AddSRSData();
            //VCLDesignDB.Util.Globals.DBConnectionString = VCLDesignDB.Util.Constants.Local_DbConnectionString;
        }
    }
}