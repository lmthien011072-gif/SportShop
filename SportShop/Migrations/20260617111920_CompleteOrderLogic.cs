using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportShop.Migrations
{
    /// <inheritdoc />
    public partial class CompleteOrderLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustomized",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCustomized",
                table: "Orders");
        }
    }
}
