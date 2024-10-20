using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Glimpse.Migrations
{
    /// <inheritdoc />
    public partial class updated_card_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedAt",
                table: "Cards",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishedAt",
                table: "Cards");
        }
    }
}
