using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_mvc.Models.DataModels
{
    [Table("BasketProductLinks")]
    public class BasketProductLinkModel
    {
        public ProductModel Product { get; set; }
        public int ProductId { get; set; }
        public BasketModel Basket { get; set; }
        public int BasketId { get; set; }
        public int CountCopies { get; set; } = 1;

        public BasketProductLinkModel() { } 

        public BasketProductLinkModel(
            ProductModel productModel, 
            int countCopies
        ){
            this.Product = productModel;
            this.ProductId = productModel.Id;
            this.CountCopies = countCopies;
        }       
    }
}