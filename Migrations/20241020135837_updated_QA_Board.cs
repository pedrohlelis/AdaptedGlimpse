using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Glimpse.Migrations
{
    /// <inheritdoc />
    public partial class updated_QA_Board : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QANonApplicable",
                table: "Boards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QANonApplicable",
                table: "Boards");
        }
    }
}
