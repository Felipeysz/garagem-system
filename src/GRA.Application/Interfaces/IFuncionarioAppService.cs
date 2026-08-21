using GRA.Application.DTOs;
using GRA.Application.Wrappers;

namespace GRA.Application.Interfaces;

public interface IFuncionarioAppService
{
    Task<ApiResponse<FuncionarioDto>> CadastrarAsync(CadastrarFuncionarioDto dto);

    Task<ApiResponse<FuncionarioDto>> AtualizarAsync(long id, AtualizarFuncionarioDto dto);

    Task<ApiResponse<string>> DeletarAsync(long id);

    Task<ApiResponse<FuncionarioDto>> BuscarPorIdAsync(long id);
}