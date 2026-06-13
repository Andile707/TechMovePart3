using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;

namespace glms_backend_api.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly TechMoveDbContext _context;

        public ServiceRequestRepository(TechMoveDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRequestModel>> GetAllAsync()
        {
            return await _context.ServiceRequests
                .Include(s => s.Contract)
                .ToListAsync();
        }

        public async Task<ServiceRequestModel?> GetByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(s => s.ServiceRequestId == id);
        }

        public async Task<ServiceRequestModel?> GetPlainByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .FirstOrDefaultAsync(s => s.ServiceRequestId == id);
        }

        public async Task<ServiceRequestModel> CreateAsync(
            ServiceRequestModel serviceRequest)
        {
            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return serviceRequest;
        }

        public async Task<ServiceRequestModel> UpdateAsync(
            ServiceRequestModel serviceRequest)
        {
            _context.ServiceRequests.Update(serviceRequest);
            await _context.SaveChangesAsync();

            return serviceRequest;
        }

        public async Task DeleteAsync(ServiceRequestModel serviceRequest)
        {
            _context.ServiceRequests.Remove(serviceRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ServiceRequests
                .AnyAsync(s => s.ServiceRequestId == id);
        }
    }
}