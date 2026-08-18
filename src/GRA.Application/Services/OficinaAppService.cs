using System.Globalization;
using System.Text;
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

    public async Task<OficinaDTO> CadastrarAsync(CadastrarOficinaDTO dto)
    {
        var oficina = new Oficina
        {
            Nome = dto.Nome,
            CNPJ = dto.CNPJ,
            Telefone = dto.Telefone,
            Email = dto.Email,
            Endereco = dto.Endereco
        };

        await _oficinaRepository.AddAsync(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return MapToDTO(oficina);
    }

    public async Task<OficinaDTO?> AtualizarAsync(long id, AtualizarOficinaDTO dto)
    {
        var oficina = await _oficinaRepository.GetByIdAsync(id);
        if (oficina is null)
            return null;

        oficina.Nome = dto.Nome;
        oficina.CNPJ = dto.CNPJ;
        oficina.Telefone = dto.Telefone;
        oficina.Email = dto.Email;
        oficina.Endereco = dto.Endereco;

        _oficinaRepository.Update(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return MapToDTO(oficina);
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

    public async Task<OficinaDTO?> BuscarPorSlugAsync(string slug)
    {
        var oficinas = await _oficinaRepository.GetAllAsync();

        var oficina = oficinas.FirstOrDefault(o =>
            string.Equals(GerarSlug(o.Nome), slug, StringComparison.OrdinalIgnoreCase));

        return oficina is null ? null : MapToDTO(oficina);
    }

    public async Task<OficinaDTO?> BuscarPorNomeAsync(string nome)
    {
        var oficina = await _oficinaRepository.SingleAsync(o => o.Nome == nome);

        return oficina is null ? null : MapToDTO(oficina);
    }

    private static OficinaDTO MapToDTO(Oficina oficina) =>
        new(
            oficina.Id,
            oficina.Nome,
            GerarSlug(oficina.Nome),
            oficina.CNPJ,
            oficina.Telefone,
            oficina.Email,
            oficina.Endereco,
            oficina.DataCadastro,
            oficina.Ativo);

    private static string GerarSlug(string nome)
    {
        var normalizado = nome.Normalize(NormalizationForm.FormD);
        var semAcentos = new StringBuilder();

        foreach (var c in normalizado)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(c);
            if (categoria != UnicodeCategory.NonSpacingMark)
                semAcentos.Append(c);
        }

        var slug = semAcentos.ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant()
            .Trim();

        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[\s-]+", "-");

        return slug.Trim('-');
    }
}