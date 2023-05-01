using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotnet_mvc.Models;
using Microsoft.Extensions.Configuration;
using dotnet_mvc.Models.DataModels;
using dotnet_mvc.Models.HelpModels;
using Microsoft.AspNetCore.Hosting;

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

        public IActionResult Index()
        {
            ProductListModel productListModel = new ProductListModel();
            productListModel.productList = db.Products;

            string webRootPath = _webHostEnvironment.WebRootPath;
            ViewData["WebRootPath"] = webRootPath;

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
