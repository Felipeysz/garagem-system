using GRA.Application;
using GRA.Infra.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace GRA.Infra.CrossCutting.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddCrossCutting(this IServiceCollection services)
    {
        services.AddInfraPersistence();
        services.AddApplicationServices();

        return services;
    }
}