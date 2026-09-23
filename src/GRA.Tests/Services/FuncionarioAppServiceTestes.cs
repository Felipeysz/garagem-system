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

public class FuncionarioAppServiceTestes
{
    private readonly Mock<IFuncionarioRepository> _funcionarioRepositoryMock;
    private readonly Mock<IOficinaRepository> _oficinaRepositoryMock;
    private readonly Mock<IValidator<CadastrarFuncionarioDto>> _cadastrarValidatorMock;
    private readonly Mock<IValidator<AtualizarFuncionarioDto>> _atualizarValidatorMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly FuncionarioAppService _subject;

    public FuncionarioAppServiceTestes()
    {
        _funcionarioRepositoryMock = new Mock<IFuncionarioRepository>();
        _oficinaRepositoryMock = new Mock<IOficinaRepository>();
        _cadastrarValidatorMock = new Mock<IValidator<CadastrarFuncionarioDto>>();
        _atualizarValidatorMock = new Mock<IValidator<AtualizarFuncionarioDto>>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _subject = new FuncionarioAppService(
            _funcionarioRepositoryMock.Object,
            _oficinaRepositoryMock.Object,
            _cadastrarValidatorMock.Object,
            _atualizarValidatorMock.Object,
            _passwordHasherMock.Object
        );
    }

    private static Oficina CriarOficinaAtiva(long id = 10) => new()
    {
        Id = id,
        Nome = "Oficina Central",
        CNPJ = "12345678000199",
        Ativo = true
    };

    private static Funcionario CriarFuncionarioExistente(long id = 1, long oficinaId = 10) => new()
    {
        Id = id,
        OficinaId = oficinaId,
        Nome = "Carlos Mecânico",
        SenhaHash = "hash-existente",
        CPF = "12345678900",
        Cargo = "Mecanico",
        Email = "carlos@email.com",
        DataAdmissao = new DateOnly(2024, 1, 10),
        Ativo = true
    };

    #region CadastrarAsync

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErrosDeValidacao()
    {
        var dto = new CadastrarFuncionarioDto(
                OficinaId: 10,
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: null,
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
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
        // A validação falha antes de qualquer consulta ao repositório
        _oficinaRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
        _funcionarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Funcionario>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarNaoEncontrado_QuandoOficinaNaoExiste()
    {
        var dto = new CadastrarFuncionarioDto(
                OficinaId: 10,
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: null,
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
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
        _funcionarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Funcionario>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErro_QuandoNomeJaExisteNaOficina()
    {
        var dto = new CadastrarFuncionarioDto(
                OficinaId: 10,
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: null,
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(CriarOficinaAtiva());

        // Email null -> só existem DUAS chamadas a ExisteAsync (Nome, depois CPF)
        // Nome -> true (o que este teste cobre); CPF não é o foco -> false
        _funcionarioRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Contains("Já existe um funcionário com esse nome nessa oficina.", result.Erros);
        _funcionarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Funcionario>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErro_QuandoCpfJaExisteNaOficina()
    {
        var dto = new CadastrarFuncionarioDto(
                OficinaId: 10,
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: null,
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(CriarOficinaAtiva());

        // Nome não é o foco -> false; CPF -> true (o que este teste cobre)
        _funcionarioRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Contains("Já existe um funcionário com esse CPF nessa oficina.", result.Erros);
        _funcionarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Funcionario>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarErro_QuandoEmailJaExisteNaOficina()
    {
        var dto = new CadastrarFuncionarioDto(
                OficinaId: 10,
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: "carlos@email.com",
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(CriarOficinaAtiva());

        // Nome e CPF não são o foco -> false, false; Email -> true (o que este teste cobre)
        _funcionarioRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _subject.CadastrarAsync(dto);

        Assert.Contains("Já existe um funcionário com esse email nessa oficina.", result.Erros);
        _funcionarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Funcionario>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_NaoDeveChecarEmail_QuandoEmailNaoInformado()
    {
        var dto = new CadastrarFuncionarioDto(
                OficinaId: 10,
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: null,
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(CriarOficinaAtiva());

        _funcionarioRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(p => p.Hash(dto.Senha))
            .Returns("hash-fake");

        var result = await _subject.CadastrarAsync(dto);

        // Só deve ter checado Nome e CPF; a checagem de email é pulada quando Email é null
        _funcionarioRepositoryMock.Verify(r => r.ExisteAsync(It.IsAny<Expression<Func<Funcionario, bool>>>()), Times.Exactly(2));
        Assert.Empty(result.Erros);
    }

    [Fact]
    public async Task CadastrarAsync_DeveCadastrarComSucesso_QuandoDadosValidos()
    {
        var dto = new CadastrarFuncionarioDto(
                OficinaId: 10,
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Senha: "senha123",
                Telefone: "11999999999",
                Email: "carlos@email.com",
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
            );

        _cadastrarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _oficinaRepositoryMock
            .Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(CriarOficinaAtiva());

        _funcionarioRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(p => p.Hash(dto.Senha))
            .Returns("hash-fake");

        var result = await _subject.CadastrarAsync(dto);

        Assert.Empty(result.Erros);
        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Carlos Mecânico", result.Data!.Nome);
        Assert.Equal("Mecanico", result.Data.Cargo);

        _passwordHasherMock.Verify(p => p.Hash(dto.Senha), Times.Once);
        _funcionarioRepositoryMock.Verify(r => r.AddAsync(It.Is<Funcionario>(f =>
            f.CPF == "12345678900" && f.SenhaHash == "hash-fake" && f.OficinaId == 10)), Times.Once);
        _funcionarioRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region AtualizarAsync

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErrosDeValidacao()
    {
        var dto = new AtualizarFuncionarioDto(
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Telefone: "11999999999",
                Email: null,
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
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
        _funcionarioRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarNaoEncontrado_QuandoFuncionarioNaoExiste()
    {
        var dto = new AtualizarFuncionarioDto(
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Telefone: "11999999999",
                Email: null,
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _funcionarioRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Funcionario?)null);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Funcionário não encontrado.", result.Erros);
        _funcionarioRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErro_QuandoNomeJaExisteNaOficina()
    {
        var funcionarioExistente = CriarFuncionarioExistente();
        var dto = new AtualizarFuncionarioDto(
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Telefone: "11999999999",
                Email: null,
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _funcionarioRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(funcionarioExistente);

        _funcionarioRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Contains("Já existe um funcionário com esse nome nessa oficina.", result.Erros);
        _funcionarioRepositoryMock.Verify(r => r.Update(It.IsAny<Funcionario>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErro_QuandoCpfJaExisteNaOficina()
    {
        var funcionarioExistente = CriarFuncionarioExistente();
        var dto = new AtualizarFuncionarioDto(
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Telefone: "11999999999",
                Email: null,
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _funcionarioRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(funcionarioExistente);

        _funcionarioRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Contains("Já existe um funcionário com esse CPF nessa oficina.", result.Erros);
        _funcionarioRepositoryMock.Verify(r => r.Update(It.IsAny<Funcionario>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarErro_QuandoEmailJaExisteNaOficina()
    {
        var funcionarioExistente = CriarFuncionarioExistente();
        var dto = new AtualizarFuncionarioDto(
                Nome: "Carlos Mecânico",
                CPF: "12345678900",
                Telefone: "11999999999",
                Email: "outro@email.com",
                Cargo: "Mecanico",
                DataAdmissao: new DateOnly(2024, 1, 10)
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _funcionarioRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(funcionarioExistente);

        _funcionarioRepositoryMock
            .SetupSequence(r => r.ExisteAsync(It.IsAny<Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Contains("Já existe um funcionário com esse email nessa oficina.", result.Erros);
        _funcionarioRepositoryMock.Verify(r => r.Update(It.IsAny<Funcionario>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarComSucesso_QuandoDadosValidos()
    {
        var funcionarioExistente = CriarFuncionarioExistente();
        var dto = new AtualizarFuncionarioDto(
                Nome: "Carlos Mecânico Sênior",
                CPF: "12345678900",
                Telefone: "11988888888",
                Email: null,
                Cargo: "Mecanico Chefe",
                DataAdmissao: new DateOnly(2024, 1, 10)
            );

        _atualizarValidatorMock
            .Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());

        _funcionarioRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(funcionarioExistente);

        _funcionarioRepositoryMock
            .Setup(r => r.ExisteAsync(It.IsAny<Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync(false);

        var result = await _subject.AtualizarAsync(1, dto);

        Assert.Empty(result.Erros);
        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Carlos Mecânico Sênior", result.Data!.Nome);
        Assert.Equal("Mecanico Chefe", result.Data.Cargo);
        _funcionarioRepositoryMock.Verify(r => r.Update(It.Is<Funcionario>(f => f.Nome == "Carlos Mecânico Sênior")), Times.Once);
        _funcionarioRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region DeletarAsync

    [Fact]
    public async Task DeletarAsync_DeveRetornarNaoEncontrado_QuandoFuncionarioNaoExiste()
    {
        _funcionarioRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Funcionario?)null);

        var result = await _subject.DeletarAsync(1);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Funcionário não encontrado.", result.Erros);
        _funcionarioRepositoryMock.Verify(r => r.Remove(It.IsAny<Funcionario>()), Times.Never);
        _funcionarioRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeletarAsync_DeveDeletarComSucesso_QuandoFuncionarioExiste()
    {
        var funcionarioExistente = CriarFuncionarioExistente();

        _funcionarioRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(funcionarioExistente);

        var result = await _subject.DeletarAsync(1);

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("Funcionário deletado com sucesso", result.Data);
        _funcionarioRepositoryMock.Verify(r => r.Remove(funcionarioExistente), Times.Once);
        _funcionarioRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region BuscarPorIdAsync

    [Fact]
    public async Task BuscarPorIdAsync_DeveRetornarNaoEncontrado_QuandoFuncionarioNaoExiste()
    {
        _funcionarioRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Funcionario?)null);

        var result = await _subject.BuscarPorIdAsync(1);

        Assert.Equal(StatusResultado.NaoEncontrado, result.Status);
        Assert.Contains("Funcionário não encontrado.", result.Erros);
    }

    [Fact]
    public async Task BuscarPorIdAsync_DeveRetornarFuncionarioComSucesso_QuandoFuncionarioExiste()
    {
        var funcionarioExistente = CriarFuncionarioExistente();

        _funcionarioRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(funcionarioExistente);

        var result = await _subject.BuscarPorIdAsync(1);

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal(funcionarioExistente.Nome, result.Data!.Nome);
        Assert.Equal(funcionarioExistente.CPF, result.Data.CPF);
    }

    #endregion
}
