using Demo.Product.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.Product.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            _ = builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen();

            _ = builder.Services.AddDbContext<AppDbContext>(options =>
            {
                _ = options
                    .UseSqlServer(builder.Configuration.GetConnectionString("Default"), opt => opt.EnableRetryOnFailure(3))
                    .LogTo(Console.WriteLine)
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging();
            });

            WebApplication app = builder.Build();

            using IServiceScope serviceScope = app.Services.CreateScope();
            IServiceProvider serviceProvider = serviceScope.ServiceProvider;
            AppDbContext appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
            // _ = appDbContext.Database.EnsureDeleted();
            _ = appDbContext.Database.EnsureCreated();

            appDbContext.Products.AddRange(
                new Domain.Product { Name = "Apple" },
                 new Domain.Product { Name = "Orange" },
                 new Domain.Product { Name = "Mango" },
                 new Domain.Product { Name = "Avocado" }
                );
            _ = appDbContext.SaveChanges();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI();
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseAuthorization();


            _ = app.MapControllers();

            app.Run();
        }
    }
}