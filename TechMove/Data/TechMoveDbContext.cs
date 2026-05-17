using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TechMove.Models;

namespace TechMove.Data
{
    public class TechMoveDbContext : DbContext
    {


        public DbSet<ClientModel> Clients => Set<ClientModel>();
        public DbSet<ContractModel> Contracts => Set<ContractModel>();
        public DbSet<ServiceRequestModel> ServiceRequests => Set<ServiceRequestModel>();
        public TechMoveDbContext(DbContextOptions<TechMoveDbContext> options) : base(options)
        {
        }
    }   
}
