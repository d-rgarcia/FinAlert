using FinAlert.Identity.Core.Domain;
using FinAlert.Identity.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinAlert.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIdentityServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IdentityContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<IdentityContext>()
        .AddDefaultTokenProviders();
    }
}
