using FluentValidation;
using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuncionariosController : ControllerBase
{
    private readonly IFuncionarioAppService _funcionarioAppService;
    private readonly IValidator<CadastrarFuncionarioDTO> _cadastrarValidator;
    private readonly IValidator<AtualizarFuncionarioDTO> _atualizarValidator;

    public FuncionariosController(
        IFuncionarioAppService funcionarioAppService,
        IValidator<CadastrarFuncionarioDTO> cadastrarValidator,
        IValidator<AtualizarFuncionarioDTO> atualizarValidator)
    {
        _funcionarioAppService = funcionarioAppService;
        _cadastrarValidator = cadastrarValidator;
        _atualizarValidator = atualizarValidator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarFuncionarioDTO dto)
    {
        var validacao = await _cadastrarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
        {
            var erros = validacao.Errors.Select(e => e.ErrorMessage);
            return BadRequest(ApiResponse<string>.ComErros(erros));
        }

        try
        {
            var funcionario = await _funcionarioAppService.CadastrarAsync(dto);
            if (funcionario is null)
                return NotFound(ApiResponse<string>.ComErro("Oficina informada não existe."));

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = funcionario.Id },
                ApiResponse<FuncionarioDTO>.ComSucesso(funcionario));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.ComErro($"Erro ao cadastrar funcionário: {ex.Message}"));
        }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(long id, [FromBody] AtualizarFuncionarioDTO dto)
    {
        var validacao = await _atualizarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
        {
            var erros = validacao.Errors.Select(e => e.ErrorMessage);
            return BadRequest(ApiResponse<string>.ComErros(erros));
        }

        try
        {
            var funcionario = await _funcionarioAppService.AtualizarAsync(id, dto);
            if (funcionario is null)
                return NotFound(ApiResponse<string>.ComErro("Funcionário não encontrado."));

            return Ok(ApiResponse<FuncionarioDTO>.ComSucesso(funcionario));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.ComErro($"Erro ao atualizar funcionário: {ex.Message}"));
        }
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deletar(long id)
    {
        try
        {
            var sucesso = await _funcionarioAppService.DeletarAsync(id);
            if (!sucesso)
                return NotFound(ApiResponse<string>.ComErro("Funcionário não encontrado."));

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.ComErro($"Erro ao deletar funcionário: {ex.Message}"));
        }
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<FuncionarioDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorId(long id)
    {
        try
        {
            var funcionario = await _funcionarioAppService.BuscarPorIdAsync(id);
            if (funcionario is null)
                return NotFound(ApiResponse<string>.ComErro("Funcionário não encontrado."));

            return Ok(ApiResponse<FuncionarioDTO>.ComSucesso(funcionario));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.ComErro($"Erro ao buscar funcionário por id: {ex.Message}"));
        }
    }
}