using FluentValidation;
using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OficinasController : ControllerBase
{
    private readonly IOficinaAppService _oficinaAppService;
    private readonly IValidator<CadastrarOficinaDto> _cadastrarValidator;
    private readonly IValidator<AtualizarOficinaDto> _atualizarValidator;

    public OficinasController(
        IOficinaAppService oficinaAppService,
        IValidator<CadastrarOficinaDto> cadastrarValidator,
        IValidator<AtualizarOficinaDto> atualizarValidator)
    {
        _oficinaAppService = oficinaAppService;
        _cadastrarValidator = cadastrarValidator;
        _atualizarValidator = atualizarValidator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarOficinaDto dto)
    {
        var validacao = await _cadastrarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
        {
            var erros = validacao.Errors.Select(e => e.ErrorMessage);
            return BadRequest(ApiResponse<string>.ComErros(erros));
        }

        try
        {
            var oficina = await _oficinaAppService.CadastrarAsync(dto);
            return CreatedAtAction(
                nameof(BuscarPorSlug),
                new { slug = oficina.Slug },
                ApiResponse<string>.ComSucesso("Oficina cadastrada com sucesso"));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.ComErro($"Erro ao cadastrar oficina: {ex.Message}"));
        }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(long id, [FromBody] AtualizarOficinaDto dto)
    {
        var validacao = await _atualizarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
        {
            var erros = validacao.Errors.Select(e => e.ErrorMessage);
            return BadRequest(ApiResponse<string>.ComErros(erros));
        }

        try
        {
            var oficina = await _oficinaAppService.AtualizarAsync(id, dto);
            if (oficina is null)
                return NotFound(ApiResponse<string>.ComErro("Oficina não encontrada."));

            return Ok(ApiResponse<OficinaDto>.ComSucesso(oficina));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.ComErro($"Erro ao atualizar oficina: {ex.Message}"));
        }
    }

    [HttpPatch("{id:long}/ativar")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Ativar(long id)
    {
        try
        {
            var sucesso = await _oficinaAppService.AtivarAsync(id);
            if (!sucesso)
                return NotFound(ApiResponse<string>.ComErro("Oficina não encontrada."));

            return Ok(ApiResponse<string>.ComSucesso("Oficina ativada com sucesso"));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.ComErro($"Erro ao ativar oficina: {ex.Message}"));
        }
    }

    [HttpPatch("{id:long}/inativar")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Inativar(long id)
    {
        try
        {
            var sucesso = await _oficinaAppService.InativarAsync(id);
            if (!sucesso)
                return NotFound(ApiResponse<string>.ComErro("Oficina não encontrada."));

            return Ok(ApiResponse<string>.ComSucesso("Oficina inativada com sucesso"));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.ComErro($"Erro ao inativar oficina: {ex.Message}"));
        }
    }

    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorSlug(string slug)
    {
        try
        {
            var oficina = await _oficinaAppService.BuscarPorSlugAsync(slug);
            if (oficina is null)
                return NotFound(ApiResponse<string>.ComErro("Oficina não encontrada."));

            return Ok(ApiResponse<OficinaDto>.ComSucesso(oficina));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.ComErro($"Erro ao buscar oficina por slug: {ex.Message}"));
        }
    }

    [HttpGet("nome/{nome}")]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorNome(string nome)
    {
        try
        {
            var oficina = await _oficinaAppService.BuscarPorNomeAsync(nome);
            if (oficina is null)
                return NotFound(ApiResponse<string>.ComErro("Oficina não encontrada."));

            return Ok(ApiResponse<OficinaDto>.ComSucesso(oficina));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.ComErro($"Erro ao buscar oficina por nome: {ex.Message}"));
        }
    }
}