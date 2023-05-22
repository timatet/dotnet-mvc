using System.Collections.Generic;
using dotnet_mvc.Models.Auxiliary;
using dotnet_mvc.Models.DataModels;

namespace dotnet_mvc.Models.HelpModels
{
  public class ProductListModel
  {
      public readonly ProductModel productModel = new ProductModel();
      public IEnumerable<ProductModel> productList { get; set; }
  }
}