using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ufas1Forms.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFaculteDomaineToSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DomaineId",
                table: "FormSubmissions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FaculteId",
                table: "FormSubmissions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_DomaineId",
                table: "FormSubmissions",
                column: "DomaineId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_FaculteId",
                table: "FormSubmissions",
                column: "FaculteId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormSubmissions_Domaines_DomaineId",
                table: "FormSubmissions",
                column: "DomaineId",
                principalTable: "Domaines",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FormSubmissions_Facultes_FaculteId",
                table: "FormSubmissions",
                column: "FaculteId",
                principalTable: "Facultes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormSubmissions_Domaines_DomaineId",
                table: "FormSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_FormSubmissions_Facultes_FaculteId",
                table: "FormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_DomaineId",
                table: "FormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_FaculteId",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "DomaineId",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "FaculteId",
                table: "FormSubmissions");
        }
    }
}
