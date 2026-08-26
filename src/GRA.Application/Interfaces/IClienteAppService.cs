using GRA.Application.DTOs;
using GRA.Application.Wrappers;

namespace GRA.Application.Interfaces;

public interface IClienteAppService
{
    Task<ApiResponse<ClienteDto>> CadastrarAsync(CadastrarClienteDto dto);

    Task<ApiResponse<ClienteDto>> AtualizarAsync(long id, AtualizarClienteDto dto);

    Task<ApiResponse<string>> DeletarAsync(long id);

    Task<ApiResponse<ClienteDto>> BuscarPorIdAsync(long id);
}