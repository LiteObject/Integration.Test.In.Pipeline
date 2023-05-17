using Demo.Product.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Demo.Product.Test
{
    public class DatabaseFixture : IDisposable
    {
        public AppDbContext Context { get; private set; }

        public DatabaseFixture()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        .Build();

            string _connectionString = config.GetConnectionString("Default") ?? throw new ApplicationException("Connection string is not found in the settings file.");

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .LogTo(m => Console.WriteLine(m), Microsoft.Extensions.Logging.LogLevel.Information)
                .UseSqlServer(_connectionString, opt => opt.EnableRetryOnFailure(3))
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .Options;

            Context = new(options);

            _ = Context.Database.EnsureDeleted();
            _ = Context.Database.EnsureCreated();

            Context.Products.AddRange(
                    new Api.Domain.Product("Apple", 1.1m),
                    new Api.Domain.Product("Orange", 2.2m),
                    new Api.Domain.Product("Mango", 3.3m),
                    new Api.Domain.Product("Avocado", 4.4m));

            _ = Context.SaveChanges();
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Database.CloseConnection();
                Context.Dispose();
            }
        }
    }
}
