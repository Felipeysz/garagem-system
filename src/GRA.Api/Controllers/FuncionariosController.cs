using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuncionariosController : ControllerBase
{
    private readonly IFuncionarioAppService _funcionarioAppService;

    public FuncionariosController(IFuncionarioAppService funcionarioAppService)
    {
        _funcionarioAppService = funcionarioAppService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(FuncionarioDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarFuncionarioDTO dto)
    {
        try
        {
            var funcionario = await _funcionarioAppService.CadastrarAsync(dto);
            if (funcionario is null)
                return NotFound(new { mensagem = "Oficina informada não existe." });

            return CreatedAtAction(nameof(BuscarPorId), new { id = funcionario.Id }, funcionario);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao cadastrar funcionário.", detalhe = ex.Message });
        }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(FuncionarioDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(long id, [FromBody] AtualizarFuncionarioDTO dto)
    {
        try
        {
            var funcionario = await _funcionarioAppService.AtualizarAsync(id, dto);
            return funcionario is null ? NotFound() : Ok(funcionario);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao atualizar funcionário.", detalhe = ex.Message });
        }
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deletar(long id)
    {
        try
        {
            var sucesso = await _funcionarioAppService.DeletarAsync(id);
            return sucesso ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao deletar funcionário.", detalhe = ex.Message });
        }
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(FuncionarioDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorId(long id)
    {
        try
        {
            var funcionario = await _funcionarioAppService.BuscarPorIdAsync(id);
            return funcionario is null ? NotFound() : Ok(funcionario);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Erro ao buscar funcionário por id.", detalhe = ex.Message });
        }
    }
}