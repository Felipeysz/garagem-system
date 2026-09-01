using GRA.Domain.Entities;

namespace GRA.Application.DTOs;

public record CadastrarPecaDto(
    long OficinaId,
    string Nome,
    string? Descricao,
    string? CodigoInterno,
    string? UnidadeMedida,
    decimal? PrecoVenda,
    int EstoqueMinimo)
{
    public static implicit operator Peca(CadastrarPecaDto dto) => new()
    {
        OficinaId = dto.OficinaId,
        Nome = dto.Nome.Trim(),
        Descricao = dto.Descricao?.Trim(),
        CodigoInterno = dto.CodigoInterno?.Trim(),
        UnidadeMedida = dto.UnidadeMedida?.Trim(),
        PrecoVenda = dto.PrecoVenda,
        EstoqueMinimo = dto.EstoqueMinimo
    };
}

public record AtualizarPecaDto(
    string Nome,
    string? Descricao,
    string? CodigoInterno,
    string? UnidadeMedida,
    decimal? PrecoVenda,
    int EstoqueMinimo);

public record PecaDto(
    long Id,
    long OficinaId,
    string Nome,
    string? Descricao,
    string? CodigoInterno,
    string? UnidadeMedida,
    decimal? PrecoVenda,
    int EstoqueMinimo,
    bool Ativo)
{
    public static implicit operator PecaDto(Peca peca) => new(
        peca.Id,
        peca.OficinaId,
        peca.Nome,
        peca.Descricao,
        peca.CodigoInterno,
        peca.UnidadeMedida,
        peca.PrecoVenda,
        peca.EstoqueMinimo,
        peca.Ativo);
}