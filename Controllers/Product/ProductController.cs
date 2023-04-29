using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using dotnet_mvc.Models.Auxiliary;
using dotnet_mvc.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace dotnet_mvc.Controllers.Product
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext db;
        IHostEnvironment environment;
        const int ImageWidth = 300;
        const int ImageHeight = 300;

        public ProductController(ILogger<ProductController> logger, ApplicationDbContext applicationDbContext, IHostEnvironment env)
        {
            _logger = logger;
            db = applicationDbContext;
            environment = env;
        }

        public IActionResult Info(int? id) {
            return View(id);
        }

        public IActionResult New() {
            ViewData["BrandList"] = new SelectList(db.Brands, "Id", "Name", "Description");
            return View();
        }

        [HttpPost]
        public IActionResult New(
            ProductModel product,
            IFormFile upload
        ) {
            // Ищем или сохраняем бренд
            BrandModel productBrand = product.Brand;
            if (productBrand is null) {
                productBrand = db.Brands.Find(product.BrandId);
                product.Brand = productBrand;
            } else {
                db.Brands.Add(productBrand);
                db.SaveChanges();
                int brandId = productBrand.Id;
                product.BrandId = brandId;
            }

            // Сохраняем изображение товара
            if(upload!=null)
            {
                string fileName = Path.GetFileName(upload.FileName);
                var extFile = fileName.Substring(fileName.Length - 3);
                if(extFile.Contains("png")|| extFile.Contains("jpg") ||
                    extFile.Contains("bmp"))
                {
                    var image = Image.Load(upload.OpenReadStream());
                    image.Mutate(x => x.Resize(ImageWidth, ImageHeight));
                    // string path = "\\wwwroot\\images\\" + fileName;
                    string path = environment.ContentRootPath + "/wwwroot/images/" + fileName;
                    image.Save(path);
                    product.ImageUrl = fileName;
                }
            }
            
            // Записываем товар в базу данных
            db.Products.Add(product);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}