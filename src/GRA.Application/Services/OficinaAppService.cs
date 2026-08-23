using System.Globalization;
using System.Text;
using FluentValidation;
using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;

namespace GRA.Application.Services;

public class OficinaAppService : IOficinaAppService
{
    private readonly IOficinaRepository _oficinaRepository;
    private readonly IValidator<CadastrarOficinaDto> _cadastrarValidator;
    private readonly IValidator<AtualizarOficinaDto> _atualizarValidator;

    public OficinaAppService(
        IOficinaRepository oficinaRepository,
        IValidator<CadastrarOficinaDto> cadastrarValidator,
        IValidator<AtualizarOficinaDto> atualizarValidator)
    {
        _oficinaRepository = oficinaRepository;
        _cadastrarValidator = cadastrarValidator;
        _atualizarValidator = atualizarValidator;
    }

    public async Task<ApiResponse<OficinaDto>> CadastrarAsync(CadastrarOficinaDto dto)
    {
        var validacao = await _cadastrarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
            return ApiResponse<OficinaDto>.ComErros(validacao.Errors.Select(e => e.ErrorMessage));

        Oficina oficina = dto;
        oficina.Slug = GerarSlug(oficina.Nome);

        await _oficinaRepository.AddAsync(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return ApiResponse<OficinaDto>.ComSucesso(oficina);
    }

    public async Task<ApiResponse<OficinaDto>> AtualizarAsync(long id, AtualizarOficinaDto dto)
    {
        var context = new ValidationContext<AtualizarOficinaDto>(dto);
        context.RootContextData["Id"] = id;

        var validacao = await _atualizarValidator.ValidateAsync(context);
        if (!validacao.IsValid)
            return ApiResponse<OficinaDto>.ComErros(validacao.Errors.Select(e => e.ErrorMessage));

        var oficina = await _oficinaRepository.GetByIdAsync(id);
        if (oficina is null)
            return ApiResponse<OficinaDto>.NaoEncontrado("Oficina não encontrada.");

        oficina.Nome = dto.Nome;
        oficina.Slug = GerarSlug(dto.Nome);
        oficina.CNPJ = dto.CNPJ;
        oficina.Telefone = dto.Telefone;
        oficina.Email = dto.Email;

        if (dto.Endereco is not null)
            oficina.Endereco = dto.Endereco.Value;

        _oficinaRepository.Update(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return ApiResponse<OficinaDto>.ComSucesso(oficina);
    }

    public async Task<ApiResponse<string>> AtivarAsync(long id)
    {
        var oficina = await _oficinaRepository.GetByIdAsync(id);
        if (oficina is null)
            return ApiResponse<string>.NaoEncontrado("Oficina não encontrada.");

        oficina.Ativo = true;

        _oficinaRepository.Update(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return ApiResponse<string>.ComSucesso("Oficina ativada com sucesso");
    }

    public async Task<ApiResponse<string>> InativarAsync(long id)
    {
        var oficina = await _oficinaRepository.GetByIdAsync(id);
        if (oficina is null)
            return ApiResponse<string>.NaoEncontrado("Oficina não encontrada.");

        oficina.Ativo = false;

        _oficinaRepository.Update(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return ApiResponse<string>.ComSucesso("Oficina inativada com sucesso");
    }

    public async Task<ApiResponse<OficinaDto>> BuscarPorSlugAsync(string slug)
    {
        var oficina = await _oficinaRepository.SingleAsync(o => o.Ativo && o.Slug == slug);
        if (oficina is null)
            return ApiResponse<OficinaDto>.NaoEncontrado("Oficina não encontrada.");

        return ApiResponse<OficinaDto>.ComSucesso(oficina);
    }

    public async Task<ApiResponse<OficinaDto>> BuscarPorNomeAsync(string nome)
    {
        var oficina = await _oficinaRepository.SingleAsync(o => o.Ativo && o.Nome == nome);
        if (oficina is null)
            return ApiResponse<OficinaDto>.NaoEncontrado("Oficina não encontrada.");

        return ApiResponse<OficinaDto>.ComSucesso(oficina);
    }

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