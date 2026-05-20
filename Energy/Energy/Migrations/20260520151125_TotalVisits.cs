using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Energy.Migrations
{
    /// <inheritdoc />
    public partial class TotalVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalVisits",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalVisits",
                table: "Users");
        }
    }
}
