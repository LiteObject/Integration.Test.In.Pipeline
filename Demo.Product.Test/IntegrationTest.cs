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
            HttpResponseMessage response = await client.GetAsync("/ProductForecast");
            Stream responseBody = await response.Content.ReadAsStreamAsync();
            List<Api.Domain.Product>? products = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Api.Domain.Product>>(responseBody);

            // ASSERT
            _ = response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.NotNull(products);
            Assert.NotEmpty(products);
            Assert.True(products.Any());
        }

        [Fact]
        public async Task Get_Cities_Should_Returns200OK()
        {
            _output.WriteLine($">>> Executing test: {nameof(Get_Cities_Should_Returns200OK)}");

            // ARRANGE
            HttpClient client = _factory.CreateClient();

            // ACT
            HttpResponseMessage response = await client.GetAsync("/WeatherForecast/cities");
            string responseBody = await response.Content.ReadAsStringAsync();
            List<Api.Domain.Product>? cities = System.Text.Json.JsonSerializer.Deserialize<List<Api.Domain.Product>>(responseBody,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // ASSERT
            _ = response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.NotNull(cities);
            Assert.NotEmpty(cities);
            Assert.True(cities.Any());
        }

        [Theory]
        [InlineData("Frisco")]
        [InlineData("Dallas")]
        [InlineData("Fargo")]
        public async Task Database_Should_Have_Cities(string cityName)
        {
            _output.WriteLine($">>> Executing test: {nameof(Database_Should_Have_Cities)}");

            // ARRANGE

            // ACT
            List<Api.Domain.Product> cities = await this.fixture.Context.Products.ToListAsync();

            // ASSERT
            Assert.NotEmpty(cities);
            Assert.True(cities.Any());
            Assert.Contains(cities, c => c.Name == cityName);
        }
    }
}