using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Enums;
using TechMove.Factories;
using TechMove.Models;
using TechMove.Service;

namespace TechMove.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly TechMoveDbContext _context;
        private readonly CurrencyService _currencyService;
        private readonly IServiceRequestFactory _serviceRequestFactory;

        public ServiceRequestController(
            TechMoveDbContext context,
            CurrencyService currencyService,
             IServiceRequestFactory serviceRequestFactory)
        {
            _context = context;
            _currencyService = currencyService;
            _serviceRequestFactory = serviceRequestFactory;
        }

        public async Task<IActionResult> Index()
        {
            var serviceRequests = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ToListAsync();

            return View(serviceRequests);
        }




        public IActionResult Create()
        {
            ViewBag.ContractId = GetContractSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestModel serviceRequest)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == serviceRequest.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("ContractId", "Please select a valid contract.");
                ViewBag.ContractId = GetContractSelectList(serviceRequest.ContractId);
                return View(serviceRequest);
            }
            //temporary logging to verify contract status is being retrieved correctly
            Console.WriteLine($"Contract ID: {contract.ContractId}");
            Console.WriteLine($"Contract Status: {contract.Status}");

            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
            {
                ModelState.AddModelError("",
                    "Service Request cannot be created because the contract is Expired or On Hold.");

                ViewBag.ContractId = GetContractSelectList(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            ModelState.Remove("CostZAR");

            decimal exchangeRate = await _currencyService.GetUsdToZarRateAsync();

            // serviceRequest.CostZAR = serviceRequest.CostUSD * exchangeRate;
            serviceRequest.CostZAR = _currencyService.ConvertUsdToZar(
                serviceRequest.CostUSD,
                exchangeRate
                 );

            if (ModelState.IsValid)
            {
                //_context.ServiceRequests.Add(serviceRequest);
                var newServiceRequest = _serviceRequestFactory.Create(
                    serviceRequest.ContractId,
                    serviceRequest.Description,
                    serviceRequest.CostUSD,
                    serviceRequest.CostZAR,
                    serviceRequest.Status
                        );

                _context.ServiceRequests.Add(newServiceRequest);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.ContractId = GetContractSelectList(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        //helper method to create a SelectList for contracts with custom display text
        private SelectList GetContractSelectList(int? selectedContractId = null)
        {
            var contracts = _context.Contracts
                .Select(c => new
                {
                    c.ContractId,
                    DisplayText = "Contract #" + c.ContractId
                                  + " - " + c.ServiceLevel
                                  + " - " + c.Status
                })
                .ToList();

            return new SelectList(contracts, "ContractId", "DisplayText", selectedContractId);
        }
        public async Task<IActionResult> Details(int id)
        {
            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(s => s.ServiceRequestId == id);

            if (serviceRequest == null)
                return NotFound();

            return View(serviceRequest);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
                return NotFound();

            ViewBag.ContractId = new SelectList(
                _context.Contracts,
                "ContractId",
                "ServiceLevel",
                serviceRequest.ContractId
            );

            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequestModel serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId)
                return NotFound();

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == serviceRequest.ContractId);

            if (contract == null)
                return NotFound();

            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
            {
                ModelState.AddModelError("", "Service Request cannot be updated because the contract is Expired or On Hold.");
            }

            decimal exchangeRate = await _currencyService.GetUsdToZarRateAsync();
            serviceRequest.CostZAR = serviceRequest.CostUSD * exchangeRate;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = serviceRequest.ServiceRequestId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.ServiceRequestId))
                        return NotFound();

                    throw;
                }
            }

            ViewBag.ContractId = new SelectList(
                _context.Contracts,
                "ContractId",
                "ServiceLevel",
                serviceRequest.ContractId
            );

            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(s => s.ServiceRequestId == id);

            if (serviceRequest == null)
                return NotFound();

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
                return NotFound();

            _context.ServiceRequests.Remove(serviceRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.ServiceRequestId == id);
        }
    }
}
