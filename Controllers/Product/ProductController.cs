using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using dotnet_mvc.Models.Auxiliary;
using dotnet_mvc.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using dotnet_mvc.Models.HelpModels;
using System.Text.Json;
using Newtonsoft.Json;

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
            if (id == null) {
                return NotFound();
            }
              
            var product = db.Products.Include(p => p.ProductCharacteristic).Include(p => p.Brand).FirstOrDefault(p => p.Id == id);
            if (product == null) {
                return NotFound();
            }
               
            return View(product);
        }

        public IActionResult New() {
            ViewData["BrandList"] = new SelectList(db.Brands, "Id", "Name", "Description");
            var attrs = ProductCharacteristic.GetAttributesNames();
            ViewData["CharacteristicList"] = new SelectList(ProductCharacteristic.GetAttributesNames(), "ShortName", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult New(
            ProductModel product,
            IFormFile upload,
            List<string> productCharacteristicList
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
                string extFile = Path.GetExtension(fileName);
                if(extFile.Contains(".png")|| extFile.Contains(".jpg") ||
                    extFile.Contains(".bmp"))
                {
                    var image = Image.Load(upload.OpenReadStream());
                    image.Mutate(x => x.Resize(ImageWidth, ImageHeight));
                    string imgGuid = Guid.NewGuid().ToString();
                    string today = DateTime.Today.ToString("yyyy-MM-dd");
                    fileName = today + "-" + imgGuid + extFile;
                    string path = environment.ContentRootPath + "/wwwroot/images/" + fileName;
                    image.Save(path);
                    product.ImageUrl = fileName;
                } 
            }

            // Сохранение выбранных характеристик
            ProductCharacteristic productCharacteristic = new ProductCharacteristic();
            productCharacteristic.SetFields(productCharacteristicList);
            db.ProductCharacteristics.Add(productCharacteristic);
            db.SaveChanges();
            product.ProductCharacteristic = productCharacteristic;
            
            // Записываем товар в базу данных
            db.Products.Add(product);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public bool Delete()
        {
            Dictionary<string, object> response = new Dictionary<string, object>();

            // Чтение данных передаваемых в POST запросе
            MemoryStream stream = new MemoryStream();
            Request.Body.CopyTo(stream);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                string requestBody = reader.ReadToEnd();
                if (requestBody.Length > 0)
                {
                    response = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);
                }
            }

            try {
                int productId = int.Parse(response["id"].ToString());
                ProductModel product = db.Products.Find(productId);
                db.Products.Remove(product);
                db.SaveChanges();
            } catch {
                return false;
            }
            
            return true;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}