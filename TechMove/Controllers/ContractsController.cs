using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using TechMove.Data;
using TechMove.Enums;
using TechMove.Models;

namespace TechMove.Controllers
{
    

    public class ContractsController : Controller
    {
        private readonly TechMoveDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly FileValidationService _fileValidationService;

        public ContractsController(
        TechMoveDbContext context,
         IWebHostEnvironment environment,
            FileValidationService fileValidationService)
        {
            _context = context;
            _environment = environment;
            _fileValidationService = fileValidationService;
        }

        public async Task<IActionResult> Index(
    DateTime? startDate,
    DateTime? endDate,
    ContractStatus? status)
        {
            var contracts = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (startDate.HasValue)
            {
                contracts = contracts.Where(c => c.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                contracts = contracts.Where(c => c.EndDate <= endDate.Value);
            }

            if (status.HasValue)
            {
                contracts = contracts.Where(c => c.Status == status.Value);
            }

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Status = status;

            return View(await contracts.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", model.ClientId);
                return View(model);
            }

            if (model.SignedAgreement == null || model.SignedAgreement.Length == 0)
            {
                ModelState.AddModelError("SignedAgreement", "Please upload a signed agreement PDF.");
                ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", model.ClientId);
                return View(model);
            }

            var extension = Path.GetExtension(model.SignedAgreement.FileName).ToLower();

            //if (extension != ".pdf" || model.SignedAgreement.ContentType != "application/pdf")
            if (!_fileValidationService.IsValidPdf(
                model.SignedAgreement.FileName,
                 model.SignedAgreement.ContentType))
            {
                ModelState.AddModelError("SignedAgreement", "Only PDF files are allowed.");
                ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", model.ClientId);
                return View(model);
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "agreements");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.SignedAgreement.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.SignedAgreement.CopyToAsync(stream);
            }

            var contract = new ContractModel
            {
                ClientId = model.ClientId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                ServiceLevel = model.ServiceLevel,
                SignedAgreementFileName = uniqueFileName,
                SignedAgreementFilePath = $"/uploads/agreements/{uniqueFileName}"
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = contract.ContractId });
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract == null)
                return NotFound();

            return View(contract);
        }

        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementFileName))
                return NotFound();

            var filePath = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "agreements",
                contract.SignedAgreementFileName
            );

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            return PhysicalFile(filePath, "application/pdf", contract.SignedAgreementFileName);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
                return NotFound();

            var model = new ContractCreateViewModel
            {
                ClientId = contract.ClientId,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status,
                ServiceLevel = contract.ServiceLevel
            };

            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            ViewBag.ContractId = contract.ContractId;
            ViewBag.CurrentFileName = contract.SignedAgreementFileName;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContractCreateViewModel model)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", model.ClientId);
                ViewBag.ContractId = id;
                ViewBag.CurrentFileName = contract.SignedAgreementFileName;
                return View(model);
            }

            contract.ClientId = model.ClientId;
            contract.StartDate = model.StartDate;
            contract.EndDate = model.EndDate;
            contract.Status = model.Status;
            contract.ServiceLevel = model.ServiceLevel;

            if (model.SignedAgreement != null && model.SignedAgreement.Length > 0)
            {
                var extension = Path.GetExtension(model.SignedAgreement.FileName).ToLower();

                if (extension != ".pdf" || model.SignedAgreement.ContentType != "application/pdf")
                {
                    ModelState.AddModelError("SignedAgreement", "Only PDF files are allowed.");
                    ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", model.ClientId);
                    ViewBag.ContractId = id;
                    ViewBag.CurrentFileName = contract.SignedAgreementFileName;
                    return View(model);
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "agreements");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                if (!string.IsNullOrEmpty(contract.SignedAgreementFileName))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, contract.SignedAgreementFileName);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.SignedAgreement.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.SignedAgreement.CopyToAsync(stream);
                }

                contract.SignedAgreementFileName = uniqueFileName;
                contract.SignedAgreementFilePath = $"/uploads/agreements/{uniqueFileName}";
            }

            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = contract.ContractId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract == null)
                return NotFound();

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
                return NotFound();

            if (!string.IsNullOrEmpty(contract.SignedAgreementFileName))
            {
                var filePath = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "agreements",
                    contract.SignedAgreementFileName
                );

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
