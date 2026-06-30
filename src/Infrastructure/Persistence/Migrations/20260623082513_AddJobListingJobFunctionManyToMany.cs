using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJobListingJobFunctionManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "JobListingJobFunction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobListingId = table.Column<int>(type: "integer", nullable: false),
                    JobFunctionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobListingJobFunction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobListingJobFunction_JobFunction_JobFunctionId",
                        column: x => x.JobFunctionId,
                        principalTable: "JobFunction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobListingJobFunction_JobListing_JobListingId",
                        column: x => x.JobListingId,
                        principalTable: "JobListing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobListingJobFunction_JobFunctionId",
                table: "JobListingJobFunction",
                column: "JobFunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListingJobFunction_JobListingId_JobFunctionId",
                table: "JobListingJobFunction",
                columns: new[] { "JobListingId", "JobFunctionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobListingJobFunction");

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
    }
}
