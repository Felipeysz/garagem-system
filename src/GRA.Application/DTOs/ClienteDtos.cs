using GRA.Domain.Entities;

namespace GRA.Application.DTOs;

public record CadastrarClienteDto(
    string Nome,
    string CPF,
    string Senha,
    string? Telefone,
    string? Email);

public record AtualizarClienteDto(
    string Nome,
    string CPF,
    string? Telefone,
    string? Email);

public record ClienteDto(
    long Id,
    string Nome,
    string CPF,
    string? Telefone,
    string? Email,
    DateTime DataCadastro,
    bool Ativo)
{
    public static implicit operator ClienteDto(Cliente cliente) => new(
        cliente.Id,
        cliente.Nome,
        cliente.CPF,
        cliente.Telefone,
        cliente.Email,
        cliente.DataCadastro,
        cliente.Ativo);
}