using System.ComponentModel.DataAnnotations;

namespace dotnet_mvc.Models.HelpModels
{
    public class LoginModel
    {
        public string EmailOrUserName { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

    }
}