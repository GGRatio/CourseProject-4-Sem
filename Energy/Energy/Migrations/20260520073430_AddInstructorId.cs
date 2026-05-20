using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Energy.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstructorId",
                table: "GroupClasses",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "GroupClasses");
        }
    }
}
