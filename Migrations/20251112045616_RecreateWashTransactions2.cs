using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wsahRecieveDelivary.Migrations
{
    public partial class RecreateWashTransactions2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WashTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderId = table.Column<int>(nullable: false),
                    TransactionType = table.Column<string>(maxLength: 20, nullable: false),
                    ProcessStageId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    TransactionDate = table.Column<DateTime>(nullable: false),
                    BatchNo = table.Column<string>(maxLength: 100, nullable: true),
                    GatePassNo = table.Column<string>(maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(maxLength: 500, nullable: true),
                    ReceivedBy = table.Column<string>(maxLength: 100, nullable: true),
                    DeliveredTo = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WashTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WashTransactions_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WashTransactions_ProcessStages_ProcessStageId",
                        column: x => x.ProcessStageId,
                        principalTable: "ProcessStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WashTransactions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WashTransactions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_WorkOrderId",
                table: "WashTransactions",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_ProcessStageId",
                table: "WashTransactions",
                column: "ProcessStageId");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_CreatedBy",
                table: "WashTransactions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_UpdatedBy",
                table: "WashTransactions",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WashTransactions");
        }
    }
}
