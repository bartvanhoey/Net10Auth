using Microsoft.EntityFrameworkCore;
using Net10Auth.API.Database;

namespace Net10Auth.API.Services.Registration;

public static class DatabaseRegistration
{
    public static void SetupDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string not found");
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
    }

    

}