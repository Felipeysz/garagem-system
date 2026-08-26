using FluentValidation;
using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Domain.Security;

namespace GRA.Application.Services;

public class ClienteAppService : IClienteAppService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<CadastrarClienteDto> _cadastrarValidator;
    private readonly IValidator<AtualizarClienteDto> _atualizarValidator;
    private readonly IPasswordHasher _passwordHasher;

    public ClienteAppService(
        IClienteRepository clienteRepository,
        IValidator<CadastrarClienteDto> cadastrarValidator,
        IValidator<AtualizarClienteDto> atualizarValidator,
        IPasswordHasher passwordHasher)
    {
        _clienteRepository = clienteRepository;
        _cadastrarValidator = cadastrarValidator;
        _atualizarValidator = atualizarValidator;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<ClienteDto>> CadastrarAsync(CadastrarClienteDto dto)
    {
        var erros = new List<string>();

        // Validação de formato (FluentValidation) — acumula em vez de retornar de imediato.
        var validacao = await _cadastrarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
            erros.AddRange(validacao.Errors.Select(e => e.ErrorMessage));

        var nomeTrim = dto.Nome.Trim();
        var cpfTrim = dto.CPF.Trim();
        var emailTrim = dto.Email?.Trim();

        // Validação de negócio (unicidade) — mesma lista, mesmo padrão acumulativo.
        if (await _clienteRepository.ExisteAsync(c => c.Ativo && c.Nome.ToUpper() == nomeTrim.ToUpper()))
            erros.Add("Já existe um cliente ativo cadastrado com esse nome.");

        if (await _clienteRepository.ExisteAsync(c => c.Ativo && c.CPF == cpfTrim))
            erros.Add("Já existe um cliente ativo cadastrado com esse CPF.");

        if (!string.IsNullOrWhiteSpace(emailTrim) &&
            await _clienteRepository.ExisteAsync(c => c.Ativo && c.Email != null && c.Email.ToUpper() == emailTrim.ToUpper()))
            erros.Add("Já existe um cliente ativo cadastrado com esse email.");

        // Só verifica e retorna no final, depois de acumular tudo.
        if (erros.Count > 0)
            return ApiResponse<ClienteDto>.ComErros(erros);

        var cliente = new Cliente
        {
            Nome = nomeTrim,
            CPF = cpfTrim,
            SenhaHash = _passwordHasher.Hash(dto.Senha),
            Telefone = dto.Telefone?.Trim(),
            Email = emailTrim
        };

        await _clienteRepository.AddAsync(cliente);
        await _clienteRepository.SaveChangesAsync();

        return ApiResponse<ClienteDto>.ComSucesso(cliente);
    }

    public async Task<ApiResponse<ClienteDto>> AtualizarAsync(long id, AtualizarClienteDto dto)
    {
        var erros = new List<string>();

        var validacao = await _atualizarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
            erros.AddRange(validacao.Errors.Select(e => e.ErrorMessage));

        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
            return ApiResponse<ClienteDto>.NaoEncontrado("Cliente não encontrado.");

        var nomeTrim = dto.Nome.Trim();
        var cpfTrim = dto.CPF.Trim();
        var emailTrim = dto.Email?.Trim();

        if (await _clienteRepository.ExisteAsync(c => c.Ativo && c.Nome.ToUpper() == nomeTrim.ToUpper() && c.Id != id))
            erros.Add("Já existe um cliente ativo cadastrado com esse nome.");

        if (await _clienteRepository.ExisteAsync(c => c.Ativo && c.CPF == cpfTrim && c.Id != id))
            erros.Add("Já existe um cliente ativo cadastrado com esse CPF.");

        if (!string.IsNullOrWhiteSpace(emailTrim) &&
            await _clienteRepository.ExisteAsync(c => c.Ativo && c.Email != null && c.Email.ToUpper() == emailTrim.ToUpper() && c.Id != id))
            erros.Add("Já existe um cliente ativo cadastrado com esse email.");

        if (erros.Count > 0)
            return ApiResponse<ClienteDto>.ComErros(erros);

        cliente.Nome = nomeTrim;
        cliente.CPF = cpfTrim;
        cliente.Telefone = dto.Telefone?.Trim();
        cliente.Email = emailTrim;

        _clienteRepository.Update(cliente);
        await _clienteRepository.SaveChangesAsync();

        return ApiResponse<ClienteDto>.ComSucesso(cliente);
    }

    public async Task<ApiResponse<string>> DeletarAsync(long id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
            return ApiResponse<string>.NaoEncontrado("Cliente não encontrado.");

        _clienteRepository.Remove(cliente);
        await _clienteRepository.SaveChangesAsync();

        return ApiResponse<string>.ComSucesso("Cliente deletado com sucesso");
    }

    public async Task<ApiResponse<ClienteDto>> BuscarPorIdAsync(long id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
            return ApiResponse<ClienteDto>.NaoEncontrado("Cliente não encontrado.");

        return ApiResponse<ClienteDto>.ComSucesso(cliente);
    }
}