using System.Security.Claims;
using GRA.Application.DTOs;
using GRA.Application.Services;
using GRA.Application.Wrappers;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Domain.Security;
using Moq;

namespace GRA.Tests.Services;

public class AuthAppServiceTestes
{
    private readonly Mock<IFuncionarioRepository> _funcionarioRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly AuthAppService _subject;

    public AuthAppServiceTestes()
    {
        _funcionarioRepositoryMock = new Mock<IFuncionarioRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        _subject = new AuthAppService(
            _funcionarioRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenGeneratorMock.Object
        );
    }

    private static Funcionario CriarFuncionarioAtivo() => new()
    {
        Id = 1,
        OficinaId = 10,
        Nome = "Carlos Mecânico",
        SenhaHash = "hash-correto",
        CPF = "12345678900",
        Cargo = "Mecanico",
        Ativo = true
    };

    private static Cliente CriarClienteAtivo() => new()
    {
        Id = 1,
        Nome = "João Silva",
        SenhaHash = "hash-correto",
        CPF = "12345678900",
        Ativo = true
    };

    #region LoginFuncionarioAsync

    [Theory]
    [InlineData(0, "12345678900", "senha123")]   // OficinaId inválido
    [InlineData(10, "", "senha123")]              // CPF vazio
    [InlineData(10, "12345678900", "")]           // Senha vazia
    public async Task LoginFuncionarioAsync_DeveRetornarNaoAutorizado_QuandoCredenciaisIncompletas(
        long oficinaId, string cpf, string senha)
    {
        var dto = new LoginFuncionarioDto(oficinaId, cpf, senha);

        var result = await _subject.LoginFuncionarioAsync(dto);

        Assert.Equal(StatusResultado.NaoAutorizado, result.Status);
        Assert.Contains("Credenciais inválidas.", result.Erros);
        // A checagem de campos ocorre antes de qualquer consulta ao repositório
        _funcionarioRepositoryMock.Verify(r => r.SingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Funcionario, bool>>>()), Times.Never);
        _jwtTokenGeneratorMock.Verify(j => j.GerarToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    [Fact]
    public async Task LoginFuncionarioAsync_DeveRetornarNaoAutorizado_QuandoFuncionarioNaoEncontrado()
    {
        var dto = new LoginFuncionarioDto(10, "12345678900", "senha123");

        _funcionarioRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync((Funcionario?)null);

        var result = await _subject.LoginFuncionarioAsync(dto);

        Assert.Equal(StatusResultado.NaoAutorizado, result.Status);
        Assert.Contains("Credenciais inválidas.", result.Erros);
        _jwtTokenGeneratorMock.Verify(j => j.GerarToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    [Fact]
    public async Task LoginFuncionarioAsync_DeveRetornarNaoAutorizado_QuandoSenhaIncorreta()
    {
        var funcionario = CriarFuncionarioAtivo();
        var dto = new LoginFuncionarioDto(funcionario.OficinaId, funcionario.CPF, "senha-errada");

        _funcionarioRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync(funcionario);

        _passwordHasherMock
            .Setup(p => p.Verify(dto.Senha, funcionario.SenhaHash))
            .Returns(false);

        var result = await _subject.LoginFuncionarioAsync(dto);

        Assert.Equal(StatusResultado.NaoAutorizado, result.Status);
        Assert.Contains("Credenciais inválidas.", result.Erros);
        _jwtTokenGeneratorMock.Verify(j => j.GerarToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    [Fact]
    public async Task LoginFuncionarioAsync_DeveTrimarCpf_AntesDeBuscar()
    {
        var dto = new LoginFuncionarioDto(10, "  12345678900  ", "senha123");

        System.Linq.Expressions.Expression<Func<Funcionario, bool>>? predicadoCapturado = null;

        _funcionarioRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Funcionario, bool>>>()))
            .Callback<System.Linq.Expressions.Expression<Func<Funcionario, bool>>>(expr => predicadoCapturado = expr)
            .ReturnsAsync((Funcionario?)null);

        await _subject.LoginFuncionarioAsync(dto);

        var funcionarioComCpfTrimado = new Funcionario
        {
            OficinaId = 10,
            CPF = "12345678900",
            Nome = "x",
            SenhaHash = "x",
            Cargo = "x",
            Ativo = true
        };

        Assert.True(predicadoCapturado!.Compile()(funcionarioComCpfTrimado));
    }

    [Fact]
    public async Task LoginFuncionarioAsync_DeveRetornarTokenComSucesso_QuandoCredenciaisValidas()
    {
        var funcionario = CriarFuncionarioAtivo();
        var dto = new LoginFuncionarioDto(funcionario.OficinaId, funcionario.CPF, "senha123");

        _funcionarioRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Funcionario, bool>>>()))
            .ReturnsAsync(funcionario);

        _passwordHasherMock
            .Setup(p => p.Verify(dto.Senha, funcionario.SenhaHash))
            .Returns(true);

        List<Claim>? claimsCapturados = null;

        _jwtTokenGeneratorMock
            .Setup(j => j.GerarToken(It.IsAny<IEnumerable<Claim>>()))
            .Callback<IEnumerable<Claim>>(claims => claimsCapturados = claims.ToList())
            .Returns("token-fake");

        var result = await _subject.LoginFuncionarioAsync(dto);

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("token-fake", result.Data!.Token);

        Assert.Contains(claimsCapturados!, c => c.Type == ClaimTypes.NameIdentifier && c.Value == funcionario.Id.ToString());
        Assert.Contains(claimsCapturados!, c => c.Type == ClaimTypes.Name && c.Value == funcionario.Nome);
        Assert.Contains(claimsCapturados!, c => c.Type == ClaimTypes.Role && c.Value == "Funcionario");
        Assert.Contains(claimsCapturados!, c => c.Type == ClaimTypes.Role && c.Value == funcionario.Cargo);
        Assert.Contains(claimsCapturados!, c => c.Type == "oficinaId" && c.Value == funcionario.OficinaId.ToString());
    }

    #endregion

    #region LoginClienteAsync

    [Theory]
    [InlineData("", "senha123")]        // CPF vazio
    [InlineData("12345678900", "")]     // Senha vazia
    public async Task LoginClienteAsync_DeveRetornarNaoAutorizado_QuandoCredenciaisIncompletas(string cpf, string senha)
    {
        var dto = new LoginClienteDto(cpf, senha);

        var result = await _subject.LoginClienteAsync(dto);

        Assert.Equal(StatusResultado.NaoAutorizado, result.Status);
        Assert.Contains("Credenciais inválidas.", result.Erros);
        _clienteRepositoryMock.Verify(r => r.SingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cliente, bool>>>()), Times.Never);
        _jwtTokenGeneratorMock.Verify(j => j.GerarToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    [Fact]
    public async Task LoginClienteAsync_DeveRetornarNaoAutorizado_QuandoClienteNaoEncontrado()
    {
        var dto = new LoginClienteDto("12345678900", "senha123");

        _clienteRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync((Cliente?)null);

        var result = await _subject.LoginClienteAsync(dto);

        Assert.Equal(StatusResultado.NaoAutorizado, result.Status);
        Assert.Contains("Credenciais inválidas.", result.Erros);
        _jwtTokenGeneratorMock.Verify(j => j.GerarToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    [Fact]
    public async Task LoginClienteAsync_DeveRetornarNaoAutorizado_QuandoSenhaIncorreta()
    {
        var cliente = CriarClienteAtivo();
        var dto = new LoginClienteDto(cliente.CPF, "senha-errada");

        _clienteRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(cliente);

        _passwordHasherMock
            .Setup(p => p.Verify(dto.Senha, cliente.SenhaHash))
            .Returns(false);

        var result = await _subject.LoginClienteAsync(dto);

        Assert.Equal(StatusResultado.NaoAutorizado, result.Status);
        Assert.Contains("Credenciais inválidas.", result.Erros);
        _jwtTokenGeneratorMock.Verify(j => j.GerarToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    [Fact]
    public async Task LoginClienteAsync_DeveTrimarCpf_AntesDeBuscar()
    {
        var dto = new LoginClienteDto("  12345678900  ", "senha123");

        System.Linq.Expressions.Expression<Func<Cliente, bool>>? predicadoCapturado = null;

        _clienteRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cliente, bool>>>()))
            .Callback<System.Linq.Expressions.Expression<Func<Cliente, bool>>>(expr => predicadoCapturado = expr)
            .ReturnsAsync((Cliente?)null);

        await _subject.LoginClienteAsync(dto);

        var clienteComCpfTrimado = new Cliente
        {
            CPF = "12345678900",
            Nome = "x",
            SenhaHash = "x",
            Ativo = true
        };

        Assert.True(predicadoCapturado!.Compile()(clienteComCpfTrimado));
    }

    [Fact]
    public async Task LoginClienteAsync_DeveRetornarTokenComSucesso_QuandoCredenciaisValidas()
    {
        var cliente = CriarClienteAtivo();
        var dto = new LoginClienteDto(cliente.CPF, "senha123");

        _clienteRepositoryMock
            .Setup(r => r.SingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(cliente);

        _passwordHasherMock
            .Setup(p => p.Verify(dto.Senha, cliente.SenhaHash))
            .Returns(true);

        List<Claim>? claimsCapturados = null;

        _jwtTokenGeneratorMock
            .Setup(j => j.GerarToken(It.IsAny<IEnumerable<Claim>>()))
            .Callback<IEnumerable<Claim>>(claims => claimsCapturados = claims.ToList())
            .Returns("token-fake");

        var result = await _subject.LoginClienteAsync(dto);

        Assert.Equal(StatusResultado.Sucesso, result.Status);
        Assert.Equal("token-fake", result.Data!.Token);

        Assert.Contains(claimsCapturados!, c => c.Type == ClaimTypes.NameIdentifier && c.Value == cliente.Id.ToString());
        Assert.Contains(claimsCapturados!, c => c.Type == ClaimTypes.Name && c.Value == cliente.Nome);
        Assert.Contains(claimsCapturados!, c => c.Type == ClaimTypes.Role && c.Value == "Cliente");
    }

    #endregion
}
