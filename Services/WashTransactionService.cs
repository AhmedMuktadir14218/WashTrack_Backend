using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using wsahRecieveDelivary.Data;
using wsahRecieveDelivary.DTOs;
using wsahRecieveDelivary.Extensions;
using wsahRecieveDelivary.Models;
using wsahRecieveDelivary.Models.Enums;

namespace wsahRecieveDelivary.Services
{
    public class WashTransactionService : IWashTransactionService
    {
        private readonly ApplicationDbContext _context;

        public WashTransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ HELPER: Centralized Bangladeshi Time Property

        // ==========================================
        // HELPER: Get Bangladesh Time
        // ==========================================
        private DateTime GetBdTime()
        {
            // ✅ Convert UTC to Bangladesh time (UTC+6)
            TimeZoneInfo bdTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, bdTimeZone);
        }

        // ==========================================
        // HELPER: Convert Transaction Date to Bangladesh Date Only
        // ==========================================
        private DateTime GetBdTransactionDate(DateTime? dtoDate)
        {
            DateTime bdNow = GetBdTime();

            if (dtoDate.HasValue)
            {
                // ✅ User sent a specific date - use that date, but in Bangladesh timezone
                // Convert DTO date (assumed to be in local timezone) to date-only, then combine with BD current time
                var dateOnly = dtoDate.Value.Date;
                var bdDateOnly = bdNow.Date; // Get today's date in BD timezone

                // ✅ If user sent today's date, use BD now
                if (dateOnly == bdDateOnly)
                {
                    return bdNow;
                }

                // ✅ If user sent a past date, use that date with BD current time
                return dateOnly.Add(bdNow.TimeOfDay);
            }

            // ✅ If no date provided, use current BD time
            return bdNow;
        }
        private DateTime BdTime => DateTime.UtcNow.AddHours(6);

        // ==========================================
        // CREATE RECEIVE
        // ==========================================
        // ==========================================
        // CREATE RECEIVE (FIXED)
        // ==========================================
        public async Task<WashTransactionResponseDto> CreateReceiveAsync(CreateWashTransactionDto dto, int userId)
        {
            try
            {
                Console.WriteLine("➕ CreateReceiveAsync called");

                var workOrder = await _context.WorkOrders.FindAsync(dto.WorkOrderId);
                if (workOrder == null)
                    throw new KeyNotFoundException($"WorkOrder with ID {dto.WorkOrderId} not found");

                var processStage = await _context.ProcessStages.FindAsync(dto.ProcessStageId);
                if (processStage == null)
                    throw new KeyNotFoundException($"ProcessStage with ID {dto.ProcessStageId} not found");

                // ✅ FIXED: Get consistent Bangladesh time
                DateTime bdTime = GetBdTime();
                Console.WriteLine($"   BD Time: {bdTime:yyyy-MM-dd HH:mm:ss.fffffff}");

                // ✅ FIXED: Get transaction date
                DateTime transactionDate = GetBdTransactionDate(dto.TransactionDate);
                Console.WriteLine($"   Transaction Date: {transactionDate:yyyy-MM-dd HH:mm:ss.fffffff}");
                Console.WriteLine($"   DTO Transaction Date: {dto.TransactionDate:yyyy-MM-dd HH:mm:ss}");

                var transaction = new WashTransaction
                {
                    WorkOrderId = dto.WorkOrderId,
                    TransactionType = TransactionType.Receive,
                    ProcessStageId = dto.ProcessStageId,
                    Quantity = dto.Quantity,
                    // ✅ FIXED: Both use same Bangladesh time
                    TransactionDate = transactionDate,
                    BatchNo = dto.BatchNo,
                    GatePassNo = dto.GatePassNo,
                    Remarks = dto.Remarks,
                    ReceivedBy = dto.ReceivedBy,
                    DeliveredTo = dto.DeliveredTo,
                    CreatedBy = userId,
                    CreatedAt = bdTime, // ✅ FIXED: Same time as TransactionDate (or very close)
                    IsActive = true
                };

                _context.WashTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Receive transaction created");
                Console.WriteLine($"   Transaction Date: {transaction.TransactionDate}");
                Console.WriteLine($"   Created At: {transaction.CreatedAt}");

                await UpdateStageBalanceAsync(dto.WorkOrderId, dto.ProcessStageId);

                return await GetByIdAsync(transaction.Id)
                    ?? throw new Exception("Failed to retrieve created transaction");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CreateReceive error: {ex.Message}");
                throw;
            }
        }

        // ==========================================
        // CREATE DELIVERY
        // ==========================================
        // ==========================================
        // CREATE DELIVERY (FIXED)
        // ==========================================
        public async Task<WashTransactionResponseDto> CreateDeliveryAsync(CreateWashTransactionDto dto, int userId)
        {
            try
            {
                Console.WriteLine("➕ CreateDeliveryAsync called");

                var workOrder = await _context.WorkOrders.FindAsync(dto.WorkOrderId);
                if (workOrder == null)
                    throw new KeyNotFoundException($"WorkOrder with ID {dto.WorkOrderId} not found");

                var processStage = await _context.ProcessStages.FindAsync(dto.ProcessStageId);
                if (processStage == null)
                    throw new KeyNotFoundException($"ProcessStage with ID {dto.ProcessStageId} not found");

                // ✅ FIXED: Get consistent Bangladesh time
                DateTime bdTime = GetBdTime();
                Console.WriteLine($"   BD Time: {bdTime:yyyy-MM-dd HH:mm:ss.fffffff}");

                // ✅ FIXED: Get transaction date
                DateTime transactionDate = GetBdTransactionDate(dto.TransactionDate);
                Console.WriteLine($"   Transaction Date: {transactionDate:yyyy-MM-dd HH:mm:ss.fffffff}");
                Console.WriteLine($"   DTO Transaction Date: {dto.TransactionDate:yyyy-MM-dd HH:mm:ss}");

                var transaction = new WashTransaction
                {
                    WorkOrderId = dto.WorkOrderId,
                    TransactionType = TransactionType.Delivery,
                    ProcessStageId = dto.ProcessStageId,
                    Quantity = dto.Quantity,
                    // ✅ FIXED: Both use same Bangladesh time
                    TransactionDate = transactionDate,
                    BatchNo = dto.BatchNo,
                    GatePassNo = dto.GatePassNo,
                    Remarks = dto.Remarks,
                    ReceivedBy = dto.ReceivedBy,
                    DeliveredTo = dto.DeliveredTo,
                    CreatedBy = userId,
                    CreatedAt = bdTime, // ✅ FIXED: Same time as TransactionDate (or very close)
                    IsActive = true
                };

                _context.WashTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Delivery transaction created");
                Console.WriteLine($"   Transaction Date: {transaction.TransactionDate}");
                Console.WriteLine($"   Created At: {transaction.CreatedAt}");

                await UpdateStageBalanceAsync(dto.WorkOrderId, dto.ProcessStageId);

                return await GetByIdAsync(transaction.Id)
                    ?? throw new Exception("Failed to retrieve created transaction");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CreateDelivery error: {ex.Message}");
                throw;
            }
        }

        // ==========================================
        // UPDATE
        // ==========================================
        // ==========================================
        // UPDATE (FIXED)
        // ==========================================
        public async Task<WashTransactionResponseDto> UpdateAsync(int id, CreateWashTransactionDto dto, int userId)
        {
            try
            {
                Console.WriteLine($"✏️ UpdateAsync called for transaction {id}");

                var transaction = await _context.WashTransactions.FindAsync(id);
                if (transaction == null || !transaction.IsActive)
                    throw new KeyNotFoundException($"Transaction with ID {id} not found");

                var oldStageId = transaction.ProcessStageId;

                // ✅ FIXED: Get consistent Bangladesh time
                DateTime bdTime = GetBdTime();
                Console.WriteLine($"   BD Time: {bdTime:yyyy-MM-dd HH:mm:ss.fffffff}");

                // ✅ FIXED: Update transaction date only if provided
                if (dto.TransactionDate.HasValue)
                {
                    DateTime newTransactionDate = GetBdTransactionDate(dto.TransactionDate);
                    Console.WriteLine($"   Updated Transaction Date: {newTransactionDate:yyyy-MM-dd HH:mm:ss.fffffff}");
                    transaction.TransactionDate = newTransactionDate;
                }

                // Update fields
                transaction.TransactionType = dto.TransactionType;
                transaction.ProcessStageId = dto.ProcessStageId;
                transaction.Quantity = dto.Quantity;
                transaction.BatchNo = dto.BatchNo;
                transaction.GatePassNo = dto.GatePassNo;
                transaction.Remarks = dto.Remarks;
                transaction.ReceivedBy = dto.ReceivedBy;
                transaction.DeliveredTo = dto.DeliveredTo;
                transaction.UpdatedBy = userId;
                transaction.UpdatedAt = bdTime; // ✅ FIXED: Use BD time

                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Transaction updated");
                Console.WriteLine($"   Transaction Date: {transaction.TransactionDate}");
                Console.WriteLine($"   Updated At: {transaction.UpdatedAt}");

                // Update balances
                if (oldStageId != transaction.ProcessStageId)
                {
                    await UpdateStageBalanceAsync(transaction.WorkOrderId, oldStageId);
                    await UpdateStageBalanceAsync(transaction.WorkOrderId, transaction.ProcessStageId);
                }
                else
                {
                    await UpdateStageBalanceAsync(transaction.WorkOrderId, oldStageId);
                }

                return await GetByIdAsync(transaction.Id)
                    ?? throw new Exception("Failed to retrieve updated transaction");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ UpdateAsync error: {ex.Message}");
                throw;
            }
        }
        // ==========================================
        // DELETE (SOFT DELETE)
        // ==========================================
        // ==========================================
        // DELETE (SOFT DELETE) - FIXED
        // ==========================================
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                Console.WriteLine($"🗑️ DeleteAsync called for transaction {id}");

                var transaction = await _context.WashTransactions.FindAsync(id);
                if (transaction == null || !transaction.IsActive)
                    return false;

                transaction.IsActive = false;
                transaction.UpdatedAt = GetBdTime(); // ✅ FIXED: Use GetBdTime()

                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Transaction soft deleted");

                await UpdateStageBalanceAsync(transaction.WorkOrderId, transaction.ProcessStageId);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DeleteAsync error: {ex.Message}");
                throw;
            }
        }

        // ==========================================
        // PRIVATE HELPER: UPDATE BALANCE
        // ==========================================
        // ==========================================
        // PRIVATE HELPER: UPDATE BALANCE
        // ==========================================
        private async Task UpdateStageBalanceAsync(int workOrderId, int processStageId)
        {
            try
            {
                var balance = await _context.ProcessStageBalances
                    .FirstOrDefaultAsync(b => b.WorkOrderId == workOrderId && b.ProcessStageId == processStageId);

                if (balance == null)
                {
                    balance = new ProcessStageBalance
                    {
                        WorkOrderId = workOrderId,
                        ProcessStageId = processStageId
                    };
                    _context.ProcessStageBalances.Add(balance);
                }

                var transactions = await _context.WashTransactions
                    .Where(t => t.WorkOrderId == workOrderId && t.ProcessStageId == processStageId && t.IsActive)
                    .ToListAsync();

                balance.TotalReceived = transactions
                    .Where(t => t.TransactionType == TransactionType.Receive)
                    .Sum(t => t.Quantity);

                balance.TotalDelivered = transactions
                    .Where(t => t.TransactionType == TransactionType.Delivery)
                    .Sum(t => t.Quantity);

                balance.CurrentBalance = balance.TotalReceived - balance.TotalDelivered;

                balance.LastReceiveDate = transactions
                    .Where(t => t.TransactionType == TransactionType.Receive)
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(t => (DateTime?)t.TransactionDate)
                    .FirstOrDefault();

                balance.LastDeliveryDate = transactions
                    .Where(t => t.TransactionType == TransactionType.Delivery)
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(t => (DateTime?)t.TransactionDate)
                    .FirstOrDefault();

                // ✅ FIXED: Use GetBdTime()
                balance.LastUpdated = GetBdTime();

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ UpdateStageBalanceAsync error: {ex.Message}");
                throw;
            }
        }

        // ==========================================
        // GET BY ID
        // ==========================================
        public async Task<WashTransactionResponseDto?> GetByIdAsync(int id)
        {
            return await _context.WashTransactions
                .Include(t => t.WorkOrder)
                .Include(t => t.ProcessStage)
                .Include(t => t.CreatedByUser)
                .Include(t => t.UpdatedByUser)
                .Where(t => t.Id == id && t.IsActive)
                .Select(t => new WashTransactionResponseDto
                {
                    Id = t.Id,
                    WorkOrderId = t.WorkOrderId,
                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
                    StyleName = t.WorkOrder.StyleName,
                    Buyer = t.WorkOrder.Buyer,
                    Factory = t.WorkOrder.Factory,
                    Line = t.WorkOrder.Line,
                    TransactionType = t.TransactionType,
                    TransactionTypeName = t.TransactionType.ToString(),
                    ProcessStageId = t.ProcessStageId,
                    ProcessStageName = t.ProcessStage.Name,
                    Quantity = t.Quantity,
                    TransactionDate = t.TransactionDate,
                    BatchNo = t.BatchNo,
                    GatePassNo = t.GatePassNo,
                    Remarks = t.Remarks,
                    ReceivedBy = t.ReceivedBy,
                    DeliveredTo = t.DeliveredTo,
                    CreatedBy = t.CreatedBy,
                    CreatedByUsername = t.CreatedByUser.Username,
                    CreatedAt = t.CreatedAt,
                    UpdatedByUsername = t.UpdatedByUser != null ? t.UpdatedByUser.Username : null,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        // ==========================================
        // GET ALL
        // ==========================================
        public async Task<List<WashTransactionResponseDto>> GetAllAsync()
        {
            return await _context.WashTransactions
                .Include(t => t.WorkOrder)
                .Include(t => t.ProcessStage)
                .Include(t => t.CreatedByUser)
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new WashTransactionResponseDto
                {
                    Id = t.Id,
                    WorkOrderId = t.WorkOrderId,
                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
                    StyleName = t.WorkOrder.StyleName,
                    Buyer = t.WorkOrder.Buyer,
                    Factory = t.WorkOrder.Factory,
                    Line = t.WorkOrder.Line,
                    TransactionType = t.TransactionType,
                    TransactionTypeName = t.TransactionType.ToString(),
                    ProcessStageId = t.ProcessStageId,
                    ProcessStageName = t.ProcessStage.Name,
                    Quantity = t.Quantity,
                    TransactionDate = t.TransactionDate,
                    BatchNo = t.BatchNo,
                    GatePassNo = t.GatePassNo,
                    Remarks = t.Remarks,
                    ReceivedBy = t.ReceivedBy,
                    DeliveredTo = t.DeliveredTo,
                    CreatedBy = t.CreatedBy,
                    CreatedByUsername = t.CreatedByUser.Username,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        // ==========================================
        // GET BY WORK ORDER
        // ==========================================
        public async Task<List<WashTransactionResponseDto>> GetByWorkOrderAsync(int workOrderId)
        {
            return await _context.WashTransactions
                .Include(t => t.WorkOrder)
                .Include(t => t.ProcessStage)
                .Include(t => t.CreatedByUser)
                .Where(t => t.WorkOrderId == workOrderId && t.IsActive)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new WashTransactionResponseDto
                {
                    Id = t.Id,
                    WorkOrderId = t.WorkOrderId,
                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
                    StyleName = t.WorkOrder.StyleName,
                    Buyer = t.WorkOrder.Buyer,
                    Factory = t.WorkOrder.Factory,
                    Line = t.WorkOrder.Line,
                    TransactionType = t.TransactionType,
                    TransactionTypeName = t.TransactionType.ToString(),
                    ProcessStageId = t.ProcessStageId,
                    ProcessStageName = t.ProcessStage.Name,
                    Quantity = t.Quantity,
                    TransactionDate = t.TransactionDate,
                    BatchNo = t.BatchNo,
                    GatePassNo = t.GatePassNo,
                    Remarks = t.Remarks,
                    ReceivedBy = t.ReceivedBy,
                    DeliveredTo = t.DeliveredTo,
                    CreatedBy = t.CreatedBy,
                    CreatedByUsername = t.CreatedByUser.Username,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        // ==========================================
        // GET BY STAGE
        // ==========================================
        public async Task<List<WashTransactionResponseDto>> GetByStageAsync(int processStageId)
        {
            return await _context.WashTransactions
                .Include(t => t.WorkOrder)
                .Include(t => t.ProcessStage)
                .Include(t => t.CreatedByUser)
                .Where(t => t.ProcessStageId == processStageId && t.IsActive)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new WashTransactionResponseDto
                {
                    Id = t.Id,
                    WorkOrderId = t.WorkOrderId,
                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
                    StyleName = t.WorkOrder.StyleName,
                    Buyer = t.WorkOrder.Buyer,
                    Factory = t.WorkOrder.Factory,
                    Line = t.WorkOrder.Line,
                    TransactionType = t.TransactionType,
                    TransactionTypeName = t.TransactionType.ToString(),
                    ProcessStageId = t.ProcessStageId,
                    ProcessStageName = t.ProcessStage.Name,
                    Quantity = t.Quantity,
                    TransactionDate = t.TransactionDate,
                    BatchNo = t.BatchNo,
                    GatePassNo = t.GatePassNo,
                    Remarks = t.Remarks,
                    ReceivedBy = t.ReceivedBy,
                    DeliveredTo = t.DeliveredTo,
                    CreatedBy = t.CreatedBy,
                    CreatedByUsername = t.CreatedByUser.Username,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        // ==========================================
        // GET BY FILTER
        // ==========================================
        public async Task<List<WashTransactionResponseDto>> GetByFilterAsync(WashTransactionFilterDto filter)
        {
            var query = _context.WashTransactions
                .Include(t => t.WorkOrder)
                .Include(t => t.ProcessStage)
                .Include(t => t.CreatedByUser)
                .Where(t => t.IsActive);

            if (filter.WorkOrderId.HasValue)
                query = query.Where(t => t.WorkOrderId == filter.WorkOrderId.Value);

            if (filter.TransactionType.HasValue)
                query = query.Where(t => t.TransactionType == filter.TransactionType.Value);

            if (filter.ProcessStageId.HasValue)
                query = query.Where(t => t.ProcessStageId == filter.ProcessStageId.Value);

            if (filter.StartDate.HasValue)
                query = query.Where(t => t.TransactionDate.Date >= filter.StartDate.Value.Date);

            if (filter.EndDate.HasValue)
                query = query.Where(t => t.TransactionDate.Date <= filter.EndDate.Value.Date);

            if (!string.IsNullOrEmpty(filter.BatchNo))
                query = query.Where(t => t.BatchNo == filter.BatchNo);

            return await query
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new WashTransactionResponseDto
                {
                    Id = t.Id,
                    WorkOrderId = t.WorkOrderId,
                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
                    StyleName = t.WorkOrder.StyleName,
                    Buyer = t.WorkOrder.Buyer,
                    Factory = t.WorkOrder.Factory,
                    Line = t.WorkOrder.Line,
                    TransactionType = t.TransactionType,
                    TransactionTypeName = t.TransactionType.ToString(),
                    ProcessStageId = t.ProcessStageId,
                    ProcessStageName = t.ProcessStage.Name,
                    Quantity = t.Quantity,
                    TransactionDate = t.TransactionDate,
                    BatchNo = t.BatchNo,
                    GatePassNo = t.GatePassNo,
                    Remarks = t.Remarks,
                    ReceivedBy = t.ReceivedBy,
                    DeliveredTo = t.DeliveredTo,
                    CreatedBy = t.CreatedBy,
                    CreatedByUsername = t.CreatedByUser.Username,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        // ==========================================
        // GET BALANCES BY WORK ORDER
        // ==========================================
        public async Task<List<ProcessBalanceDto>> GetBalancesByWorkOrderAsync(int workOrderId)
        {
            var workOrder = await _context.WorkOrders.FindAsync(workOrderId);
            if (workOrder == null)
                throw new KeyNotFoundException($"WorkOrder with ID {workOrderId} not found");

            var balances = await _context.ProcessStageBalances
                .Include(b => b.ProcessStage)
                .Where(b => b.WorkOrderId == workOrderId)
                .ToListAsync();

            return balances.Select(b => new ProcessBalanceDto
            {
                WorkOrderId = workOrder.Id,
                WorkOrderNo = workOrder.WorkOrderNo,
                StyleName = workOrder.StyleName,
                ProcessStageId = b.ProcessStageId,
                ProcessStageName = b.ProcessStage.Name,
                TotalReceived = b.TotalReceived,
                TotalDelivered = b.TotalDelivered,
                CurrentBalance = b.CurrentBalance,
                LastReceiveDate = b.LastReceiveDate,
                LastDeliveryDate = b.LastDeliveryDate
            }).ToList();
        }

        // ==========================================
        // GET WASH STATUS
        // ==========================================
        public async Task<WorkOrderWashStatusDto?> GetWashStatusAsync(int workOrderId)
        {
            var workOrder = await _context.WorkOrders.FindAsync(workOrderId);
            if (workOrder == null) return null;

            var balances = await _context.ProcessStageBalances
                .Include(b => b.ProcessStage)
                .Where(b => b.WorkOrderId == workOrderId)
                .OrderBy(b => b.ProcessStage.DisplayOrder)
                .ToListAsync();

            var result = new WorkOrderWashStatusDto
            {
                WorkOrderId = workOrder.Id,
                WorkOrderNo = workOrder.WorkOrderNo,
                StyleName = workOrder.StyleName,
                Buyer = workOrder.Buyer,
                Factory = workOrder.Factory,
                Line = workOrder.Line,
                WashType = workOrder.WashType,
                OrderQuantity = workOrder.OrderQuantity,
                StageBalances = new Dictionary<string, ProcessBalanceDto>()
            };

            foreach (var balance in balances)
            {
                var balanceDto = new ProcessBalanceDto
                {
                    WorkOrderId = workOrder.Id,
                    WorkOrderNo = workOrder.WorkOrderNo,
                    StyleName = workOrder.StyleName,
                    ProcessStageId = balance.ProcessStageId,
                    ProcessStageName = balance.ProcessStage.Name,
                    TotalReceived = balance.TotalReceived,
                    TotalDelivered = balance.TotalDelivered,
                    CurrentBalance = balance.CurrentBalance,
                    LastReceiveDate = balance.LastReceiveDate,
                    LastDeliveryDate = balance.LastDeliveryDate
                };

                result.StageBalances[balance.ProcessStage.Name] = balanceDto;
            }

            result.TotalReceived = balances.Sum(b => b.TotalReceived);
            result.TotalDelivered = balances.Sum(b => b.TotalDelivered);
            result.OverallBalance = balances.Sum(b => b.CurrentBalance);

            if (workOrder.OrderQuantity > 0)
            {
                result.CompletionPercentage = Math.Round((decimal)result.TotalDelivered / workOrder.OrderQuantity * 100, 2);
            }

            return result;
        }

        // ==========================================
        // GET ALL WASH STATUSES (OPTIMIZED - No N+1)
        // ==========================================
        public async Task<List<WorkOrderWashStatusDto>> GetAllWashStatusesAsync()
        {
            var workOrders = await _context.WorkOrders.ToListAsync();

            // Get all balances with related data in ONE query
            var allBalances = await _context.ProcessStageBalances
                .Include(b => b.ProcessStage)
                .ToListAsync();

            var results = new List<WorkOrderWashStatusDto>();

            foreach (var workOrder in workOrders)
            {
                // Filter balances for this workorder (in-memory operation)
                var workOrderBalances = allBalances
                    .Where(b => b.WorkOrderId == workOrder.Id)
                    .OrderBy(b => b.ProcessStage.DisplayOrder)
                    .ToList();

                var result = new WorkOrderWashStatusDto
                {
                    WorkOrderId = workOrder.Id,
                    WorkOrderNo = workOrder.WorkOrderNo,
                    StyleName = workOrder.StyleName,
                    Buyer = workOrder.Buyer,
                    Factory = workOrder.Factory,
                    Line = workOrder.Line,
                    WashType = workOrder.WashType,
                    OrderQuantity = workOrder.OrderQuantity,
                    StageBalances = new Dictionary<string, ProcessBalanceDto>()
                };

                // Build stage balances dictionary
                foreach (var balance in workOrderBalances)
                {
                    var balanceDto = new ProcessBalanceDto
                    {
                        WorkOrderId = workOrder.Id,
                        WorkOrderNo = workOrder.WorkOrderNo,
                        StyleName = workOrder.StyleName,
                        ProcessStageId = balance.ProcessStageId,
                        ProcessStageName = balance.ProcessStage.Name,
                        TotalReceived = balance.TotalReceived,
                        TotalDelivered = balance.TotalDelivered,
                        CurrentBalance = balance.CurrentBalance,
                        LastReceiveDate = balance.LastReceiveDate,
                        LastDeliveryDate = balance.LastDeliveryDate
                    };

                    result.StageBalances[balance.ProcessStage.Name] = balanceDto;
                }

                // Calculate overall summary
                result.TotalReceived = workOrderBalances.Sum(b => b.TotalReceived);
                result.TotalDelivered = workOrderBalances.Sum(b => b.TotalDelivered);
                result.OverallBalance = workOrderBalances.Sum(b => b.CurrentBalance);

                if (workOrder.OrderQuantity > 0)
                {
                    result.CompletionPercentage = Math.Round(
                        (decimal)result.TotalDelivered / workOrder.OrderQuantity * 100,
                        2);
                }

                results.Add(result);
            }

            return results;
        }

        // ==========================================
        // GET STAGE SUMMARY
        // ==========================================
        public async Task<List<ProcessStageSummaryDto>> GetStageSummaryAsync()
        {
            var allStages = await _context.ProcessStages
                .Where(ps => ps.IsActive)
                .OrderBy(ps => ps.DisplayOrder)
                .ToListAsync();

            var summaries = new List<ProcessStageSummaryDto>();

            foreach (var stage in allStages)
            {
                var transactions = await _context.WashTransactions
                    .Where(t => t.ProcessStageId == stage.Id && t.IsActive)
                    .ToListAsync();

                var summary = new ProcessStageSummaryDto
                {
                    ProcessStageId = stage.Id,
                    ProcessStageName = stage.Name,
                    TotalReceiveCount = transactions.Count(t => t.TransactionType == TransactionType.Receive),
                    TotalDeliveryCount = transactions.Count(t => t.TransactionType == TransactionType.Delivery),
                    TotalReceivedQty = transactions.Where(t => t.TransactionType == TransactionType.Receive).Sum(t => t.Quantity),
                    TotalDeliveredQty = transactions.Where(t => t.TransactionType == TransactionType.Delivery).Sum(t => t.Quantity),
                    CurrentBalance = transactions.Where(t => t.TransactionType == TransactionType.Receive).Sum(t => t.Quantity) -
                                   transactions.Where(t => t.TransactionType == TransactionType.Delivery).Sum(t => t.Quantity)
                };

                summaries.Add(summary);
            }

            return summaries;
        }

        // ==========================================
        // GET RECEIVES BY STAGE
        // ==========================================
        public async Task<List<WashTransactionResponseDto>> GetReceivesByStageAsync(int processStageId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.WashTransactions
                .Include(t => t.WorkOrder)
                .Include(t => t.ProcessStage)
                .Include(t => t.CreatedByUser)
                .Where(t => t.ProcessStageId == processStageId && t.TransactionType == TransactionType.Receive && t.IsActive);

            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate.Date <= endDate.Value.Date);

            return await query
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new WashTransactionResponseDto
                {
                    Id = t.Id,
                    WorkOrderId = t.WorkOrderId,
                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
                    StyleName = t.WorkOrder.StyleName,
                    Buyer = t.WorkOrder.Buyer,
                    Factory = t.WorkOrder.Factory,
                    Line = t.WorkOrder.Line,
                    TransactionType = t.TransactionType,
                    TransactionTypeName = t.TransactionType.ToString(),
                    ProcessStageId = t.ProcessStageId,
                    ProcessStageName = t.ProcessStage.Name,
                    Quantity = t.Quantity,
                    TransactionDate = t.TransactionDate,
                    BatchNo = t.BatchNo,
                    GatePassNo = t.GatePassNo,
                    Remarks = t.Remarks,
                    ReceivedBy = t.ReceivedBy,
                    DeliveredTo = t.DeliveredTo,
                    CreatedBy = t.CreatedBy,
                    CreatedByUsername = t.CreatedByUser.Username,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        // ==========================================
        // GET DELIVERIES BY STAGE
        // ==========================================
        public async Task<List<WashTransactionResponseDto>> GetDeliveriesByStageAsync(int processStageId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.WashTransactions
                .Include(t => t.WorkOrder)
                .Include(t => t.ProcessStage)
                .Include(t => t.CreatedByUser)
                .Where(t => t.ProcessStageId == processStageId && t.TransactionType == TransactionType.Delivery && t.IsActive);

            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate.Date <= endDate.Value.Date);

            return await query
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new WashTransactionResponseDto
                {
                    Id = t.Id,
                    WorkOrderId = t.WorkOrderId,
                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
                    StyleName = t.WorkOrder.StyleName,
                    Buyer = t.WorkOrder.Buyer,
                    Factory = t.WorkOrder.Factory,
                    Line = t.WorkOrder.Line,
                    TransactionType = t.TransactionType,
                    TransactionTypeName = t.TransactionType.ToString(),
                    ProcessStageId = t.ProcessStageId,
                    ProcessStageName = t.ProcessStage.Name,
                    Quantity = t.Quantity,
                    TransactionDate = t.TransactionDate,
                    BatchNo = t.BatchNo,
                    GatePassNo = t.GatePassNo,
                    Remarks = t.Remarks,
                    ReceivedBy = t.ReceivedBy,
                    DeliveredTo = t.DeliveredTo,
                    CreatedBy = t.CreatedBy,
                    CreatedByUsername = t.CreatedByUser.Username,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        // ==========================================
        // PRIVATE HELPER: GET BALANCE
        // ==========================================
        private async Task<int> GetCurrentBalanceAsync(int workOrderId, int processStageId)
        {
            var balance = await _context.ProcessStageBalances
                .FirstOrDefaultAsync(b => b.WorkOrderId == workOrderId && b.ProcessStageId == processStageId);

            return balance?.CurrentBalance ?? 0;
        }

        // ==========================================
        // GET PAGINATED WITH FAST SEARCH & FILTERS
        // ==========================================
        // ==========================================
        // GET PAGINATED WITH FAST SEARCH & FILTERS (UPDATED)
        // ==========================================
        // ==========================================
        // GET PAGINATED WITH FAST SEARCH & FILTERS (FIXED)
        // ==========================================
        public async Task<PaginatedResponseDto<WashTransactionResponseDto>> GetPaginatedAsync(
            TransactionPaginationRequestDto request)
        {
            try
            {
                Console.WriteLine("📄 GetPaginatedAsync called");
                Console.WriteLine($"   Page: {request.Page}");
                Console.WriteLine($"   PageSize: {request.PageSize}");
                Console.WriteLine($"   SearchTerm: {request.SearchTerm}");
                Console.WriteLine($"   Buyer: {request.Buyer}");
                Console.WriteLine($"   Factory: {request.Factory}");
                Console.WriteLine($"   Unit: {request.Unit}"); // ✅ ADDED
                Console.WriteLine($"   ProcessStageId: {request.ProcessStageId}");
                Console.WriteLine($"   TransactionTypeId: {request.TransactionTypeId}");
                Console.WriteLine($"   StartDate: {request.StartDate:yyyy-MM-dd}");
                Console.WriteLine($"   EndDate: {request.EndDate:yyyy-MM-dd}");
                Console.WriteLine($"   SortBy: {request.SortBy}");
                Console.WriteLine($"   SortOrder: {request.SortOrder}");

                // Build query with AsNoTracking for read-only performance
                var query = _context.WashTransactions
                    .AsNoTracking()
                    .Include(t => t.WorkOrder)
                    .Include(t => t.ProcessStage)
                    .Include(t => t.CreatedByUser)
                    .Include(t => t.UpdatedByUser)
                    .Where(t => t.IsActive)
                    .AsQueryable();

                Console.WriteLine($"📊 Initial query count (before filters): {query.Count()}");

                // Apply global search
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    Console.WriteLine($"🔍 Applying search: {request.SearchTerm}");
                    query = query.SearchTransaction(request.SearchTerm);
                    Console.WriteLine($"   After search: {query.Count()} records");
                }

                // ✅ FIXED: Apply advanced filters with all parameters
                if (!string.IsNullOrEmpty(request.Buyer) ||
                    !string.IsNullOrEmpty(request.Factory) ||
                    !string.IsNullOrEmpty(request.Unit) || // ✅ ADDED
                    request.ProcessStageId.HasValue ||
                    request.TransactionTypeId.HasValue ||
                    request.StartDate.HasValue ||
                    request.EndDate.HasValue)
                {
                    Console.WriteLine("🎛️ Applying filters...");

                    if (!string.IsNullOrEmpty(request.Buyer))
                    {
                        Console.WriteLine($"   Filter Buyer: {request.Buyer}");
                        query = query.Where(t => t.WorkOrder.Buyer.ToLower().Contains(request.Buyer.ToLower()));
                    }

                    if (!string.IsNullOrEmpty(request.Factory))
                    {
                        Console.WriteLine($"   Filter Factory: {request.Factory}");
                        query = query.Where(t => t.WorkOrder.Factory.ToLower() == request.Factory.ToLower());
                    }

                    if (!string.IsNullOrEmpty(request.Unit)) // ✅ ADDED
                    {
                        Console.WriteLine($"   Filter Unit: {request.Unit}");
                        query = query.Where(t => t.WorkOrder.Unit.ToLower() == request.Unit.ToLower());
                    }

                    if (request.ProcessStageId.HasValue)
                    {
                        Console.WriteLine($"   Filter ProcessStageId: {request.ProcessStageId}");
                        query = query.Where(t => t.ProcessStageId == request.ProcessStageId.Value);
                    }

                    if (request.TransactionTypeId.HasValue)
                    {
                        Console.WriteLine($"   Filter TransactionTypeId: {request.TransactionTypeId}");
                        query = query.Where(t => (int)t.TransactionType == request.TransactionTypeId.Value);
                    }

                    // ✅ CRITICAL: Date filtering
                    if (request.StartDate.HasValue)
                    {
                        Console.WriteLine($"   Filter StartDate: {request.StartDate:yyyy-MM-dd}");
                        var startDateOnly = request.StartDate.Value.Date;
                        query = query.Where(t => t.TransactionDate.Date >= startDateOnly);
                        Console.WriteLine($"   After StartDate filter: {query.Count()} records");
                    }

                    if (request.EndDate.HasValue)
                    {
                        Console.WriteLine($"   Filter EndDate: {request.EndDate:yyyy-MM-dd}");
                        var endDateOnly = request.EndDate.Value.Date;
                        query = query.Where(t => t.TransactionDate.Date <= endDateOnly);
                        Console.WriteLine($"   After EndDate filter: {query.Count()} records");
                    }

                    Console.WriteLine($"   After all filters: {query.Count()} records");
                }

                // Apply sorting
                Console.WriteLine($"📊 Applying sort: {request.SortBy} ({request.SortOrder})");
                query = query.ApplyTransactionSort(request.SortBy, request.SortOrder);

                // Get total count BEFORE pagination
                var totalCount = await query.CountAsync();
                Console.WriteLine($"📊 Total count after all filters: {totalCount}");

                // Calculate total pages
                var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
                Console.WriteLine($"📄 Total pages: {totalPages}");

                // Apply pagination
                var skip = (request.Page - 1) * request.PageSize;
                Console.WriteLine($"📄 Skip: {skip}, Take: {request.PageSize}");

                var data = await query
                    .Skip(skip)
                    .Take(request.PageSize)
                    .Select(t => new WashTransactionResponseDto
                    {
                        Id = t.Id,
                        WorkOrderId = t.WorkOrderId,
                        WorkOrderNo = t.WorkOrder.WorkOrderNo,
                        StyleName = t.WorkOrder.StyleName,
                        Buyer = t.WorkOrder.Buyer,
                        Factory = t.WorkOrder.Factory,
                        Line = t.WorkOrder.Line,
                        TransactionType = t.TransactionType,
                        TransactionTypeName = t.TransactionType.ToString(),
                        ProcessStageId = t.ProcessStageId,
                        ProcessStageName = t.ProcessStage.Name,
                        Quantity = t.Quantity,
                        TransactionDate = t.TransactionDate,
                        BatchNo = t.BatchNo,
                        GatePassNo = t.GatePassNo,
                        Remarks = t.Remarks,
                        ReceivedBy = t.ReceivedBy,
                        DeliveredTo = t.DeliveredTo,
                        CreatedBy = t.CreatedBy,
                        CreatedByUsername = t.CreatedByUser.Username,
                        CreatedAt = t.CreatedAt,
                        UpdatedByUsername = t.UpdatedByUser != null ? t.UpdatedByUser.Username : null,
                        UpdatedAt = t.UpdatedAt
                    })
                    .ToListAsync();

                Console.WriteLine($"✅ Loaded {data.Count} records for page {request.Page}");

                return new PaginatedResponseDto<WashTransactionResponseDto>
                {
                    Success = true,
                    Message = totalCount == 0 ? "No transactions found" : null,
                    Data = data,
                    Pagination = new PaginationMetadata
                    {
                        CurrentPage = request.Page,
                        PageSize = request.PageSize,
                        TotalRecords = totalCount,
                        TotalPages = totalPages,
                        HasPrevious = request.Page > 1,
                        HasNext = request.Page < totalPages
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GetPaginatedAsync Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");

                return new PaginatedResponseDto<WashTransactionResponseDto>
                {
                    Success = false,
                    Message = $"Error retrieving transactions: {ex.Message}",
                    Data = new List<WashTransactionResponseDto>(),
                    Pagination = new PaginationMetadata()
                };
            }
        }

        // ==========================================
        // EXPORT TO CSV WITH DATE FILTER
        // ==========================================
        /// <summary>
        /// Export transactions to CSV with all filters including date range
        /// Date Range Explanation:
        /// - startDate: Starting date (e.g., 2025-11-12)
        /// - endDate: Ending date (e.g., 2025-11-15)
        /// - Filters data from startDate to endDate (inclusive on both ends)
        /// </summary>
        // ==========================================
        // EXPORT TO CSV (SERVER-SIDE) - UPDATED
        // ==========================================
        public async Task<byte[]> ExportToCSVAsync(
            string? searchTerm = null,
            string? buyer = null,
            string? factory = null,
            string? unit = null, // ✅ ADDED
            int? processStageId = null,
            int? transactionTypeId = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                Console.WriteLine($"📥 CSV Export Request:");
                Console.WriteLine($"   SearchTerm: {searchTerm}");
                Console.WriteLine($"   Buyer: {buyer}");
                Console.WriteLine($"   Factory: {factory}");
                Console.WriteLine($"   Unit: {unit}"); // ✅ ADDED
                Console.WriteLine($"   ProcessStageId: {processStageId}");
                Console.WriteLine($"   TransactionTypeId: {transactionTypeId}");
                Console.WriteLine($"   StartDate: {startDate:yyyy-MM-dd}");
                Console.WriteLine($"   EndDate: {endDate:yyyy-MM-dd}");

                // ✅ Build query with all includes needed
                var query = _context.WashTransactions
                    .Include(t => t.WorkOrder)
                    .Include(t => t.ProcessStage)
                    .Where(t => t.IsActive)
                    .AsQueryable();

                // ✅ Apply Buyer filter
                if (!string.IsNullOrEmpty(buyer))
                {
                    Console.WriteLine($"   Applying Buyer filter: {buyer}");
                    query = query.Where(t => t.WorkOrder.Buyer.ToLower().Contains(buyer.ToLower()));
                }

                // ✅ Apply Factory filter
                if (!string.IsNullOrEmpty(factory))
                {
                    Console.WriteLine($"   Applying Factory filter: {factory}");
                    query = query.Where(t => t.WorkOrder.Factory.ToLower() == factory.ToLower());
                }

                // ✅ ADDED: Apply Unit filter
                if (!string.IsNullOrEmpty(unit))
                {
                    Console.WriteLine($"   Applying Unit filter: {unit}");
                    query = query.Where(t => t.WorkOrder.Unit.ToLower() == unit.ToLower());
                }

                // ✅ Apply ProcessStage filter
                if (processStageId.HasValue)
                {
                    Console.WriteLine($"   Applying ProcessStage filter: {processStageId}");
                    query = query.Where(t => t.ProcessStageId == processStageId.Value);
                }

                // ✅ Apply TransactionType filter
                if (transactionTypeId.HasValue)
                {
                    Console.WriteLine($"   Applying TransactionType filter: {transactionTypeId}");
                    query = query.Where(t => (int)t.TransactionType == transactionTypeId.Value);
                }

                // ✅ Apply Date Range Filter
                if (startDate.HasValue)
                {
                    Console.WriteLine($"   Applying StartDate filter: {startDate:yyyy-MM-dd}");
                    query = query.Where(t => t.TransactionDate.Date >= startDate.Value.Date);
                }

                if (endDate.HasValue)
                {
                    Console.WriteLine($"   Applying EndDate filter: {endDate:yyyy-MM-dd}");
                    query = query.Where(t => t.TransactionDate.Date <= endDate.Value.Date);
                }

                // ✅ Apply Search Term filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    Console.WriteLine($"   Applying SearchTerm filter: {searchTerm}");
                    var lowerSearchTerm = searchTerm.ToLower().Trim();
                    query = query.Where(t =>
                        t.WorkOrder.WorkOrderNo.ToLower().Contains(lowerSearchTerm) ||
                        t.WorkOrder.Buyer.ToLower().Contains(lowerSearchTerm) ||
                        t.WorkOrder.StyleName.ToLower().Contains(lowerSearchTerm) ||
                        t.WorkOrder.Factory.ToLower().Contains(lowerSearchTerm) ||
                        t.WorkOrder.Unit.ToLower().Contains(lowerSearchTerm) || // ✅ ADDED
                        t.ProcessStage.Name.ToLower().Contains(lowerSearchTerm) ||
                        (t.BatchNo != null && t.BatchNo.ToLower().Contains(lowerSearchTerm)) ||
                        (t.GatePassNo != null && t.GatePassNo.ToLower().Contains(lowerSearchTerm)) ||
                        t.Quantity.ToString().Contains(lowerSearchTerm)
                    );
                }

                // ✅ Execute query and get data
                var transactions = await query
                    .OrderByDescending(t => t.TransactionDate)
                    .ThenByDescending(t => t.CreatedAt)
                    .Select(t => new
                    {
                        Id = t.Id,
                        WorkOrderNo = t.WorkOrder.WorkOrderNo,
                        StyleName = t.WorkOrder.StyleName,
                        Buyer = t.WorkOrder.Buyer,
                        Factory = t.WorkOrder.Factory,
                        Unit = t.WorkOrder.Unit, // ✅ ADDED
                        FastReactNo = t.WorkOrder.FastReactNo ?? "-",
                        Color = t.WorkOrder.Color,
                        StageName = t.ProcessStage.Name,
                        TransactionType = t.TransactionType.ToString(),
                        Quantity = t.Quantity,
                        TransactionDate = t.TransactionDate,
                        BatchNo = t.BatchNo ?? "-",
                        GatePassNo = t.GatePassNo ?? "-",
                        Remarks = t.Remarks ?? "-",
                        ReceivedBy = t.ReceivedBy ?? "-",
                        DeliveredTo = t.DeliveredTo ?? "-",
                        CreatedAt = t.CreatedAt // ✅ CHANGED: Using CreatedAt
                    })
                    .ToListAsync();

                Console.WriteLine($"   Total records found: {transactions.Count}");

                if (transactions.Count == 0)
                {
                    Console.WriteLine($"❌ No transactions found for the given criteria");
                    throw new Exception("No transactions found matching your criteria");
                }

                Console.WriteLine($"📥 Generating CSV for {transactions.Count} transactions...");

                // ✅ Generate CSV content
                var stringBuilder = new StringBuilder();

                using (var stringWriter = new StringWriter(stringBuilder))
                {
                    using (var csv = new CsvWriter(stringWriter, CultureInfo.InvariantCulture))
                    {
                        // Write CSV Headers
                        csv.WriteField("ID");
                        csv.WriteField("Work Order No");
                        csv.WriteField("Style Name");
                        csv.WriteField("Buyer");
                        csv.WriteField("Factory");
                        csv.WriteField("Unit"); // ✅ ADDED
                        csv.WriteField("FastReact No");
                        csv.WriteField("Color");
                        csv.WriteField("Process Stage");
                        csv.WriteField("Transaction Type");
                        csv.WriteField("Quantity");
                        csv.WriteField("Transaction Date");
                        csv.WriteField("Batch No");
                        csv.WriteField("Gate Pass No");
                        csv.WriteField("Remarks");
                        csv.WriteField("Received By");
                        csv.WriteField("Delivered To");
                        csv.WriteField("Created At"); // ✅ CHANGED
                        csv.NextRecord(); // ✅ FIXED: No await

                        // Write CSV Data Rows
                        foreach (var transaction in transactions)
                        {
                            csv.WriteField(transaction.Id);
                            csv.WriteField(transaction.WorkOrderNo);
                            csv.WriteField(transaction.StyleName);
                            csv.WriteField(transaction.Buyer);
                            csv.WriteField(transaction.Factory);
                            csv.WriteField(transaction.Unit); // ✅ ADDED
                            csv.WriteField(transaction.FastReactNo);
                            csv.WriteField(transaction.Color);
                            csv.WriteField(transaction.StageName);
                            csv.WriteField(transaction.TransactionType);
                            csv.WriteField(transaction.Quantity);
                            csv.WriteField(transaction.TransactionDate.ToString("yyyy-MM-dd"));
                            csv.WriteField(transaction.BatchNo);
                            csv.WriteField(transaction.GatePassNo);
                            csv.WriteField(transaction.Remarks);
                            csv.WriteField(transaction.ReceivedBy);
                            csv.WriteField(transaction.DeliveredTo);
                            csv.WriteField(transaction.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")); // ✅ CHANGED
                            csv.NextRecord(); // ✅ FIXED: No await
                        }

                        csv.Flush();
                    }
                }

                // ✅ Convert string to bytes
                var csvContent = stringBuilder.ToString();
                var result = Encoding.UTF8.GetBytes(csvContent);

                if (result.Length == 0)
                {
                    Console.WriteLine($"❌ CSV content is empty!");
                    throw new Exception("Failed to generate CSV content");
                }

                Console.WriteLine($"✅ CSV generated successfully!");
                Console.WriteLine($"   Size: {result.Length} bytes");
                Console.WriteLine($"   Records: {transactions.Count}");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CSV Export Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new Exception($"Error exporting transactions: {ex.Message}", ex);
            }
        }
    }
}




//using CsvHelper;
//using Microsoft.EntityFrameworkCore;
//using System.Globalization;
//using System.Text;
//using wsahRecieveDelivary.Data;
//using wsahRecieveDelivary.DTOs;
//using wsahRecieveDelivary.Extensions;
//using wsahRecieveDelivary.Models;
//using wsahRecieveDelivary.Models.Enums;

//namespace wsahRecieveDelivary.Services
//{
//    public class WashTransactionService : IWashTransactionService
//    {
//        private readonly ApplicationDbContext _context;

//        public WashTransactionService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // ✅ HELPER: Centralized Bangladeshi Time Property
//        // This ensures consistency across all methods
//        private DateTime BdTime => DateTime.UtcNow.AddHours(6);

//        // ==========================================
//        // CREATE RECEIVE
//        // ==========================================
//        public async Task<WashTransactionResponseDto> CreateReceiveAsync(CreateWashTransactionDto dto, int userId)
//        {
//            var workOrder = await _context.WorkOrders.FindAsync(dto.WorkOrderId);
//            if (workOrder == null)
//                throw new KeyNotFoundException($"WorkOrder with ID {dto.WorkOrderId} not found");

//            var processStage = await _context.ProcessStages.FindAsync(dto.ProcessStageId);
//            if (processStage == null)
//                throw new KeyNotFoundException($"ProcessStage with ID {dto.ProcessStageId} not found");

//            DateTime finalTransactionDate;
//            if (dto.TransactionDate.HasValue)
//            {
//                // Take the user's date (2025-11-26) and add the current time (e.g. 14:30:00)
//                finalTransactionDate = dto.TransactionDate.Value.Date.Add(BdTime.TimeOfDay);
//            }
//            else
//            {
//                finalTransactionDate = BdTime;
//            }
//            var transaction = new WashTransaction
//            {
//                WorkOrderId = dto.WorkOrderId,
//                TransactionType = TransactionType.Receive,
//                ProcessStageId = dto.ProcessStageId,
//                Quantity = dto.Quantity,
//                // ✅ CHANGED: Uses BdTime if date is null
//                TransactionDate = finalTransactionDate,
//                BatchNo = dto.BatchNo,
//                GatePassNo = dto.GatePassNo,
//                Remarks = dto.Remarks,
//                ReceivedBy = dto.ReceivedBy,
//                DeliveredTo = dto.DeliveredTo,
//                CreatedBy = userId,
//                // ✅ CHANGED: Explicitly set CreatedAt to BdTime
//                CreatedAt = BdTime,
//                IsActive = true
//            };

//            _context.WashTransactions.Add(transaction);
//            await _context.SaveChangesAsync();

//            await UpdateStageBalanceAsync(dto.WorkOrderId, dto.ProcessStageId);

//            return await GetByIdAsync(transaction.Id)
//                ?? throw new Exception("Failed to retrieve created transaction");
//        }

//        // ==========================================
//        // CREATE DELIVERY
//        // ==========================================
//        public async Task<WashTransactionResponseDto> CreateDeliveryAsync(CreateWashTransactionDto dto, int userId)
//        {
//            var workOrder = await _context.WorkOrders.FindAsync(dto.WorkOrderId);
//            if (workOrder == null)
//                throw new KeyNotFoundException($"WorkOrder with ID {dto.WorkOrderId} not found");

//            var processStage = await _context.ProcessStages.FindAsync(dto.ProcessStageId);
//            if (processStage == null)
//                throw new KeyNotFoundException($"ProcessStage with ID {dto.ProcessStageId} not found");
//            DateTime finalTransactionDate;
//            if (dto.TransactionDate.HasValue)
//            {
//                // Take the user's date (2025-11-26) and add the current time (e.g. 14:30:00)
//                finalTransactionDate = dto.TransactionDate.Value.Date.Add(BdTime.TimeOfDay);
//            }
//            else
//            {
//                finalTransactionDate = BdTime;
//            }

//            var transaction = new WashTransaction
//            {
//                WorkOrderId = dto.WorkOrderId,
//                TransactionType = TransactionType.Delivery,
//                ProcessStageId = dto.ProcessStageId,
//                Quantity = dto.Quantity,
//                // ✅ CHANGED: Uses BdTime
//                TransactionDate = finalTransactionDate,
//                BatchNo = dto.BatchNo,
//                GatePassNo = dto.GatePassNo,
//                Remarks = dto.Remarks,
//                ReceivedBy = dto.ReceivedBy,
//                DeliveredTo = dto.DeliveredTo,
//                CreatedBy = userId,
//                // ✅ CHANGED: Uses BdTime
//                CreatedAt = BdTime,
//                IsActive = true
//            };

//            _context.WashTransactions.Add(transaction);
//            await _context.SaveChangesAsync();

//            await UpdateStageBalanceAsync(dto.WorkOrderId, dto.ProcessStageId);

//            return await GetByIdAsync(transaction.Id)
//                ?? throw new Exception("Failed to retrieve created transaction");
//        }

//        // ==========================================
//        // UPDATE
//        // ==========================================
//        public async Task<WashTransactionResponseDto> UpdateAsync(int id, CreateWashTransactionDto dto, int userId)
//        {
//            var transaction = await _context.WashTransactions.FindAsync(id);
//            if (transaction == null || !transaction.IsActive)
//                throw new KeyNotFoundException($"Transaction with ID {id} not found");

//            var oldStageId = transaction.ProcessStageId;


//            if (dto.TransactionDate.HasValue)
//            {
//                // If the user changes the date, update the date part but keep current time
//                // Or you can keep the ORIGINAL time if you prefer:
//                // transaction.TransactionDate = dto.TransactionDate.Value.Date.Add(transaction.TransactionDate.TimeOfDay);

//                // But based on your request, let's set it to Current Time + Selected Date:
//                transaction.TransactionDate = dto.TransactionDate.Value.Date.Add(BdTime.TimeOfDay);
//            }

//            // Update fields
//            transaction.TransactionType = dto.TransactionType;
//            transaction.ProcessStageId = dto.ProcessStageId;
//            transaction.Quantity = dto.Quantity;
//            transaction.TransactionDate = dto.TransactionDate ?? transaction.TransactionDate;
//            transaction.BatchNo = dto.BatchNo;
//            transaction.GatePassNo = dto.GatePassNo;
//            transaction.Remarks = dto.Remarks;
//            transaction.ReceivedBy = dto.ReceivedBy;
//            transaction.DeliveredTo = dto.DeliveredTo;
//            transaction.UpdatedBy = userId;
//            // ✅ CHANGED: Uses BdTime
//            transaction.UpdatedAt = BdTime;

//            await _context.SaveChangesAsync();

//            // Update balances
//            await UpdateStageBalanceAsync(transaction.WorkOrderId, oldStageId);
//            if (oldStageId != transaction.ProcessStageId)
//            {
//                await UpdateStageBalanceAsync(transaction.WorkOrderId, transaction.ProcessStageId);
//            }

//            return await GetByIdAsync(transaction.Id)
//                ?? throw new Exception("Failed to retrieve updated transaction");
//        }

//        // ==========================================
//        // DELETE (SOFT DELETE)
//        // ==========================================
//        public async Task<bool> DeleteAsync(int id)
//        {
//            var transaction = await _context.WashTransactions.FindAsync(id);
//            if (transaction == null || !transaction.IsActive)
//                return false;

//            transaction.IsActive = false;
//            // ✅ CHANGED: Uses BdTime
//            transaction.UpdatedAt = BdTime;

//            await _context.SaveChangesAsync();

//            await UpdateStageBalanceAsync(transaction.WorkOrderId, transaction.ProcessStageId);

//            return true;
//        }

//        // ==========================================
//        // PRIVATE HELPER: UPDATE BALANCE
//        // ==========================================
//        private async Task UpdateStageBalanceAsync(int workOrderId, int processStageId)
//        {
//            var balance = await _context.ProcessStageBalances
//                .FirstOrDefaultAsync(b => b.WorkOrderId == workOrderId && b.ProcessStageId == processStageId);

//            if (balance == null)
//            {
//                balance = new ProcessStageBalance
//                {
//                    WorkOrderId = workOrderId,
//                    ProcessStageId = processStageId
//                };
//                _context.ProcessStageBalances.Add(balance);
//            }

//            var transactions = await _context.WashTransactions
//                .Where(t => t.WorkOrderId == workOrderId && t.ProcessStageId == processStageId && t.IsActive)
//                .ToListAsync();

//            balance.TotalReceived = transactions
//                .Where(t => t.TransactionType == TransactionType.Receive)
//                .Sum(t => t.Quantity);

//            balance.TotalDelivered = transactions
//                .Where(t => t.TransactionType == TransactionType.Delivery)
//                .Sum(t => t.Quantity);

//            balance.CurrentBalance = balance.TotalReceived - balance.TotalDelivered;

//            balance.LastReceiveDate = transactions
//                .Where(t => t.TransactionType == TransactionType.Receive)
//                .OrderByDescending(t => t.TransactionDate)
//                .Select(t => (DateTime?)t.TransactionDate)
//                .FirstOrDefault();

//            balance.LastDeliveryDate = transactions
//                .Where(t => t.TransactionType == TransactionType.Delivery)
//                .OrderByDescending(t => t.TransactionDate)
//                .Select(t => (DateTime?)t.TransactionDate)
//                .FirstOrDefault();

//            // ✅ CHANGED: Uses BdTime
//            balance.LastUpdated = BdTime;

//            await _context.SaveChangesAsync();
//        }

//        // ... [KEEP ALL OTHER GET METHODS (GetById, GetAll, etc.) EXACTLY THE SAME] ...

//        // I have omitted the GET methods here to save space, 
//        // as they do not manipulate dates, they only read them.
//        // Copy-paste your existing GET methods here.
//        public async Task<WashTransactionResponseDto?> GetByIdAsync(int id)
//        {
//            return await _context.WashTransactions
//                .Include(t => t.WorkOrder)
//                .Include(t => t.ProcessStage)
//                .Include(t => t.CreatedByUser)
//                .Include(t => t.UpdatedByUser)
//                .Where(t => t.Id == id && t.IsActive)
//                .Select(t => new WashTransactionResponseDto
//                {
//                    Id = t.Id,
//                    WorkOrderId = t.WorkOrderId,
//                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
//                    StyleName = t.WorkOrder.StyleName,
//                    Buyer = t.WorkOrder.Buyer,
//                    Factory = t.WorkOrder.Factory,
//                    Line = t.WorkOrder.Line,
//                    TransactionType = t.TransactionType,
//                    TransactionTypeName = t.TransactionType.ToString(),
//                    ProcessStageId = t.ProcessStageId,
//                    ProcessStageName = t.ProcessStage.Name,
//                    Quantity = t.Quantity,
//                    TransactionDate = t.TransactionDate,
//                    BatchNo = t.BatchNo,
//                    GatePassNo = t.GatePassNo,
//                    Remarks = t.Remarks,
//                    ReceivedBy = t.ReceivedBy,
//                    DeliveredTo = t.DeliveredTo,
//                    CreatedBy = t.CreatedBy,
//                    CreatedByUsername = t.CreatedByUser.Username,
//                    CreatedAt = t.CreatedAt,
//                    UpdatedByUsername = t.UpdatedByUser != null ? t.UpdatedByUser.Username : null,
//                    UpdatedAt = t.UpdatedAt
//                })
//                .FirstOrDefaultAsync();
//        }

//        public async Task<List<WashTransactionResponseDto>> GetAllAsync()
//        {
//            return await _context.WashTransactions
//                .Include(t => t.WorkOrder)
//                .Include(t => t.ProcessStage)
//                .Include(t => t.CreatedByUser)
//                .Where(t => t.IsActive)
//                .OrderByDescending(t => t.TransactionDate)
//                .Select(t => new WashTransactionResponseDto
//                {
//                    Id = t.Id,
//                    WorkOrderId = t.WorkOrderId,
//                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
//                    StyleName = t.WorkOrder.StyleName,
//                    Buyer = t.WorkOrder.Buyer,
//                    Factory = t.WorkOrder.Factory,
//                    Line = t.WorkOrder.Line,
//                    TransactionType = t.TransactionType,
//                    TransactionTypeName = t.TransactionType.ToString(),
//                    ProcessStageId = t.ProcessStageId,
//                    ProcessStageName = t.ProcessStage.Name,
//                    Quantity = t.Quantity,
//                    TransactionDate = t.TransactionDate,
//                    BatchNo = t.BatchNo,
//                    GatePassNo = t.GatePassNo,
//                    Remarks = t.Remarks,
//                    ReceivedBy = t.ReceivedBy,
//                    DeliveredTo = t.DeliveredTo,
//                    CreatedBy = t.CreatedBy,
//                    CreatedByUsername = t.CreatedByUser.Username,
//                    CreatedAt = t.CreatedAt
//                })
//                .ToListAsync();
//        }

//        // ==========================================
//        // GET BY WORK ORDER
//        // ==========================================
//        public async Task<List<WashTransactionResponseDto>> GetByWorkOrderAsync(int workOrderId)
//        {
//            return await _context.WashTransactions
//                .Include(t => t.WorkOrder)
//                .Include(t => t.ProcessStage)
//                .Include(t => t.CreatedByUser)
//                .Where(t => t.WorkOrderId == workOrderId && t.IsActive)
//                .OrderByDescending(t => t.TransactionDate)
//                .Select(t => new WashTransactionResponseDto
//                {
//                    Id = t.Id,
//                    WorkOrderId = t.WorkOrderId,
//                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
//                    StyleName = t.WorkOrder.StyleName,
//                    Buyer = t.WorkOrder.Buyer,
//                    Factory = t.WorkOrder.Factory,
//                    Line = t.WorkOrder.Line,
//                    TransactionType = t.TransactionType,
//                    TransactionTypeName = t.TransactionType.ToString(),
//                    ProcessStageId = t.ProcessStageId,
//                    ProcessStageName = t.ProcessStage.Name,
//                    Quantity = t.Quantity,
//                    TransactionDate = t.TransactionDate,
//                    BatchNo = t.BatchNo,
//                    GatePassNo = t.GatePassNo,
//                    Remarks = t.Remarks,
//                    ReceivedBy = t.ReceivedBy,
//                    DeliveredTo = t.DeliveredTo,
//                    CreatedBy = t.CreatedBy,
//                    CreatedByUsername = t.CreatedByUser.Username,
//                    CreatedAt = t.CreatedAt
//                })
//                .ToListAsync();
//        }

//        // ==========================================
//        // GET BY STAGE
//        // ==========================================
//        public async Task<List<WashTransactionResponseDto>> GetByStageAsync(int processStageId)
//        {
//            return await _context.WashTransactions
//                .Include(t => t.WorkOrder)
//                .Include(t => t.ProcessStage)
//                .Include(t => t.CreatedByUser)
//                .Where(t => t.ProcessStageId == processStageId && t.IsActive)
//                .OrderByDescending(t => t.TransactionDate)
//                .Select(t => new WashTransactionResponseDto
//                {
//                    Id = t.Id,
//                    WorkOrderId = t.WorkOrderId,
//                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
//                    StyleName = t.WorkOrder.StyleName,
//                    Buyer = t.WorkOrder.Buyer,
//                    Factory = t.WorkOrder.Factory,
//                    Line = t.WorkOrder.Line,
//                    TransactionType = t.TransactionType,
//                    TransactionTypeName = t.TransactionType.ToString(),
//                    ProcessStageId = t.ProcessStageId,
//                    ProcessStageName = t.ProcessStage.Name,
//                    Quantity = t.Quantity,
//                    TransactionDate = t.TransactionDate,
//                    BatchNo = t.BatchNo,
//                    GatePassNo = t.GatePassNo,
//                    Remarks = t.Remarks,
//                    ReceivedBy = t.ReceivedBy,
//                    DeliveredTo = t.DeliveredTo,
//                    CreatedBy = t.CreatedBy,
//                    CreatedByUsername = t.CreatedByUser.Username,
//                    CreatedAt = t.CreatedAt
//                })
//                .ToListAsync();
//        }

//        // ==========================================
//        // GET BY FILTER
//        // ==========================================
//        public async Task<List<WashTransactionResponseDto>> GetByFilterAsync(WashTransactionFilterDto filter)
//        {
//            var query = _context.WashTransactions
//                .Include(t => t.WorkOrder)
//                .Include(t => t.ProcessStage)
//                .Include(t => t.CreatedByUser)
//                .Where(t => t.IsActive);

//            if (filter.WorkOrderId.HasValue)
//                query = query.Where(t => t.WorkOrderId == filter.WorkOrderId.Value);

//            if (filter.TransactionType.HasValue)
//                query = query.Where(t => t.TransactionType == filter.TransactionType.Value);

//            if (filter.ProcessStageId.HasValue)
//                query = query.Where(t => t.ProcessStageId == filter.ProcessStageId.Value);

//            if (filter.StartDate.HasValue)
//                query = query.Where(t => t.TransactionDate >= filter.StartDate.Value);

//            if (filter.EndDate.HasValue)
//                query = query.Where(t => t.TransactionDate <= filter.EndDate.Value);

//            if (!string.IsNullOrEmpty(filter.BatchNo))
//                query = query.Where(t => t.BatchNo == filter.BatchNo);

//            return await query
//                .OrderByDescending(t => t.TransactionDate)
//                .Select(t => new WashTransactionResponseDto
//                {
//                    Id = t.Id,
//                    WorkOrderId = t.WorkOrderId,
//                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
//                    StyleName = t.WorkOrder.StyleName,
//                    Buyer = t.WorkOrder.Buyer,
//                    Factory = t.WorkOrder.Factory,
//                    Line = t.WorkOrder.Line,
//                    TransactionType = t.TransactionType,
//                    TransactionTypeName = t.TransactionType.ToString(),
//                    ProcessStageId = t.ProcessStageId,
//                    ProcessStageName = t.ProcessStage.Name,
//                    Quantity = t.Quantity,
//                    TransactionDate = t.TransactionDate,
//                    BatchNo = t.BatchNo,
//                    GatePassNo = t.GatePassNo,
//                    Remarks = t.Remarks,
//                    ReceivedBy = t.ReceivedBy,
//                    DeliveredTo = t.DeliveredTo,
//                    CreatedBy = t.CreatedBy,
//                    CreatedByUsername = t.CreatedByUser.Username,
//                    CreatedAt = t.CreatedAt
//                })
//                .ToListAsync();
//        }

//        // ==========================================
//        // GET BALANCES BY WORK ORDER
//        // ==========================================
//        public async Task<List<ProcessBalanceDto>> GetBalancesByWorkOrderAsync(int workOrderId)
//        {
//            var workOrder = await _context.WorkOrders.FindAsync(workOrderId);
//            if (workOrder == null)
//                throw new KeyNotFoundException($"WorkOrder with ID {workOrderId} not found");

//            var balances = await _context.ProcessStageBalances
//                .Include(b => b.ProcessStage)
//                .Where(b => b.WorkOrderId == workOrderId)
//                .ToListAsync();

//            return balances.Select(b => new ProcessBalanceDto
//            {
//                WorkOrderId = workOrder.Id,
//                WorkOrderNo = workOrder.WorkOrderNo,
//                StyleName = workOrder.StyleName,
//                ProcessStageId = b.ProcessStageId,
//                ProcessStageName = b.ProcessStage.Name,
//                TotalReceived = b.TotalReceived,
//                TotalDelivered = b.TotalDelivered,
//                CurrentBalance = b.CurrentBalance,
//                LastReceiveDate = b.LastReceiveDate,
//                LastDeliveryDate = b.LastDeliveryDate
//            }).ToList();
//        }

//        // ==========================================
//        // GET WASH STATUS
//        // ==========================================
//        public async Task<WorkOrderWashStatusDto?> GetWashStatusAsync(int workOrderId)
//        {
//            var workOrder = await _context.WorkOrders.FindAsync(workOrderId);
//            if (workOrder == null) return null;

//            var balances = await _context.ProcessStageBalances
//                .Include(b => b.ProcessStage)
//                .Where(b => b.WorkOrderId == workOrderId)
//                .OrderBy(b => b.ProcessStage.DisplayOrder)
//                .ToListAsync();

//            var result = new WorkOrderWashStatusDto
//            {
//                WorkOrderId = workOrder.Id,
//                WorkOrderNo = workOrder.WorkOrderNo,
//                StyleName = workOrder.StyleName,
//                Buyer = workOrder.Buyer,
//                Factory = workOrder.Factory,
//                Line = workOrder.Line,
//                WashType = workOrder.WashType,
//                OrderQuantity = workOrder.OrderQuantity,
//                StageBalances = new Dictionary<string, ProcessBalanceDto>()
//            };

//            foreach (var balance in balances)
//            {
//                var balanceDto = new ProcessBalanceDto
//                {
//                    WorkOrderId = workOrder.Id,
//                    WorkOrderNo = workOrder.WorkOrderNo,
//                    StyleName = workOrder.StyleName,
//                    ProcessStageId = balance.ProcessStageId,
//                    ProcessStageName = balance.ProcessStage.Name,
//                    TotalReceived = balance.TotalReceived,
//                    TotalDelivered = balance.TotalDelivered,
//                    CurrentBalance = balance.CurrentBalance,
//                    LastReceiveDate = balance.LastReceiveDate,
//                    LastDeliveryDate = balance.LastDeliveryDate
//                };

//                result.StageBalances[balance.ProcessStage.Name] = balanceDto;
//            }

//            result.TotalReceived = balances.Sum(b => b.TotalReceived);
//            result.TotalDelivered = balances.Sum(b => b.TotalDelivered);
//            result.OverallBalance = balances.Sum(b => b.CurrentBalance);

//            if (workOrder.OrderQuantity > 0)
//            {
//                result.CompletionPercentage = Math.Round((decimal)result.TotalDelivered / workOrder.OrderQuantity * 100, 2);
//            }

//            return result;
//        }

//        // ==========================================
//        // GET ALL WASH STATUSES
//        // ==========================================
//        public async Task<List<WorkOrderWashStatusDto>> GetAllWashStatusesAsync()
//        {
//            var workOrders = await _context.WorkOrders.ToListAsync();
//            var results = new List<WorkOrderWashStatusDto>();

//            foreach (var workOrder in workOrders)
//            {
//                var status = await GetWashStatusAsync(workOrder.Id);
//                if (status != null)
//                {
//                    results.Add(status);
//                }
//            }

//            return results;
//        }

//        // ==========================================
//        // GET STAGE SUMMARY
//        // ==========================================
//        public async Task<List<ProcessStageSummaryDto>> GetStageSummaryAsync()
//        {
//            var allStages = await _context.ProcessStages
//                .Where(ps => ps.IsActive)
//                .OrderBy(ps => ps.DisplayOrder)
//                .ToListAsync();

//            var summaries = new List<ProcessStageSummaryDto>();

//            foreach (var stage in allStages)
//            {
//                var transactions = await _context.WashTransactions
//                    .Where(t => t.ProcessStageId == stage.Id && t.IsActive)
//                    .ToListAsync();

//                var summary = new ProcessStageSummaryDto
//                {
//                    ProcessStageId = stage.Id,
//                    ProcessStageName = stage.Name,
//                    TotalReceiveCount = transactions.Count(t => t.TransactionType == TransactionType.Receive),
//                    TotalDeliveryCount = transactions.Count(t => t.TransactionType == TransactionType.Delivery),
//                    TotalReceivedQty = transactions.Where(t => t.TransactionType == TransactionType.Receive).Sum(t => t.Quantity),
//                    TotalDeliveredQty = transactions.Where(t => t.TransactionType == TransactionType.Delivery).Sum(t => t.Quantity),
//                    CurrentBalance = transactions.Where(t => t.TransactionType == TransactionType.Receive).Sum(t => t.Quantity) -
//                                   transactions.Where(t => t.TransactionType == TransactionType.Delivery).Sum(t => t.Quantity)
//                };

//                summaries.Add(summary);
//            }

//            return summaries;
//        }

//        // ==========================================
//        // GET RECEIVES BY STAGE
//        // ==========================================
//        public async Task<List<WashTransactionResponseDto>> GetReceivesByStageAsync(int processStageId, DateTime? startDate = null, DateTime? endDate = null)
//        {
//            var query = _context.WashTransactions
//                .Include(t => t.WorkOrder)
//                .Include(t => t.ProcessStage)
//                .Include(t => t.CreatedByUser)
//                .Where(t => t.ProcessStageId == processStageId && t.TransactionType == TransactionType.Receive && t.IsActive);

//            if (startDate.HasValue)
//                query = query.Where(t => t.TransactionDate >= startDate.Value);

//            if (endDate.HasValue)
//                query = query.Where(t => t.TransactionDate <= endDate.Value);

//            return await query
//                .OrderByDescending(t => t.TransactionDate)
//                .Select(t => new WashTransactionResponseDto
//                {
//                    Id = t.Id,
//                    WorkOrderId = t.WorkOrderId,
//                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
//                    StyleName = t.WorkOrder.StyleName,
//                    Buyer = t.WorkOrder.Buyer,
//                    Factory = t.WorkOrder.Factory,
//                    Line = t.WorkOrder.Line,
//                    TransactionType = t.TransactionType,
//                    TransactionTypeName = t.TransactionType.ToString(),
//                    ProcessStageId = t.ProcessStageId,
//                    ProcessStageName = t.ProcessStage.Name,
//                    Quantity = t.Quantity,
//                    TransactionDate = t.TransactionDate,
//                    BatchNo = t.BatchNo,
//                    GatePassNo = t.GatePassNo,
//                    Remarks = t.Remarks,
//                    ReceivedBy = t.ReceivedBy,
//                    DeliveredTo = t.DeliveredTo,
//                    CreatedBy = t.CreatedBy,
//                    CreatedByUsername = t.CreatedByUser.Username,
//                    CreatedAt = t.CreatedAt
//                })
//                .ToListAsync();
//        }

//        // ==========================================
//        // GET DELIVERIES BY STAGE
//        // ==========================================
//        public async Task<List<WashTransactionResponseDto>> GetDeliveriesByStageAsync(int processStageId, DateTime? startDate = null, DateTime? endDate = null)
//        {
//            var query = _context.WashTransactions
//                .Include(t => t.WorkOrder)
//                .Include(t => t.ProcessStage)
//                .Include(t => t.CreatedByUser)
//                .Where(t => t.ProcessStageId == processStageId && t.TransactionType == TransactionType.Delivery && t.IsActive);

//            if (startDate.HasValue)
//                query = query.Where(t => t.TransactionDate >= startDate.Value);

//            if (endDate.HasValue)
//                query = query.Where(t => t.TransactionDate <= endDate.Value);

//            return await query
//                .OrderByDescending(t => t.TransactionDate)
//                .Select(t => new WashTransactionResponseDto
//                {
//                    Id = t.Id,
//                    WorkOrderId = t.WorkOrderId,
//                    WorkOrderNo = t.WorkOrder.WorkOrderNo,
//                    StyleName = t.WorkOrder.StyleName,
//                    Buyer = t.WorkOrder.Buyer,
//                    Factory = t.WorkOrder.Factory,
//                    Line = t.WorkOrder.Line,
//                    TransactionType = t.TransactionType,
//                    TransactionTypeName = t.TransactionType.ToString(),
//                    ProcessStageId = t.ProcessStageId,
//                    ProcessStageName = t.ProcessStage.Name,
//                    Quantity = t.Quantity,
//                    TransactionDate = t.TransactionDate,
//                    BatchNo = t.BatchNo,
//                    GatePassNo = t.GatePassNo,
//                    Remarks = t.Remarks,
//                    ReceivedBy = t.ReceivedBy,
//                    DeliveredTo = t.DeliveredTo,
//                    CreatedBy = t.CreatedBy,
//                    CreatedByUsername = t.CreatedByUser.Username,
//                    CreatedAt = t.CreatedAt
//                })
//                .ToListAsync();
//        }

//        // ==========================================
//        // PRIVATE HELPER: GET BALANCE
//        // ==========================================
//        private async Task<int> GetCurrentBalanceAsync(int workOrderId, int processStageId)
//        {
//            var balance = await _context.ProcessStageBalances
//                .FirstOrDefaultAsync(b => b.WorkOrderId == workOrderId && b.ProcessStageId == processStageId);

//            return balance?.CurrentBalance ?? 0;
//        }


//        // ==========================================
//        // GET PAGINATED WITH FAST SEARCH & FILTERS
//        // ==========================================
//        public async Task<PaginatedResponseDto<WashTransactionResponseDto>> GetPaginatedAsync(
//            TransactionPaginationRequestDto request)
//        {
//            try
//            {
//                // Build query with AsNoTracking for read-only performance
//                var query = _context.WashTransactions
//                    .AsNoTracking()
//                    .Include(t => t.WorkOrder)
//                    .Include(t => t.ProcessStage)
//                    .Include(t => t.CreatedByUser)
//                    .Include(t => t.UpdatedByUser)
//                    .Where(t => t.IsActive)
//                    .AsQueryable();

//                // Apply global search
//                query = query.SearchTransaction(request.SearchTerm);

//                // Apply advanced filters
//                query = query.ApplyTransactionFilters(
//                    request.Buyer,
//                    request.Factory,
//                    request.ProcessStageId,
//                    request.TransactionTypeId,
//                    request.FromDate,
//                    request.ToDate
//                );

//                // Apply sorting
//                query = query.ApplyTransactionSort(request.SortBy, request.SortOrder);

//                // Get total count BEFORE pagination
//                var totalCount = await query.CountAsync();

//                // Calculate total pages
//                var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

//                // Apply pagination
//                var skip = (request.Page - 1) * request.PageSize;
//                var data = await query
//                    .Skip(skip)
//                    .Take(request.PageSize)
//                    .Select(t => new WashTransactionResponseDto
//                    {
//                        Id = t.Id,
//                        WorkOrderId = t.WorkOrderId,
//                        WorkOrderNo = t.WorkOrder.WorkOrderNo,
//                        StyleName = t.WorkOrder.StyleName,
//                        Buyer = t.WorkOrder.Buyer,
//                        Factory = t.WorkOrder.Factory,
//                        Line = t.WorkOrder.Line,
//                        TransactionType = t.TransactionType,
//                        TransactionTypeName = t.TransactionType.ToString(),
//                        ProcessStageId = t.ProcessStageId,
//                        ProcessStageName = t.ProcessStage.Name,
//                        Quantity = t.Quantity,
//                        TransactionDate = t.TransactionDate,
//                        BatchNo = t.BatchNo,
//                        GatePassNo = t.GatePassNo,
//                        Remarks = t.Remarks,
//                        ReceivedBy = t.ReceivedBy,
//                        DeliveredTo = t.DeliveredTo,
//                        CreatedBy = t.CreatedBy,
//                        CreatedByUsername = t.CreatedByUser.Username,
//                        CreatedAt = t.CreatedAt,
//                        UpdatedByUsername = t.UpdatedByUser != null ? t.UpdatedByUser.Username : null,
//                        UpdatedAt = t.UpdatedAt
//                    })
//                    .ToListAsync();

//                return new PaginatedResponseDto<WashTransactionResponseDto>
//                {
//                    Success = true,
//                    Message = totalCount == 0 ? "No transactions found" : null,
//                    Data = data,
//                    Pagination = new PaginationMetadata
//                    {
//                        CurrentPage = request.Page,
//                        PageSize = request.PageSize,
//                        TotalRecords = totalCount,
//                        TotalPages = totalPages,
//                        HasPrevious = request.Page > 1,
//                        HasNext = request.Page < totalPages
//                    }
//                };
//            }
//            catch (Exception ex)
//            {
//                return new PaginatedResponseDto<WashTransactionResponseDto>
//                {
//                    Success = false,
//                    Message = $"Error retrieving transactions: {ex.Message}",
//                    Data = new List<WashTransactionResponseDto>(),
//                    Pagination = new PaginationMetadata()
//                };
//            }
//        }

//        // ==========================================
//        // EXPORT TO CSV (SERVER-SIDE)
//        // ==========================================
//        // ==========================================
//        // EXPORT TO CSV (SERVER-SIDE)
//        // ==========================================
//        public async Task<byte[]> ExportToCSVAsync(
//            string? searchTerm = null,
//            string? buyer = null,
//            string? factory = null,
//            int? processStageId = null,
//            int? transactionTypeId = null,
//            DateTime? startDate = null,
//            DateTime? endDate = null)
//        {
//            try
//            {
//                // Build query
//                var query = _context.WashTransactions
//                    .Include(t => t.WorkOrder)
//                    .Include(t => t.ProcessStage)
//                    .Where(t => t.IsActive)
//                    .AsQueryable();

//                // Apply filters
//                if (!string.IsNullOrEmpty(buyer))
//                    query = query.Where(t => t.WorkOrder.Buyer.ToLower().Contains(buyer.ToLower()));

//                if (!string.IsNullOrEmpty(factory))
//                    query = query.Where(t => t.WorkOrder.Factory.ToLower() == factory.ToLower());

//                if (processStageId.HasValue)
//                    query = query.Where(t => t.ProcessStageId == processStageId.Value);

//                if (transactionTypeId.HasValue)
//                    query = query.Where(t => (int)t.TransactionType == transactionTypeId.Value);

//                if (startDate.HasValue)
//                    query = query.Where(t => t.TransactionDate >= startDate.Value);

//                if (endDate.HasValue)
//                    query = query.Where(t => t.TransactionDate <= endDate.Value.AddDays(1).AddTicks(-1));

//                // Apply search
//                if (!string.IsNullOrEmpty(searchTerm))
//                {
//                    var lowerSearchTerm = searchTerm.ToLower().Trim();
//                    query = query.Where(t =>
//                        t.WorkOrder.WorkOrderNo.ToLower().Contains(lowerSearchTerm) ||
//                        t.WorkOrder.Buyer.ToLower().Contains(lowerSearchTerm) ||
//                        t.WorkOrder.StyleName.ToLower().Contains(lowerSearchTerm) ||
//                        t.ProcessStage.Name.ToLower().Contains(lowerSearchTerm) ||
//                        (t.BatchNo != null && t.BatchNo.ToLower().Contains(lowerSearchTerm)) ||
//                        (t.GatePassNo != null && t.GatePassNo.ToLower().Contains(lowerSearchTerm)) ||
//                        t.Quantity.ToString().Contains(lowerSearchTerm)
//                    );
//                }

//                // Get data
//                var transactions = await query
//                    .OrderByDescending(t => t.TransactionDate)
//                    .Select(t => new
//                    {
//                        Id = t.Id,
//                        WorkOrderNo = t.WorkOrder.WorkOrderNo,
//                        StyleName = t.WorkOrder.StyleName,
//                        Buyer = t.WorkOrder.Buyer,
//                        Factory = t.WorkOrder.Factory,
//                        FastReactNo = t.WorkOrder.FastReactNo ?? "-",
//                        StageName = t.ProcessStage.Name,
//                        TransactionType = t.TransactionType.ToString(),
//                        Quantity = t.Quantity,
//                        TransactionDate = t.TransactionDate,
//                        BatchNo = t.BatchNo ?? "-",
//                        GatePassNo = t.GatePassNo ?? "-",
//                        Remarks = t.Remarks ?? "-",
//                        ReceivedBy = t.ReceivedBy ?? "-",
//                        DeliveredTo = t.DeliveredTo ?? "-",
//                        CreatedAt = t.CreatedAt
//                    })
//                    .ToListAsync();

//                if (transactions.Count == 0)
//                {
//                    throw new Exception("No transactions found to export");
//                }

//                Console.WriteLine($"📥 Generating CSV for {transactions.Count} transactions");

//                // ✅ FIXED: Simpler approach with StringWriter
//                var stringBuilder = new StringBuilder();

//                using (var stringWriter = new StringWriter(stringBuilder))
//                {
//                    using (var csv = new CsvWriter(stringWriter, CultureInfo.InvariantCulture))
//                    {
//                        // Write headers
//                        csv.WriteField("ID");
//                        csv.WriteField("Work Order No");
//                        csv.WriteField("Style Name");
//                        csv.WriteField("Buyer");
//                        csv.WriteField("Factory");
//                        csv.WriteField("FastReact No");
//                        csv.WriteField("Process Stage");
//                        csv.WriteField("Transaction Type");
//                        csv.WriteField("Quantity");
//                        csv.WriteField("Transaction Date");
//                        csv.WriteField("Batch No");
//                        csv.WriteField("Gate Pass No");
//                        csv.WriteField("Remarks");
//                        csv.WriteField("Received By");
//                        csv.WriteField("Delivered To");
//                        csv.WriteField("Created At");
//                        await csv.NextRecordAsync();

//                        // Write data rows
//                        foreach (var transaction in transactions)
//                        {
//                            csv.WriteField(transaction.Id);
//                            csv.WriteField(transaction.WorkOrderNo);
//                            csv.WriteField(transaction.StyleName);
//                            csv.WriteField(transaction.Buyer);
//                            csv.WriteField(transaction.Factory);
//                            csv.WriteField(transaction.FastReactNo);
//                            csv.WriteField(transaction.StageName);
//                            csv.WriteField(transaction.TransactionType);
//                            csv.WriteField(transaction.Quantity);
//                            csv.WriteField(transaction.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"));
//                            csv.WriteField(transaction.BatchNo);
//                            csv.WriteField(transaction.GatePassNo);
//                            csv.WriteField(transaction.Remarks);
//                            csv.WriteField(transaction.ReceivedBy);
//                            csv.WriteField(transaction.DeliveredTo);
//                            csv.WriteField(transaction.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
//                            await csv.NextRecordAsync();
//                        }
//                    }
//                }

//                // Convert string to bytes
//                var csvContent = stringBuilder.ToString();
//                var result = Encoding.UTF8.GetBytes(csvContent);

//                if (result.Length == 0)
//                {
//                    throw new Exception("Failed to generate CSV data");
//                }

//                Console.WriteLine($"✅ CSV generated successfully - Size: {result.Length} bytes");
//                return result;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"❌ CSV Export Error: {ex.Message}");
//                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
//                throw new Exception($"Error exporting transactions: {ex.Message}", ex);
//            }
//        }
//    }
//}