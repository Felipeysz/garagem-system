namespace GRA.Application.DTOs;

public record CadastrarFuncionarioDTO(
    long OficinaId,
    string Nome,
    string CPF,
    string? Telefone,
    string? Email,
    string Cargo,
    DateOnly DataAdmissao);

public record AtualizarFuncionarioDTO(
    string Nome,
    string CPF,
    string? Telefone,
    string? Email,
    string Cargo,
    DateOnly DataAdmissao);

public record FuncionarioDTO(
    long Id,
    long OficinaId,
    string Nome,
    string CPF,
    string? Telefone,
    string? Email,
    string Cargo,
    DateOnly DataAdmissao,
    bool Ativo);