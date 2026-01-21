using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ufas1Forms.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadingDropdowns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentFieldId",
                table: "FormFields",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormFields_ParentFieldId",
                table: "FormFields",
                column: "ParentFieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormFields_FormFields_ParentFieldId",
                table: "FormFields",
                column: "ParentFieldId",
                principalTable: "FormFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormFields_FormFields_ParentFieldId",
                table: "FormFields");

            migrationBuilder.DropIndex(
                name: "IX_FormFields_ParentFieldId",
                table: "FormFields");

            migrationBuilder.DropColumn(
                name: "ParentFieldId",
                table: "FormFields");
        }
    }
}
