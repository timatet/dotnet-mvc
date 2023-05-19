using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using dotnet_mvc.Models.DataModels;
using dotnet_mvc.Models.HelpModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace dotnet_mvc.Controllers.Catalog
{
    public class CatalogController : Controller
    {
        private readonly ILogger<CatalogController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public CatalogController(
            ILogger<CatalogController> logger,
            ApplicationDbContext applicationDbContext
        ) {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            ProductListModel productListModel = new ProductListModel();
            productListModel.productList = _applicationDbContext.Products.Include(p => p.Brand);

            return View(productListModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}