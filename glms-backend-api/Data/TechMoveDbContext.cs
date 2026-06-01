using Microsoft.EntityFrameworkCore;
using TechMove.Models;

namespace TechMove.Data
{
    public class TechMoveDbContext : DbContext
    {
        public TechMoveDbContext(DbContextOptions<TechMoveDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClientModel> Clients => Set<ClientModel>();
        public DbSet<ContractModel> Contracts => Set<ContractModel>();
        public DbSet<ServiceRequestModel> ServiceRequests => Set<ServiceRequestModel>();
    }
}
