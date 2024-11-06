using FinAlert.AlertStore.Core.Contracts;
using FinAlert.AlertStore.Infrastructure;
using FinAlert.AlertStore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinAlert.AlertServices.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAlertServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AlertDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IAlertRepository, AlertRepository>();
            services.AddScoped<IPriceAlertService, PriceAlertService>();

            return services;
        }
    }
}