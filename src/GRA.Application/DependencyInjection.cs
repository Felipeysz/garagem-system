using FluentValidation;
using GRA.Application.Interfaces;
using GRA.Application.Services;
using GRA.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace GRA.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOficinaAppService, OficinaAppService>();
        services.AddScoped<IFuncionarioAppService, FuncionarioAppService>();

        services.AddValidatorsFromAssemblyContaining<CadastrarOficinaDtoValidator>();

        return services;
    }
}