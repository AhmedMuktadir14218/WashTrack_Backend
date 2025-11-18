using System.ComponentModel.DataAnnotations;

namespace wsahRecieveDelivary.DTOs
{
    // For Creating/Updating Single WorkOrder
    public class WorkOrderDto
    {
        [Required]
        public string Factory { get; set; } = string.Empty;

        [Required]
        public string Line { get; set; } = string.Empty;

        [Required]
        public string Unit { get; set; } = string.Empty;

        [Required]
        public string Buyer { get; set; } = string.Empty;

        public string BuyerDepartment { get; set; } = string.Empty;

        [Required]
        public string StyleName { get; set; } = string.Empty;

        public string FastReactNo { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        [Required]
        public string WorkOrderNo { get; set; } = string.Empty;

        public string WashType { get; set; } = string.Empty;
        public int OrderQuantity { get; set; }
        public int CutQty { get; set; }

        public DateTime? TOD { get; set; }
        public DateTime? SewingCompDate { get; set; }
        public DateTime? FirstRCVDate { get; set; }
        public DateTime? WashApprovalDate { get; set; }
        public DateTime? WashTargetDate { get; set; }

        public int TotalWashReceived { get; set; }
        public int TotalWashDelivery { get; set; }
        public int WashBalance { get; set; }
        public int FromReceived { get; set; }

        public string? Marks { get; set; }
    }

    // For Excel Bulk Upload Response
    public class WorkOrderBulkUploadResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public int SuccessCount { get; set; }
        public int UpdatedCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    // For Getting WorkOrder Details
    public class WorkOrderResponseDto
    {
        public int Id { get; set; }
        public string Factory { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Buyer { get; set; } = string.Empty;
        public string BuyerDepartment { get; set; } = string.Empty;
        public string StyleName { get; set; } = string.Empty;
        public string FastReactNo { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string WorkOrderNo { get; set; } = string.Empty;
        public string WashType { get; set; } = string.Empty;
        public int OrderQuantity { get; set; }
        public int CutQty { get; set; }
        public DateTime? TOD { get; set; }
        public DateTime? SewingCompDate { get; set; }
        public DateTime? FirstRCVDate { get; set; }
        public DateTime? WashApprovalDate { get; set; }
        public DateTime? WashTargetDate { get; set; }
        public int TotalWashReceived { get; set; }
        public int TotalWashDelivery { get; set; }
        public int WashBalance { get; set; }
        public int FromReceived { get; set; }
        public string? Marks { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;
        public string? UpdatedByUsername { get; set; }
    }
}