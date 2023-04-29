using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using dotnet_mvc.Models.Auxiliary;
using dotnet_mvc.Models.DataModels;
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
        
        [NotMapped]
        public int? BrandId { get; set; }

        public BrandModel Brand { get; set; }

        public double Weight { get; set; }

        public string Material { get; set; }

        public int Size { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ImageUrl { get; set; }

    }
}