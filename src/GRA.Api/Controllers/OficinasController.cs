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
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarOficinaDTO dto)
    {
        var oficina = await _oficinaAppService.CadastrarAsync(dto);

        return CreatedAtAction(nameof(BuscarPorSlug), new { slug = oficina.Slug }, oficina);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(OficinaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    }

    [HttpPatch("{id:long}/inativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    }

    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(OficinaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarPorSlug(string slug)
    {
        var oficina = await _oficinaAppService.BuscarPorSlugAsync(slug);

        return oficina is null ? NotFound() : Ok(oficina);
    }

    [HttpGet("nome/{nome}")]
    [ProducesResponseType(typeof(OficinaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarPorNome(string nome)
    {
        var oficina = await _oficinaAppService.BuscarPorNomeAsync(nome);

        return oficina is null ? NotFound() : Ok(oficina);
    }
}