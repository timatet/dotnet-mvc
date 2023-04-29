﻿using System;
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
            string connection_string = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection sqlConnection = new SqlConnection(connection_string);
            sqlConnection.Open();
            SqlCommand testSqlCommand = new SqlCommand("SELECT COUNT(*) FROM Logs", sqlConnection);
            int count = (int) testSqlCommand.ExecuteScalar();

            ViewData["TotalData"] = count;

            sqlConnection.Close();

            var products = db.Products;
            return View(products);
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