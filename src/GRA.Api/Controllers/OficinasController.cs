using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OficinasController : ControllerBase
{
    private readonly IOficinaAppService _oficinaAppService;

    public OficinasController(IOficinaAppService oficinaAppService)
    {
        _oficinaAppService = oficinaAppService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OficinaDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarOficinaDTO dto)
    {
        try
        {
            var oficina = await _oficinaAppService.CadastrarAsync(dto);
            return CreatedAtAction(nameof(BuscarPorSlug), new { slug = oficina.Slug }, oficina);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao cadastrar oficina.", detalhe = ex.Message });
        }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(OficinaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(long id, [FromBody] AtualizarOficinaDTO dto)
    {
        try
        {
            var oficina = await _oficinaAppService.AtualizarAsync(id, dto);
            return Ok(oficina);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao atualizar oficina.", detalhe = ex.Message });
        }
    }

    [HttpPatch("{id:long}/inativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Inativar(long id)
    {
        try
        {
            await _oficinaAppService.InativarAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao inativar oficina.", detalhe = ex.Message });
        }
    }

    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(OficinaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorSlug(string slug)
    {
        try
        {
            var oficina = await _oficinaAppService.BuscarPorSlugAsync(slug);
            return oficina is null ? NotFound() : Ok(oficina);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao buscar oficina por slug.", detalhe = ex.Message });
        }
    }

    [HttpGet("nome/{nome}")]
    [ProducesResponseType(typeof(OficinaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorNome(string nome)
    {
        try
        {
            var oficina = await _oficinaAppService.BuscarPorNomeAsync(nome);
            return oficina is null ? NotFound() : Ok(oficina);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao buscar oficina por nome.", detalhe = ex.Message });
        }
    }
}