using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryAPI.Migrations
{
    /// <inheritdoc />
    public partial class lalalalalal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_CreatedById",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_CreatedById",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Books",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Books");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CreatedById",
                table: "Books",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_CreatedById",
                table: "Books",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
