using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace wsahRecieveDelivary.Migrations
{
    /// <inheritdoc />
    public partial class ConvertProcessStageToDynamic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WashTransactions_WorkOrderId_ProcessStage_TransactionType",
                table: "WashTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProcessStageBalances_WorkOrderId_ProcessStage",
                table: "ProcessStageBalances");

            migrationBuilder.DropColumn(
                name: "ProcessStage",
                table: "WashTransactions");

            migrationBuilder.DropColumn(
                name: "ProcessStage",
                table: "ProcessStageBalances");

            migrationBuilder.AddColumn<int>(
                name: "ProcessStageId",
                table: "WashTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessStageId",
                table: "ProcessStageBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProcessStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessStages", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Unwash Process", "Unwash" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Second Dry Process", "2nd Dry" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "First Wash Process", "1st Wash" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[] { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Final Wash Process", true, "Final Wash" });

            migrationBuilder.InsertData(
                table: "ProcessStages",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "First Dry Process", 1, true, "1st Dry", null },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Unwash Process", 2, true, "Unwash", null },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Second Dry Process", 3, true, "2nd Dry", null },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "First Wash Process", 4, true, "1st Wash", null },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Final Wash Process", 5, true, "Final Wash", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_ProcessStageId",
                table: "WashTransactions",
                column: "ProcessStageId");

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_WorkOrderId_ProcessStageId_TransactionType",
                table: "WashTransactions",
                columns: new[] { "WorkOrderId", "ProcessStageId", "TransactionType" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStageBalances_ProcessStageId",
                table: "ProcessStageBalances",
                column: "ProcessStageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStageBalances_WorkOrderId_ProcessStageId",
                table: "ProcessStageBalances",
                columns: new[] { "WorkOrderId", "ProcessStageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStages_DisplayOrder",
                table: "ProcessStages",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStages_Name",
                table: "ProcessStages",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessStageBalances_ProcessStages_ProcessStageId",
                table: "ProcessStageBalances",
                column: "ProcessStageId",
                principalTable: "ProcessStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WashTransactions_ProcessStages_ProcessStageId",
                table: "WashTransactions",
                column: "ProcessStageId",
                principalTable: "ProcessStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessStageBalances_ProcessStages_ProcessStageId",
                table: "ProcessStageBalances");

            migrationBuilder.DropForeignKey(
                name: "FK_WashTransactions_ProcessStages_ProcessStageId",
                table: "WashTransactions");

            migrationBuilder.DropTable(
                name: "ProcessStages");

            migrationBuilder.DropIndex(
                name: "IX_WashTransactions_ProcessStageId",
                table: "WashTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WashTransactions_WorkOrderId_ProcessStageId_TransactionType",
                table: "WashTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProcessStageBalances_ProcessStageId",
                table: "ProcessStageBalances");

            migrationBuilder.DropIndex(
                name: "IX_ProcessStageBalances_WorkOrderId_ProcessStageId",
                table: "ProcessStageBalances");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "ProcessStageId",
                table: "WashTransactions");

            migrationBuilder.DropColumn(
                name: "ProcessStageId",
                table: "ProcessStageBalances");

            migrationBuilder.AddColumn<string>(
                name: "ProcessStage",
                table: "WashTransactions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProcessStage",
                table: "ProcessStageBalances",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Second Dry Process", "2nd Dry" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "First Wash Process", "1st Wash" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Final Wash Process", "Final Wash" });

            migrationBuilder.CreateIndex(
                name: "IX_WashTransactions_WorkOrderId_ProcessStage_TransactionType",
                table: "WashTransactions",
                columns: new[] { "WorkOrderId", "ProcessStage", "TransactionType" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStageBalances_WorkOrderId_ProcessStage",
                table: "ProcessStageBalances",
                columns: new[] { "WorkOrderId", "ProcessStage" },
                unique: true);
        }
    }
}
