using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Enums;
using TechMove.Models;

namespace TechMove.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
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

        [HttpGet]
        public async Task<IActionResult> GetContracts(
            DateTime? startDate,
            DateTime? endDate,
            ContractStatus? status)
        {
            var contracts = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (startDate.HasValue)
                contracts = contracts.Where(c => c.StartDate >= startDate.Value);

            if (endDate.HasValue)
                contracts = contracts.Where(c => c.EndDate <= endDate.Value);

            if (status.HasValue)
                contracts = contracts.Where(c => c.Status == status.Value);

            return Ok(await contracts.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContract(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract == null)
                return NotFound();

            return Ok(contract);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContract([FromForm] ContractCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.SignedAgreement == null || model.SignedAgreement.Length == 0)
            {
                return BadRequest("Please upload a signed agreement PDF.");
            }

            if (!_fileValidationService.IsValidPdf(
                model.SignedAgreement.FileName,
                model.SignedAgreement.ContentType))
            {
                return BadRequest("Only PDF files are allowed.");
            }

            var uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "agreements"
            );

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName =
                $"{Guid.NewGuid()}_{Path.GetFileName(model.SignedAgreement.FileName)}";

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

            return CreatedAtAction(
                nameof(GetContract),
                new { id = contract.ContractId },
                contract
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(
            int id,
            [FromForm] ContractCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
                return NotFound();

            contract.ClientId = model.ClientId;
            contract.StartDate = model.StartDate;
            contract.EndDate = model.EndDate;
            contract.Status = model.Status;
            contract.ServiceLevel = model.ServiceLevel;

            if (model.SignedAgreement != null && model.SignedAgreement.Length > 0)
            {
                if (!_fileValidationService.IsValidPdf(
                    model.SignedAgreement.FileName,
                    model.SignedAgreement.ContentType))
                {
                    return BadRequest("Only PDF files are allowed.");
                }

                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "agreements"
                );

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                if (!string.IsNullOrEmpty(contract.SignedAgreementFileName))
                {
                    var oldFilePath = Path.Combine(
                        uploadsFolder,
                        contract.SignedAgreementFileName
                    );

                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                var uniqueFileName =
                    $"{Guid.NewGuid()}_{Path.GetFileName(model.SignedAgreement.FileName)}";

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

            return Ok(contract);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateContractStatus(
            int id,
            [FromBody] ContractStatus status)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
                return NotFound();

            contract.Status = status;
            await _context.SaveChangesAsync();

            return Ok(contract);
        }

        [HttpGet("{id}/agreement")]
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

            return PhysicalFile(
                filePath,
                "application/pdf",
                contract.SignedAgreementFileName
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
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
                    System.IO.File.Delete(filePath);
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}