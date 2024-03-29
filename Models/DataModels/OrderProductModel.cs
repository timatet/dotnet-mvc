using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc.Models.DataModels
{
    public class OrderProductModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public OrderModel Order { get; set; }
        public int OrderId { get; set; }
        public ProductModel Product { get; set; }
        public int ProductId { get; set; }

        [Display(Name = "Количество")]
        public int Quantity { get; set; }
    }
}