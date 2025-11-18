using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wsahRecieveDelivary.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Factory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Line = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Buyer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BuyerDepartment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StyleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FastReactNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkOrderNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WashType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrderQuantity = table.Column<int>(type: "int", nullable: false),
                    CutQty = table.Column<int>(type: "int", nullable: false),
                    TOD = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SewingCompDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstRCVDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WashApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WashTargetDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalWashReceived = table.Column<int>(type: "int", nullable: false),
                    TotalWashDelivery = table.Column<int>(type: "int", nullable: false),
                    WashBalance = table.Column<int>(type: "int", nullable: false),
                    FromReceived = table.Column<int>(type: "int", nullable: false),
                    Marks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStageBalances_WorkOrderId_ProcessStage",
                table: "ProcessStageBalances",
                columns: new[] { "WorkOrderId", "ProcessStage" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_BatchNo",
                table: "WashTransactions",
                column: "BatchNo");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_CreatedBy",
                table: "WashTransactions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_TransactionDate",
                table: "WashTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_UpdatedBy",
                table: "WashTransactions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_WorkOrderId",
                table: "WashTransactions",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_WorkOrderId_ProcessStage_TransactionType",
                table: "WashTransactions",
                columns: new[] { "WorkOrderId", "ProcessStage", "TransactionType" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_Buyer_WorkOrderNo",
                table: "WorkOrders",
                columns: new[] { "Buyer", "WorkOrderNo" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_CreatedBy",
                table: "WorkOrders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_Factory_Line_WorkOrderNo",
                table: "WorkOrders",
                columns: new[] { "Factory", "Line", "WorkOrderNo" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_UpdatedBy",
                table: "WorkOrders",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_WorkOrderNo",
                table: "WorkOrders",
                column: "WorkOrderNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessStageBalances");

            migrationBuilder.DropTable(
                name: "WashTransactions");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 6, 9, 43, 59, 759, DateTimeKind.Utc).AddTicks(2697));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 6, 9, 43, 59, 759, DateTimeKind.Utc).AddTicks(2699));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 6, 9, 43, 59, 759, DateTimeKind.Utc).AddTicks(2700));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 6, 9, 43, 59, 759, DateTimeKind.Utc).AddTicks(2702));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 6, 9, 43, 59, 759, DateTimeKind.Utc).AddTicks(2585));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 6, 9, 43, 59, 759, DateTimeKind.Utc).AddTicks(2586));
        }
    }
}
