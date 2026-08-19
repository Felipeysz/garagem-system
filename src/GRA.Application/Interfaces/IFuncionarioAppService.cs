using GRA.Application.DTOs;

namespace GRA.Application.Interfaces;

public interface IFuncionarioAppService
{
    Task<FuncionarioDTO?> CadastrarAsync(CadastrarFuncionarioDTO dto);

    Task<FuncionarioDTO?> AtualizarAsync(long id, AtualizarFuncionarioDTO dto);

    Task<bool> DeletarAsync(long id);

    Task<FuncionarioDTO?> BuscarPorIdAsync(long id);
}