using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMS2CoreProductEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EeoData_AspNetUsers_CreatedBy",
                table: "EeoData");

            migrationBuilder.DropForeignKey(
                name: "FK_EeoData_AspNetUsers_LastModifiedBy",
                table: "EeoData");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkExperience_AspNetUsers_CreatedBy",
                table: "WorkExperience");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkExperience_AspNetUsers_LastModifiedBy",
                table: "WorkExperience");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkExperienceBullet_AspNetUsers_CreatedBy",
                table: "WorkExperienceBullet");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkExperienceBullet_AspNetUsers_LastModifiedBy",
                table: "WorkExperienceBullet");

            migrationBuilder.DropIndex(
                name: "IX_WorkExperienceBullet_CreatedBy",
                table: "WorkExperienceBullet");

            migrationBuilder.DropIndex(
                name: "IX_WorkExperienceBullet_LastModifiedBy",
                table: "WorkExperienceBullet");

            migrationBuilder.DropIndex(
                name: "IX_WorkExperience_CreatedBy",
                table: "WorkExperience");

            migrationBuilder.DropIndex(
                name: "IX_WorkExperience_LastModifiedBy",
                table: "WorkExperience");

            migrationBuilder.DropIndex(
                name: "IX_EeoData_CreatedBy",
                table: "EeoData");

            migrationBuilder.DropIndex(
                name: "IX_EeoData_LastModifiedBy",
                table: "EeoData");

            migrationBuilder.CreateTable(
                name: "JobListing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalId = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Company = table.Column<string>(type: "text", nullable: false),
                    CompanyLogoUrl = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    About = table.Column<string>(type: "text", nullable: true),
                    Responsibilities = table.Column<string>(type: "text", nullable: true),
                    Requirements = table.Column<string>(type: "text", nullable: true),
                    Benefits = table.Column<string>(type: "text", nullable: true),
                    RequiredSkills = table.Column<string>(type: "text", nullable: true),
                    Industry = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: false),
                    WorkModel = table.Column<int>(type: "integer", nullable: false),
                    JobType = table.Column<int>(type: "integer", nullable: false),
                    ExperienceLevel = table.Column<int>(type: "integer", nullable: false),
                    SalaryMin = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    SalaryMax = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    YearsRequired = table.Column<int>(type: "integer", nullable: true),
                    ApplicantCount = table.Column<int>(type: "integer", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApplyUrl = table.Column<string>(type: "text", nullable: false),
                    H1BSupported = table.Column<bool>(type: "boolean", nullable: false),
                    AiSummary = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobListing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobListing_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobListing_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resume",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    FileType = table.Column<int>(type: "integer", nullable: false),
                    ParseStatus = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Media = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resume", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resume_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resume_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resume_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobApplication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplication_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplication_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplication_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplication_JobListing_JobId",
                        column: x => x.JobId,
                        principalTable: "JobListing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserJobMatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    OverallScore = table.Column<decimal>(type: "numeric", nullable: false),
                    ExperienceScore = table.Column<decimal>(type: "numeric", nullable: false),
                    SkillScore = table.Column<decimal>(type: "numeric", nullable: false),
                    TitleScore = table.Column<decimal>(type: "numeric", nullable: false),
                    IndustryScore = table.Column<decimal>(type: "numeric", nullable: false),
                    MatchedSkillsJson = table.Column<string>(type: "text", nullable: false),
                    MatchTier = table.Column<int>(type: "integer", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    JobListingId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJobMatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserJobMatch_JobListing_JobListingId",
                        column: x => x.JobListingId,
                        principalTable: "JobListing",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserSavedJob",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSavedJob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSavedJob_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSavedJob_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSavedJob_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSavedJob_JobListing_JobId",
                        column: x => x.JobId,
                        principalTable: "JobListing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_CreatedBy",
                table: "JobApplication",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_JobId",
                table: "JobApplication",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_LastModifiedBy",
                table: "JobApplication",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_UserId",
                table: "JobApplication",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobListing_CreatedBy",
                table: "JobListing",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_JobListing_ExternalId_Source",
                table: "JobListing",
                columns: new[] { "ExternalId", "Source" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobListing_LastModifiedBy",
                table: "JobListing",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Resume_CreatedBy",
                table: "Resume",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Resume_LastModifiedBy",
                table: "Resume",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Resume_UserId",
                table: "Resume",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserJobMatch_JobListingId",
                table: "UserJobMatch",
                column: "JobListingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedJob_CreatedBy",
                table: "UserSavedJob",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedJob_JobId",
                table: "UserSavedJob",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedJob_LastModifiedBy",
                table: "UserSavedJob",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedJob_UserId_JobId",
                table: "UserSavedJob",
                columns: new[] { "UserId", "JobId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobApplication");

            migrationBuilder.DropTable(
                name: "Resume");

            migrationBuilder.DropTable(
                name: "UserJobMatch");

            migrationBuilder.DropTable(
                name: "UserSavedJob");

            migrationBuilder.DropTable(
                name: "JobListing");

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperienceBullet_CreatedBy",
                table: "WorkExperienceBullet",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperienceBullet_LastModifiedBy",
                table: "WorkExperienceBullet",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperience_CreatedBy",
                table: "WorkExperience",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperience_LastModifiedBy",
                table: "WorkExperience",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EeoData_CreatedBy",
                table: "EeoData",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EeoData_LastModifiedBy",
                table: "EeoData",
                column: "LastModifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_EeoData_AspNetUsers_CreatedBy",
                table: "EeoData",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EeoData_AspNetUsers_LastModifiedBy",
                table: "EeoData",
                column: "LastModifiedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkExperience_AspNetUsers_CreatedBy",
                table: "WorkExperience",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkExperience_AspNetUsers_LastModifiedBy",
                table: "WorkExperience",
                column: "LastModifiedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkExperienceBullet_AspNetUsers_CreatedBy",
                table: "WorkExperienceBullet",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkExperienceBullet_AspNetUsers_LastModifiedBy",
                table: "WorkExperienceBullet",
                column: "LastModifiedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
