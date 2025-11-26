using Microsoft.EntityFrameworkCore;
using wsahRecieveDelivary.Data;
using wsahRecieveDelivary.DTOs;
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

        // ==========================================
        // CREATE RECEIVE
        // ==========================================
        public async Task<WashTransactionResponseDto> CreateReceiveAsync(CreateWashTransactionDto dto, int userId)
        {
            var workOrder = await _context.WorkOrders.FindAsync(dto.WorkOrderId);
            if (workOrder == null)
                throw new KeyNotFoundException($"WorkOrder with ID {dto.WorkOrderId} not found");

            var processStage = await _context.ProcessStages.FindAsync(dto.ProcessStageId);
            if (processStage == null)
                throw new KeyNotFoundException($"ProcessStage with ID {dto.ProcessStageId} not found");

            var transaction = new WashTransaction
            {
                WorkOrderId = dto.WorkOrderId,
                TransactionType = TransactionType.Receive,
                ProcessStageId = dto.ProcessStageId,
                Quantity = dto.Quantity,
                TransactionDate = dto.TransactionDate ?? DateTime.UtcNow,
                BatchNo = dto.BatchNo,
                GatePassNo = dto.GatePassNo,
                Remarks = dto.Remarks,
                ReceivedBy = dto.ReceivedBy,
                DeliveredTo = dto.DeliveredTo,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.WashTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            await UpdateStageBalanceAsync(dto.WorkOrderId, dto.ProcessStageId);

            return await GetByIdAsync(transaction.Id)
                ?? throw new Exception("Failed to retrieve created transaction");
        }

        // ==========================================
        // CREATE DELIVERY
        // ==========================================
        public async Task<WashTransactionResponseDto> CreateDeliveryAsync(CreateWashTransactionDto dto, int userId)
        {
            var workOrder = await _context.WorkOrders.FindAsync(dto.WorkOrderId);
            if (workOrder == null)
                throw new KeyNotFoundException($"WorkOrder with ID {dto.WorkOrderId} not found");

            var processStage = await _context.ProcessStages.FindAsync(dto.ProcessStageId);
            if (processStage == null)
                throw new KeyNotFoundException($"ProcessStage with ID {dto.ProcessStageId} not found");

            var currentBalance = await GetCurrentBalanceAsync(dto.WorkOrderId, dto.ProcessStageId);
            //if (currentBalance < dto.Quantity)
            //{
            //    throw new InvalidOperationException(
            //        $"Insufficient balance in {processStage.Name}. Available: {currentBalance}, Requested: {dto.Quantity}");
            //}

            var transaction = new WashTransaction
            {
                WorkOrderId = dto.WorkOrderId,
                TransactionType = TransactionType.Delivery,
                ProcessStageId = dto.ProcessStageId,
                Quantity = dto.Quantity,
                TransactionDate = dto.TransactionDate ?? DateTime.UtcNow,
                BatchNo = dto.BatchNo,
                GatePassNo = dto.GatePassNo,
                Remarks = dto.Remarks,
                ReceivedBy = dto.ReceivedBy,
                DeliveredTo = dto.DeliveredTo,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.WashTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            await UpdateStageBalanceAsync(dto.WorkOrderId, dto.ProcessStageId);

            return await GetByIdAsync(transaction.Id)
                ?? throw new Exception("Failed to retrieve created transaction");
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
                query = query.Where(t => t.TransactionDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(t => t.TransactionDate <= filter.EndDate.Value);

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
        // UPDATE
        // ==========================================
        public async Task<WashTransactionResponseDto> UpdateAsync(int id, CreateWashTransactionDto dto, int userId)
        {
            var transaction = await _context.WashTransactions.FindAsync(id);
            if (transaction == null || !transaction.IsActive)
                throw new KeyNotFoundException($"Transaction with ID {id} not found");

            var oldStageId = transaction.ProcessStageId;

            // Update fields
            transaction.TransactionType = dto.TransactionType;
            transaction.ProcessStageId = dto.ProcessStageId;
            transaction.Quantity = dto.Quantity;
            transaction.TransactionDate = dto.TransactionDate ?? transaction.TransactionDate;
            transaction.BatchNo = dto.BatchNo;
            transaction.GatePassNo = dto.GatePassNo;
            transaction.Remarks = dto.Remarks;
            transaction.ReceivedBy = dto.ReceivedBy;
            transaction.DeliveredTo = dto.DeliveredTo;
            transaction.UpdatedBy = userId;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Update balances
            await UpdateStageBalanceAsync(transaction.WorkOrderId, oldStageId);
            if (oldStageId != transaction.ProcessStageId)
            {
                await UpdateStageBalanceAsync(transaction.WorkOrderId, transaction.ProcessStageId);
            }

            return await GetByIdAsync(transaction.Id)
                ?? throw new Exception("Failed to retrieve updated transaction");
        }

        // ==========================================
        // DELETE (SOFT DELETE)
        // ==========================================
        public async Task<bool> DeleteAsync(int id)
        {
            var transaction = await _context.WashTransactions.FindAsync(id);
            if (transaction == null || !transaction.IsActive)
                return false;

            transaction.IsActive = false;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Update balance
            await UpdateStageBalanceAsync(transaction.WorkOrderId, transaction.ProcessStageId);

            return true;
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

            // Map balances to dictionary
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

            // Calculate totals
            result.TotalReceived = balances.Sum(b => b.TotalReceived);
            result.TotalDelivered = balances.Sum(b => b.TotalDelivered);
            result.OverallBalance = balances.Sum(b => b.CurrentBalance);

            // Calculate completion percentage
            if (workOrder.OrderQuantity > 0)
            {
                result.CompletionPercentage = Math.Round((decimal)result.TotalDelivered / workOrder.OrderQuantity * 100, 2);
            }

            return result;
        }

        // ==========================================
        // GET ALL WASH STATUSES
        // ==========================================
        public async Task<List<WorkOrderWashStatusDto>> GetAllWashStatusesAsync()
        {
            var workOrders = await _context.WorkOrders.ToListAsync();
            var results = new List<WorkOrderWashStatusDto>();

            foreach (var workOrder in workOrders)
            {
                var status = await GetWashStatusAsync(workOrder.Id);
                if (status != null)
                {
                    results.Add(status);
                }
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
                query = query.Where(t => t.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);

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
                query = query.Where(t => t.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);

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
        // PRIVATE HELPER METHODS
        // ==========================================

        private async Task UpdateStageBalanceAsync(int workOrderId, int processStageId)
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

            // Calculate from active transactions
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

            balance.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private async Task<int> GetCurrentBalanceAsync(int workOrderId, int processStageId)
        {
            var balance = await _context.ProcessStageBalances
                .FirstOrDefaultAsync(b => b.WorkOrderId == workOrderId && b.ProcessStageId == processStageId);

            return balance?.CurrentBalance ?? 0;
        }
    }
}