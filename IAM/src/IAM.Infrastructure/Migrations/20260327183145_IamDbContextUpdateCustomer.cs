using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IamDbContextUpdateCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_super_user",
                table: "users",
                newName: "is_system_admin");

            migrationBuilder.AddColumn<bool>(
                name: "is_master",
                table: "customers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_master",
                table: "customers");

            migrationBuilder.RenameColumn(
                name: "is_system_admin",
                table: "users",
                newName: "is_super_user");
        }
    }
}
