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
            Endereco = MapToEntity(dto.Endereco)
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

        if (dto.Endereco is null)
        {
            oficina.Endereco = null;
        }
        else if (oficina.Endereco is null)
        {
            oficina.Endereco = MapToEntity(dto.Endereco);
        }
        else
        {
            oficina.Endereco.Logradouro = dto.Endereco.Logradouro;
            oficina.Endereco.Numero = dto.Endereco.Numero;
            oficina.Endereco.Complemento = dto.Endereco.Complemento;
            oficina.Endereco.Bairro = dto.Endereco.Bairro;
            oficina.Endereco.Cidade = dto.Endereco.Cidade;
            oficina.Endereco.Estado = dto.Endereco.Estado;
            oficina.Endereco.CEP = dto.Endereco.CEP;
        }

        _oficinaRepository.Update(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return MapToDTO(oficina);
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

    public async Task<OficinaDTO?> BuscarPorSlugAsync(string slug)
    {
        var oficinas = await _oficinaRepository.GetAllAsync();

        var oficina = oficinas.FirstOrDefault(o =>
            o.Ativo &&
            string.Equals(GerarSlug(o.Nome), slug, StringComparison.OrdinalIgnoreCase));

        return oficina is null ? null : MapToDTO(oficina);
    }

    public async Task<OficinaDTO?> BuscarPorNomeAsync(string nome)
    {
        var oficina = await _oficinaRepository.SingleAsync(o => o.Ativo && o.Nome == nome);

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
        MapToDTO(oficina.Endereco),
        oficina.DataCadastro,
        oficina.Ativo);

    private static EnderecoDTO? MapToDTO(Endereco? endereco) =>
        endereco is null
            ? null
            : new EnderecoDTO(
                endereco.Logradouro,
                endereco.Numero,
                endereco.Complemento,
                endereco.Bairro,
                endereco.Cidade,
                endereco.Estado,
                endereco.CEP);

    private static Endereco? MapToEntity(EnderecoDTO? dto) =>
        dto is null
            ? null
            : new Endereco
            {
                Logradouro = dto.Logradouro,
                Numero = dto.Numero,
                Complemento = dto.Complemento,
                Bairro = dto.Bairro,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                CEP = dto.CEP
            };

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