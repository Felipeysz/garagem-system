using System.Security.Claims;
using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using GRA.Domain.Repositories;
using GRA.Domain.Security;

namespace GRA.Application.Services;

public class AuthAppService : IAuthAppService
{
    private const string MensagemCredenciaisInvalidas = "Credenciais inválidas.";

    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthAppService(
        IFuncionarioRepository funcionarioRepository,
        IClienteRepository clienteRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _funcionarioRepository = funcionarioRepository;
        _clienteRepository = clienteRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginFuncionarioAsync(LoginFuncionarioDto dto)
    {
        if (dto.OficinaId <= 0 || string.IsNullOrWhiteSpace(dto.CPF) || string.IsNullOrWhiteSpace(dto.Senha))
            return ApiResponse<LoginResponseDto>.NaoAutorizado(MensagemCredenciaisInvalidas);

        var cpfTrim = dto.CPF.Trim();

        var funcionario = await _funcionarioRepository.SingleAsync(f =>
            f.Ativo && f.OficinaId == dto.OficinaId && f.CPF == cpfTrim);

        if (funcionario is null || !_passwordHasher.Verify(dto.Senha, funcionario.SenhaHash))
            return ApiResponse<LoginResponseDto>.NaoAutorizado(MensagemCredenciaisInvalidas);

        var roles = new[] { "Funcionario", funcionario.Cargo };

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, funcionario.Id.ToString()),
            new(ClaimTypes.Name, funcionario.Nome),
            new(ClaimTypes.Role, "Funcionario"),
            new(ClaimTypes.Role, funcionario.Cargo),
            new("oficinaId", funcionario.OficinaId.ToString())
        };

        var token = _jwtTokenGenerator.GerarToken(claims);

        var response = new LoginResponseDto(
            funcionario.Id,
            funcionario.Nome,
            roles,
            funcionario.OficinaId,
            funcionario.Cargo,
            token);

        return ApiResponse<LoginResponseDto>.ComSucesso(response);
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginClienteAsync(LoginClienteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CPF) || string.IsNullOrWhiteSpace(dto.Senha))
            return ApiResponse<LoginResponseDto>.NaoAutorizado(MensagemCredenciaisInvalidas);

        var cpfTrim = dto.CPF.Trim();

        var cliente = await _clienteRepository.SingleAsync(c => c.Ativo && c.CPF == cpfTrim);

        if (cliente is null || !_passwordHasher.Verify(dto.Senha, cliente.SenhaHash))
            return ApiResponse<LoginResponseDto>.NaoAutorizado(MensagemCredenciaisInvalidas);

        var roles = new[] { "Cliente" };

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
            new(ClaimTypes.Name, cliente.Nome),
            new(ClaimTypes.Role, "Cliente")
        };

        var token = _jwtTokenGenerator.GerarToken(claims);

        var response = new LoginResponseDto(
            cliente.Id,
            cliente.Nome,
            roles,
            null,
            null,
            token);

        return ApiResponse<LoginResponseDto>.ComSucesso(response);
    }
}