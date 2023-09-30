﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vastra.API.DBContexts;

#nullable disable

namespace Vastra.API.Migrations
{
    [DbContext(typeof(VastraContext))]
    partial class VastraContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Vastra.API.Entities.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AddressId"), 1L, 1);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PinCode")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tag")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Type")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("AddressId");

                    b.HasIndex("UserId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Vastra.API.Entities.CartItem", b =>
                {
                    b.Property<int>("CartItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartItemId"), 1L, 1);

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<float>("Value")
                        .HasColumnType("real");

                    b.HasKey("CartItemId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("Vastra.API.Entities.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"), 1L, 1);

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("int");

                    b.HasKey("CategoryId");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryId = 1,
                            CategoryName = "Men",
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(1894),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(1917)
                        },
                        new
                        {
                            CategoryId = 2,
                            CategoryName = "Women",
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2012),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2013)
                        },
                        new
                        {
                            CategoryId = 3,
                            CategoryName = "Kids",
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2018),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2019)
                        },
                        new
                        {
                            CategoryId = 4,
                            CategoryName = "T Shirts",
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2022),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2024),
                            ParentCategoryId = 1
                        },
                        new
                        {
                            CategoryId = 5,
                            CategoryName = "Full Sleeve T Shirts",
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2026),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2028),
                            ParentCategoryId = 4
                        },
                        new
                        {
                            CategoryId = 6,
                            CategoryName = "Tops",
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2030),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2032),
                            ParentCategoryId = 2
                        },
                        new
                        {
                            CategoryId = 7,
                            CategoryName = "French Tops",
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2034),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2036),
                            ParentCategoryId = 6
                        },
                        new
                        {
                            CategoryId = 8,
                            CategoryName = "Half Sleeve T Shirts",
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2038),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2040),
                            ParentCategoryId = 4
                        });
                });

            modelBuilder.Entity("Vastra.API.Entities.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"), 1L, 1);

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<float>("Value")
                        .HasColumnType("real");

                    b.HasKey("OrderId");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Vastra.API.Entities.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"), 1L, 1);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Colour")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("SKU")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Vastra.API.Entities.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"), 1L, 1);

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            RoleId = 1,
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2369),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2372),
                            RoleName = "Admin"
                        },
                        new
                        {
                            RoleId = 2,
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2376),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2378),
                            RoleName = "User"
                        });
                });

            modelBuilder.Entity("Vastra.API.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RefreshTokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2863),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2866),
                            EmailId = "admin@vastra.com",
                            FirstName = "admin",
                            LastName = "admin",
                            Password = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=",
                            PhoneNumber = "9661734253",
                            RefreshTokenExpires = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            RoleId = 1
                        },
                        new
                        {
                            UserId = 2,
                            DateAdded = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2956),
                            DateModified = new DateTime(2023, 9, 30, 17, 30, 19, 468, DateTimeKind.Local).AddTicks(2958),
                            EmailId = "User1@vastra.com",
                            FirstName = "Sumit",
                            LastName = "Ranjan",
                            Password = "sMkwGjEJzLviG2lXyrFPM2pISrmuqnel/t9MV2Itvs0=",
                            PhoneNumber = "8804225153",
                            RefreshTokenExpires = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            RoleId = 2
                        });
                });

            modelBuilder.Entity("Vastra.API.Entities.Address", b =>
                {
                    b.HasOne("Vastra.API.Entities.User", "User")
                        .WithMany("Addresses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Vastra.API.Entities.CartItem", b =>
                {
                    b.HasOne("Vastra.API.Entities.Order", "Order")
                        .WithMany("CartItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vastra.API.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Vastra.API.Entities.Category", b =>
                {
                    b.HasOne("Vastra.API.Entities.Category", "ParentCategory")
                        .WithMany("ChildCategories")
                        .HasForeignKey("ParentCategoryId");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("Vastra.API.Entities.Order", b =>
                {
                    b.HasOne("Vastra.API.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Vastra.API.Entities.Product", b =>
                {
                    b.HasOne("Vastra.API.Entities.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Vastra.API.Entities.User", b =>
                {
                    b.HasOne("Vastra.API.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Vastra.API.Entities.Category", b =>
                {
                    b.Navigation("ChildCategories");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("Vastra.API.Entities.Order", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("Vastra.API.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Vastra.API.Entities.User", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
