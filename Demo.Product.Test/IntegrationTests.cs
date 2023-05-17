using Demo.Product.Api;
using Demo.Product.Test.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace Demo.Product.Test
{
    /// <summary>
    /// More on "Integration tests in ASP.NET Core":
    /// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
    /// </summary>
    [Collection("Database collection")]
    public class IntegrationTests :
        IClassFixture<DatabaseFixture>,
        IClassFixture<WebAppFactory>
    {
        private readonly WebAppFactory _factory;
        private ITestOutputHelper _output;
        DatabaseFixture fixture;

        public IntegrationTests(ITestOutputHelper output, WebAppFactory factory, DatabaseFixture databaseFixture)
        {
            this._output = output;
            this._factory = factory;
            this.fixture = databaseFixture;
        }

        [Fact]
        public async Task Get_Products_Should_Return200OK()
        {
            _output.WriteLine($">>> Executing test: {nameof(Get_Products_Should_Return200OK)}");

            // ARRANGE
            HttpClient client = _factory.CreateDefaultClient(new LoggingHandler(new HttpClientHandler(), _output));
            // client.DefaultRequestHeaders.Authorization = new(TestAuthenticationHandler.AuthenticationSchema);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // ACT
            HttpResponseMessage response = await client.GetAsync("/products");
            // _output.WriteLine($"Response:\n{await response.Content.ReadAsStringAsync()}");

            Stream responseBody = await response.Content.ReadAsStreamAsync();
            List<Api.DTO.Product>? products = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Api.DTO.Product>>(responseBody, options);

            // ASSERT
            _ = response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.NotNull(products);
            Assert.NotEmpty(products);
            Assert.True(products.Any());
            Assert.All(products, p => Assert.True(p.Name.Length > 0));
            Assert.All(products, p => Assert.True(p.UnitPrice > 0));
        }

        [Fact]
        public async Task Get_Product_By_Id_Should_Return200OK()
        {
            _output.WriteLine($">>> Executing test: {nameof(Get_Product_By_Id_Should_Return200OK)}");

            // ARRANGE
            HttpClient client = _factory.CreateDefaultClient(new LoggingHandler(new HttpClientHandler(), _output));
            // HttpClient client = _factory.CreateClient();
            // client.DefaultRequestHeaders.Authorization = new(TestAuthenticationHandler.AuthenticationSchema);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // ACT
            HttpResponseMessage response = await client.GetAsync("/products/1");
            Stream responseBody = await response.Content.ReadAsStreamAsync();
            Api.DTO.Product? product = await System.Text.Json.JsonSerializer.DeserializeAsync<Api.DTO.Product>(responseBody, options);

            // ASSERT
            _ = response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.NotNull(product);
            Assert.True(product.Id > 0);
        }

        [Fact]
        public async Task Post_Product_Should_Return201()
        {
            // ARRANGE
            HttpClient client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new(TestAuthenticationHandler.AuthenticationSchema);
            Api.Domain.Product newProduct = new("Test Product", 1.99m);

            HttpContent? content = new StringContent(JsonConvert.SerializeObject(newProduct), Encoding.UTF8, "application/json");

            // ACT
            HttpResponseMessage response = await client.PostAsync("/products", content);
            _output.WriteLine($"StatusCode: {(int)response.StatusCode} - {response.ReasonPhrase}");
            _output.WriteLine(await response.Content.ReadAsStringAsync());

            // ASSERT            
            Assert.True(response.IsSuccessStatusCode);
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

        /// <summary>
        /// This class is here ONLY for debugging purpose.
        /// </summary>
        public class LoggingHandler : DelegatingHandler
        {
            private ITestOutputHelper _output;

            public LoggingHandler(HttpMessageHandler innerHandler, ITestOutputHelper output)
                : base(innerHandler)
            {
                _output = output;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                _output.WriteLine("\nREQUEST:");
                _output.WriteLine(request.ToString());

                if (request.Headers.Count() > 0)
                {
                    _output.WriteLine("Request Header");
                    foreach (var header in request.Headers)
                    {
                        _output.WriteLine($"{header.Key}: {header.Value}");
                    }
                }

                if (request.Content != null)
                {
                    _output.WriteLine(await request.Content.ReadAsStringAsync());
                }

                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

                _output.WriteLine("\nRESPONSE:");
                _output.WriteLine(response.ToString());

                if (response.Content != null)
                {
                    _output.WriteLine(await response.Content.ReadAsStringAsync());
                }

                return response;
            }
        }
    }
}