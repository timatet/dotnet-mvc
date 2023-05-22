using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "Категория")]
        public CategoryEnum Category { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Цена")]
        public double Cost { get; set; }

        [Display(Name = "Остаток на складе")]
        public int CountInStack { get; set; }

        [Display(Name = "Бренд")]
        public BrandModel Brand { get; set; }
        public int BrandId { get; set; }

        [Display(Name = "Характеристики")]
        public ProductCharacteristic ProductCharacteristic { get; set; }

        [Display(Name = "Изображение")]
        public string ImageUrl { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}