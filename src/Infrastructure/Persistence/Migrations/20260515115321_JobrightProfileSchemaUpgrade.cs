using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class JobrightProfileSchemaUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "UserProfile");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "UserProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "UserProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "UserProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "UserProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "UserProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubUrl",
                table: "UserProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationMode",
                table: "UserProfile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Race/SexualOrientation were free-form strings; clear them before retyping to int enum.
            migrationBuilder.Sql(@"ALTER TABLE ""EeoData"" ALTER COLUMN ""SexualOrientation"" TYPE integer USING NULL;");
            migrationBuilder.Sql(@"ALTER TABLE ""EeoData"" ALTER COLUMN ""Race"" TYPE integer USING NULL;");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "Education",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "City",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "County",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "GitHubUrl",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "LocationMode",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "UserProfile");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "UserProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SexualOrientation",
                table: "EeoData",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Race",
                table: "EeoData",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "Education",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
