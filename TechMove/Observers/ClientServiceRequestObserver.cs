using Microsoft.Extensions.Logging;
using TechMove.Enums;
using TechMove.Models;

namespace TechMove.Observers
{
    public class ClientServiceRequestObserver : IServiceRequestObserver
    {
        private readonly ILogger<ClientServiceRequestObserver> _logger;

        public ClientServiceRequestObserver(
            ILogger<ClientServiceRequestObserver> logger)
        {
            _logger = logger;
        }

        public void Update(ServiceRequestModel serviceRequest)
        {
            switch (serviceRequest.Status)
            {
                case ServiceRequestStatus.InProgress:
                    _logger.LogInformation(
                        "Client notified: Your service request is now in progress.");
                    break;

                case ServiceRequestStatus.Completed:
                    _logger.LogInformation(
                        "Client notified: Your service request has been completed.");
                    break;

                case ServiceRequestStatus.Cancelled:
                    _logger.LogInformation(
                        "Client notified: Your service request has been cancelled.");
                    break;
            }
        }
    }
}
