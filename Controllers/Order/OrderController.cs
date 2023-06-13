using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using dotnet_mvc.Models.Auxiliary;
using dotnet_mvc.Models.DataModels;
using dotnet_mvc.Models.HelpModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Net.Codecrete.QrCodeGenerator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dotnet_mvc.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderController(
            ILogger<OrderController> logger, 
            ApplicationDbContext applicationDbContext, 
            UserManager<UserModel> userManager,
            IWebHostEnvironment webHostEnvironment,
            SignInManager<UserModel> signInManager
        ){
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public IActionResult Creation(string orderList)
        {
            IEnumerable<int[]> products = JsonConvert.DeserializeObject<IEnumerable<int[]>>(orderList);
            IEnumerable<BasketProductLinkModel> basketProductLinkModels = products.Select(x => new BasketProductLinkModel(
                _applicationDbContext.Products.FirstOrDefault(p => p.Id == x[0]),
                x[1]
            ));
            
            bool userIsSignedIn = _signInManager.IsSignedIn(User);

            OrderFormationModel orderFormationModel = new OrderFormationModel();

            OrderModel orderModel = new OrderModel();
            if (userIsSignedIn) {
                orderModel.User = _userManager.GetUserAsync(User).Result;
            }
            orderModel.TotalPrice = basketProductLinkModels.Sum(e => e.CountCopies * e.Product.Cost);
            orderModel.CreationDate = DateTime.Now;
            orderFormationModel.orderModel = orderModel;
            
            List<OrderProductModel> orderProductModels = new List<OrderProductModel>();
            foreach (var basketProductLinkModel in basketProductLinkModels) {
                OrderProductModel orderProductModel = new OrderProductModel();

                orderProductModel.ProductId = basketProductLinkModel.Product.Id;

                orderProductModel.Quantity = basketProductLinkModel.CountCopies;
                orderProductModels.Add(orderProductModel);
            }
            orderFormationModel.orderProductModels =  JsonConvert.SerializeObject(orderProductModels);

            var DeliveryMethods = Enum
                .GetValues(typeof(DeliveryMethodEnum))
                .Cast<DeliveryMethodEnum>()
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

            ViewData["DeliveryMethods"] = new SelectList(DeliveryMethods, "Key", "Value", DeliveryMethods.First().Key);

            return View(orderFormationModel);
        }
        
        [HttpPost]
        public IActionResult Formation(OrderFormationModel orderFormationModel) {

            OrderModel orderModel = orderFormationModel.orderModel;
            if (orderModel.User.Id == Guid.Empty) {
                orderModel.User = null;
            } else {
                orderModel.User = _applicationDbContext.Users.FirstOrDefault(u => u.Id == orderModel.User.Id);
            }
            orderModel.ConfirmationDate = DateTime.Now;
            _applicationDbContext.Orders.Add(orderModel);
            _applicationDbContext.SaveChanges();

            IEnumerable<OrderProductModel> orderProductModels = JsonConvert.DeserializeObject<IEnumerable<OrderProductModel>>(orderFormationModel.orderProductModels);

            foreach (var orderProduct in orderProductModels) {
                orderProduct.Order = orderModel;
                orderProduct.Product = _applicationDbContext.Products.FirstOrDefault(p => p.Id == orderProduct.ProductId);
                _applicationDbContext.OrderProducts.Add(orderProduct);
                _applicationDbContext.SaveChanges();
            }

            string orderDataForQr = string.Format("{0}", orderModel.Id);
            var qr = QrCode.EncodeText(orderDataForQr, QrCode.Ecc.High);
            string svg = qr.ToSvgString(1);

            bool ordersQrCodeDirectoryExist = System.IO.Directory.Exists(_webHostEnvironment.WebRootPath + "/orders/");
            if(!ordersQrCodeDirectoryExist)
                System.IO.Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "/orders/");

            using(var fileStream = new FileStream(_webHostEnvironment.WebRootPath + "/orders/order_" + orderDataForQr + ".svg", FileMode.Create))
            {
                fileStream.Write(Encoding.UTF8.GetBytes(svg));
            }

            return View(orderModel);
        }

        [HttpPost]
        public bool Remove()
        {
            if (!_signInManager.IsSignedIn(User) || !User.IsInRole("Admin")) { 
                return false;
            }

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

            int orderId = int.Parse(response["order_id"].ToString());
            OrderModel orderModel = _applicationDbContext.Orders.FirstOrDefault(o => o.Id == orderId);

            if (orderModel != null) {
                IEnumerable<OrderProductModel> orderProducts = _applicationDbContext.OrderProducts.Where(op => op.OrderId == orderId);
                _applicationDbContext.OrderProducts.RemoveRange(orderProducts);
                _applicationDbContext.SaveChanges();

                _applicationDbContext.Orders.Remove(orderModel);
                _applicationDbContext.SaveChanges();
            } else {
                return false;
            }
            
            return true;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}