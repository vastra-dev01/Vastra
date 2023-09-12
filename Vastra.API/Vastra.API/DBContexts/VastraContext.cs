using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Vastra.API.Entities;
using Vastra.API.Services;

namespace Vastra.API.DBContexts
{
    public class VastraContext : DbContext
    {
        //private readonly IConfiguration _configuration;

        public VastraContext(DbContextOptions options) : base(options) 
        {
            //_configuration = configuration;   
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    options.UseSqlServer(_configuration.GetConnectionString("VastraTestConnection"));
        //}

        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull); //TODO : Delete not working

            //sample category data
            modelBuilder.Entity<Category>().HasData(
            new Category("Men")
            {
                CategoryId = 1,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Category("Women")
            {
                CategoryId = 2,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Category("Kids")
            {
                CategoryId = 3,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Category("T Shirts")
            {
                CategoryId = 4,
                ParentCategoryId = 1,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Category("Full Sleeve T Shirts")
            {
                CategoryId = 5,
                ParentCategoryId = 4,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Category("Tops")
            {
                CategoryId = 6,
                ParentCategoryId = 2,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Category("French Tops")
            {
                CategoryId = 7,
                ParentCategoryId = 6,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now
            },
            new Category("Half Sleeve T Shirts")
            {
                CategoryId = 8,
                ParentCategoryId = 4,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now
            }
            );

            //pre-defined roles
            modelBuilder.Entity<Role>().HasData(
            new Role()
            {
                RoleName = "Admin",
                RoleId = 1,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now,
            },
            new Role()
            {
                RoleName = "User",
                RoleId = 2,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now
            }
                );

            //pre-defined users
            modelBuilder.Entity<User>().HasData(
            new User("VastraAdmin", 
            "1", 
            "9999999999", 
            Hashing.GetSha256Hash("Vastra@2024"))
            {
                UserId = 1,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now,
                RoleId = 1,
                EmailId = "vastra.admin@vastra.com"
            },
            new User("VastraUser",
            "1",
            "8888888888",
            Hashing.GetSha256Hash("VastraUser@2024"))
            {
                UserId = 2,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now,
                RoleId = 2,
                EmailId = "vastra.user@vastra.com"
            }
                );
            base.OnModelCreating(modelBuilder);
        }

    }
}
