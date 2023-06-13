using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace dotnet_mvc.Models.DataModels
{
  public class UserModel : IdentityUser<Guid>
  {
      public string FullName { get; set; }
      public DateTime BirthdayDate { get; set; }
  }
}