using Microsoft.EntityFrameworkCore;

namespace dotnet_mvc.Models.DataModels
{
  /*
      > dotnet ef migrations add InitialCreate - генерация классов для создания таблиц
      > dotnet ef database update - обновление базы данных
  */
  public class ApplicationDbContext : DbContext
    {

        public DbSet<UserModel> Users { get; set; }
        public DbSet<LogModel> Logs { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductCharacteristic> ProductCharacteristics { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public ApplicationDbContext()
        {
            
        }

    }
}