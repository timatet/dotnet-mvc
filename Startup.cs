using System.Text;
using System.Data.Common;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using dotnet_mvc.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Identity;
using dotnet_mvc.Models.Auxiliary;
using dotnet_mvc.Helpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace dotnet_mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            Globals.Initialize();

            services.AddControllersWithViews();

            string ConnectionString = String.Format("Server={0}; Database={1}; User Id={2}; Password={3}; {4}",
                Globals.DATABASE_SERVER,
                Globals.DATABASE_NAME,
                Globals.DATABASE_USERNAME,
                Globals.DATABASE_PASSWORD,
                _configuration["ConnectionSettings:Options:Default"]
            );

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(ConnectionString)
            );

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddIdentity<UserModel, UserRoleModel>(opts =>
                {
                    opts.User.RequireUniqueEmail = true;
                    opts.Password.RequiredLength = 6;
                    opts.Password.RequireDigit = true;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequireLowercase = true;
                }
            ).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddErrorDescriber<MultilanguageIdentityErrorDescriber>()
            .AddDefaultTokenProviders();
        }

        public async Task Initialize(
            ApplicationDbContext applicationDbContext, 
            UserManager<UserModel> userManager, 
            RoleManager<UserRoleModel> roleManager
        ){

            applicationDbContext.Database.Migrate();

            // Create roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new UserRoleModel() { Name = "Admin", NormalizedName = "Администратор" });
            }

            if (!await roleManager.RoleExistsAsync("UserManager"))
            {
                await roleManager.CreateAsync(new UserRoleModel() { Name = "UserManager", NormalizedName = "Менеджер" });
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new UserRoleModel() { Name = "User", NormalizedName = "Пользователь" });
            }

            // Create admin user
            if (!applicationDbContext.Users.ToList().Exists(u => u.UserName == "admin"))
            {
                UserModel userModel = new UserModel();
                userModel.UserName = "admin";
                userModel.Email = "admin@timatet.ru";
                userModel.EmailConfirmed = true;
                var umod = await userManager.CreateAsync(userModel, "Admin78");
            } else {
                UserModel userModel = applicationDbContext.Users.FirstOrDefault(u => u.UserName == "admin");
                await userManager.AddToRoleAsync(userModel, "Admin");
            }

            if (applicationDbContext.Products.FirstOrDefault(p => p.Name == "Ботинки Zamberlan") == null) {
                BrandModel brand = new BrandModel() {
                    Name = "Zembarlan",
                    Description = "Zamberlan - итальянская компания по производству горной обуви с 1929 года. " +
                        "Ботинки Zamberlan, сделаны на небольшом семейном производстве в Италии по высочайшим стандартам качества. " +
                        "Горные ботинки Zamberlan сочетают в себе добрые ремесленные традиции с современными технологиями, именно " +
                        "в них впервые была использована легендарная подошва Vibram. Среди пользователей обувь Zamberlan славится " +
                        "невероятным удобством, для каждой модели в компании разрабатывается своя колодка."
                };
                applicationDbContext.Brands.Add(brand);
                applicationDbContext.SaveChanges();

                ProductCharacteristic productCharacteristic = new ProductCharacteristic() {
                    Weight = "0.871 кг",
                    Material = "Кожа и синтетика",
                    MoistureProtection = "Мембрана Gore-Tex",
                    ImpactProtection = "Есть",
                    UserGender = "Женский",
                    Size = "37, 38, 40"
                };
                applicationDbContext.ProductCharacteristics.Add(productCharacteristic);
                applicationDbContext.SaveChanges();

                ProductModel zamberlanDefault = new ProductModel() {
                    Name = "Ботинки Zamberlan",
                    Category = CategoryEnum.MountainTourism,
                    Description = "Эффективная, износостойкая и универсальная модель альпинистских ботинок. " +
                        "Подходят для альпинизма, ледолазания и иных высокогорных активностей. " +
                        "Использование в конструкции подошвы углеродного волокна позволило сделать их самыми лёгкими из аналогичных моделей бренда. " +
                        "Экономия в весе составляет порядка 7%." +
                        "Верх исполнен из цельнокроеной высококлассной кожи и суперпрочного материала Cordura, стратегически размещённого в местах сгиба. " +
                        "Широкий резиновый рант по периметру служит дополнительным элементом защиты. " +
                        "Эластичная облегающая гетра защищает от попадания внутрь мелкого мусора. " +
                        "Мембрана отрабатывает влагозащитный и пароотводящий функционал, одновременно обеспечивая дополнительную теплоизоляцию. " +
                        "Технология ZTECH Technical alpine last отвечает за максимальную анатомичность посадки. " +
                        "Подошва с участками различной плотности надёжна и долговечна.",
                        Cost = 35992,
                        CountInStack = 2,
                        Brand = brand,
                        ProductCharacteristic = productCharacteristic,
                        ImageUrl = "zamberlan.png"                
                };
                applicationDbContext.Products.Add(zamberlanDefault);
                applicationDbContext.SaveChanges();            
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder applicationBuilder, 
            IWebHostEnvironment webHostEnvironment, 
            ApplicationDbContext applicationDbContext, 
            UserManager<UserModel> userManager, 
            RoleManager<UserRoleModel> roleManager
        ){
            Initialize(applicationDbContext, userManager, roleManager).Wait();

            Globals.Initialize();

            if (webHostEnvironment.IsDevelopment())
            {
                //applicationBuilder.UseDeveloperExceptionPage();
                applicationBuilder.UseStatusCodePagesWithRedirects("/Error/{0}");
            }
            else
            {
                applicationBuilder.UseStatusCodePagesWithRedirects("/Error/{0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                applicationBuilder.UseHsts();
            }
            
            applicationBuilder.UseStaticFiles();
            
            /* Организация доступа к `node_modules` по адресу `/vendor` из `_Layout.cshtml` */
            applicationBuilder.UseStaticFiles(new StaticFileOptions{
                FileProvider = new PhysicalFileProvider(Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "node_modules"
                )),
                RequestPath = new PathString("/vendor")
            });

            applicationBuilder.UseRouting();

            applicationBuilder.UseAuthentication();
            applicationBuilder.UseAuthorization();

            applicationBuilder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
