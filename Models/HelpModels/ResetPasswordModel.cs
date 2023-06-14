using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace dotnet_mvc.Models.HelpModels
{
    public class ResetPasswordModel
    {
        public string oldPassword { set; get; }
        public string newPassword { set; get; }
        public string repeatNewPassword { set; get; } 
    }

}