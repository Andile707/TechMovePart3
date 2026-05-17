using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using TechMove.Enums;

namespace TechMove.Models
{
    public class ServiceRequestModel
    {
        [Key]
        public int ServiceRequestId { get; set; }

        [Required]
        public int ContractId { get; set; }

        public ContractModel? Contract { get; set; }

        [Required, StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        // USD amount entered by user
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostUSD { get; set; }

        // Converted ZAR amount
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostZAR { get; set; }

        [Required]
        public ServiceRequestStatus Status { get; set; }

        
    }
}
