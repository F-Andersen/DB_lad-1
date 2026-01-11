using Microsoft.EntityFrameworkCore;
using BD_LAB_TEST_PROJECT.Data.Repositories;
using BD_LAB_TEST_PROJECT.Exceptions;

namespace BD_LAB_TEST_PROJECT.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Database"));
            });

            services.AddScoped<ITravelPlanRepository, TravelPlanRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();

            services.AddExceptionHandler<CustomExceptionHandler>();

            return services;
        }
    }
}
