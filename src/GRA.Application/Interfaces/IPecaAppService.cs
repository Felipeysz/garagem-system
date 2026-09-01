using GRA.Application.DTOs;
using GRA.Application.Wrappers;

namespace GRA.Application.Interfaces;

public interface IPecaAppService
{
    Task<ApiResponse<PecaDto>> CadastrarAsync(CadastrarPecaDto dto);

    Task<ApiResponse<PecaDto>> AtualizarAsync(long id, AtualizarPecaDto dto);

    Task<ApiResponse<string>> AtivarAsync(long id);

    Task<ApiResponse<string>> InativarAsync(long id);

    Task<ApiResponse<PecaDto>> BuscarPorIdAsync(long id);
}