using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;

namespace GRA.Application.Services;

public class OficinaAppService : IOficinaAppService
{
    private readonly IOficinaRepository _oficinaRepository;

    public OficinaAppService(IOficinaRepository oficinaRepository)
    {
        _oficinaRepository = oficinaRepository;
    }

    public async Task<OficinaDto> CadastrarAsync(CadastrarOficinaDto dto)
    {
        Oficina oficina = dto;
        oficina.Slug = GerarSlug(oficina.Nome);

        await _oficinaRepository.AddAsync(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return oficina;
    }

    public async Task<OficinaDto?> AtualizarAsync(long id, AtualizarOficinaDto dto)
    {
        var oficina = await _oficinaRepository.GetByIdAsync(id);
        if (oficina is null)
            return null;

        oficina.Nome = dto.Nome;
        oficina.Slug = GerarSlug(dto.Nome);
        oficina.CNPJ = dto.CNPJ;
        oficina.Telefone = dto.Telefone;
        oficina.Email = dto.Email;

        if (dto.Endereco is not null)
            oficina.Endereco = dto.Endereco.Value;

        _oficinaRepository.Update(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return oficina;
    }

    public async Task<bool> AtivarAsync(long id)
    {
        var oficina = await _oficinaRepository.GetByIdAsync(id);
        if (oficina is null)
            return false;

        oficina.Ativo = true;

        _oficinaRepository.Update(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> InativarAsync(long id)
    {
        var oficina = await _oficinaRepository.GetByIdAsync(id);
        if (oficina is null)
            return false;

        oficina.Ativo = false;

        _oficinaRepository.Update(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return true;
    }

    public async Task<OficinaDto?> BuscarPorSlugAsync(string slug)
    {
        var oficina = await _oficinaRepository.SingleAsync(o => o.Ativo && o.Slug == slug);

        if (oficina is null)
            return null;

        return oficina;
    }

    public async Task<OficinaDto?> BuscarPorNomeAsync(string nome)
    {
        var oficina = await _oficinaRepository.SingleAsync(o => o.Ativo && o.Nome == nome);

        if (oficina is null)
            return null;

        return oficina;
    }

    private static string GerarSlug(string nome)
    {
        var normalizado = nome.Normalize(System.Text.NormalizationForm.FormD);
        var semAcentos = new System.Text.StringBuilder();

        foreach (var c in normalizado)
        {
            var categoria = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (categoria != System.Globalization.UnicodeCategory.NonSpacingMark)
                semAcentos.Append(c);
        }

        var slug = semAcentos.ToString()
            .Normalize(System.Text.NormalizationForm.FormC)
            .ToLowerInvariant()
            .Trim();

        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[\s-]+", "-");

        return slug.Trim('-');
    }
}