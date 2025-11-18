using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;
using wsahRecieveDelivary.Data;
using wsahRecieveDelivary.DTOs;
using wsahRecieveDelivary.Models;

namespace wsahRecieveDelivary.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly ApplicationDbContext _context;

        public WorkOrderService(ApplicationDbContext context)
        {
            _context = context;
            // Set EPPlus License Context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        // ==========================================
        // CREATE WORK ORDER
        // ==========================================
        public async Task<WorkOrderResponseDto> CreateAsync(WorkOrderDto dto, int userId)
        {
            // Check if WorkOrderNo already exists
            if (await _context.WorkOrders.AnyAsync(w => w.WorkOrderNo == dto.WorkOrderNo))
            {
                throw new InvalidOperationException($"Work Order No '{dto.WorkOrderNo}' already exists");
            }

            var workOrder = new WorkOrder
            {
                Factory = dto.Factory,
                Line = dto.Line,
                Unit = dto.Unit,
                Buyer = dto.Buyer,
                BuyerDepartment = dto.BuyerDepartment,
                StyleName = dto.StyleName,
                FastReactNo = dto.FastReactNo,
                Color = dto.Color,
                WorkOrderNo = dto.WorkOrderNo,
                WashType = dto.WashType,
                OrderQuantity = dto.OrderQuantity,
                CutQty = dto.CutQty,
                TOD = dto.TOD,
                SewingCompDate = dto.SewingCompDate,
                FirstRCVDate = dto.FirstRCVDate,
                WashApprovalDate = dto.WashApprovalDate,
                WashTargetDate = dto.WashTargetDate,
                TotalWashReceived = dto.TotalWashReceived,
                TotalWashDelivery = dto.TotalWashDelivery,
                WashBalance = dto.WashBalance,
                FromReceived = dto.FromReceived,
                Marks = dto.Marks,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.WorkOrders.Add(workOrder);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(workOrder.Id) ?? throw new Exception("Failed to retrieve created work order");
        }

        // ==========================================
        // UPDATE WORK ORDER
        // ==========================================
        public async Task<WorkOrderResponseDto> UpdateAsync(int id, WorkOrderDto dto, int userId)
        {
            var workOrder = await _context.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                throw new KeyNotFoundException($"Work Order with ID {id} not found");
            }

            // Check if updating WorkOrderNo to existing one
            if (workOrder.WorkOrderNo != dto.WorkOrderNo &&
                await _context.WorkOrders.AnyAsync(w => w.WorkOrderNo == dto.WorkOrderNo))
            {
                throw new InvalidOperationException($"Work Order No '{dto.WorkOrderNo}' already exists");
            }

            workOrder.Factory = dto.Factory;
            workOrder.Line = dto.Line;
            workOrder.Unit = dto.Unit;
            workOrder.Buyer = dto.Buyer;
            workOrder.BuyerDepartment = dto.BuyerDepartment;
            workOrder.StyleName = dto.StyleName;
            workOrder.FastReactNo = dto.FastReactNo;
            workOrder.Color = dto.Color;
            workOrder.WorkOrderNo = dto.WorkOrderNo;
            workOrder.WashType = dto.WashType;
            workOrder.OrderQuantity = dto.OrderQuantity;
            workOrder.CutQty = dto.CutQty;
            workOrder.TOD = dto.TOD;
            workOrder.SewingCompDate = dto.SewingCompDate;
            workOrder.FirstRCVDate = dto.FirstRCVDate;
            workOrder.WashApprovalDate = dto.WashApprovalDate;
            workOrder.WashTargetDate = dto.WashTargetDate;
            workOrder.TotalWashReceived = dto.TotalWashReceived;
            workOrder.TotalWashDelivery = dto.TotalWashDelivery;
            workOrder.WashBalance = dto.WashBalance;
            workOrder.FromReceived = dto.FromReceived;
            workOrder.Marks = dto.Marks;
            workOrder.UpdatedBy = userId;
            workOrder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(workOrder.Id) ?? throw new Exception("Failed to retrieve updated work order");
        }

        // ==========================================
        // DELETE WORK ORDER
        // ==========================================
        public async Task<bool> DeleteAsync(int id)
        {
            var workOrder = await _context.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                return false;
            }

            _context.WorkOrders.Remove(workOrder);
            await _context.SaveChangesAsync();
            return true;
        }

        // ==========================================
        // GET BY ID
        // ==========================================
        public async Task<WorkOrderResponseDto?> GetByIdAsync(int id)
        {
            return await _context.WorkOrders
                .Include(w => w.CreatedByUser)
                .Include(w => w.UpdatedByUser)
                .Where(w => w.Id == id)
                .Select(w => new WorkOrderResponseDto
                {
                    Id = w.Id,
                    Factory = w.Factory,
                    Line = w.Line,
                    Unit = w.Unit,
                    Buyer = w.Buyer,
                    BuyerDepartment = w.BuyerDepartment,
                    StyleName = w.StyleName,
                    FastReactNo = w.FastReactNo,
                    Color = w.Color,
                    WorkOrderNo = w.WorkOrderNo,
                    WashType = w.WashType,
                    OrderQuantity = w.OrderQuantity,
                    CutQty = w.CutQty,
                    TOD = w.TOD,
                    SewingCompDate = w.SewingCompDate,
                    FirstRCVDate = w.FirstRCVDate,
                    WashApprovalDate = w.WashApprovalDate,
                    WashTargetDate = w.WashTargetDate,
                    TotalWashReceived = w.TotalWashReceived,
                    TotalWashDelivery = w.TotalWashDelivery,
                    WashBalance = w.WashBalance,
                    FromReceived = w.FromReceived,
                    Marks = w.Marks,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt,
                    CreatedByUsername = w.CreatedByUser.Username,
                    UpdatedByUsername = w.UpdatedByUser != null ? w.UpdatedByUser.Username : null
                })
                .FirstOrDefaultAsync();
        }

        // ==========================================
        // GET BY WORK ORDER NO
        // ==========================================
        public async Task<WorkOrderResponseDto?> GetByWorkOrderNoAsync(string workOrderNo)
        {
            return await _context.WorkOrders
                .Include(w => w.CreatedByUser)
                .Include(w => w.UpdatedByUser)
                .Where(w => w.WorkOrderNo == workOrderNo)
                .Select(w => new WorkOrderResponseDto
                {
                    Id = w.Id,
                    Factory = w.Factory,
                    Line = w.Line,
                    Unit = w.Unit,
                    Buyer = w.Buyer,
                    BuyerDepartment = w.BuyerDepartment,
                    StyleName = w.StyleName,
                    FastReactNo = w.FastReactNo,
                    Color = w.Color,
                    WorkOrderNo = w.WorkOrderNo,
                    WashType = w.WashType,
                    OrderQuantity = w.OrderQuantity,
                    CutQty = w.CutQty,
                    TOD = w.TOD,
                    SewingCompDate = w.SewingCompDate,
                    FirstRCVDate = w.FirstRCVDate,
                    WashApprovalDate = w.WashApprovalDate,
                    WashTargetDate = w.WashTargetDate,
                    TotalWashReceived = w.TotalWashReceived,
                    TotalWashDelivery = w.TotalWashDelivery,
                    WashBalance = w.WashBalance,
                    FromReceived = w.FromReceived,
                    Marks = w.Marks,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt,
                    CreatedByUsername = w.CreatedByUser.Username,
                    UpdatedByUsername = w.UpdatedByUser != null ? w.UpdatedByUser.Username : null
                })
                .FirstOrDefaultAsync();
        }

        // ==========================================
        // GET ALL
        // ==========================================
        public async Task<List<WorkOrderResponseDto>> GetAllAsync()
        {
            return await _context.WorkOrders
                .Include(w => w.CreatedByUser)
                .Include(w => w.UpdatedByUser)
                .OrderByDescending(w => w.CreatedAt)
                .Select(w => new WorkOrderResponseDto
                {
                    Id = w.Id,
                    Factory = w.Factory,
                    Line = w.Line,
                    Unit = w.Unit,
                    Buyer = w.Buyer,
                    BuyerDepartment = w.BuyerDepartment,
                    StyleName = w.StyleName,
                    FastReactNo = w.FastReactNo,
                    Color = w.Color,
                    WorkOrderNo = w.WorkOrderNo,
                    WashType = w.WashType,
                    OrderQuantity = w.OrderQuantity,
                    CutQty = w.CutQty,
                    TOD = w.TOD,
                    SewingCompDate = w.SewingCompDate,
                    FirstRCVDate = w.FirstRCVDate,
                    WashApprovalDate = w.WashApprovalDate,
                    WashTargetDate = w.WashTargetDate,
                    TotalWashReceived = w.TotalWashReceived,
                    TotalWashDelivery = w.TotalWashDelivery,
                    WashBalance = w.WashBalance,
                    FromReceived = w.FromReceived,
                    Marks = w.Marks,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt,
                    CreatedByUsername = w.CreatedByUser.Username,
                    UpdatedByUsername = w.UpdatedByUser != null ? w.UpdatedByUser.Username : null
                })
                .ToListAsync();
        }

        // ==========================================
        // BULK UPLOAD FROM EXCEL
        // ==========================================
        public async Task<WorkOrderBulkUploadResponseDto> BulkUploadFromExcelAsync(IFormFile file, int userId)
        {
            var response = new WorkOrderBulkUploadResponseDto { Success = false };

            try
            {
                if (file == null || file.Length == 0)
                {
                    response.Message = "No file uploaded";
                    return response;
                }

                if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
                {
                    response.Message = "Invalid file format. Please upload Excel file (.xlsx or .xls)";
                    return response;
                }

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension?.Rows ?? 0;

                if (rowCount < 2)
                {
                    response.Message = "Excel file has no data rows";
                    return response;
                }

                // Calculate actual data rows (skip empty rows)
                int actualDataRows = 0;
                for (int row = 2; row <= rowCount; row++)
                {
                    var workOrderNo = worksheet.Cells[row, 9].Value?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(workOrderNo))
                    {
                        actualDataRows++;
                    }
                }

                response.TotalRecords = actualDataRows;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        // Read Work Order No
                        var workOrderNo = worksheet.Cells[row, 9].Value?.ToString()?.Trim();

                        // Skip empty rows
                        if (string.IsNullOrEmpty(workOrderNo))
                        {
                            continue; // Don't count as failed
                        }

                        // Check if WorkOrder already exists
                        var existingWorkOrder = await _context.WorkOrders
                            .FirstOrDefaultAsync(w => w.WorkOrderNo == workOrderNo);

                        var workOrder = existingWorkOrder ?? new WorkOrder();

                        // Map Excel columns to WorkOrder properties
                        workOrder.Factory = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? "";
                        workOrder.Line = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? "";
                        workOrder.Unit = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? "";
                        workOrder.Buyer = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? "";
                        workOrder.BuyerDepartment = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? "";
                        workOrder.StyleName = worksheet.Cells[row, 6].Value?.ToString()?.Trim() ?? "";
                        workOrder.FastReactNo = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? "";
                        workOrder.Color = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? "";
                        workOrder.WorkOrderNo = workOrderNo;
                        workOrder.WashType = worksheet.Cells[row, 10].Value?.ToString()?.Trim() ?? "";

                        // Parse quantities
                        workOrder.OrderQuantity = ParseInt(worksheet.Cells[row, 11].Value?.ToString());
                        workOrder.CutQty = ParseInt(worksheet.Cells[row, 12].Value?.ToString());

                        // Parse dates
                        workOrder.TOD = ParseDate(worksheet.Cells[row, 13].Value);
                        workOrder.SewingCompDate = ParseDate(worksheet.Cells[row, 14].Value);
                        workOrder.FirstRCVDate = ParseDate(worksheet.Cells[row, 15].Value);
                        workOrder.WashApprovalDate = ParseDate(worksheet.Cells[row, 16].Value);
                        workOrder.WashTargetDate = ParseDate(worksheet.Cells[row, 17].Value);

                        // Parse wash quantities
                        workOrder.TotalWashReceived = ParseInt(worksheet.Cells[row, 18].Value?.ToString());
                        workOrder.TotalWashDelivery = ParseInt(worksheet.Cells[row, 19].Value?.ToString());

                        // ✅ UPDATED: Column 20 might contain "Wash Balance" or combined data
                        // Try to parse it, if fails, calculate from Received - Delivery
                        var washBalanceStr = worksheet.Cells[row, 20].Value?.ToString()?.Trim();
                        workOrder.WashBalance = ParseInt(washBalanceStr);

                        // If WashBalance is 0 and we have data, calculate it
                        if (workOrder.WashBalance == 0 && (workOrder.TotalWashReceived > 0 || workOrder.TotalWashDelivery > 0))
                        {
                            workOrder.WashBalance = workOrder.TotalWashReceived - workOrder.TotalWashDelivery;
                        }

                        // ✅ UPDATED: FromReceived and Marks might be in column 20 or 21
                        // Check if column 21 has data
                        var col21Value = worksheet.Cells[row, 21].Value?.ToString()?.Trim();

                        // If column 21 looks like a number, it's FromReceived
                        if (!string.IsNullOrEmpty(col21Value) && int.TryParse(col21Value.Replace(",", ""), out _))
                        {
                            workOrder.FromReceived = ParseInt(col21Value);
                            workOrder.Marks = worksheet.Cells[row, 22].Value?.ToString()?.Trim();
                        }
                        else
                        {
                            // Column 21 is Marks
                            workOrder.FromReceived = 0;
                            workOrder.Marks = col21Value;
                        }

                        if (existingWorkOrder != null)
                        {
                            // Update existing record
                            workOrder.UpdatedBy = userId;
                            workOrder.UpdatedAt = DateTime.UtcNow;
                            response.UpdatedCount++;
                        }
                        else
                        {
                            // Create new record
                            workOrder.CreatedBy = userId;
                            workOrder.CreatedAt = DateTime.UtcNow;
                            _context.WorkOrders.Add(workOrder);
                            response.SuccessCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        var workOrderNo = worksheet.Cells[row, 9].Value?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(workOrderNo)) // Only log error if row has Work Order No
                        {
                            response.FailedCount++;
                            response.Errors.Add($"Row {row} (WO: {workOrderNo}): {ex.Message}");
                        }
                    }
                }

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = $"Bulk upload completed. Success: {response.SuccessCount}, Updated: {response.UpdatedCount}, Failed: {response.FailedCount}";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error processing file: {ex.Message}";
            }

            return response;
        }

        // ==========================================
        // HELPER METHODS
        // ==========================================

        // Parse integer with comma removal
        private int ParseInt(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            // Remove commas and spaces
            value = value.Replace(",", "").Replace(" ", "").Trim();

            if (int.TryParse(value, out int result))
                return result;

            return 0;
        }

        // Parse date from various formats
        private DateTime? ParseDate(object? value)
        {
            if (value == null)
                return null;

            // If already DateTime
            if (value is DateTime dateTime)
                return dateTime;

            // Try parse string
            string? dateString = value.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            // Try common date formats
            string[] formats =
            {
                "dd-MMM-yy",      // 30-Oct-25
                "dd-MMM-yyyy",    // 30-Oct-2025
                "dd/MM/yyyy",     // 30/10/2025
                "yyyy-MM-dd",     // 2025-10-30
                "dd-MM-yyyy",     // 30-10-2025
                "MM/dd/yyyy"      // 10/30/2025
            };

            if (DateTime.TryParseExact(dateString, formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate))
            {
                return parsedDate;
            }

            // Try general parse
            if (DateTime.TryParse(dateString, out DateTime generalDate))
            {
                return generalDate;
            }

            return null;
        }
    }
}