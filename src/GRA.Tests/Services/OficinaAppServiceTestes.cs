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

public class OficinaAppServiceTestes
{
    private readonly Mock<IOficinaRepository> _oficinaRepositoryMock;
    private readonly Mock<IValidator<CadastrarOficinaDto>> _cadastrarValidatorMock;
    private readonly Mock<IValidator<AtualizarOficinaDto>> _atualizarValidatorMock;
    private readonly OficinaAppService _subject;

    public OficinaAppServiceTestes()
    {
        _oficinaRepositoryMock = new Mock<IOficinaRepository>();
        _cadastrarValidatorMock = new Mock<IValidator<CadastrarOficinaDto>>();
        _atualizarValidatorMock = new Mock<IValidator<AtualizarOficinaDto>>();

        _subject = new OficinaAppService(
            _oficinaRepositoryMock.Object,
            _cadastrarValidatorMock.Object,
            _atualizarValidatorMock.Object
        );
    }

    private static Oficina CriarOficinaExistente(long id = 1) => new()
    {
        Id = id,
        Nome = "Oficina Central",
        Slug = "oficina-central",
        CNPJ = "12345678000199",
        Email = "contato@oficinacentral.com",
        Ativo = true
    };

    #region CadastrarAsync

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErrosDeValidacao()
    {
        var dto = new CadastrarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "12345678000199",
                Telefone: "1133334444",
                Email: null,
                Endereco: null
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
        _oficinaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Oficina>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErro_QuandoNomeJaExisteAtiva()
    {
        var dto = new CadastrarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "12345678000199",
                Telefone: "1133334444",
                Email: null,
                Endereco: null
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        // Email null -> só existem DUAS chamadas a ExisteAsync (Nome, depois CNPJ)
        // Nome -> true (o que este teste cobre); CNPJ não é o foco -> false
        _oficinaRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Contains("Já existe uma oficina ativa cadastrada com esse nome.", result.Erros);
        _oficinaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Oficina>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErro_QuandoCnpjJaExisteAtiva()
    {
        var dto = new CadastrarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "12345678000199",
                Telefone: "1133334444",
                Email: null,
                Endereco: null
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        // Nome não é o foco -> false; CNPJ -> true (o que este teste cobre)
        _oficinaRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Contains("Já existe uma oficina ativa cadastrada com esse CNPJ.", result.Erros);
        _oficinaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Oficina>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErro_QuandoEmailJaExisteAtiva()
    {
        var dto = new CadastrarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "12345678000199",
                Telefone: "1133334444",
                Email: "contato@oficinacentral.com",
                Endereco: null
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        // Nome e CNPJ não são o foco -> false, false; Email -> true (o que este teste cobre)
        _oficinaRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Contains("Já existe uma oficina ativa cadastrada com esse email.", result.Erros);
        _oficinaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Oficina>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_NaoDeveChecarEmail_QuandoEmailNaoInformado()
    {
        var dto = new CadastrarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "12345678000199",
                Telefone: "1133334444",
                Email: null,
                Endereco: null
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(false);

        var result = await _subject.CadastrarAsync(dto);

        // Só deve ter checado Nome e CNPJ; a checagem de email é pulada quando Email é null
        _oficinaRepositoryMock.Verify(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()), Times.Exactly(2));
        Assert.Empty(result.Erros);
    }

    [Fact]
    public async Task CadastrarAsync_DeveGerarSlug_ComAcentosEEspacosENormalizado()
    {
        var dto = new CadastrarOficinaDto(
                Nome: "Oficina São Paulo - Centro",
                CNPJ: "12345678000199",
                Telefone: null,
                Email: null,
                Endereco: null
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(false);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Equal("oficina-sao-paulo-centro", result.Data!.Slug);
    }

    [Fact]
    public async Task CadastrarAsync_DeveCadastrarComSucesso_QuandoDadosValidos()
    {
        var dto = new CadastrarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "  12345678000199  ",
                Telefone: "1133334444",
                Email: "contato@oficinacentral.com",
                Endereco: null
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(false);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Empty(result.Erros);
        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Oficina Central", result.Data!.Nome);
        Assert.Equal("12345678000199", result.Data.CNPJ);
        _oficinaRepositoryMock.Verify(r => r.AddAsync(It.Is<Oficina>(o => o.CNPJ == "12345678000199" && o.Slug == "oficina-central")), Times.Once);
        _oficinaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region AtualizarAsync

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErrosDeValidacao()
    {
        var dto = new AtualizarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "12345678000199",
                Telefone: "1133334444",
                Email: null,
                Endereco: null
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
        _oficinaRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarNaoEncontrado_QuandoOficinaNaoExiste()
    {
        var dto = new AtualizarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "12345678000199",
                Telefone: "1133334444",
                Email: null,
                Endereco: null
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Oficina?)null);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Oficina não encontrada.", result.Erros);
        _oficinaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErro_QuandoNomeJaExisteAtiva()
    {
        var oficinaExistente = CriarOficinaExistente();
        var dto = new AtualizarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "12345678000199",
                Telefone: "1133334444",
                Email: null,
                Endereco: null
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(oficinaExistente);

        _oficinaRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Contains("Já existe uma oficina ativa cadastrada com esse nome.", result.Erros);
        _oficinaRepositoryMock.Verify(r => r.Update(It.IsAny<Oficina>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErro_QuandoCnpjJaExisteAtiva()
    {
        var oficinaExistente = CriarOficinaExistente();
        var dto = new AtualizarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "12345678000199",
                Telefone: "1133334444",
                Email: null,
                Endereco: null
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(oficinaExistente);

        _oficinaRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Contains("Já existe uma oficina ativa cadastrada com esse CNPJ.", result.Erros);
        _oficinaRepositoryMock.Verify(r => r.Update(It.IsAny<Oficina>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErro_QuandoEmailJaExisteAtiva()
    {
        var oficinaExistente = CriarOficinaExistente();
        var dto = new AtualizarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "12345678000199",
                Telefone: "1133334444",
                Email: "outro@email.com",
                Endereco: null
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(oficinaExistente);

        _oficinaRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Contains("Já existe uma oficina ativa cadastrada com esse email.", result.Erros);
        _oficinaRepositoryMock.Verify(r => r.Update(It.IsAny<Oficina>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_NaoDeveAlterarEndereco_QuandoEnderecoNaoInformado()
    {
        var oficinaExistente = CriarOficinaExistente();
        var enderecoOriginal = new Endereco
        {
            Logradouro = "Rua A",
            Numero = "100",
            Bairro = "Centro",
            Cidade = "São Paulo",
            Estado = "SP",
            CEP = "01000000"
        };
        oficinaExistente.Endereco = enderecoOriginal;

        var dto = new AtualizarOficinaDto(
                Nome: "Oficina Central",
                CNPJ: "12345678000199",
                Telefone: "1133334444",
                Email: null,
                Endereco: null
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(oficinaExistente);

        _oficinaRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(false);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Same(enderecoOriginal, oficinaExistente.Endereco);
        Assert.Equal("Rua A", result.Data!.Endereco!.Value.Logradouro);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarComSucesso_QuandoDadosValidos()
    {
        var oficinaExistente = CriarOficinaExistente();
        var novoEndereco = new EnderecoDto(
                Logradouro: "Rua Nova",
                Numero: "200",
                Complemento: null,
                Bairro: "Jardins",
                Cidade: "São Paulo",
                Estado: "SP",
                CEP: "02000000"
            );

        var dto = new AtualizarOficinaDto(
                Nome: "Oficina Central Renovada",
                CNPJ: "12345678000199",
                Telefone: "1133335555",
                Email: null,
                Endereco: novoEndereco
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(oficinaExistente);

        _oficinaRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(false);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Empty(result.Erros);
        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Oficina Central Renovada", result.Data!.Nome);
        Assert.Equal("oficina-central-renovada", result.Data.Slug);
        Assert.Equal("Rua Nova", result.Data.Endereco!.Value.Logradouro);
        _oficinaRepositoryMock.Verify(r => r.Update(It.Is<Oficina>(o => o.Nome == "Oficina Central Renovada")), Times.Once);
        _oficinaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region AtivarAsync

    [Fact]
    public async Task AtivarAsync_DeveRetornarNaoEncontrado_QuandoOficinaNaoExiste()
    {
        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Oficina?)null);

        var result = await _subject.AtivarAsync(1);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Oficina não encontrada.", result.Erros);
        _oficinaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtivarAsync_DeveAtivarComSucesso_QuandoOficinaExiste()
    {
        var oficinaExistente = CriarOficinaExistente();
        oficinaExistente.Ativo = false;

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(oficinaExistente);

        var result = await _subject.AtivarAsync(1);

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Oficina ativada com sucesso", result.Data);
        Assert.True(oficinaExistente.Ativo);
        _oficinaRepositoryMock.Verify(r => r.Update(oficinaExistente), Times.Once);
        _oficinaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region InativarAsync

    [Fact]
    public async Task InativarAsync_DeveRetornarNaoEncontrado_QuandoOficinaNaoExiste()
    {
        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Oficina?)null);

        var result = await _subject.InativarAsync(1);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Oficina não encontrada.", result.Erros);
        _oficinaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task InativarAsync_DeveInativarComSucesso_QuandoOficinaExiste()
    {
        var oficinaExistente = CriarOficinaExistente();

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(oficinaExistente);

        var result = await _subject.InativarAsync(1);

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Oficina inativada com sucesso", result.Data);
        Assert.False(oficinaExistente.Ativo);
        _oficinaRepositoryMock.Verify(r => r.Update(oficinaExistente), Times.Once);
        _oficinaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region BuscarPorSlugAsync

    [Fact]
    public async Task BuscarPorSlugAsync_DeveRetornarNaoEncontrado_QuandoOficinaNaoExiste()
    {
        _oficinaRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync((Oficina?)null);

        var result = await _subject.BuscarPorSlugAsync("oficina-central");

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Oficina não encontrada.", result.Erros);
    }

    [Fact]
    public async Task BuscarPorSlugAsync_DeveRetornarOficinaComSucesso_QuandoOficinaExiste()
    {
        var oficinaExistente = CriarOficinaExistente();

        _oficinaRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(oficinaExistente);

        var result = await _subject.BuscarPorSlugAsync("oficina-central");

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal(oficinaExistente.Nome, result.Data!.Nome);
    }

    #endregion

    #region BuscarPorNomeAsync

    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarNaoEncontrado_QuandoOficinaNaoExiste()
    {
        _oficinaRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync((Oficina?)null);

        var result = await _subject.BuscarPorNomeAsync("Oficina Central");

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Oficina não encontrada.", result.Erros);
    }

    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarOficinaComSucesso_QuandoOficinaExiste()
    {
        var oficinaExistente = CriarOficinaExistente();

        _oficinaRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Oficina, bool>>>()))
            .ReturnsAsync(oficinaExistente);

        var result = await _subject.BuscarPorNomeAsync("Oficina Central");

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal(oficinaExistente.CNPJ, result.Data!.CNPJ);
    }

    #endregion
}
