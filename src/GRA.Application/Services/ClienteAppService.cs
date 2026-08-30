using FluentValidation;
using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Domain.Security;
using GRA.Domain.ValueObjects;

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
        var validacao = await _cadastrarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
            return ApiResponse<ClienteDto>.ComErros(validacao.Errors.Select(e => e.ErrorMessage));

        var nomeTrim = dto.Nome.Trim();
        var cpf = Cpf.Parse(dto.CPF);
        var emailTrim = dto.Email?.Trim();

        var erros = new List<string>();

        if (await _clienteRepository.ExisteAsync(c => c.Ativo && c.CPF == cpf))
            erros.Add("Já existe um cliente ativo cadastrado com esse CPF.");

        if (!string.IsNullOrWhiteSpace(emailTrim) &&
            await _clienteRepository.ExisteAsync(c => c.Ativo && c.Email != null && c.Email.ToUpper() == emailTrim.ToUpper()))
            erros.Add("Já existe um cliente ativo cadastrado com esse email.");

        if (erros.Count > 0)
            return ApiResponse<ClienteDto>.ComErros(erros);

        var cliente = new Cliente
        {
            Nome = nomeTrim,
            CPF = cpf,
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
        var validacao = await _atualizarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
            return ApiResponse<ClienteDto>.ComErros(validacao.Errors.Select(e => e.ErrorMessage));

        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
            return ApiResponse<ClienteDto>.NaoEncontrado("Cliente não encontrado.");

        var nomeTrim = dto.Nome.Trim();
        var cpf = Cpf.Parse(dto.CPF);
        var emailTrim = dto.Email?.Trim();

        var erros = new List<string>();

        if (await _clienteRepository.ExisteAsync(c => c.Ativo && c.CPF == cpf && c.Id != id))
            erros.Add("Já existe um cliente ativo cadastrado com esse CPF.");

        if (!string.IsNullOrWhiteSpace(emailTrim) &&
            await _clienteRepository.ExisteAsync(c => c.Ativo && c.Email != null && c.Email.ToUpper() == emailTrim.ToUpper() && c.Id != id))
            erros.Add("Já existe um cliente ativo cadastrado com esse email.");

        if (erros.Count > 0)
            return ApiResponse<ClienteDto>.ComErros(erros);

        cliente.Nome = nomeTrim;
        cliente.CPF = cpf;
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