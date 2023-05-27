using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_mvc.Models.Auxiliary;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc.Models.DataModels
{
    [Table("Orders")]
    public class OrderModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; } //0

        [Display(Name = "Общая стоимость")]
        public double TotalPrice { get; set; } //0

        public DateTime CreationDate { get; set; } 
        public DateTime ConfirmationDate { get; set; } 
        public DateTime IssueDate { get; set; } 

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }
        [Display(Name = "Номер")]
        public string Phone { get; set; }

        [Display(Name = "Способ доставки")]
        public DeliveryMethodEnum DeliveryMethod { get; set; }

        public OrderStatus Status { get; set; } 
        public UserModel User { get; set; }
    }
}