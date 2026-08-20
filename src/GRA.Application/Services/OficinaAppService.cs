using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;

namespace GRA.Application.Services;

public class OficinaAppService : IOficinaAppService
{
    private readonly IOficinaRepository _oficinaRepository;

    public OficinaAppService(IOficinaRepository oficinaRepository)
    {
        _oficinaRepository = oficinaRepository;
    }

    public async Task<OficinaDTO> CadastrarAsync(CadastrarOficinaDTO dto)
    {
        Oficina oficina = dto;

        await _oficinaRepository.AddAsync(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return oficina;
    }

    public async Task<OficinaDTO?> AtualizarAsync(long id, AtualizarOficinaDTO dto)
    {
        var oficina = await _oficinaRepository.GetByIdAsync(id);
        if (oficina is null)
            return null;

        oficina.Nome = dto.Nome;
        oficina.CNPJ = dto.CNPJ;
        oficina.Telefone = dto.Telefone;
        oficina.Email = dto.Email;

        if (dto.Endereco is null)
        {
            oficina.Endereco = null;
        }
        else if (oficina.Endereco is null)
        {
            oficina.Endereco = dto.Endereco.Value;
        }
        else
        {
            var endereco = dto.Endereco.Value;

            oficina.Endereco.Logradouro = endereco.Logradouro;
            oficina.Endereco.Numero = endereco.Numero;
            oficina.Endereco.Complemento = endereco.Complemento;
            oficina.Endereco.Bairro = endereco.Bairro;
            oficina.Endereco.Cidade = endereco.Cidade;
            oficina.Endereco.Estado = endereco.Estado;
            oficina.Endereco.CEP = endereco.CEP;
        }

        _oficinaRepository.Update(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return oficina;
    }

    public async Task<bool> AtivarAsync(long id)
    {
        var oficina = await _oficinaRepository.GetByIdAsync(id);
        if (oficina is null)
            return false;

        oficina.Ativo = true;

        _oficinaRepository.Update(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> InativarAsync(long id)
    {
        var oficina = await _oficinaRepository.GetByIdAsync(id);
        if (oficina is null)
            return false;

        oficina.Ativo = false;

        _oficinaRepository.Update(oficina);
        await _oficinaRepository.SaveChangesAsync();

        return true;
    }

    public async Task<OficinaDTO?> BuscarPorSlugAsync(string slug)
    {
        var oficinas = await _oficinaRepository.GetAllAsync();

        var oficina = oficinas.FirstOrDefault(o =>
            o.Ativo &&
            string.Equals(OficinaDTO.GerarSlug(o.Nome), slug, StringComparison.OrdinalIgnoreCase));

        return oficina is null ? null : oficina;
    }

    public async Task<OficinaDTO?> BuscarPorNomeAsync(string nome)
    {
        var oficina = await _oficinaRepository.SingleAsync(o => o.Ativo && o.Nome == nome);

        return oficina is null ? null : oficina;
    }
}