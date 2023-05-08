using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using dotnet_mvc.Models.Auxiliary;
using dotnet_mvc.Models.HelpModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnet_mvc.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{*statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    return View("Notice", 
                        new NoticeModel(
                            NoticeType.Error404,
                            "Ошибка 404",
                            "Упс.. Запрашиваемая страница не найдена"
                        )
                    );
            }

            return View("Notice", 
                new NoticeModel(
                    NoticeType.Error404,
                    "Ошибка 404",
                    "Упс.. Запрашиваемая страница не найдена"
                )
            );
        }
    }
}