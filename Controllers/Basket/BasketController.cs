using System.Data.Common;
using System.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using dotnet_mvc.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Web;
using Newtonsoft.Json;
using dotnet_mvc.Models.Auxiliary;
using dotnet_mvc.Models.HelpModels;
using dotnet_mvc.Helpers;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace dotnet_mvc.Controllers.Basket
{
    public class BasketController : Controller
    {
        private readonly ILogger<BasketController> _logger;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public BasketController(
            ILogger<BasketController> logger,
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            ApplicationDbContext applicationDbContext
        ){
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost]
        public bool RemoveItemFromBasket() {

            Dictionary<string, object> request = new Dictionary<string, object>();

            MemoryStream stream = new MemoryStream();
            Request.Body.CopyTo(stream);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                string requestBody = reader.ReadToEnd();
                if (requestBody.Length > 0)
                {
                    request = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);
                }
            }

            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            
            if (!userIsSignedIn) {
                try {
                    int product_id = int.Parse(request["product_id"].ToString());

                    BasketHelper.RemoveFromCookieBasket(product_id, Request, Response);

                    return true;
                } catch {
                    return false;
                }
            }

            return false;
        }

        [HttpGet]
        public int GetBasketProductCount() {
            var basket = BasketHelper.GetBasketFromCookie(Request, Response);
            int TotalCount = 0;

            foreach (var productKVP in basket) {
                ProductModel product = _applicationDbContext.Products.ToList().Find(p => p.Id == productKVP.Key);
                TotalCount += productKVP.Value;
            }

            return TotalCount;
        }

        [HttpGet]
        public double GetBasketTotalCost() {

            var basket = BasketHelper.GetBasketFromCookie(Request, Response);
            double TotalCost = 0;

            foreach (var productKVP in basket) {
                ProductModel product = _applicationDbContext.Products.ToList().Find(p => p.Id == productKVP.Key);
                TotalCost += product.Cost * productKVP.Value;
            }

            return TotalCost;
        }

        [HttpPost]
        public bool ChangeProductCount() {

            Dictionary<string, object> request = new Dictionary<string, object>();
            Dictionary<string, object> response = new Dictionary<string, object>();

            MemoryStream stream = new MemoryStream();
            Request.Body.CopyTo(stream);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                string requestBody = reader.ReadToEnd();
                if (requestBody.Length > 0)
                {
                    request = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);
                }
            }

            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            
            if (!userIsSignedIn) {
                try {
                    int product_id = int.Parse(request["product_id"].ToString());
                    int product_count = int.Parse(request["product_count"].ToString());

                    BasketHelper.UpdateCount(product_id, product_count, Response, Request);

                    return true;
                } catch {
                    return false;
                }
            }

            return false;
        }

        [HttpPost]
        public bool AddProductToBasket() {

            Dictionary<string, object> response = new Dictionary<string, object>();

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

            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            
            if (!userIsSignedIn) {
                try {
                    int product_id = int.Parse(response["product_id"].ToString());
                    ProductModel product = _applicationDbContext.Products.ToList().Find(p => p.Id == product_id);
                    BasketHelper.AddToCookieBasket(product, Request, Response);
                    return true;
                } catch {
                    return false;
                }
            }

            return false;
        }

        [HttpGet]
        public IActionResult Index()
        {
            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            BasketProductListModel basketProductListModel = new BasketProductListModel();

            if (!userIsSignedIn) {

                List<ProductModel> products = _applicationDbContext.Products.Include(p => p.ProductCharacteristic).ToList();
                Dictionary<ProductModel, int> basketFromCookie = 
                    BasketHelper.GetBasketFromCookie(Request, Response)
                    .ToDictionary(kvp => products.Find(x => x.Id == kvp.Key), kvp => kvp.Value);
               
                Dictionary<ProductModel, int> actualBasket = new Dictionary<ProductModel, int>();
                foreach (var product_kvp in basketFromCookie) {
                    ProductModel productModel = _applicationDbContext.Products.ToList().Find(p => p.Id == product_kvp.Key.Id);
                    if (productModel != null) {
                        actualBasket.Add(productModel, product_kvp.Value);
                    }
                }

                basketProductListModel.productList = actualBasket;

                BasketHelper.RestructureBasket(actualBasket.ToDictionary(x => x.Key.Id, x => x.Value), Response, Request);
            } else {
                basketProductListModel.productList = new Dictionary<ProductModel, int>();
            }

            ViewData["ProductCountInShop"] = _applicationDbContext.Products.Count();

            return View(basketProductListModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}