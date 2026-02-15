using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZulAi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UniverseStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CurrentTick = table.Column<int>(type: "int", nullable: false),
                    GridWidth = table.Column<int>(type: "int", nullable: false, defaultValue: 80),
                    GridHeight = table.Column<int>(type: "int", nullable: false, defaultValue: 40),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastTickAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniverseStates", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Atoms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    PositionX = table.Column<double>(type: "double", nullable: false),
                    PositionY = table.Column<double>(type: "double", nullable: false),
                    Energy = table.Column<double>(type: "double", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    IsAlive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Symbol = table.Column<string>(type: "char(1)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UniverseStateId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atoms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atoms_UniverseStates_UniverseStateId",
                        column: x => x.UniverseStateId,
                        principalTable: "UniverseStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GeneratedOutputs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Tick = table.Column<int>(type: "int", nullable: false),
                    AsciiArt = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GridWidth = table.Column<int>(type: "int", nullable: false),
                    GridHeight = table.Column<int>(type: "int", nullable: false),
                    AtomCount = table.Column<int>(type: "int", nullable: false),
                    ConnectionCount = table.Column<int>(type: "int", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UniverseStateId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedOutputs_UniverseStates_UniverseStateId",
                        column: x => x.UniverseStateId,
                        principalTable: "UniverseStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Interactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    AtomId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ConnectionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Tick = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OccurredAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UniverseStateId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interactions_UniverseStates_UniverseStateId",
                        column: x => x.UniverseStateId,
                        principalTable: "UniverseStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AtomConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SourceAtomId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TargetAtomId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Strength = table.Column<double>(type: "double", nullable: false),
                    TickFormed = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtomConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtomConnections_Atoms_SourceAtomId",
                        column: x => x.SourceAtomId,
                        principalTable: "Atoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AtomConnections_Atoms_TargetAtomId",
                        column: x => x.TargetAtomId,
                        principalTable: "Atoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AtomConnections_SourceAtomId",
                table: "AtomConnections",
                column: "SourceAtomId");

            migrationBuilder.CreateIndex(
                name: "IX_AtomConnections_TargetAtomId",
                table: "AtomConnections",
                column: "TargetAtomId");

            migrationBuilder.CreateIndex(
                name: "IX_Atoms_UniverseStateId_IsAlive",
                table: "Atoms",
                columns: new[] { "UniverseStateId", "IsAlive" });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedOutputs_UniverseStateId_Tick",
                table: "GeneratedOutputs",
                columns: new[] { "UniverseStateId", "Tick" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interactions_UniverseStateId_Tick",
                table: "Interactions",
                columns: new[] { "UniverseStateId", "Tick" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtomConnections");

            migrationBuilder.DropTable(
                name: "GeneratedOutputs");

            migrationBuilder.DropTable(
                name: "Interactions");

            migrationBuilder.DropTable(
                name: "Atoms");

            migrationBuilder.DropTable(
                name: "UniverseStates");
        }
    }
}
