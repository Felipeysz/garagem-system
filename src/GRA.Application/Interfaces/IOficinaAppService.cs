using GRA.Application.DTOs;

namespace GRA.Application.Interfaces;

public interface IOficinaAppService
{
    Task<OficinaDTO> CadastrarAsync(CadastrarOficinaDTO dto);

    Task<OficinaDTO?> AtualizarAsync(long id, AtualizarOficinaDTO dto);

    Task<bool> InativarAsync(long id);

    Task<OficinaDTO?> BuscarPorSlugAsync(string slug);

    Task<OficinaDTO?> BuscarPorNomeAsync(string nome);
}