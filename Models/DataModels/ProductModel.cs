using System.ComponentModel.DataAnnotations.Schema;
using dotnet_mvc.Models.Auxiliary;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc.Models.DataModels
{
  [Table("Products")]
    public class ProductModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public CategoryEnum Category { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Cost { get; set; }

        public int CountInStack { get; set; }

        public BrandModel Brand { get; set; }

        public ProductCharacteristic ProductCharacteristic { get; set; }

        public string ImageUrl { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}