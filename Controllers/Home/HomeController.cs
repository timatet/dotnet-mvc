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

namespace dotnet_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IConfiguration _configuration { get; }

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            
        } 

        public IActionResult Index()
        {
            string connection_string = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection sqlConnection = new SqlConnection(connection_string);
            sqlConnection.Open();
            SqlCommand testSqlCommand = new SqlCommand("SELECT COUNT(*) FROM Logs", sqlConnection);
            int count = (int) testSqlCommand.ExecuteScalar();

            ViewData["TotalData"] = count;

            sqlConnection.Close();

            return View();
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
