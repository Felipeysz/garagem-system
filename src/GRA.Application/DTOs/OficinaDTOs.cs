namespace GRA.Application.DTOs;

public record CadastrarOficinaDTO(
    string Nome,
    string CNPJ,
    string? Telefone,
    string? Email,
    string? Endereco);

public record AtualizarOficinaDTO(
    string Nome,
    string CNPJ,
    string? Telefone,
    string? Email,
    string? Endereco);

public record OficinaDTO(
    long Id,
    string Nome,
    string Slug,
    string CNPJ,
    string? Telefone,
    string? Email,
    string? Endereco,
    DateTime DataCadastro,
    bool Ativo);