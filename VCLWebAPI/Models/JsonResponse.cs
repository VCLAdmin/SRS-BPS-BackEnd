namespace VCLWebAPI.Models
{
    public class JsonResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public JsonResponse()
        {
            Success = false;
            Message = string.Empty;
            Data = new { };
        }
    }
}