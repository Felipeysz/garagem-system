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

    public OficinasController(IOficinaAppService oficinaAppService)
    {
        _oficinaAppService = oficinaAppService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarOficinaDto dto)
    {
        var response = await _oficinaAppService.CadastrarAsync(dto);

        return response.Status switch
        {
            StatusResultado.Sucesso => CreatedAtAction(nameof(BuscarPorSlug), new { slug = response.Data!.Slug }, response),
            StatusResultado.ValidacaoFalhou => UnprocessableEntity(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(long id, [FromBody] AtualizarOficinaDto dto)
    {
        var response = await _oficinaAppService.AtualizarAsync(id, dto);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response),
            StatusResultado.NaoEncontrado => NotFound(response),
            StatusResultado.ValidacaoFalhou => UnprocessableEntity(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }

    [HttpPatch("{id:long}/ativar")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Ativar(long id)
    {
        var response = await _oficinaAppService.AtivarAsync(id);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response),
            StatusResultado.NaoEncontrado => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }

    [HttpPatch("{id:long}/inativar")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Inativar(long id)
    {
        var response = await _oficinaAppService.InativarAsync(id);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response),
            StatusResultado.NaoEncontrado => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }

    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorSlug(string slug)
    {
        var response = await _oficinaAppService.BuscarPorSlugAsync(slug);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response),
            StatusResultado.NaoEncontrado => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }

    [HttpGet("nome/{nome}")]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OficinaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorNome(string nome)
    {
        var response = await _oficinaAppService.BuscarPorNomeAsync(nome);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response),
            StatusResultado.NaoEncontrado => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }
}