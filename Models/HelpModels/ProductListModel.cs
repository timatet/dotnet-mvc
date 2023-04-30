using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_mvc.Models.HelpModels
{
    public class ProductListModel
    {
        public IEnumerable<dotnet_mvc.Models.DataModels.ProductModel> productList { get; set; }
    }
}