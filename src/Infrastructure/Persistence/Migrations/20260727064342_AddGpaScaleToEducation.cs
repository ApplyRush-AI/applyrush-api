using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGpaScaleToEducation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GpaScale",
                table: "Education",
                type: "integer",
                nullable: false,
                // 1 = GpaScale.FourPoint: existing rows predate the scale and were entered on the US 4.0 scale.
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GpaScale",
                table: "Education");
        }
    }
}
