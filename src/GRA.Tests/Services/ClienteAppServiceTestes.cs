using FluentValidation;
using FluentValidation.Results;
using GRA.Application.DTOs;
using GRA.Application.Services;
using GRA.Application.Wrappers;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Domain.Security;
using Moq;
using System.Linq.Expressions;

namespace GRA.Tests.Services;

public class ClienteAppServiceTestes
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IValidator<CadastrarClienteDto>> _cadastrarValidatorMock;
    private readonly Mock<IValidator<AtualizarClienteDto>> _atualizarValidatorMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly ClienteAppService _subject;

    public ClienteAppServiceTestes()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _cadastrarValidatorMock = new Mock<IValidator<CadastrarClienteDto>>();
        _atualizarValidatorMock = new Mock<IValidator<AtualizarClienteDto>>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _subject = new ClienteAppService(
            _clienteRepositoryMock.Object,
            _cadastrarValidatorMock.Object,
            _atualizarValidatorMock.Object,
            _passwordHasherMock.Object
        );
    }

    private static Cliente CriarClienteExistente(long id = 1) => new()
    {
        Id = id,
        Nome = "João Silva",
        CPF = "12345678900",
        SenhaHash = "hash-existente",
        Telefone = "11999999999",
        Email = "joao@email.com",
        Ativo = true
    };

    #region CadastrarAsync

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErrosDeValidacao()
    {
        var dto = new CadastrarClienteDto(
                Nome: "João Silva",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: null
            );
        var errors = new List<ValidationFailure>
            {
                new("Nome", "Nome é obrigatório"),
                new("CPF", "CPF inválido")
            };

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult(errors));

        var result = await _subject.CadastrarAsync(dto);

        Assert.True(result.Erros.Count != 0);
        Assert.Equal(errors.Select(e => e.ErrorMessage), result.Erros);
        _clienteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErro_QuandoCpfJaExisteAtivo()
    {
        var dto = new CadastrarClienteDto(
                Nome: "João Silva",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: null
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _clienteRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(true);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Contains("Já existe um cliente ativo cadastrado com esse CPF.", result.Erros);
        _clienteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErro_QuandoEmailJaExisteAtivo()
    {
        var dto = new CadastrarClienteDto(
                Nome: "João Silva",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: "joao@email.com"
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _clienteRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Contains("Já existe um cliente ativo cadastrado com esse email.", result.Erros);
        _clienteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_NaoDeveChecarEmail_QuandoEmailNaoInformado()
    {
        var dto = new CadastrarClienteDto(
                Nome: "João Silva",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: null
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _clienteRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(p => p.Hash(dto.Senha))
            .Returns("hash-fake");

        var result = await _subject.CadastrarAsync(dto);

        _clienteRepositoryMock.Verify(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()), Times.Once);
        Assert.Empty(result.Erros);
    }

    [Fact]
    public async Task CadastrarAsync_DeveAcumularErros_QuandoCpfEEmailJaExistemAtivos()
    {
        var dto = new CadastrarClienteDto(
                Nome: "João Silva",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: "joao@email.com"
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _clienteRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(true)
            .ReturnsAsync(true);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Equal(2, result.Erros.Count);
        Assert.Contains("Já existe um cliente ativo cadastrado com esse CPF.", result.Erros);
        Assert.Contains("Já existe um cliente ativo cadastrado com esse email.", result.Erros);
        _clienteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveTrimarCpf_AntesDeCompararEPersistir()
    {
        var dto = new CadastrarClienteDto(
                Nome: "João Silva",
                CPF: "  12345678900  ",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: null
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _clienteRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(p => p.Hash(dto.Senha))
            .Returns("hash-fake");

        var result = await _subject.CadastrarAsync(dto);

        Assert.Equal("12345678900", result.Data!.CPF);
        _clienteRepositoryMock.Verify(r => r.AddAsync(It.Is<Cliente>(c => c.CPF == "12345678900")), Times.Once);
    }

    [Fact]
    public async Task CadastrarAsync_DeveCadastrarComSucesso_QuandoDadosValidos()
    {
        var dto = new CadastrarClienteDto(
                Nome: "João Silva",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: "joao@email.com"
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _clienteRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(p => p.Hash(dto.Senha))
            .Returns("hash-fake");

        var result = await _subject.CadastrarAsync(dto);

        Assert.Empty(result.Erros);
        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("João Silva", result.Data!.Nome);
        Assert.Equal("joao@email.com", result.Data.Email);

        _passwordHasherMock.Verify(p => p.Hash(dto.Senha), Times.Once);
        _clienteRepositoryMock.Verify(r => r.AddAsync(It.Is<Cliente>(c => c.CPF == "12345678900" && c.SenhaHash == "hash-fake")), Times.Once);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region AtualizarAsync

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErrosDeValidacao()
    {
        var dto = new AtualizarClienteDto(
                Nome: "João Silva",
                CPF: "12345678900",
                Telefone: "11999999999",
                Email: null
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
        // A validação falha antes de buscar o cliente; nenhuma consulta ao repositório deve ocorrer
        _clienteRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarNaoEncontrado_QuandoClienteNaoExiste()
    {
        var dto = new AtualizarClienteDto(
                Nome: "João Silva",
                CPF: "12345678900",
                Telefone: "11999999999",
                Email: null
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _clienteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Cliente?)null);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Cliente não encontrado.", result.Erros);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErro_QuandoCpfJaExisteAtivo()
    {
        var clienteExistente = CriarClienteExistente();
        var dto = new AtualizarClienteDto(
                Nome: "João Silva",
                CPF: "12345678900",
                Telefone: "11999999999",
                Email: null
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _clienteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(clienteExistente);

        // Email null -> só existe UMA chamada a ExisteAsync (checagem de CPF)
        _clienteRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(true);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Contains("Já existe um cliente ativo cadastrado com esse CPF.", result.Erros);
        _clienteRepositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Never);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErro_QuandoEmailJaExisteAtivo()
    {
        var clienteExistente = CriarClienteExistente();
        var dto = new AtualizarClienteDto(
                Nome: "João Silva",
                CPF: "12345678900",
                Telefone: "11999999999",
                Email: "outro@email.com"
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _clienteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(clienteExistente);

        // CPF não é o foco deste teste -> false; Email -> true (o que o teste realmente cobre)
        _clienteRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Contains("Já existe um cliente ativo cadastrado com esse email.", result.Erros);
        _clienteRepositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Never);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarComSucesso_QuandoDadosValidos()
    {
        var clienteExistente = CriarClienteExistente();
        var dto = new AtualizarClienteDto(
                Nome: "João Silva Atualizado",
                CPF: "12345678900",
                Telefone: "11988888888",
                Email: null
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _clienteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(clienteExistente);

        _clienteRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(false);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Empty(result.Erros);
        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("João Silva Atualizado", result.Data!.Nome);
        Assert.Equal("11988888888", result.Data.Telefone);
        _clienteRepositoryMock.Verify(r => r.Update(It.Is<Cliente>(c => c.Nome == "João Silva Atualizado")), Times.Once);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region DeletarAsync

    [Fact]
    public async Task DeletarAsync_DeveRetornarNaoEncontrado_QuandoClienteNaoExiste()
    {
        _clienteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Cliente?)null);

        var result = await _subject.DeletarAsync(1);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Cliente não encontrado.", result.Erros);
        _clienteRepositoryMock.Verify(r => r.Remove(It.IsAny<Cliente>()), Times.Never);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeletarAsync_DeveDeletarComSucesso_QuandoClienteExiste()
    {
        var clienteExistente = CriarClienteExistente();

        _clienteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(clienteExistente);

        var result = await _subject.DeletarAsync(1);

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Cliente deletado com sucesso", result.Data);
        _clienteRepositoryMock.Verify(r => r.Remove(clienteExistente), Times.Once);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region BuscarPorIdAsync

    [Fact]
    public async Task BuscarPorIdAsync_DeveRetornarNaoEncontrado_QuandoClienteNaoExiste()
    {
        _clienteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Cliente?)null);

        var result = await _subject.BuscarPorIdAsync(1);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Cliente não encontrado.", result.Erros);
    }

    [Fact]
    public async Task BuscarPorIdAsync_DeveRetornarClienteComSucesso_QuandoClienteExiste()
    {
        var clienteExistente = CriarClienteExistente();

        _clienteRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(clienteExistente);

        var result = await _subject.BuscarPorIdAsync(1);

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal(clienteExistente.Nome, result.Data!.Nome);
        Assert.Equal(clienteExistente.CPF, result.Data.CPF);
    }

    #endregion
}