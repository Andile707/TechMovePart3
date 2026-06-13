using glms_backend_api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.Enums;
using TechMove.Factories;
using TechMove.Models;
using TechMove.Observers;
using TechMove.Repositories;
using TechMove.Strategies;

namespace TechMove.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IServiceRequestFactory _serviceRequestFactory;
        private readonly IServiceCostStrategy _serviceCostStrategy;
        private readonly ILoggerFactory _loggerFactory;

        public ServiceRequestsController(
            IServiceRequestRepository serviceRequestRepository,
            IContractRepository contractRepository,
            IServiceRequestFactory serviceRequestFactory,
            IServiceCostStrategy serviceCostStrategy,
            ILoggerFactory loggerFactory)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _contractRepository = contractRepository;
            _serviceRequestFactory = serviceRequestFactory;
            _serviceCostStrategy = serviceCostStrategy;
            _loggerFactory = loggerFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceRequests()
        {
            var serviceRequests = await _serviceRequestRepository.GetAllAsync();
            return Ok(serviceRequests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceRequest(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (serviceRequest == null)
                return NotFound();

            return Ok(serviceRequest);
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceRequest(ServiceRequestModel serviceRequest)
        {
            var contract = await _contractRepository.GetPlainByIdAsync(serviceRequest.ContractId);

            if (contract == null)
                return BadRequest("Please select a valid contract.");

            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
            {
                return BadRequest("Service Request cannot be created because the contract is Expired or On Hold.");
            }

            serviceRequest.CostZAR =
                await _serviceCostStrategy.CalculateCostZarAsync(serviceRequest);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newServiceRequest = _serviceRequestFactory.Create(
                serviceRequest.ContractId,
                serviceRequest.Description,
                serviceRequest.CostUSD,
                serviceRequest.CostZAR,
                serviceRequest.Status
            );

            var createdServiceRequest =
                await _serviceRequestRepository.CreateAsync(newServiceRequest);

            return CreatedAtAction(
                nameof(GetServiceRequest),
                new { id = createdServiceRequest.ServiceRequestId },
                createdServiceRequest
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(
            int id,
            ServiceRequestModel serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId)
                return BadRequest("Service request ID mismatch.");

            var contract = await _contractRepository.GetPlainByIdAsync(serviceRequest.ContractId);

            if (contract == null)
                return BadRequest("Invalid contract.");

            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
            {
                return BadRequest("Service Request cannot be updated because the contract is Expired or On Hold.");
            }

            serviceRequest.CostZAR =
                await _serviceCostStrategy.CalculateCostZarAsync(serviceRequest);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedServiceRequest =
                    await _serviceRequestRepository.UpdateAsync(serviceRequest);

                NotifyObservers(updatedServiceRequest);

                return Ok(updatedServiceRequest);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _serviceRequestRepository.ExistsAsync(id))
                    return NotFound();

                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetPlainByIdAsync(id);

            if (serviceRequest == null)
                return NotFound();

            await _serviceRequestRepository.DeleteAsync(serviceRequest);

            return NoContent();
        }

        private void NotifyObservers(ServiceRequestModel serviceRequest)
        {
            var subject = new ServiceRequestSubject();

            subject.Attach(
                new AdminServiceRequestObserver(
                    _loggerFactory.CreateLogger<AdminServiceRequestObserver>()));

            subject.Attach(
                new ClientServiceRequestObserver(
                    _loggerFactory.CreateLogger<ClientServiceRequestObserver>()));

            subject.Notify(serviceRequest);
        }
    }
}