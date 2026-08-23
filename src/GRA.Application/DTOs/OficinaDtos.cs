using GRA.Domain.Entities;

namespace GRA.Application.DTOs;

public readonly record struct EnderecoDto(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Estado,
    string CEP)
{
    public static implicit operator Endereco(EnderecoDto dto) => new()
    {
        Logradouro = dto.Logradouro,
        Numero = dto.Numero,
        Complemento = dto.Complemento,
        Bairro = dto.Bairro,
        Cidade = dto.Cidade,
        Estado = dto.Estado,
        CEP = dto.CEP
    };

    public static implicit operator EnderecoDto(Endereco endereco) => new(
        endereco.Logradouro,
        endereco.Numero,
        endereco.Complemento,
        endereco.Bairro,
        endereco.Cidade,
        endereco.Estado,
        endereco.CEP);
}

public record CadastrarOficinaDto(
    string Nome,
    string CNPJ,
    string? Telefone,
    string? Email,
    EnderecoDto? Endereco)
{
    public static implicit operator Oficina(CadastrarOficinaDto dto) => new()
    {
        Nome = dto.Nome.Trim(),
        CNPJ = dto.CNPJ.Trim(),
        Telefone = dto.Telefone?.Trim(),
        Email = dto.Email?.Trim(),
        Endereco = dto.Endereco is null ? null : (Endereco)dto.Endereco.Value
    };
}

public record AtualizarOficinaDto(
    string Nome,
    string CNPJ,
    string? Telefone,
    string? Email,
    EnderecoDto? Endereco);

public record OficinaDto(
    long Id,
    string Nome,
    string Slug,
    string CNPJ,
    string? Telefone,
    string? Email,
    EnderecoDto? Endereco,
    DateTime DataCadastro,
    bool Ativo)
{
    public static implicit operator OficinaDto(Oficina oficina) => new(
        oficina.Id,
        oficina.Nome,
        oficina.Slug,
        oficina.CNPJ,
        oficina.Telefone,
        oficina.Email,
        oficina.Endereco is null ? null : (EnderecoDto)oficina.Endereco,
        oficina.DataCadastro,
        oficina.Ativo);
}