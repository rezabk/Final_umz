using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ScoreaddedtoProjectAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAnsweredProject");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "ProjectAnswer",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "ee1e08d3-e7cf-49df-87d2-5b0492c7102b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "563c47de-cabf-42ed-ba10-8a6d1fa215eb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "53744853-5d6d-4702-b39a-d65a68457506");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "ProjectAnswer");

            migrationBuilder.CreateTable(
                name: "UserAnsweredProject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    InsertByUserId = table.Column<string>(type: "text", nullable: true),
                    InsertTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: true),
                    RemoveByUserId = table.Column<string>(type: "text", nullable: true),
                    RemoveTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateByUserId = table.Column<string>(type: "text", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnsweredProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnsweredProject_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAnsweredProject_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "02bab734-351b-405d-a739-7e1360fcf50b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "538e4e9e-fe0e-4ba4-99c3-ac7c9af8c6c9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "df179d45-1de2-4c2b-adac-23dda44ec41b");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnsweredProject_ProjectId",
                table: "UserAnsweredProject",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnsweredProject_UserId",
                table: "UserAnsweredProject",
                column: "UserId");
        }
    }
}
