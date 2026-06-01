using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Enums;
using TechMove.Factories;
using TechMove.Models;
using TechMove.Observers;
using TechMove.Strategies;

namespace TechMove.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly TechMoveDbContext _context;
        private readonly IServiceRequestFactory _serviceRequestFactory;
        private readonly IServiceCostStrategy _serviceCostStrategy;
        private readonly ILoggerFactory _loggerFactory;

        public ServiceRequestsController(
            TechMoveDbContext context,
            IServiceRequestFactory serviceRequestFactory,
            IServiceCostStrategy serviceCostStrategy,
            ILoggerFactory loggerFactory)
        {
            _context = context;
            _serviceRequestFactory = serviceRequestFactory;
            _serviceCostStrategy = serviceCostStrategy;
            _loggerFactory = loggerFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceRequests()
        {
            var serviceRequests = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ToListAsync();

            return Ok(serviceRequests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceRequest(int id)
        {
            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(s => s.ServiceRequestId == id);

            if (serviceRequest == null)
                return NotFound();

            return Ok(serviceRequest);
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceRequest(ServiceRequestModel serviceRequest)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == serviceRequest.ContractId);

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

            _context.ServiceRequests.Add(newServiceRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetServiceRequest),
                new { id = newServiceRequest.ServiceRequestId },
                newServiceRequest
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(
            int id,
            ServiceRequestModel serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId)
                return BadRequest("Service request ID mismatch.");

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == serviceRequest.ContractId);

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
                _context.ServiceRequests.Update(serviceRequest);
                await _context.SaveChangesAsync();

                NotifyObservers(serviceRequest);

                return Ok(serviceRequest);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ServiceRequestExists(id))
                    return NotFound();

                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
                return NotFound();

            _context.ServiceRequests.Remove(serviceRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> ServiceRequestExists(int id)
        {
            return await _context.ServiceRequests
                .AnyAsync(e => e.ServiceRequestId == id);
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