using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Serilog;

namespace Net10Auth.API.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

// public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
// {
//     public ApplicationDbContext CreateDbContext(string[] args)
//     {
//         var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
//
//         // 🛠 Build configuration from appsettings files
//         var configuration = new ConfigurationBuilder()
//             .SetBasePath(Directory.GetCurrentDirectory())
//             .AddJsonFile("appsettings.json", optional: false)
//             .AddJsonFile($"appsettings.{environment}.json", optional: true)
//             .AddEnvironmentVariables()
//             .Build();
//
//
//         var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//         optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
//
//         return new ApplicationDbContext(optionsBuilder.Options);
//     }
// }