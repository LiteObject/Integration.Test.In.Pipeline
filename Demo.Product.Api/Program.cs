using Demo.Product.Api.Data;
using Demo.Product.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Demo.Product.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            _ = builder.Services.AddAutoMapper(typeof(Program));
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

            AuthSetup.Init(builder.Services);

            _ = builder.Services.AddScoped<IOrderService, OrderService>();

            WebApplication app = builder.Build();

            using IServiceScope serviceScope = app.Services.CreateScope();
            IServiceProvider serviceProvider = serviceScope.ServiceProvider;
            AppDbContext appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
            // _ = appDbContext.Database.EnsureDeleted();
            _ = appDbContext.Database.EnsureCreated();

            // Seed if the table is empty
            if (!appDbContext.Products.Any())
            {
                appDbContext.Products.AddRange(
                    new Domain.Product("Apple", 1.1m),
                    new Domain.Product("Orange", 2.2m),
                    new Domain.Product("Mango", 3.3m),
                    new Domain.Product("Avocado", 4.4m));

                _ = appDbContext.SaveChanges();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI();
                _ = app.MapFallback(() => Results.Redirect("/swagger"));
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseAuthorization();

            _ = app.MapControllers();

            app.Run();
        }
    }
}