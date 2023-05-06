using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace dotnet_mvc.Models.DataModels
{
  /*
      > dotnet ef database update 0 && dotnet ef migrations remove && dotnet ef migrations add new && dotnet ef database update
      > dotnet ef migrations add InitialCreate - генерация классов для создания таблиц
      > dotnet ef database update - обновление базы данных
  */
    public class ApplicationDbContext : IdentityDbContext<UserModel, UserRoleModel, Guid>
    {

        public DbSet<LogModel> Logs { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductCharacteristic> ProductCharacteristics { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // User	        Представляет пользователя.
            // Role	        Представляет роль.
            // UserClaim	Представляет утверждение о том, что пользователь обладает.
            // UserToken	Представляет маркер проверки подлинности для пользователя.
            // UserLogin	Связывает пользователя с именем входа.
            // RoleClaim	Представляет утверждение, которое предоставляется всем пользователям в роли.
            // UserRole	    Сущность соединения, которая связывает пользователей и роли.

            modelBuilder.Entity<UserModel>().ToTable("Users");
            modelBuilder.Entity<UserRoleModel>().ToTable("Roles");

            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        }

    }
}