using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wsahRecieveDelivary.Migrations
{
    /// <inheritdoc />
    public partial class AddWashTransactionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create WashTransactions table
            migrationBuilder.CreateTable(
                name: "WashTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProcessStage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GatePassNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveredTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WashTransactions", x => x.Id);
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
                    table.ForeignKey(
                        name: "FK_WashTransactions_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes for WashTransactions
            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_BatchNo",
                table: "WashTransactions",
                column: "BatchNo");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_TransactionDate",
                table: "WashTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_WorkOrderId",
                table: "WashTransactions",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_WorkOrderId_ProcessStage_TransactionType",
                table: "WashTransactions",
                columns: new[] { "WorkOrderId", "ProcessStage", "TransactionType" });

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_CreatedBy",
                table: "WashTransactions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_UpdatedBy",
                table: "WashTransactions",
                column: "UpdatedBy");

            // Create ProcessStageBalances table
            migrationBuilder.CreateTable(
                name: "ProcessStageBalances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderId = table.Column<int>(type: "int", nullable: false),
                    ProcessStage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalReceived = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalDelivered = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CurrentBalance = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessStageBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessStageBalances_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create unique index for ProcessStageBalances
            migrationBuilder.CreateIndex(
                name: "IX_ProcessStageBalances_WorkOrderId_ProcessStage",
                table: "ProcessStageBalances",
                columns: new[] { "WorkOrderId", "ProcessStage" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "WashTransactions");
            migrationBuilder.DropTable(name: "ProcessStageBalances");
        }
    }
}