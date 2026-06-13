using glms_backend_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMove.Enums;
using TechMove.Models;
using TechMove.Repositories;

namespace TechMove.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly FileValidationService _fileValidationService;

        public ContractsController(
            IContractRepository contractRepository,
            IWebHostEnvironment environment,
            FileValidationService fileValidationService)
        {
            _contractRepository = contractRepository;
            _environment = environment;
            _fileValidationService = fileValidationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetContracts(
            DateTime? startDate,
            DateTime? endDate,
            ContractStatus? status)
        {
            var contracts = await _contractRepository.GetAllAsync(startDate, endDate, status);
            return Ok(contracts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContract(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

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
                return BadRequest("Please upload a signed agreement PDF.");

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

            var createdContract = await _contractRepository.CreateAsync(contract);

            return CreatedAtAction(
                nameof(GetContract),
                new { id = createdContract.ContractId },
                createdContract
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(
            int id,
            [FromForm] ContractCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contract = await _contractRepository.GetPlainByIdAsync(id);

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

            var updatedContract = await _contractRepository.UpdateAsync(contract);

            return Ok(updatedContract);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateContractStatus(
            int id,
            [FromBody] ContractStatus status)
        {
            var contract = await _contractRepository.GetPlainByIdAsync(id);

            if (contract == null)
                return NotFound();

            contract.Status = status;

            var updatedContract = await _contractRepository.UpdateAsync(contract);

            return Ok(updatedContract);
        }

        [HttpGet("{id}/agreement")]
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _contractRepository.GetPlainByIdAsync(id);

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
            var contract = await _contractRepository.GetPlainByIdAsync(id);

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

            await _contractRepository.DeleteAsync(contract);

            return NoContent();
        }
    }
}