namespace glms_frontend_web.Models
{
    public class ContractModel
    {
        public int ContractId { get; set; }

        public int ClientId { get; set; }
        public ClientModel? Client { get; set; }

        public string ContractTitle { get; set; } = string.Empty;

        public ServiceLevel ServiceLevel { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ContractStatus Status { get; set; }

        public string SignedAgreementFileName { get; set; } = string.Empty;

        public ICollection<ServiceRequestModel> ServiceRequests { get; set; }
            = new List<ServiceRequestModel>();
    }
}