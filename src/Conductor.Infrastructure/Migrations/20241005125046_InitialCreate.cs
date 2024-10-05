using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Conductor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "deployment",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("deployment_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "number",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("number_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "process",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("process_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "target",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    deployment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    process_id = table.Column<Guid>(type: "uuid", nullable: false),
                    revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parallel_count = table.Column<int>(type: "integer", nullable: false),
                    buffer_size = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("target_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_target_deployment_deployment_id",
                        column: x => x.deployment_id,
                        principalSchema: "public",
                        principalTable: "deployment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "revision",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    process_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    is_draft = table.Column<bool>(type: "boolean", nullable: false),
                    release_notes = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<JsonElement>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("revision_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_revision_process_process_id",
                        column: x => x.process_id,
                        principalSchema: "public",
                        principalTable: "process",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "number",
                columns: new[] { "id", "kind", "value" },
                values: new object[,]
                {
                    { new Guid("105493e5-9d9b-46eb-be31-a3906514bf2e"), 1, 1 },
                    { new Guid("53b9986a-b2c6-4e7e-9ea1-ac02ba315154"), 3, 1 },
                    { new Guid("9d7f4492-3bde-4dc7-805d-31c3ab0448e6"), 2, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_revision_process_id",
                schema: "public",
                table: "revision",
                column: "process_id");

            migrationBuilder.CreateIndex(
                name: "IX_target_deployment_id",
                schema: "public",
                table: "target",
                column: "deployment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "number",
                schema: "public");

            migrationBuilder.DropTable(
                name: "revision",
                schema: "public");

            migrationBuilder.DropTable(
                name: "target",
                schema: "public");

            migrationBuilder.DropTable(
                name: "process",
                schema: "public");

            migrationBuilder.DropTable(
                name: "deployment",
                schema: "public");
        }
    }
}
