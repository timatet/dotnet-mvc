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

        [HttpGet]
        public int GetBasketProductCount() {
            
            int TotalCount = 0;
            
            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            
            // BASKET: Get count of products in basket
            if (!userIsSignedIn) { // in cookies
                var basket = BasketHelper.GetBasketFromCookie(Request, Response);

                foreach (var productKVP in basket) {
                    ProductModel product = _applicationDbContext.Products.ToList().Find(p => p.Id == productKVP.Key);
                    TotalCount += productKVP.Value;
                }
            } else { // in db
                UserModel user = _userManager.GetUserAsync(User).Result;

                BasketModel basketModel = _applicationDbContext.Baskets.FirstOrDefault(b => b.UserId == user.Id);
                if (basketModel != null) {  
                    TotalCount =  _applicationDbContext.BasketProductLinks.Sum(b => b.CountCopies);
                }            
            }

            return TotalCount;
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

            int product_id = int.Parse(request["product_id"].ToString());

            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            
            // BASKET: Remove product from basket
            try {
                if (!userIsSignedIn) { // in cookies
                    BasketHelper.RemoveFromCookieBasket(product_id, Request, Response);

                    return true;
                } else { // in db
                    UserModel user = _userManager.GetUserAsync(User).Result;
                    
                    BasketModel basketModel = _applicationDbContext.Baskets.FirstOrDefault(b => b.UserId == user.Id);
                    BasketProductLinkModel basketProductLinkModel = _applicationDbContext.BasketProductLinks
                        .Where(b => b.BasketId == basketModel.Id)
                        .FirstOrDefault(p => p.ProductId == product_id);
                    _applicationDbContext.Remove(basketProductLinkModel);
                    _applicationDbContext.SaveChanges();

                    return true;
                }
            } catch {
                return false;
            }
        }

        [HttpGet]
        public double GetBasketTotalCost() {

            double TotalCost = 0;

            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            
            // BASKET: Get total sum of products
            if (!userIsSignedIn) { // from cookies
                var basket = BasketHelper.GetBasketFromCookie(Request, Response);

                foreach (var productKVP in basket) {
                    ProductModel product = _applicationDbContext.Products.ToList().Find(p => p.Id == productKVP.Key);
                    TotalCost += product.Cost * productKVP.Value;
                }
            } else { // from db
                UserModel user = _userManager.GetUserAsync(User).Result;
                
                BasketModel basketModel = _applicationDbContext.Baskets.FirstOrDefault(b => b.UserId == user.Id);
                if (basketModel == null) {
                    basketModel = new BasketModel();
                    basketModel.User = user;
                    _applicationDbContext.Baskets.Add(basketModel);
                    _applicationDbContext.SaveChanges();
                }

                TotalCost = _applicationDbContext.BasketProductLinks.Where(b => b.BasketId == basketModel.Id)
                    .Sum(p => p.Product.Cost * p.CountCopies);
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

            int product_id = int.Parse(request["product_id"].ToString());
            int product_count = int.Parse(request["product_count"].ToString());
            
            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            
            // BASKET: Update count od product
            try {
                if (!userIsSignedIn) { // in cookies
                    BasketHelper.UpdateCount(product_id, product_count, Response, Request);

                    return true;
                } else { // in db
                    UserModel user = _userManager.GetUserAsync(User).Result;
                    
                    BasketModel basketModel = _applicationDbContext.Baskets.FirstOrDefault(b => b.UserId == user.Id);
                    BasketProductLinkModel basketProductLinkModel = _applicationDbContext.BasketProductLinks
                        .Where(b => b.BasketId == basketModel.Id)
                        .FirstOrDefault(p => p.ProductId == product_id);
                    basketProductLinkModel.CountCopies = product_count;
                    _applicationDbContext.BasketProductLinks.Update(basketProductLinkModel);
                    _applicationDbContext.SaveChanges();

                    return true;
                }
            } catch {
                return false;
            }
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

            int product_id = int.Parse(response["product_id"].ToString());
            ProductModel product = _applicationDbContext.Products.FirstOrDefault(p => p.Id == product_id);

            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            
            // BASKET: Add new product to basket
            try {
                if (!userIsSignedIn) { // into cookies
                    BasketHelper.AddToCookieBasket(product, Request, Response);
                    
                    return true;
                } else { // into db
                    UserModel user = _userManager.GetUserAsync(User).Result;

                    BasketModel basketModel = _applicationDbContext.Baskets.FirstOrDefault(b => b.UserId == user.Id);

                    if (basketModel == null) {  
                        basketModel = new BasketModel();
                        basketModel.User = user;
                        _applicationDbContext.Baskets.Add(basketModel);
                        _applicationDbContext.SaveChanges();
                    } 

                    BasketProductLinkModel basketProductLinkModel = _applicationDbContext.BasketProductLinks
                        .Where(b => b.BasketId == basketModel.Id).FirstOrDefault(b => b.ProductId == product_id);

                    if (basketProductLinkModel != null) {
                        basketProductLinkModel.CountCopies += 1;
                        _applicationDbContext.BasketProductLinks.Update(basketProductLinkModel);
                        _applicationDbContext.SaveChanges();
                    } else {
                        basketProductLinkModel = new BasketProductLinkModel();
                        basketProductLinkModel.Basket = basketModel;
                        basketProductLinkModel.Product = product;
                        _applicationDbContext.BasketProductLinks.Add(basketProductLinkModel);
                        _applicationDbContext.SaveChanges();
                    
                    }

                    return true;
                }
            } catch {
                return false;
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            BasketProductListModel basketProductListModel = new BasketProductListModel();

            // BASKET: Get list of products
            if (!userIsSignedIn) { // from cookies
                List<ProductModel> products = _applicationDbContext.Products.Include(p => p.ProductCharacteristic).ToList();
                Dictionary<ProductModel, int> basketFromCookie = 
                    BasketHelper.GetBasketFromCookie(Request, Response)
                        .ToDictionary(kvp => products.Find(x => x.Id == kvp.Key), kvp => kvp.Value);
               
                Dictionary<ProductModel, int> actualBasket = new Dictionary<ProductModel, int>();
                foreach (var product_kvp in basketFromCookie) {
                    ProductModel productModel = _applicationDbContext.Products.FirstOrDefault(p => p.Id == product_kvp.Key.Id);
                    if (productModel != null) {
                        actualBasket.Add(productModel, product_kvp.Value);
                    }
                }

                basketProductListModel.productList = actualBasket;

                BasketHelper.RestructureBasket(actualBasket.ToDictionary(x => x.Key.Id, x => x.Value), Response, Request);
            } else { // from db
                UserModel user = _userManager.GetUserAsync(User).Result;

                BasketModel basketModel = _applicationDbContext.Baskets.FirstOrDefault(b => b.UserId == user.Id);
                if (basketModel != null) {  
                    IEnumerable<BasketProductLinkModel> basketProductLinkModels = 
                        _applicationDbContext.BasketProductLinks.Include(b => b.Product).Include(b => b.Product.ProductCharacteristic)
                            .Where(b => b.BasketId == basketModel.Id).ToList(); 
                    basketProductListModel.productList = basketProductLinkModels.ToDictionary(k => k.Product, v => v.CountCopies);
                } else {
                    BasketModel newBasketModel = new BasketModel();
                    newBasketModel.User = user;
                    _applicationDbContext.Baskets.Add(newBasketModel);
                    _applicationDbContext.SaveChanges();

                    basketProductListModel.productList = new Dictionary<ProductModel, int>();
                }                
            }

            ViewData["ProductCountInShop"] = _applicationDbContext.Products.Count();

            if (userIsSignedIn) {
                ViewData["UserEmail"] = _userManager.GetUserAsync(User).Result.Email;
            } else {
                // TODO: Если пользователь не авторизован , то подставлять последнее значение из куки?
                ViewData["UserEmail"] = string.Empty;
            }

            return View(basketProductListModel);
        }

        [HttpGet]
        public IActionResult Clear()
        {
            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            BasketProductListModel basketProductListModel = new BasketProductListModel();

            // BASKET: Clear all products
            if (!userIsSignedIn) { // from cookies
                BasketHelper.RestructureBasket(new Dictionary<int, int>(), Response, Request);
            } else { // from db
                UserModel user = _userManager.GetUserAsync(User).Result;

                BasketModel basketModel = _applicationDbContext.Baskets.FirstOrDefault(b => b.UserId == user.Id);
                if (basketModel != null) {  
                    IEnumerable<BasketProductLinkModel> basketProductLinkModels = 
                        _applicationDbContext.BasketProductLinks.Include(b => b.Product).Include(b => b.Product.ProductCharacteristic)
                            .Where(b => b.BasketId == basketModel.Id).ToList(); 

                    _applicationDbContext.BasketProductLinks.RemoveRange(basketProductLinkModels);
                    _applicationDbContext.SaveChanges();
                } else {
                    BasketModel newBasketModel = new BasketModel();
                    newBasketModel.User = user;
                    _applicationDbContext.Baskets.Add(newBasketModel);
                    _applicationDbContext.SaveChanges();
                }                
            }

            basketProductListModel.productList = new Dictionary<ProductModel, int>();
            ViewData["ProductCountInShop"] = _applicationDbContext.Products.Count();

            return RedirectToAction("Index", "Basket", basketProductListModel);
        }

        [HttpPost]
        public async Task<bool> SendBasket(string email) {

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

            string email_br = response["email"].ToString();
            bool userIsSignedIn = _signInManager.IsSignedIn(User);

            Dictionary<ProductModel, int> basketProductsList = new Dictionary<ProductModel, int> ();
            
            // BASKET: Send Basket to Email
            try {
                if (!userIsSignedIn) { // into cookies
                    List<ProductModel> products = _applicationDbContext.Products.Include(p => p.ProductCharacteristic).ToList();

                    // Список товаров из куки
                    basketProductsList = 
                        BasketHelper.GetBasketFromCookie(Request, Response)
                            .ToDictionary(
                                kvp => _applicationDbContext.Products
                                    .Include(p => p.ProductCharacteristic)
                                        .FirstOrDefault(x => x.Id == kvp.Key), 
                                kvp => kvp.Value
                            );
                } else { // into db
                    UserModel user = _userManager.GetUserAsync(User).Result;

                    BasketModel basketModel = _applicationDbContext.Baskets.FirstOrDefault(b => b.UserId == user.Id);

                    if (basketModel == null) {  
                        basketModel = new BasketModel();
                        basketModel.User = user;
                        _applicationDbContext.Baskets.Add(basketModel);
                        _applicationDbContext.SaveChanges();
                    } 

                    // Список товаров из бд
                    basketProductsList = 
                        _applicationDbContext.BasketProductLinks.Include(b => b.Product).Include(b => b.Product.ProductCharacteristic)
                            .Where(b => b.BasketId == basketModel.Id)
                                .ToDictionary(b => b.Product, b => b.CountCopies);
                }
            } catch {
                return false;
            }

            // Формирование письма
            string email_brief = HtmlGenerator.GetBasketHtml(basketProductsList);

            var emailService = new EmailService();
            await emailService.SendEmailAsync(email_br, "Ваши товары", email_brief);
            try {
                await emailService.SendEmailAsync(email_br, "Ваши товары", email_brief);

                return true;
            } catch {
                return false;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}