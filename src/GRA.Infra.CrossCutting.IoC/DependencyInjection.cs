using FluentValidation;
using GRA.Application.Interfaces;
using GRA.Application.Security;
using GRA.Application.Services;
using GRA.Application.Validators;
using GRA.Domain.Repositories;
using GRA.Domain.Security;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GRA.Infra.CrossCutting.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddCrossCutting(this IServiceCollection services)
    {
        services.AddPersistence();
        services.AddSecurity();
        services.AddApplication();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddDbContext<GRAContext>(options =>
            options.UseInMemoryDatabase("GRA"));

        services.AddScoped<IOficinaRepository, OficinaRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();
        services.AddScoped<ITipoServicoRepository, TipoServicoRepository>();
        services.AddScoped<IServicoRepository, ServicoRepository>();
        services.AddScoped<IPecaRepository, PecaRepository>();
        services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
        services.AddScoped<IOrdemServicoServicoRepository, OrdemServicoServicoRepository>();
        services.AddScoped<IOrcamentoRepository, OrcamentoRepository>();
        services.AddScoped<IMovimentacaoEstoqueRepository, MovimentacaoEstoqueRepository>();

        return services;
    }

    private static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGeneratorAdapter>();

        return services;
    }

    private static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOficinaAppService, OficinaAppService>();
        services.AddScoped<IFuncionarioAppService, FuncionarioAppService>();
        services.AddScoped<IClienteAppService, ClienteAppService>();
        services.AddScoped<IAuthAppService, AuthAppService>();
        services.AddScoped<IPecaAppService, PecaAppService>();

        services.AddValidatorsFromAssemblyContaining<CadastrarOficinaDtoValidator>();

        return services;
    }
}