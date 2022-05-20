namespace VCLWebAPI.Models.Account
{
    public class AccountApiModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Language { get; set; }
        public UserApiModel User { get; set; }
    }
}