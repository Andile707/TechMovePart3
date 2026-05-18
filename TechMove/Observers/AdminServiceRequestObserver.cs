using Microsoft.Extensions.Logging;
using TechMove.Enums;
using TechMove.Models;

namespace TechMove.Observers
{
    public class AdminServiceRequestObserver : IServiceRequestObserver
    {
        private readonly ILogger<AdminServiceRequestObserver> _logger;

        public AdminServiceRequestObserver(
            ILogger<AdminServiceRequestObserver> logger)
        {
            _logger = logger;
        }

        public void Update(ServiceRequestModel serviceRequest)
        {
            switch (serviceRequest.Status)
            {
                case ServiceRequestStatus.InProgress:
                    _logger.LogInformation(
                        "Admin notified: Service request is now in progress.");
                    break;

                case ServiceRequestStatus.Completed:
                    _logger.LogInformation(
                        "Admin notified: Service request completed.");
                    break;

                case ServiceRequestStatus.Cancelled:
                    _logger.LogInformation(
                        "Admin notified: Service request cancelled.");
                    break;
            }
        }
    }
}