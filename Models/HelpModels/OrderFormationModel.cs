using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_mvc.Models.DataModels;

namespace dotnet_mvc.Models.HelpModels
{
    public class OrderFormationModel
    {
        public OrderModel orderModel { get; set; }
        public string orderProductModels { get; set; }
    }
}