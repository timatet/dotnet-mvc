using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc.Models.DataModels
{
    [Table("Baskets")]
    public class BasketModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public UserModel User { get; set; }
        public Guid UserId { get; set; }
    }
}