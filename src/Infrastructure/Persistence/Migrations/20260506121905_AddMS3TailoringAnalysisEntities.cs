using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMS3TailoringAnalysisEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResumeAnalysis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OverallGrade = table.Column<int>(type: "integer", nullable: false),
                    UrgentFixCount = table.Column<int>(type: "integer", nullable: false),
                    CriticalFixCount = table.Column<int>(type: "integer", nullable: false),
                    OptionalFixCount = table.Column<int>(type: "integer", nullable: false),
                    Issues = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreditsUsed = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeAnalysis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeAnalysis_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResumeAnalysis_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResumeAnalysis_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeTailoring",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ResumeId = table.Column<int>(type: "integer", nullable: true),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    TailoredContent = table.Column<string>(type: "text", nullable: false),
                    ScoreBefore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ScoreAfter = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    CreditsUsed = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SectionsToEnhanceJson = table.Column<string>(type: "text", nullable: false),
                    KeywordsToInjectJson = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeTailoring", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeTailoring_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResumeTailoring_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResumeTailoring_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResumeTailoring_JobListing_JobId",
                        column: x => x.JobId,
                        principalTable: "JobListing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResumeTailoring_Resume_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResumeAnalysis_CreatedBy",
                table: "ResumeAnalysis",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeAnalysis_LastModifiedBy",
                table: "ResumeAnalysis",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeAnalysis_UserId",
                table: "ResumeAnalysis",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeTailoring_CreatedBy",
                table: "ResumeTailoring",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeTailoring_JobId",
                table: "ResumeTailoring",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeTailoring_LastModifiedBy",
                table: "ResumeTailoring",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeTailoring_ResumeId",
                table: "ResumeTailoring",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeTailoring_UserId",
                table: "ResumeTailoring",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResumeAnalysis");

            migrationBuilder.DropTable(
                name: "ResumeTailoring");
        }
    }
}
