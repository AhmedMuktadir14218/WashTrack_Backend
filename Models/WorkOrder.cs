using System.ComponentModel.DataAnnotations;

namespace wsahRecieveDelivary.Models
{
    public class WorkOrder
    {
        public int Id { get; set; }

        // Factory Information
        [Required]
        [StringLength(100)]
        public string Factory { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Line { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Unit { get; set; } = string.Empty;

        // Buyer Information
        [Required]
        [StringLength(100)]
        public string Buyer { get; set; } = string.Empty;

        [StringLength(100)]
        public string BuyerDepartment { get; set; } = string.Empty;

        // Style Information
        [Required]
        [StringLength(200)]
        public string StyleName { get; set; } = string.Empty;

        [StringLength(100)]
        public string FastReactNo { get; set; } = string.Empty;

        [StringLength(100)]
        public string Color { get; set; } = string.Empty;

        // Work Order Information
        [Required]
        [StringLength(100)]
        public string WorkOrderNo { get; set; } = string.Empty;  // Unique identifier

        [StringLength(100)]
        public string WashType { get; set; } = string.Empty;

        // Quantities
        public int OrderQuantity { get; set; }
        public int CutQty { get; set; }

        // Dates
        public DateTime? TOD { get; set; }  // Target Order Date
        public DateTime? SewingCompDate { get; set; }
        public DateTime? FirstRCVDate { get; set; }
        public DateTime? WashApprovalDate { get; set; }
        public DateTime? WashTargetDate { get; set; }

        // Wash Information
        public int TotalWashReceived { get; set; }
        public int TotalWashDelivery { get; set; }
        public int WashBalance { get; set; }
        public int FromReceived { get; set; }

        // Additional Information
        [StringLength(500)]
        public string? Marks { get; set; }

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }  // User ID
        public int? UpdatedBy { get; set; }  // User ID

        // Navigation Properties
        public User CreatedByUser { get; set; } = null!;
        public User? UpdatedByUser { get; set; }
    }
}