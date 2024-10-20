using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Glimpse.Migrations
{
    /// <inheritdoc />
    public partial class updated_card_model_again : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResponsibleSuperiorId",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponsibleSuperiorId",
                table: "Cards");
        }
    }
}
