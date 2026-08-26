using GRA.Domain.Security;
using Microsoft.Extensions.DependencyInjection;

namespace GRA.Infra.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraSecurity(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGeneratorAdapter>();

        return services;
    }
}