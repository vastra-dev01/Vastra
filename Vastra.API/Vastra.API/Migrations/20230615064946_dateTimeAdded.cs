using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vastra.API.Migrations
{
    public partial class dateTimeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "Users",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Users",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "Roles",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Roles",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "Products",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Products",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "CartItems",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "CartItems",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "Addresses",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Addresses",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "getutcdate()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "Addresses");
        }
    }
}
