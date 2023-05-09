using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_mvc.Models.DataModels;

namespace dotnet_mvc.Models.HelpModels
{
    public class BasketProductListModel
    {
        public IDictionary<ProductModel, int> productList { get; set; }
    }
}