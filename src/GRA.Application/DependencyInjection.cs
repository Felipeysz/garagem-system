using GRA.Application.Interfaces;
using GRA.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GRA.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOficinaAppService, OficinaAppService>();

        return services;
    }
}