using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotnet_mvc.Models;
using Microsoft.Extensions.Configuration;
using dotnet_mvc.Models.DataModels;
using dotnet_mvc.Models.HelpModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using dotnet_mvc.Models.Auxiliary;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using dotnet_mvc.Helpers;

namespace dotnet_mvc.Controllers
{
  public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public IConfiguration _configuration { get; }
        
        private readonly ApplicationDbContext db;

        public HomeController(
            ILogger<HomeController> logger, 
            IConfiguration configuration, 
            ApplicationDbContext applicationDbContext, 
            IWebHostEnvironment webHostEnvironment
        )
        {
            _logger = logger;
            _configuration = configuration;
            db = applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
        } 

        [HttpGet]
        public IActionResult Index(
            int filterBrandSelectId = -1,
            string filterCharacteristicSelect = "NoSelect",
            string filterCharacteristicName = "",
            string filterCategoriesSelect = "NoSelect"
        )
        {
            ProductListModel productListModel = new ProductListModel();
            productListModel.productList = doFilter(
                db.Products.Include(p => p.ProductCharacteristic).Include(p => p.Brand),
                filterBrandSelectId,
                filterCharacteristicSelect,
                filterCharacteristicName,
                filterCategoriesSelect
            );

            string webRootPath = _webHostEnvironment.WebRootPath;
            ViewData["WebRootPath"] = webRootPath;

            // TODO: Проверка предлагаемых свойств для фильтрации.
            //  Если свойство есть, то выделить selected его параметр в списках
            //  Индикатор на кнопке что хотя бы один фильтр применен  
            ViewData["BrandList"] = new SelectList(db.Brands, "Id", "Name", "Description");
            ViewData["CharacteristicList"] = new SelectList(ProductCharacteristic.GetAttributesNames(), "ShortName", "Name");
            var categoryEnumList = Enum
                .GetValues(typeof(CategoryEnum))
                .Cast<CategoryEnum>()
                .Select(
                    p => new KeyValuePair<string, string>(
                        p.ToString(),
                        p.GetType()
                        .GetMember(p.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName()
                    )
                );
            ViewData["CategoriesList"] = new SelectList(categoryEnumList, "Key", "Value");

            return View(productListModel);
        }
        
        public IEnumerable<ProductModel> doFilter (
            IEnumerable<ProductModel> productList,
            int filterBrandSelectId,
            string filterCharacteristicSelect,
            string filterCharacteristicName,
            string filterCategoriesSelect
        ) {
            List<Predicate<ProductModel>> predicateList = new List<Predicate<ProductModel>>();

            if (filterBrandSelectId != -1) {
                predicateList.Add(x => x.Brand.Id == filterBrandSelectId);
            }

            if (filterCharacteristicSelect != "NoSelect" && !string.IsNullOrEmpty(filterCharacteristicName)) {
                predicateList.Add(
                    x => x.ProductCharacteristic
                        .GetType()
                        .GetProperty(filterCharacteristicSelect)
                        .GetValue(x.ProductCharacteristic)
                        ?.ToString() == filterCharacteristicName
                );
            }

            if (filterCategoriesSelect != "NoSelect") {
                predicateList.Add(x => x.Category.ToString() == filterCategoriesSelect);
            }

            var masterPredicate = PredicateMaster.And(predicateList.ToArray());
            return productList.Where(x => masterPredicate(x));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
