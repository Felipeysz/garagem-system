using FluentValidation;
using FluentValidation.Results;
using GRA.Application.DTOs;
using GRA.Application.Services;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Domain.Security;
using Moq;

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
}
