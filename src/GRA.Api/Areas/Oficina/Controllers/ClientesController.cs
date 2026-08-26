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
public class ClientesController : ControllerBase
{
    private readonly IClienteAppService _clienteAppService;

    public ClientesController(IClienteAppService clienteAppService)
    {
        _clienteAppService = clienteAppService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarClienteDto dto)
    {
        var response = await _clienteAppService.CadastrarAsync(dto);

        return response.Status switch
        {
            StatusResultado.Sucesso => CreatedAtAction(nameof(BuscarPorId), new { id = response.Data!.Id }, response),
            StatusResultado.ValidacaoFalhou => UnprocessableEntity(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(long id, [FromBody] AtualizarClienteDto dto)
    {
        var response = await _clienteAppService.AtualizarAsync(id, dto);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response),
            StatusResultado.NaoEncontrado => NotFound(response),
            StatusResultado.ValidacaoFalhou => UnprocessableEntity(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deletar(long id)
    {
        var response = await _clienteAppService.DeletarAsync(id);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response),
            StatusResultado.NaoEncontrado => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorId(long id)
    {
        var response = await _clienteAppService.BuscarPorIdAsync(id);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response),
            StatusResultado.NaoEncontrado => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }
}