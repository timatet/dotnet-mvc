using System.Collections.Generic;

namespace dotnet_mvc.Models.HelpModels
{
  public class ProductListModel
    {
        public IEnumerable<dotnet_mvc.Models.DataModels.ProductModel> productList { get; set; }
    }
}