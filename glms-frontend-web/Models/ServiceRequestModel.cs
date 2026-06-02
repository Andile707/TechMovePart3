namespace glms_frontend_web.Models
{
    public class ServiceRequestModel
    {
        public int ServiceRequestId { get; set; }

        public int ContractId { get; set; }
        public ContractModel? Contract { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal CostUSD { get; set; }
        public decimal CostZAR { get; set; }


        public DateTime RequestDate { get; set; }
        public ServiceRequestStatus Status { get; set; }

    }
}