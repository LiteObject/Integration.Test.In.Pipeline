using Demo.Product.Api.Data;
using Demo.Product.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Demo.Product.Test
{
    public class DatabaseFixture : IDisposable
    {
        public AppDbContext Context { get; private set; }

        //private readonly ITestOutputHelper _output;

        public DatabaseFixture()
        {
            // this._output = output;

            IConfigurationRoot config = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        .Build();

            string _connectionString = config.GetConnectionString("Default");

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .LogTo(m => Console.WriteLine(m), Microsoft.Extensions.Logging.LogLevel.Information)
                .UseSqlServer(_connectionString, opt => opt.EnableRetryOnFailure(3))
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .Options;

            Context = new(options);

            _ = Context.Database.EnsureCreated();

            Context.Products.AddRange(
                    new Api.Domain.Product { Name = "Dallas" },
                    new Api.Domain.Product { Name = "Frisco" },
                    new Api.Domain.Product { Name = "Addison" },
                    new Api.Domain.Product { Name = "Fargo" });

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
