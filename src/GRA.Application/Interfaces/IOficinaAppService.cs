using GRA.Application.DTOs;
using GRA.Application.Wrappers;

namespace GRA.Application.Interfaces;

public interface IOficinaAppService
{
    Task<ApiResponse<OficinaDto>> CadastrarAsync(CadastrarOficinaDto dto);

    Task<ApiResponse<OficinaDto>> AtualizarAsync(long id, AtualizarOficinaDto dto);

    Task<ApiResponse<string>> AtivarAsync(long id);

    Task<ApiResponse<string>> InativarAsync(long id);

    Task<ApiResponse<OficinaDto>> BuscarPorSlugAsync(string slug);

    Task<ApiResponse<OficinaDto>> BuscarPorNomeAsync(string nome);
}