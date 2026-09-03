using FluentValidation;
using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;

namespace GRA.Application.Services;

public class PecaAppService : IPecaAppService
{
    private readonly IPecaRepository _pecaRepository;
    private readonly IOficinaRepository _oficinaRepository;
    private readonly IValidator<CadastrarPecaDto> _cadastrarValidator;
    private readonly IValidator<AtualizarPecaDto> _atualizarValidator;

    public PecaAppService(
        IPecaRepository pecaRepository,
        IOficinaRepository oficinaRepository,
        IValidator<CadastrarPecaDto> cadastrarValidator,
        IValidator<AtualizarPecaDto> atualizarValidator)
    {
        _pecaRepository = pecaRepository;
        _oficinaRepository = oficinaRepository;
        _cadastrarValidator = cadastrarValidator;
        _atualizarValidator = atualizarValidator;
    }

    public async Task<ApiResponse<PecaDto>> CadastrarAsync(CadastrarPecaDto dto)
    {
        var validacao = await _cadastrarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
        {
            return ApiResponse<PecaDto>.ComErros(validacao.Errors.Select(e => e.ErrorMessage));
        }

        var oficina = await _oficinaRepository.GetByIdAsync(dto.OficinaId);
        if (oficina is null)
        { 
            return ApiResponse<PecaDto>.NaoEncontrado("Oficina informada não existe.");
        }

        var codigoTrim = dto.CodigoInterno?.Trim();

        if (!string.IsNullOrWhiteSpace(codigoTrim) &&
            await _pecaRepository.ExisteAsync(p => p.Ativo && p.OficinaId == dto.OficinaId &&
                p.CodigoInterno != null && p.CodigoInterno.ToUpper() == codigoTrim.ToUpper())) 
        { 
            return ApiResponse<PecaDto>.ComErros(["Já existe uma peça ativa com esse código interno nessa oficina."]);
        }

        Peca peca = dto;

        await _pecaRepository.AddAsync(peca);
        await _pecaRepository.SaveChangesAsync();

        return ApiResponse<PecaDto>.ComSucesso(peca);
    }

    public async Task<ApiResponse<PecaDto>> AtualizarAsync(long id, AtualizarPecaDto dto)
    {
        var validacao = await _atualizarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
        {
            return ApiResponse<PecaDto>.ComErros(validacao.Errors.Select(e => e.ErrorMessage));
        }

        var peca = await _pecaRepository.GetByIdAsync(id);
        if (peca is null)
        {
            return ApiResponse<PecaDto>.NaoEncontrado("Peça não encontrada.");
        }

        var codigoTrim = dto.CodigoInterno?.Trim();
        var oficinaId = peca.OficinaId;

        if (!string.IsNullOrWhiteSpace(codigoTrim) &&
            await _pecaRepository.ExisteAsync(p => p.Id != id && p.Ativo && p.OficinaId == oficinaId &&
                p.CodigoInterno != null && p.CodigoInterno.ToUpper() == codigoTrim.ToUpper()))
        { 
            return ApiResponse<PecaDto>.ComErros(["Já existe uma peça ativa com esse código interno nessa oficina."]); 
        }

        peca.Nome = dto.Nome.Trim();
        peca.Descricao = dto.Descricao?.Trim();
        peca.CodigoInterno = codigoTrim;
        peca.UnidadeMedida = dto.UnidadeMedida?.Trim();
        peca.PrecoVenda = dto.PrecoVenda;
        peca.EstoqueMinimo = dto.EstoqueMinimo;

        _pecaRepository.Update(peca);
        await _pecaRepository.SaveChangesAsync();

        return ApiResponse<PecaDto>.ComSucesso(peca);
    }

    public async Task<ApiResponse<string>> AtivarAsync(long id)
    {
        var peca = await _pecaRepository.GetByIdAsync(id);
        if (peca is null)
            return ApiResponse<string>.NaoEncontrado("Peça não encontrada.");

        peca.Ativo = true;

        _pecaRepository.Update(peca);
        await _pecaRepository.SaveChangesAsync();

        return ApiResponse<string>.ComSucesso("Peça ativada com sucesso");
    }

    public async Task<ApiResponse<string>> InativarAsync(long id)
    {
        var peca = await _pecaRepository.GetByIdAsync(id);
        if (peca is null)
            return ApiResponse<string>.NaoEncontrado("Peça não encontrada.");

        peca.Ativo = false;

        _pecaRepository.Update(peca);
        await _pecaRepository.SaveChangesAsync();

        return ApiResponse<string>.ComSucesso("Peça inativada com sucesso");
    }

    public async Task<ApiResponse<PecaDto>> BuscarPorIdAsync(long id)
    {
        var peca = await _pecaRepository.GetByIdAsync(id);
        if (peca is null) 
        { 
            return ApiResponse<PecaDto>.NaoEncontrado("Peça não encontrada.");
        }

        return ApiResponse<PecaDto>.ComSucesso(peca);
    }
}