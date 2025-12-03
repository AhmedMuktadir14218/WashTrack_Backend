using System.ComponentModel.DataAnnotations;
using wsahRecieveDelivary.Models.Enums;

namespace wsahRecieveDelivary.DTOs
{
    // ==========================================
    // CREATE TRANSACTION DTO
    // ==========================================
    public class CreateWashTransactionDto
    {
        [Required(ErrorMessage = "WorkOrderId is required")]
        public int WorkOrderId { get; set; }

        [Required(ErrorMessage = "TransactionType is required")]
        public TransactionType TransactionType { get; set; }

        [Required(ErrorMessage = "ProcessStageId is required")]
        public int ProcessStageId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        // ❌ REMOVED: public DateTime? TransactionDate { get; set; }

        [StringLength(100)]
        public string? BatchNo { get; set; }

        [StringLength(100)]
        public string? GatePassNo { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(100)]
        public string? ReceivedBy { get; set; }

        [StringLength(100)]
        public string? DeliveredTo { get; set; }
    }

    // ==========================================
    // TRANSACTION RESPONSE DTO
    // ==========================================
    public class WashTransactionResponseDto
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public string WorkOrderNo { get; set; } = string.Empty;
        public string StyleName { get; set; } = string.Empty;
        public string Buyer { get; set; } = string.Empty;
        public string Factory { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;

        public TransactionType TransactionType { get; set; }
        public string TransactionTypeName { get; set; } = string.Empty;

        public int ProcessStageId { get; set; }  // ✅ CHANGED
        public string ProcessStageName { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }

        public string? BatchNo { get; set; }
        public string? GatePassNo { get; set; }
        public string? Remarks { get; set; }
        public string? ReceivedBy { get; set; }
        public string? DeliveredTo { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? UpdatedByUsername { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // ==========================================
    // PROCESS BALANCE DTO
    // ==========================================
    public class ProcessBalanceDto
    {
        public int WorkOrderId { get; set; }
        public string WorkOrderNo { get; set; } = string.Empty;
        public string StyleName { get; set; } = string.Empty;

        public int ProcessStageId { get; set; }  // ✅ CHANGED
        public string ProcessStageName { get; set; } = string.Empty;

        public int TotalReceived { get; set; }
        public int TotalDelivered { get; set; }
        public int CurrentBalance { get; set; }
        public DateTime? LastReceiveDate { get; set; }
        public DateTime? LastDeliveryDate { get; set; }
    }

    // ==========================================
    // WORK ORDER WASH STATUS DTO
    // ==========================================
    public class WorkOrderWashStatusDto
    {
        public int WorkOrderId { get; set; }
        public string WorkOrderNo { get; set; } = string.Empty;
        public string StyleName { get; set; } = string.Empty;
        public string Buyer { get; set; } = string.Empty;
        public string Factory { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public string WashType { get; set; } = string.Empty;
        public int OrderQuantity { get; set; }

        // ✅ CHANGED: Now using Dictionary for dynamic stages
        public Dictionary<string, ProcessBalanceDto> StageBalances { get; set; } = new();

        // Overall summary
        public int TotalReceived { get; set; }
        public int TotalDelivered { get; set; }
        public int OverallBalance { get; set; }
        public decimal CompletionPercentage { get; set; }
    }

    // ==========================================
    // FILTER DTO
    // ==========================================
    public class WashTransactionFilterDto
    {
        public int? WorkOrderId { get; set; }
        public TransactionType? TransactionType { get; set; }
        public int? ProcessStageId { get; set; }  // ✅ CHANGED
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? BatchNo { get; set; }
    }

    // ==========================================
    // SUMMARY REPORT DTO
    // ==========================================
    public class ProcessStageSummaryDto
    {
        public int ProcessStageId { get; set; }  // ✅ CHANGED
        public string ProcessStageName { get; set; } = string.Empty;
        public int TotalReceiveCount { get; set; }
        public int TotalDeliveryCount { get; set; }
        public int TotalReceivedQty { get; set; }
        public int TotalDeliveredQty { get; set; }
        public int CurrentBalance { get; set; }
    }
}