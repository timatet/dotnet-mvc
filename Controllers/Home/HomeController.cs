using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotnet_mvc.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using dotnet_mvc.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using dotnet_mvc.Models.HelpModels;

namespace dotnet_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IConfiguration _configuration { get; }
        private readonly ApplicationDbContext db;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _configuration = configuration;
            db = applicationDbContext;
        } 

        public IActionResult Index()
        {
            ProductListModel productListModel = new ProductListModel();
            productListModel.productList = db.Products;

            return View(productListModel);
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
