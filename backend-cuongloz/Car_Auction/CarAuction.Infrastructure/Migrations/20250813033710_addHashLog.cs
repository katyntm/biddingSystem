using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAuction.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addHashLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CriteriaHashLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriteriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MD5Hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriaHashLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriteriaHashLogs_Criteria_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "Criteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StepHashLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MD5Hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepHashLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepHashLogs_Steps_StepId",
                        column: x => x.StepId,
                        principalTable: "Steps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TacticHashLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TacticId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MD5Hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TacticHashLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TacticHashLogs_Tactics_TacticId",
                        column: x => x.TacticId,
                        principalTable: "Tactics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaHashLogs_CriteriaId",
                table: "CriteriaHashLogs",
                column: "CriteriaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StepHashLogs_StepId",
                table: "StepHashLogs",
                column: "StepId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TacticHashLogs_TacticId",
                table: "TacticHashLogs",
                column: "TacticId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriteriaHashLogs");

            migrationBuilder.DropTable(
                name: "StepHashLogs");

            migrationBuilder.DropTable(
                name: "TacticHashLogs");
        }
    }
}
