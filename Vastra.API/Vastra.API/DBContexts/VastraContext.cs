using Microsoft.EntityFrameworkCore;
using Vastra.API.Entities;

namespace Vastra.API.DBContexts
{
    public class VastraContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public VastraContext(IConfiguration configuration)
        {
            _configuration = configuration;   
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_configuration.GetConnectionString("VastraTestConnection"));
        }

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
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Category>().HasData(
            new Category("Men")
            {
                CategoryId = 1,
            },
            new Category("Women")
            {
                CategoryId = 2,
            },
            new Category("Kids")
            {
                CategoryId = 3,
            },
            new Category("T Shirts")
            {
                CategoryId = 4,
                ParentCategoryId = 1
            },
            new Category("Full Sleeve T Shirts")
            {
                CategoryId = 5,
                ParentCategoryId = 4
            },
            new Category("Tops")
            {
                CategoryId = 6,
                ParentCategoryId = 2
            },
            new Category("French Tops")
            {
                CategoryId = 7,
                ParentCategoryId = 6
            },
            new Category("Half Sleeve T Shirts")
            {
                CategoryId = 8,
                ParentCategoryId = 4
            }
            );
            modelBuilder.Entity<Address>()
                .Property(p => p.DateModified)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnUpdate();
            modelBuilder.Entity<Address>()
                .Property(p => p.DateAdded)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<CartItem>()
                .Property(p => p.DateModified)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnUpdate();
            modelBuilder.Entity<CartItem>()
                .Property(p => p.DateAdded)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Category>()
                .Property(p => p.DateModified)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnUpdate();
            modelBuilder.Entity<Category>()
                .Property(p => p.DateAdded)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Order>()
                .Property(p => p.DateModified)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnUpdate();
            modelBuilder.Entity<Order>()
                .Property(p => p.DateAdded)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Product>()
                .Property(p => p.DateModified)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnUpdate();
            modelBuilder.Entity<Product>()
                .Property(p => p.DateAdded)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Role>()
                .Property(p => p.DateModified)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnUpdate();
            modelBuilder.Entity<Role>()
                .Property(p => p.DateAdded)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .Property(p => p.DateModified)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnUpdate();
            modelBuilder.Entity<User>()
                .Property(p => p.DateAdded)
                .HasComputedColumnSql("getutcdate()")
                .ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }

    }
}
