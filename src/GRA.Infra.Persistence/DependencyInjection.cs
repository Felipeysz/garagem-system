using GRA.Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GRA.Infra.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraPersistence(this IServiceCollection services)
    {
        services.AddDbContext<GRAContext>(options =>
            options.UseInMemoryDatabase("GRA"));

        return services;
    }
}
