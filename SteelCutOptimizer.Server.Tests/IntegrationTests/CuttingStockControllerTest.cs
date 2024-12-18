using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using SteelCutOptimizer.Server;

namespace IntegrationTests{
    public class CuttingStockControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CuttingStockControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SolveViableProblem()
        {
            // Arrange
            List<StockItem> stockItems = new List<StockItem>()
            {
                new StockItem { Length = 1000 }
            };
            List<OrderItem> orderItems = new List<OrderItem>()
            {
                new OrderItem { Length = 275, Count = 20 },
                new OrderItem { Length = 300, Count = 20, MaxRelax = 15},
                new OrderItem { Length = 310, Count = 20 },
                new OrderItem { Length = 476, Count = 20, MaxRelax = 4 },
            };
            var problemData = new CuttingStockProblemDataDTO { StockList = stockItems, OrderList = orderItems, AlgorithmSettings = new() };

            // Act
            Thread.Sleep(250); //avoid triggering requestRateLimiter
            var response = await _client.PostAsJsonAsync("/CuttingStock", problemData);

            // Assert
            response.EnsureSuccessStatusCode();
            var dto = await response.Content.ReadFromJsonAsync<CuttingStockResultsDTO>();
            Assert.NotNull(dto);
            Assert.NotNull(dto.Patterns);
        }

        [Fact]
        public async Task SolveUnknownProblem()
        {
            // Arrange
            var problemData = new CuttingStockProblemDataDTO
            {
                AlgorithmSettings = { MainObjective = "empty", RelaxationType = "empty"}
            };

            // Act
            Thread.Sleep(250); //avoid triggering requestRateLimiter
            var response = await _client.PostAsJsonAsync("/CuttingStock", problemData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(errorMessage);
        }
    }
}