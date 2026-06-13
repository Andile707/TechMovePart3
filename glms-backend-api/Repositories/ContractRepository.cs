using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Enums;
using TechMove.Models;

namespace glms_backend_api.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly TechMoveDbContext _context;

        public ContractRepository(TechMoveDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContractModel>> GetAllAsync(
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

            return await contracts.ToListAsync();
        }

        public async Task<ContractModel?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.ContractId == id);
        }

        public async Task<ContractModel?> GetPlainByIdAsync(int id)
        {
            return await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == id);
        }

        public async Task<ContractModel> CreateAsync(ContractModel contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<ContractModel> UpdateAsync(ContractModel contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task DeleteAsync(ContractModel contract)
        {
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
        }
    }
}