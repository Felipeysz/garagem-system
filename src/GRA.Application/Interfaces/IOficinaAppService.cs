using GRA.Application.DTOs;

namespace GRA.Application.Interfaces;

public interface IOficinaAppService
{
    Task<OficinaDto> CadastrarAsync(CadastrarOficinaDto dto);

    Task<OficinaDto?> AtualizarAsync(long id, AtualizarOficinaDto dto);

    Task<bool> AtivarAsync(long id);

    Task<bool> InativarAsync(long id);

    Task<OficinaDto?> BuscarPorSlugAsync(string slug);

    Task<OficinaDto?> BuscarPorNomeAsync(string nome);
}