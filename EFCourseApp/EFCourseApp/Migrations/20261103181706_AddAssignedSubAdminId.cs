using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCourseApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedSubAdminId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedSubAdminId",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedSubAdminId",
                table: "Customers");
        }
    }
}
