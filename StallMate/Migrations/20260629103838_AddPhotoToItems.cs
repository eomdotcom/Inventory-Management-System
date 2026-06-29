using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StallMate.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Items",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Items");
        }
    }
}
