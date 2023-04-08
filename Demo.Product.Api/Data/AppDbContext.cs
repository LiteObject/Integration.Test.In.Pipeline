using Microsoft.EntityFrameworkCore;

namespace Demo.Product.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Domain.Product> Products { get; set; }
    }
}
