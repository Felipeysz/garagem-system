using FluentValidation;
using FluentValidation.Results;
using GRA.Application.DTOs;
using GRA.Application.Services;
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

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErrosDeValidacao()
    {
        var dto = new CadastrarClienteDto(
                Nome: "João Silva",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: "joao@email.com"
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
                Email: "joao@email.com"
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        // 1ª chamada a ExisteAsync = checagem de CPF -> true
        // 2ª chamada a ExisteAsync = checagem de Email -> false
        _clienteRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

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

        // 1ª chamada a ExisteAsync = checagem de CPF -> false
        // 2ª chamada a ExisteAsync = checagem de Email -> true
        _clienteRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Contains("Já existe um cliente ativo cadastrado com esse email.", result.Erros);
        _clienteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
        _clienteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}