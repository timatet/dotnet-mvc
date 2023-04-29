using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace dotnet_mvc.Models.DataModels
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string NickName { get; set; }

        public string FullName { get; set; }
        public DateTime BirthdayDate { get; set; }
        public bool IsAdmin { get; set; }
    }
}