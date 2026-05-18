using System.ComponentModel.DataAnnotations;
using TechMove.Enums;

namespace TechMove.Models
{
    public class ContractModel
    {
        [Key]
        public int ContractId { get; set; }

        [Required]
        public int ClientId { get; set; }

        public ClientModel? Client { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        [Required]
        public ServiceLevel ServiceLevel { get; set; }

        // Uploaded PDF details
        public string? SignedAgreementFileName { get; set; }
        public string? SignedAgreementFilePath { get; set; }

        public ICollection<ServiceRequestModel> ServiceRequests { get; set; } = new List<ServiceRequestModel>();

        
    }
}
