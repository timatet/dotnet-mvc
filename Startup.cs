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
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"))
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
            // string[] roles = new string[] { "UserManager", "StaffManager" };
            // foreach (string role in roles)
            // {
            //     if (!await roleManager.RoleExistsAsync(role))
            //     {
            //         await roleManager.CreateAsync(new IdentityRole(role));
            //     }
            // }

            // Create admin user
            if (!applicationDbContext.Users.ToList().Exists(u => u.UserName == "admin"))
            {
                UserModel userModel = new UserModel();
                userModel.UserName = "admin";
                userModel.Email = "admin@timatet.ru";
                userModel.IsAdmin = true;
                userModel.EmailConfirmed = true;
                var umod = await userManager.CreateAsync(userModel, "Admin78");
            }

            // Ensure admin privileges
            // ApplicationUser admin = await userManager.FindByEmailAsync("info@example.com");
            // foreach (string role in roles)
            // {
            //     await userManager.AddToRoleAsync(admin, role);
            // }
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
