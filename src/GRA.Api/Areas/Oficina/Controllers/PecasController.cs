using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Api.Areas.Oficina.Controllers;

[Area("Oficina")]
[ApiController]
[Route("api/oficina/[controller]")]
[Authorize(Roles = "Funcionario")]
public class PecasController : ControllerBase
{
    private readonly IPecaAppService _pecaAppService;

    public PecasController(IPecaAppService pecaAppService)
    {
        _pecaAppService = pecaAppService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PecaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PecaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<PecaDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarPecaDto dto)
    {
        var response = await _pecaAppService.CadastrarAsync(dto);

        return TratarResposta(
            response,
            sucessoCustomizado: r => CreatedAtAction(nameof(BuscarPorId), new { id = r.Data!.Id }, r));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<PecaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PecaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<PecaDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(long id, [FromBody] AtualizarPecaDto dto)
    {
        var response = await _pecaAppService.AtualizarAsync(id, dto);

        return TratarResposta(response);
    }

    [HttpPatch("{id:long}/ativar")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Ativar(long id)
    {
        var response = await _pecaAppService.AtivarAsync(id);

        return TratarResposta(response, tratarValidacaoFalhou: false);
    }

    [HttpPatch("{id:long}/inativar")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Inativar(long id)
    {
        var response = await _pecaAppService.InativarAsync(id);

        return TratarResposta(response, tratarValidacaoFalhou: false);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<PecaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PecaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorId(long id)
    {
        var response = await _pecaAppService.BuscarPorIdAsync(id);

        return TratarResposta(response, tratarValidacaoFalhou: false);
    }
    private IActionResult TratarResposta<T>(
        ApiResponse<T> response,
        Func<ApiResponse<T>, IActionResult>? sucessoCustomizado = null,
        bool tratarValidacaoFalhou = true)
    {
        return response.Status switch
        {
            StatusResultado.Sucesso => sucessoCustomizado?.Invoke(response) ?? Ok(response),
            StatusResultado.NaoEncontrado => NotFound(response),
            StatusResultado.ValidacaoFalhou when tratarValidacaoFalhou => UnprocessableEntity(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }
}