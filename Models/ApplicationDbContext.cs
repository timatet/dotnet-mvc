using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace dotnet_mvc.Models.DataModels
{
    /*
        > dotnet ef migrations add InitialCreate - генерация классов для создания таблиц
        > dotnet ef database update - обновление базы данных
    */
    public class ApplicationDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public ApplicationDbContext()
        {
            
        }

    }
}