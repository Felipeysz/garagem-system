namespace GRA.Application.DTOs;

public readonly record struct EnderecoDTO(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Estado,
    string CEP);

public record CadastrarOficinaDTO(
    string Nome,
    string CNPJ,
    string? Telefone,
    string? Email,
    EnderecoDTO? Endereco);

public record AtualizarOficinaDTO(
    string Nome,
    string CNPJ,
    string? Telefone,
    string? Email,
    EnderecoDTO? Endereco);

public record OficinaDTO(
    long Id,
    string Nome,
    string Slug,
    string CNPJ,
    string? Telefone,
    string? Email,
    EnderecoDTO? Endereco,
    DateTime DataCadastro,
    bool Ativo);