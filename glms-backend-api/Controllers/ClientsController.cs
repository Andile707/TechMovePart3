using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.Models;
using TechMove.Repositories;

namespace TechMove.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientModel>>> GetClients()
        {
            var clients = await _clientRepository.GetAllAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientModel>> GetClient(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<ClientModel>> CreateClient(ClientModel client)
        {
            var createdClient = await _clientRepository.CreateAsync(client);

            return CreatedAtAction(
                nameof(GetClient),
                new { id = createdClient.ClientId },
                createdClient
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, ClientModel client)
        {
            if (id != client.ClientId)
                return BadRequest();

            try
            {
                var updated = await _clientRepository.UpdateAsync(id, client);

                if (!updated)
                    return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _clientRepository.ExistsAsync(id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var deleted = await _clientRepository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}