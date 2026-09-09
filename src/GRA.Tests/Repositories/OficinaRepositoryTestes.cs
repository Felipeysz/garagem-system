using GRA.Domain.Entities;
using GRA.Infra.Persistence.Context;
using GRA.Infra.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GRA.Tests.Repositories;

public class OficinaRepositoryTestes
{
    private static GRAContext CriarContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<GRAContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new GRAContext(options);
    }

    [Fact]
    public async Task Adicionar_DeveInserirOficinaNoBanco()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var contextArranger = CriarContext(dbName))
        {
            var repository = new OficinaRepository(contextArranger);
            var novaOficina = new Oficina { Id = 1, Nome = "Oficina Central", CNPJ = "62892231000104" };


            await repository.AddAsync(novaOficina);
            await contextArranger.SaveChangesAsync();
        }


        using var contextAssert = CriarContext(dbName);
        var oficinaDoBanco = await contextAssert.Oficinas.FindAsync(1L);
        Assert.NotNull(oficinaDoBanco);
        Assert.Equal("Oficina Central", oficinaDoBanco.Nome);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarOficinaCorreta()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var contextArranger = CriarContext(dbName))
        {
            contextArranger.Oficinas.Add(new Oficina { Id = 2, Nome = "Oficina Norte", CNPJ = "24294201000107" });
            await contextArranger.SaveChangesAsync();
        }

        using var contextAct = CriarContext(dbName);
        var repository = new OficinaRepository(contextAct);
        var resultado = await repository.GetByIdAsync(2L);

        Assert.NotNull(resultado);
        Assert.Equal("Oficina Norte", resultado.Nome);
    }
}
