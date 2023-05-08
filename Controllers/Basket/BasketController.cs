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
                    BasketHelper.AddToCookieBasket(int.Parse(response["product_id"].ToString()), Request, Response);
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
                List<int> basketFromCookie = BasketHelper.GetBasketFromCookie(Request, Response);
               
                List<ProductModel> actualBasket = new List<ProductModel>();
                foreach (int product_id in basketFromCookie) {
                    ProductModel productModel = _applicationDbContext.Products.ToList().Find(p => p.Id == product_id);
                    if (productModel != null) {
                        actualBasket.Add(productModel);
                    }
                }

                basketProductListModel.productList = actualBasket;

                BasketHelper.RestructureBasket(actualBasket.Select(p => p.Id).ToList(), Response, Request);
            } else {
                basketProductListModel.productList = new List<ProductModel>();
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