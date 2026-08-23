using FluentValidation;
using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;

namespace GRA.Application.Services;

public class FuncionarioAppService : IFuncionarioAppService
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IOficinaRepository _oficinaRepository;
    private readonly IValidator<CadastrarFuncionarioDto> _cadastrarValidator;
    private readonly IValidator<AtualizarFuncionarioDto> _atualizarValidator;

    public FuncionarioAppService(
        IFuncionarioRepository funcionarioRepository,
        IOficinaRepository oficinaRepository,
        IValidator<CadastrarFuncionarioDto> cadastrarValidator,
        IValidator<AtualizarFuncionarioDto> atualizarValidator)
    {
        _funcionarioRepository = funcionarioRepository;
        _oficinaRepository = oficinaRepository;
        _cadastrarValidator = cadastrarValidator;
        _atualizarValidator = atualizarValidator;
    }

    public async Task<ApiResponse<FuncionarioDto>> CadastrarAsync(CadastrarFuncionarioDto dto)
    {
        var validacao = await _cadastrarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
            return ApiResponse<FuncionarioDto>.ComErros(validacao.Errors.Select(e => e.ErrorMessage));

        var oficina = await _oficinaRepository.GetByIdAsync(dto.OficinaId);
        if (oficina is null)
            return ApiResponse<FuncionarioDto>.NaoEncontrado("Oficina informada não existe.");

        Funcionario funcionario = dto;

        await _funcionarioRepository.AddAsync(funcionario);
        await _funcionarioRepository.SaveChangesAsync();

        return ApiResponse<FuncionarioDto>.ComSucesso(funcionario);
    }

    public async Task<ApiResponse<FuncionarioDto>> AtualizarAsync(long id, AtualizarFuncionarioDto dto)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario is null)
            return ApiResponse<FuncionarioDto>.NaoEncontrado("Funcionário não encontrado.");

        var context = new ValidationContext<AtualizarFuncionarioDto>(dto);
        context.RootContextData["Id"] = id;
        context.RootContextData["OficinaId"] = funcionario.OficinaId;

        var validacao = await _atualizarValidator.ValidateAsync(context);
        if (!validacao.IsValid)
            return ApiResponse<FuncionarioDto>.ComErros(validacao.Errors.Select(e => e.ErrorMessage));

        funcionario.Nome = dto.Nome;
        funcionario.CPF = dto.CPF;
        funcionario.Telefone = dto.Telefone;
        funcionario.Email = dto.Email;
        funcionario.Cargo = dto.Cargo;
        funcionario.DataAdmissao = dto.DataAdmissao;

        _funcionarioRepository.Update(funcionario);
        await _funcionarioRepository.SaveChangesAsync();

        return ApiResponse<FuncionarioDto>.ComSucesso(funcionario);
    }

    public async Task<ApiResponse<string>> DeletarAsync(long id)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario is null)
            return ApiResponse<string>.NaoEncontrado("Funcionário não encontrado.");

        _funcionarioRepository.Remove(funcionario);
        await _funcionarioRepository.SaveChangesAsync();

        return ApiResponse<string>.ComSucesso("Funcionário deletado com sucesso");
    }

    public async Task<ApiResponse<FuncionarioDto>> BuscarPorIdAsync(long id)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario is null)
            return ApiResponse<FuncionarioDto>.NaoEncontrado("Funcionário não encontrado.");

        return ApiResponse<FuncionarioDto>.ComSucesso(funcionario);
    }
}