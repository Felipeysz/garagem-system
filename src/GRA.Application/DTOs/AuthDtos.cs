namespace GRA.Application.DTOs;

public record LoginFuncionarioDto(
    long OficinaId,
    string CPF,
    string Senha);

public record LoginClienteDto(
    string CPF,
    string Senha);

public record LoginResponseDto(string Token);