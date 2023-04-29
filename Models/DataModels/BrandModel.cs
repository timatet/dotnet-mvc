using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc.Models.DataModels
{
    [Table("Brands")]
    public class BrandModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

    }
}