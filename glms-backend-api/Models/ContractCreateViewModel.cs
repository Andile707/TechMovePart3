using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using TechMove.Enums;

namespace TechMove.Models
{
    public class ContractCreateViewModel
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        [Required]
        public ServiceLevel ServiceLevel { get; set; }

        [Required]
        [Display(Name = "Signed Agreement PDF")]
        public IFormFile SignedAgreement { get; set; } = null!;
    }
}
