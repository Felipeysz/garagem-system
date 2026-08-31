using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthAppService _authAppService;

    public AuthController(IAuthAppService authAppService)
    {
        _authAppService = authAppService;
    }

    [HttpPost("login/funcionario")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginFuncionario([FromBody] LoginFuncionarioDto dto)
    {
        var response = await _authAppService.LoginFuncionarioAsync(dto);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response.Data),
            StatusResultado.NaoAutorizado => Unauthorized(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost("login/cliente")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginCliente([FromBody] LoginClienteDto dto)
    {
        var response = await _authAppService.LoginClienteAsync(dto);

        return response.Status switch
        {
            StatusResultado.Sucesso => Ok(response.Data),
            StatusResultado.NaoAutorizado => Unauthorized(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}