using FluentValidation;
using FluentValidation.Results;
using GRA.Application.DTOs;
using GRA.Application.Services;
using GRA.Application.Wrappers;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using Moq;
using System.Linq.Expressions;

namespace GRA.Tests.Services;

public class PecaAppServiceTestes
{
    private readonly Mock<IPecaRepository> _pecaRepositoryMock;
    private readonly Mock<IOficinaRepository> _oficinaRepositoryMock;
    private readonly Mock<IValidator<CadastrarPecaDto>> _cadastrarValidatorMock;
    private readonly Mock<IValidator<AtualizarPecaDto>> _atualizarValidatorMock;
    private readonly PecaAppService _subject;

    public PecaAppServiceTestes()
    {
        _pecaRepositoryMock = new Mock<IPecaRepository>();
        _oficinaRepositoryMock = new Mock<IOficinaRepository>();
        _cadastrarValidatorMock = new Mock<IValidator<CadastrarPecaDto>>();
        _atualizarValidatorMock = new Mock<IValidator<AtualizarPecaDto>>();

        _subject = new PecaAppService(
            _pecaRepositoryMock.Object,
            _oficinaRepositoryMock.Object,
            _cadastrarValidatorMock.Object,
            _atualizarValidatorMock.Object
        );
    }

    private static Oficina CriarOficinaAtiva(long id = 10) => new()
    {
        Id = id,
        Nome = "Oficina Central",
        CNPJ = "12345678000199",
        Ativo = true
    };

    private static Peca CriarPecaExistente(long id = 1, long oficinaId = 10) => new()
    {
        Id = id,
        OficinaId = oficinaId,
        Nome = "Pastilha de Freio",
        CodigoInterno = "PF-001",
        EstoqueMinimo = 5,
        PrecoVenda = 89.90m,
        Ativo = true
    };

    #region CadastrarAsync

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErrosDeValidacao()
    {
        var dto = new CadastrarPecaDto(
                OficinaId: 10,
                Nome: "Pastilha de Freio",
                Descricao: null,
                CodigoInterno: null,
                UnidadeMedida: null,
                PrecoVenda: 89.90m,
                EstoqueMinimo: 5
            );
        var errors = new List<ValidationFailure>
            {
                new("Nome", "Nome é obrigatório")
            };

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult(errors));

        var result = await _subject.CadastrarAsync(dto);

        Assert.Equal(errors.Select(e => e.ErrorMessage), result.Erros);
        _oficinaRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
        _pecaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Peca>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarNaoEncontrado_QuandoOficinaNaoExiste()
    {
        var dto = new CadastrarPecaDto(
                OficinaId: 10,
                Nome: "Pastilha de Freio",
                Descricao: null,
                CodigoInterno: null,
                UnidadeMedida: null,
                PrecoVenda: 89.90m,
                EstoqueMinimo: 5
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync((Oficina?)null);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Oficina informada não existe.", result.Erros);
        _pecaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Peca>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErro_QuandoCodigoInternoJaExisteAtiva()
    {
        var dto = new CadastrarPecaDto(
                OficinaId: 10,
                Nome: "Pastilha de Freio",
                Descricao: null,
                CodigoInterno: "PF-001",
                UnidadeMedida: null,
                PrecoVenda: 89.90m,
                EstoqueMinimo: 5
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(CriarOficinaAtiva());

        _pecaRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Peca, bool>>>()))
            .ReturnsAsync(true);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Contains("Já existe uma peça ativa com esse código interno nessa oficina.", result.Erros);
        _pecaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Peca>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_NaoDeveChecarCodigoInterno_QuandoCodigoInternoNaoInformado()
    {
        var dto = new CadastrarPecaDto(
                OficinaId: 10,
                Nome: "Pastilha de Freio",
                Descricao: null,
                CodigoInterno: null,
                UnidadeMedida: null,
                PrecoVenda: 89.90m,
                EstoqueMinimo: 5
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(CriarOficinaAtiva());

        var result = await _subject.CadastrarAsync(dto);

        // Sem código interno, a checagem de duplicidade é pulada inteiramente
        _pecaRepositoryMock.Verify(r => r.ExisteAsync(It.IsAny<Expression<Func<Peca, bool>>>()), Times.Never);
        Assert.Empty(result.Erros);
    }

    [Fact]
    public async Task CadastrarAsync_DeveCadastrarComSucesso_QuandoDadosValidos()
    {
        var dto = new CadastrarPecaDto(
                OficinaId: 10,
                Nome: "Pastilha de Freio",
                Descricao: "Pastilha dianteira",
                CodigoInterno: "PF-001",
                UnidadeMedida: "UN",
                PrecoVenda: 89.90m,
                EstoqueMinimo: 5
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(CriarOficinaAtiva());

        _pecaRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Peca, bool>>>()))
            .ReturnsAsync(false);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Empty(result.Erros);
        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Pastilha de Freio", result.Data!.Nome);
        Assert.Equal("PF-001", result.Data.CodigoInterno);
        _pecaRepositoryMock.Verify(r => r.AddAsync(It.Is<Peca>(p => p.OficinaId == 10 && p.CodigoInterno == "PF-001")), Times.Once);
        _pecaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region AtualizarAsync

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErrosDeValidacao()
    {
        var dto = new AtualizarPecaDto(
                Nome: "Pastilha de Freio",
                Descricao: null,
                CodigoInterno: null,
                UnidadeMedida: null,
                PrecoVenda: 89.90m,
                EstoqueMinimo: 5
            );
        var errors = new List<ValidationFailure>
            {
                new("Nome", "Nome é obrigatório")
            };

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult(errors));

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Equal(errors.Select(e => e.ErrorMessage), result.Erros);
        _pecaRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarNaoEncontrado_QuandoPecaNaoExiste()
    {
        var dto = new AtualizarPecaDto(
                Nome: "Pastilha de Freio",
                Descricao: null,
                CodigoInterno: null,
                UnidadeMedida: null,
                PrecoVenda: 89.90m,
                EstoqueMinimo: 5
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _pecaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Peca?)null);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Peça não encontrada.", result.Erros);
        _pecaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErro_QuandoCodigoInternoJaExisteAtiva()
    {
        var pecaExistente = CriarPecaExistente();
        var dto = new AtualizarPecaDto(
                Nome: "Pastilha de Freio",
                Descricao: null,
                CodigoInterno: "PF-002",
                UnidadeMedida: null,
                PrecoVenda: 89.90m,
                EstoqueMinimo: 5
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _pecaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pecaExistente);

        _pecaRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Peca, bool>>>()))
            .ReturnsAsync(true);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Contains("Já existe uma peça ativa com esse código interno nessa oficina.", result.Erros);
        _pecaRepositoryMock.Verify(r => r.Update(It.IsAny<Peca>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_NaoDeveChecarCodigoInterno_QuandoCodigoInternoNaoInformado()
    {
        var pecaExistente = CriarPecaExistente();
        var dto = new AtualizarPecaDto(
                Nome: "Pastilha de Freio",
                Descricao: null,
                CodigoInterno: null,
                UnidadeMedida: null,
                PrecoVenda: 89.90m,
                EstoqueMinimo: 5
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _pecaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pecaExistente);

        var result = await _subject.AtualizarAsync(1, dto);

        _pecaRepositoryMock.Verify(r => r.ExisteAsync(It.IsAny<Expression<Func<Peca, bool>>>()), Times.Never);
        Assert.Empty(result.Erros);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarComSucesso_QuandoDadosValidos()
    {
        var pecaExistente = CriarPecaExistente();
        var dto = new AtualizarPecaDto(
                Nome: "Pastilha de Freio Cerâmica",
                Descricao: "Pastilha dianteira cerâmica",
                CodigoInterno: "PF-001",
                UnidadeMedida: "UN",
                PrecoVenda: 109.90m,
                EstoqueMinimo: 8
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _pecaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pecaExistente);

        _pecaRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Peca, bool>>>()))
            .ReturnsAsync(false);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Empty(result.Erros);
        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Pastilha de Freio Cerâmica", result.Data!.Nome);
        Assert.Equal(109.90m, result.Data.PrecoVenda);
        Assert.Equal(8, result.Data.EstoqueMinimo);
        _pecaRepositoryMock.Verify(r => r.Update(It.Is<Peca>(p => p.Nome == "Pastilha de Freio Cerâmica")), Times.Once);
        _pecaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region AtivarAsync

    [Fact]
    public async Task AtivarAsync_DeveRetornarNaoEncontrado_QuandoPecaNaoExiste()
    {
        _pecaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Peca?)null);

        var result = await _subject.AtivarAsync(1);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Peça não encontrada.", result.Erros);
        _pecaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtivarAsync_DeveAtivarComSucesso_QuandoPecaExiste()
    {
        var pecaExistente = CriarPecaExistente();
        pecaExistente.Ativo = false;

        _pecaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pecaExistente);

        var result = await _subject.AtivarAsync(1);

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Peça ativada com sucesso", result.Data);
        Assert.True(pecaExistente.Ativo);
        _pecaRepositoryMock.Verify(r => r.Update(pecaExistente), Times.Once);
        _pecaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region InativarAsync

    [Fact]
    public async Task InativarAsync_DeveRetornarNaoEncontrado_QuandoPecaNaoExiste()
    {
        _pecaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Peca?)null);

        var result = await _subject.InativarAsync(1);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Peça não encontrada.", result.Erros);
        _pecaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task InativarAsync_DeveInativarComSucesso_QuandoPecaExiste()
    {
        var pecaExistente = CriarPecaExistente();

        _pecaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pecaExistente);

        var result = await _subject.InativarAsync(1);

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Peça inativada com sucesso", result.Data);
        Assert.False(pecaExistente.Ativo);
        _pecaRepositoryMock.Verify(r => r.Update(pecaExistente), Times.Once);
        _pecaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region BuscarPorIdAsync

    [Fact]
    public async Task BuscarPorIdAsync_DeveRetornarNaoEncontrado_QuandoPecaNaoExiste()
    {
        _pecaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Peca?)null);

        var result = await _subject.BuscarPorIdAsync(1);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Peça não encontrada.", result.Erros);
    }

    [Fact]
    public async Task BuscarPorIdAsync_DeveRetornarPecaComSucesso_QuandoPecaExiste()
    {
        var pecaExistente = CriarPecaExistente();

        _pecaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(pecaExistente);

        var result = await _subject.BuscarPorIdAsync(1);

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal(pecaExistente.Nome, result.Data!.Nome);
        Assert.Equal(pecaExistente.CodigoInterno, result.Data.CodigoInterno);
    }

    #endregion
}
