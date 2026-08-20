using GRA.Domain.Entities;

namespace GRA.Application.DTOs;

public readonly record struct EnderecoDTO(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Estado,
    string CEP)
{
    public static implicit operator Endereco(EnderecoDTO dto) => new()
    {
        Logradouro = dto.Logradouro,
        Numero = dto.Numero,
        Complemento = dto.Complemento,
        Bairro = dto.Bairro,
        Cidade = dto.Cidade,
        Estado = dto.Estado,
        CEP = dto.CEP
    };

    public static implicit operator EnderecoDTO(Endereco endereco) => new(
        endereco.Logradouro,
        endereco.Numero,
        endereco.Complemento,
        endereco.Bairro,
        endereco.Cidade,
        endereco.Estado,
        endereco.CEP);
}

public record CadastrarOficinaDTO(
    string Nome,
    string CNPJ,
    string? Telefone,
    string? Email,
    EnderecoDTO? Endereco)
{
    public static implicit operator Oficina(CadastrarOficinaDTO dto) => new()
    {
        Nome = dto.Nome,
        CNPJ = dto.CNPJ,
        Telefone = dto.Telefone,
        Email = dto.Email,
        Endereco = dto.Endereco is null ? null : (Endereco)dto.Endereco.Value
    };
}

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
    bool Ativo)
{
    public static implicit operator OficinaDTO(Oficina oficina) => new(
        oficina.Id,
        oficina.Nome,
        oficina.Slug,
        oficina.CNPJ,
        oficina.Telefone,
        oficina.Email,
        oficina.Endereco is null ? null : (EnderecoDTO)oficina.Endereco,
        oficina.DataCadastro,
        oficina.Ativo);
}