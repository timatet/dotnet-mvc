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
            BasketModel basketModel, 
            int countCopies
        ){
            this.Product = productModel;
            this.Basket = basketModel;
            this.CountCopies = countCopies;
        }

        public BasketProductLinkModel(
            ProductModel productModel, 
            BasketModel basketModel
        ){
            this.Product = productModel;
            this.Basket = basketModel;
        }        
    }
}