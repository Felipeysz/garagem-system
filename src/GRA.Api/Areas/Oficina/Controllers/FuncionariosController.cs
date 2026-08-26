using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Api.Areas.Oficina.Controllers;

[Area("Oficina")]
[ApiController]
[Route("api/oficina/[controller]")]
[Authorize(Roles = "Gerente")]
public class FuncionariosController : ControllerBase
{
    private readonly IFuncionarioAppService _funcionarioAppService;

    public FuncionariosController(IFuncionarioAppService funcionarioAppService)
    {
        _funcionarioAppService = funcionarioAppService;
    }

    [HttpPost("inicial")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CadastrarInicial([FromBody] CadastrarFuncionarioInicialDto dto)
    {
        var response = await _funcionarioAppService.CadastrarInicialAsync(dto);

        return response.Status switch
        {
            StatusResultado.Sucesso => CreatedAtAction(nameof(BuscarPorId), new { id = response.Data!.Id }, response),
            StatusResultado.NaoEncontrado => NotFound(response),
            StatusResultado.ValidacaoFalhou => UnprocessableEntity(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarFuncionarioDto dto)
    {
        var response = await _funcionarioAppService.CadastrarAsync(dto);

        return response.Status switch
        {
            StatusResultado.Sucesso => CreatedAtAction(nameof(BuscarPorId), new { id = response.Data!.Id }, response),
            StatusResultado.NaoEncontrado => NotFound(response),
            StatusResultado.ValidacaoFalhou => UnprocessableEntity(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDto>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(long id, [FromBody] AtualizarFuncionarioDto dto)
    {
        var response = await _funcionarioAppService.AtualizarAsync(id, dto);

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
        var response = await _funcionarioAppService.DeletarAsync(id);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response),
            StatusResultado.NaoEncontrado => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorId(long id)
    {
        var response = await _funcionarioAppService.BuscarPorIdAsync(id);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response),
            StatusResultado.NaoEncontrado => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response)
        };
    }
}