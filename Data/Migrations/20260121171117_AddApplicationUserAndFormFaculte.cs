using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ufas1Forms.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserAndFormFaculte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FaculteId",
                table: "Forms",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FaculteId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_FaculteId",
                table: "Forms",
                column: "FaculteId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FaculteId",
                table: "AspNetUsers",
                column: "FaculteId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Facultes_FaculteId",
                table: "AspNetUsers",
                column: "FaculteId",
                principalTable: "Facultes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Facultes_FaculteId",
                table: "Forms",
                column: "FaculteId",
                principalTable: "Facultes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Facultes_FaculteId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Facultes_FaculteId",
                table: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_Forms_FaculteId",
                table: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FaculteId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FaculteId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "FaculteId",
                table: "AspNetUsers");
        }
    }
}
