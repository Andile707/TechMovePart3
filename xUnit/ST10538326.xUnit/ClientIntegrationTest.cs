using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using TechMove.Models;
using Xunit;
using glms_backend_api;

namespace TechMove.Tests
{
    public class ClientIntegrationTests
        : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ClientIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetClients_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/api/clients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateClient_ThenGetClient_ReturnsCreatedClient()
        {
            var newClient = new ClientModel
            {
                Name = "Integration Test Client",
                TelephoneNumber = "0123456789",
                Email = "integration@test.com",
                Region = "Gauteng"
            };

            var postResponse =
                await _client.PostAsJsonAsync("/api/clients", newClient);

            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var createdClient =
                await postResponse.Content.ReadFromJsonAsync<ClientModel>();

            Assert.NotNull(createdClient);
            Assert.True(createdClient!.ClientId > 0);

            var getResponse =
                await _client.GetAsync(
                    $"/api/clients/{createdClient.ClientId}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var fetchedClient =
                await getResponse.Content.ReadFromJsonAsync<ClientModel>();

            Assert.NotNull(fetchedClient);

            Assert.Equal(
                createdClient.ClientId,
                fetchedClient!.ClientId);

            Assert.Equal(
                "Integration Test Client",
                fetchedClient.Name);

            Assert.Equal(
                "0123456789",
                fetchedClient.TelephoneNumber);

            Assert.Equal(
                "integration@test.com",
                fetchedClient.Email);

            Assert.Equal(
                "Gauteng",
                fetchedClient.Region);
        }
    }
}