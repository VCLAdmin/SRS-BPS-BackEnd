//using System.Net;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Web.Http;

//namespace VCLWebAPI.Results
//{
//    public class ChallengeResult : IHttpActionResult
//    {
//        public ChallengeResult(string loginProvider, Microsoft.AspNetCore.Mvc.ControllerBase controller)
//        {
//            LoginProvider = loginProvider;
//            Request = controller.Request;
//        }

//        public string LoginProvider { get; set; }
//        public HttpRequestMessage Request { get; set; }

//        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
//        {
//            Request.GetOwinContext().Authentication.Challenge(LoginProvider);

//            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
//            response.RequestMessage = Request;
//            return Task.FromResult(response);
//        }
//    }
//}