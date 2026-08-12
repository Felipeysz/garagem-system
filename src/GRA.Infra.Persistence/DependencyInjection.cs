using GRA.Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GRA.Infra.Persistence;

public static class DependencyInjection
{
    /// <summary>
    /// Registra a persistência usando o provider InMemory do EF Core.
    /// Repositórios/interfaces/serviços ficam para uma etapa futura -
    /// por ora o GRAContext já é suficiente para começar a trabalhar.
    /// </summary>
    public static IServiceCollection AddInfraPersistence(this IServiceCollection services)
    {
        services.AddDbContext<GRAContext>(options =>
            options.UseInMemoryDatabase("GRA"));

        return services;
    }
}
