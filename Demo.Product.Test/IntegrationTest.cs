using Demo.Product.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Demo.Product.Test
{
    /// <summary>
    /// More on "Integration tests in ASP.NET Core":
    /// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
    /// </summary>
    [Collection("Database collection")]
    public class IntegrationTest :
        IClassFixture<DatabaseFixture>,
        IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private ITestOutputHelper _output;
        DatabaseFixture fixture;

        public IntegrationTest(ITestOutputHelper output, WebApplicationFactory<Program> factory, DatabaseFixture databaseFixture)
        {
            this._output = output;
            this._factory = factory;
            this.fixture = databaseFixture;
        }

        [Fact]
        public async Task Get_Products_Should_Returns200OK()
        {
            _output.WriteLine($">>> Executing test: {nameof(Get_Products_Should_Returns200OK)}");

            // ARRANGE
            HttpClient client = _factory.CreateClient();

            // ACT
            HttpResponseMessage response = await client.GetAsync("/products");
            Stream responseBody = await response.Content.ReadAsStreamAsync();
            List<Api.Domain.Product>? products = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Api.Domain.Product>>(responseBody);

            // ASSERT
            _ = response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.NotNull(products);
            Assert.NotEmpty(products);
            Assert.True(products.Any());
        }

        [Fact]
        public async Task Get_Product_By_Id_Should_Returns200OK()
        {
            _output.WriteLine($">>> Executing test: {nameof(Get_Product_By_Id_Should_Returns200OK)}");

            // ARRANGE
            HttpClient client = _factory.CreateClient();

            // ACT
            HttpResponseMessage response = await client.GetAsync("/products");
            Stream responseBody = await response.Content.ReadAsStreamAsync();
            Api.Domain.Product? product = await System.Text.Json.JsonSerializer.DeserializeAsync<Api.Domain.Product>(responseBody);

            // ASSERT
            _ = response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.NotNull(product);
        }

        [Theory]
        [InlineData("Mango")]
        [InlineData("Orange")]
        [InlineData("Apple")]
        public async Task Database_Should_Have_Products(string productName)
        {
            _output.WriteLine($">>> Executing test: {nameof(Database_Should_Have_Products)}");

            // ARRANGE

            // ACT
            List<Api.Domain.Product> products = await this.fixture.Context.Products.ToListAsync();

            // ASSERT
            Assert.NotEmpty(products);
            Assert.True(products.Any());
            Assert.Contains(products, c => c.Name == productName);
        }
    }
}