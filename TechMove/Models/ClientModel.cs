using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace TechMove.Models
{
    public class ClientModel
    {
        [Key]
        public int ClientId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string ContactDetails { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Region { get; set; } = string.Empty;

        public ICollection<ContractModel> Contracts { get; set; } = new List<ContractModel>();
    }
}
