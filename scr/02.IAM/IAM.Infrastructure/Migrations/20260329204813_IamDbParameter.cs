using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IamDbParameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "parameters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    list_items = table.Column<string>(type: "text", nullable: false),
                    external_list_endpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_customer_editable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parameters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parameter_customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parameter_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parameter_customers", x => x.id);
                    table.ForeignKey(
                        name: "fk_parameter_customers_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_parameter_customers_parameters_parameter_id",
                        column: x => x.parameter_id,
                        principalTable: "parameters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_parameter_customers_customer_id",
                table: "parameter_customers",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_parameter_customers_parameter_id",
                table: "parameter_customers",
                column: "parameter_id");

            migrationBuilder.CreateIndex(
                name: "ix_parameters_group_key",
                table: "parameters",
                columns: new[] { "group", "key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parameter_customers");

            migrationBuilder.DropTable(
                name: "parameters");
        }
    }
}
