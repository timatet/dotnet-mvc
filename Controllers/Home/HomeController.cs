﻿using System.ComponentModel.DataAnnotations;
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
using Microsoft.AspNetCore.Identity;

namespace dotnet_mvc.Controllers
{
  public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        public IConfiguration _configuration { get; }
        
        private readonly ApplicationDbContext _applicationDbContext;

        public HomeController(
            ILogger<HomeController> logger, 
            IConfiguration configuration, 
            ApplicationDbContext applicationDbContext, 
            IWebHostEnvironment webHostEnvironment,
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager
        )
        {
            _logger = logger;
            _configuration = configuration;
            _applicationDbContext = applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
            _signInManager = signInManager;
            _userManager = userManager;
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
                _applicationDbContext.Products.Include(p => p.ProductCharacteristic).Include(p => p.Brand),
                filterBrandSelectId,
                filterCharacteristicSelect,
                filterCharacteristicName,
                filterCategoriesSelect
            );

            string webRootPath = _webHostEnvironment.WebRootPath;
            ViewData["WebRootPath"] = webRootPath;

            ViewData["BrandList"] = new SelectList(_applicationDbContext.Brands, "Id", "Name", _applicationDbContext.Brands.FirstOrDefault(b => b.Id == filterBrandSelectId)?.Id);

            var CharacteristicListAttributes = ProductCharacteristic.GetAttributesNames();
            ViewData["CharacteristicList"] = new SelectList(CharacteristicListAttributes, "ShortName", "Name", 
                CharacteristicListAttributes.FirstOrDefault(c => c.ShortName == filterCharacteristicSelect)?.ShortName);
            ViewData["CharacteristicDisabled"] = filterCharacteristicSelect == "NoSelect" ? true : false;
            ViewData["CharacteristicName"] = filterCharacteristicName;
            var categoryEnumList = Enum
                .GetValues(typeof(CategoryEnum))
                .Cast<CategoryEnum>()
                .ToDictionary(
                    p => p.ToString(),
                    p => p.GetType()
                        .GetMember(p.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName()
                );
            ViewData["CategoriesList"] = new SelectList(categoryEnumList, "Key", "Value", 
                categoryEnumList.FirstOrDefault(c => c.Key == filterCategoriesSelect).Key);

            bool filterHidden = true;
            if (filterBrandSelectId != -1 || filterCharacteristicSelect != "NoSelect" 
                || filterCharacteristicName != "" || filterCategoriesSelect != "NoSelect") {
                filterHidden = false;
            }
            ViewData["filterHidden"] = filterHidden;

            /*----------------------------------------------------------------------------*/

            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            int TotalCount = 0;
            
            // BASKET: Set counter on button
            if (!userIsSignedIn) { // from cookies
                var basket = BasketHelper.GetBasketFromCookie(Request, Response);
            
                foreach (var productKVP in basket) {
                    TotalCount += productKVP.Value;
                }

                ViewData["TotalProduct"] = TotalCount;
            } else { //from bd
                UserModel user = _userManager.GetUserAsync(User).Result;

                BasketModel basketModel = _applicationDbContext.Baskets.FirstOrDefault(b => b.UserId == user.Id);
                if (basketModel != null) {  
                    TotalCount =  _applicationDbContext.BasketProductLinks.Sum(b => b.CountCopies);    
                    ViewData["TotalProduct"] = TotalCount;   
                }
            }

            if (TotalCount == 0) {
                ViewData["TotalProductHidden"] = true;
            } else {
                ViewData["TotalProductHidden"] = false;
            }

            /*----------------------------------------------------------------------------*/

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
