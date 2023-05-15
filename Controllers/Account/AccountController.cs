using System;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotnet_mvc.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using dotnet_mvc.Models.HelpModels;
using dotnet_mvc.Helpers;
using Microsoft.AspNetCore.Authorization;
using dotnet_mvc.Models.DataModels;
using dotnet_mvc.Models.Auxiliary;

namespace dotnet_mvc.Controllers
{
  public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        public IConfiguration _configuration { get; }

        public AccountController(
            ILogger<AccountController> logger, 
            IConfiguration configuration, 
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            ApplicationDbContext applicationDbContext
        ){
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
        } 

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            if (userIsSignedIn) { 
                // string currentDisplayUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);
                return View("Notice", 
                    new NoticeModel(
                        NoticeType.IsAuthorized,
                        "Вы уже авторизованы!",
                        "У вас не получится авторизоваться повторно."
                    )
                );
            }

            return View(
                new LoginModel { 
                    ReturnUrl = returnUrl 
                }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            if (userIsSignedIn) { 
                // string currentDisplayUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);
                return View("Notice", NoticeModel.GetAccessErrorNoticeModel());
            }

            if (ModelState.IsValid)
            {
                UserModel userByEmailAttempt = await _userManager.FindByEmailAsync(loginModel.EmailOrUserName);
                UserModel userByUserNameAttempt = await _userManager.FindByNameAsync(loginModel.EmailOrUserName);
                
                // Проверка попытки входа по EMail
                if (userByEmailAttempt != null)
                {
                    if (userByEmailAttempt.EmailConfirmed)
                    {
                        var result = await _signInManager.PasswordSignInAsync(
                            userByEmailAttempt, loginModel.Password, loginModel.RememberMe, false);
                        if (result.Succeeded)
                        {
                            // проверяем, принадлежит ли URL приложению
                            if (!string.IsNullOrEmpty(loginModel.ReturnUrl) && Url.IsLocalUrl(loginModel.ReturnUrl))
                            {
                                return Redirect(loginModel.ReturnUrl);
                            }

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewData["LoginErrorMsg"] = "Неправильный логин и (или) пароль";
                        }
                    }
                    else
                    {
                        ViewData["LoginErrorMsg"] = "Неподтвержден адрес электронной почты";
                    }
                } else {
                    // Проверка попытки входа по логину
                    if (userByUserNameAttempt != null)
                    {
                        if (userByUserNameAttempt.EmailConfirmed)
                        {
                            var result = await _signInManager.PasswordSignInAsync(
                                userByUserNameAttempt, loginModel.Password, loginModel.RememberMe, false);
                            if (result.Succeeded)
                            {
                                // проверяем, принадлежит ли URL приложению
                                if (!string.IsNullOrEmpty(loginModel.ReturnUrl) && Url.IsLocalUrl(loginModel.ReturnUrl))
                                {
                                    return Redirect(loginModel.ReturnUrl);
                                }

                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewData["LoginErrorMsg"] = "Неправильный логин и (или) пароль";
                            }
                        }
                        else
                        {
                            ViewData["LoginErrorMsg"] = "Неподтвержден адрес электронной почты";
                        }
                    } else {
                        ViewData["LoginErrorMsg"] = "Неправильный логин и (или) пароль";
                    }
                }
            }
            
            return View(loginModel);
        }

        [HttpPost]
        public async Task<string> Logout()
        {   
            try {
                await _signInManager.SignOutAsync();
                return "success";
            } catch {
                return "error";
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            if (userIsSignedIn) { 
                // string currentDisplayUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);
                return View("Notice", 
                    new NoticeModel(
                        NoticeType.IsAuthorized,
                        "Вы уже авторизованы!",
                        "У вас не получится зарегистрироваться, когда вы уже вошли на сайт."
                    )
                );
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            if (userIsSignedIn) { 
                // string currentDisplayUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);
                return View("Notice", NoticeModel.GetAccessErrorNoticeModel());
            }

            if (ModelState.IsValid)
            {
                UserModel user = new UserModel() { 
                    Email = registerModel.Email,
                    UserName = registerModel.Login,
                    FullName = registerModel.FullName,
                    BirthdayDate = registerModel.BirthdayDate
                };
                
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account",
                        new { 
                            userId = user.Id, 
                            code = code 
                        },
                        protocol: HttpContext.Request.Scheme
                    );

                    var emailService = new EmailService();
                    try {
                        await emailService.SendEmailAsync(registerModel.Email, "Confirm your account",
                            $"Подтвердите регистрацию пройдя по ссылке: <a href ='{callbackUrl}'>link</a>");
                    } catch {
                        _applicationDbContext.Users.Remove(user);
                        _applicationDbContext.SaveChanges();

                        NoticeModel errorNotice = new NoticeModel(
                            NoticeType.DrizzleError,
                            "Ошибка подключения к серверу",
                            "В настоящее время регистрация недоступна. Попробуйте пожалуйста позже!"
                        );

                        return View("Notice", errorNotice);
                    }

                    NoticeModel noticeModel = new NoticeModel(
                        NoticeType.WaitTime,
                        "Завершение регистрации",
                        "Для завершения регистрации проверьте электронную почту " + registerModel.Email + 
                            " и пройдите по ссылке, указанной в письме"
                    );

                    return View("Notice", noticeModel);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ViewData["RegisterErrorMsg"] = error.Description;
                    }
                }
            }

            return View(registerModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return View("Error");
            var user = await _userManager.FindByIdAsync(userId);
            if(user==null)
                return View("Error");
            var result =await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                // установка куки
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }
            
            return View("Error");
        }

        [HttpGet]
        public IActionResult Index(Guid id)
        {   
            bool userIsSignedIn = _signInManager.IsSignedIn(User);
            if (!userIsSignedIn) { 
                return View("Notice", NoticeModel.GetAccessErrorNoticeModel());
            }
            
            UserModel user = _userManager.GetUserAsync(User).Result;
            bool userIsReal = user.Id == id ? true : false;
            if (!userIsReal) { 
                return View("Notice", NoticeModel.GetAccessErrorNoticeModel());
            }

            AccountModel accountModel = new AccountModel();
            accountModel.user = user;

            return View(accountModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
