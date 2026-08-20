using GRA.Domain.Entities;

namespace GRA.Application.DTOs;

public record CadastrarFuncionarioDTO(
    long OficinaId,
    string Nome,
    string CPF,
    string? Telefone,
    string? Email,
    string Cargo,
    DateOnly DataAdmissao)
{
    public static implicit operator Funcionario(CadastrarFuncionarioDTO dto) => new()
    {
        OficinaId = dto.OficinaId,
        Nome = dto.Nome,
        CPF = dto.CPF,
        Telefone = dto.Telefone,
        Email = dto.Email,
        Cargo = dto.Cargo,
        DataAdmissao = dto.DataAdmissao
    };
}

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
    bool Ativo)
{
    public static implicit operator FuncionarioDTO(Funcionario funcionario) => new(
        funcionario.Id,
        funcionario.OficinaId,
        funcionario.Nome,
        funcionario.CPF,
        funcionario.Telefone,
        funcionario.Email,
        funcionario.Cargo,
        funcionario.DataAdmissao,
        funcionario.Ativo);
}