using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;

namespace TechMove.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly TechMoveDbContext _context;

        public ClientRepository(TechMoveDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClientModel>> GetAllAsync()
        {
            return await _context.Clients
                .Include(c => c.Contracts)
                .ToListAsync();
        }

        public async Task<ClientModel?> GetByIdAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(c => c.ClientId == id);
        }

        public async Task<ClientModel> CreateAsync(ClientModel client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<bool> UpdateAsync(int id, ClientModel client)
        {
            if (id != client.ClientId)
                return false;

            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Clients.AnyAsync(c => c.ClientId == id);
        }
    }
}