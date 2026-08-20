using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;

namespace GRA.Application.Services;

public class FuncionarioAppService : IFuncionarioAppService
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IOficinaRepository _oficinaRepository;

    public FuncionarioAppService(
        IFuncionarioRepository funcionarioRepository,
        IOficinaRepository oficinaRepository)
    {
        _funcionarioRepository = funcionarioRepository;
        _oficinaRepository = oficinaRepository;
    }

    public async Task<FuncionarioDTO?> CadastrarAsync(CadastrarFuncionarioDTO dto)
    {
        var oficina = await _oficinaRepository.GetByIdAsync(dto.OficinaId);
        if (oficina is null)
            return null;

        Funcionario funcionario = dto;

        await _funcionarioRepository.AddAsync(funcionario);
        await _funcionarioRepository.SaveChangesAsync();

        return funcionario;
    }

    public async Task<FuncionarioDTO?> AtualizarAsync(long id, AtualizarFuncionarioDTO dto)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario is null)
            return null;

        funcionario.Nome = dto.Nome;
        funcionario.CPF = dto.CPF;
        funcionario.Telefone = dto.Telefone;
        funcionario.Email = dto.Email;
        funcionario.Cargo = dto.Cargo;
        funcionario.DataAdmissao = dto.DataAdmissao;

        _funcionarioRepository.Update(funcionario);
        await _funcionarioRepository.SaveChangesAsync();

        return funcionario;
    }

    public async Task<bool> DeletarAsync(long id)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario is null)
            return false;

        _funcionarioRepository.Remove(funcionario);
        await _funcionarioRepository.SaveChangesAsync();

        return true;
    }

    public async Task<FuncionarioDTO?> BuscarPorIdAsync(long id)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);

        if (funcionario is null)
            return null;

        return funcionario;
    }
}