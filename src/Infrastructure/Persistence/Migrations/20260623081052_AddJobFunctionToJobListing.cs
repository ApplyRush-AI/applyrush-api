using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJobFunctionToJobListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobFunctionId",
                table: "JobListing",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobListing_JobFunctionId",
                table: "JobListing",
                column: "JobFunctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobListing_JobFunction_JobFunctionId",
                table: "JobListing",
                column: "JobFunctionId",
                principalTable: "JobFunction",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobListing_JobFunction_JobFunctionId",
                table: "JobListing");

            migrationBuilder.DropIndex(
                name: "IX_JobListing_JobFunctionId",
                table: "JobListing");

            migrationBuilder.DropColumn(
                name: "JobFunctionId",
                table: "JobListing");
        }
    }
}
