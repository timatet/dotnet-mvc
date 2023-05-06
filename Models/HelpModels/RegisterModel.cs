using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_mvc.Models.HelpModels
{
    public class RegisterModel
    {
        public string Email { get; set; }
        
        public string Login { get; set; }

        public string Password { get; set; }

        public string PasswordCommit { get; set; }

        public string FullName { get; set; }
        public DateTime BirthdayDate { get; set; }

        public bool PrivatePolicyCheck { get; set; }
    }
}