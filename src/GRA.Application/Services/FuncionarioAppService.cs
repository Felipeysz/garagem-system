using FluentValidation;
using GRA.Application.DTOs;
using GRA.Application.Interfaces;
using GRA.Application.Wrappers;
using GRA.Domain.Entities;
using GRA.Domain.Repositories;
using GRA.Domain.Security;

namespace GRA.Application.Services;

public class FuncionarioAppService : IFuncionarioAppService
{
    private const string CargoGerente = "Gerente";

    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IOficinaRepository _oficinaRepository;
    private readonly IValidator<CadastrarFuncionarioInicialDto> _cadastrarInicialValidator;
    private readonly IValidator<CadastrarFuncionarioDto> _cadastrarValidator;
    private readonly IValidator<AtualizarFuncionarioDto> _atualizarValidator;
    private readonly IPasswordHasher _passwordHasher;

    public FuncionarioAppService(
        IFuncionarioRepository funcionarioRepository,
        IOficinaRepository oficinaRepository,
        IValidator<CadastrarFuncionarioInicialDto> cadastrarInicialValidator,
        IValidator<CadastrarFuncionarioDto> cadastrarValidator,
        IValidator<AtualizarFuncionarioDto> atualizarValidator,
        IPasswordHasher passwordHasher)
    {
        _funcionarioRepository = funcionarioRepository;
        _oficinaRepository = oficinaRepository;
        _cadastrarInicialValidator = cadastrarInicialValidator;
        _cadastrarValidator = cadastrarValidator;
        _atualizarValidator = atualizarValidator;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<FuncionarioDto>> CadastrarInicialAsync(CadastrarFuncionarioInicialDto dto)
    {
        var validacao = await _cadastrarInicialValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
            return ApiResponse<FuncionarioDto>.ComErros(validacao.Errors.Select(e => e.ErrorMessage));

        var oficina = await _oficinaRepository.GetByIdAsync(dto.OficinaId);
        if (oficina is null)
            return ApiResponse<FuncionarioDto>.NaoEncontrado("Oficina informada não existe.");

        var jaTemFuncionario = await _funcionarioRepository.ExisteAsync(f => f.OficinaId == dto.OficinaId);
        if (jaTemFuncionario)
            return ApiResponse<FuncionarioDto>.ComErros(["Esta oficina já possui funcionários cadastrados. Peça a um Gerente para realizar o cadastro."]);

        var nomeTrim = dto.Nome.Trim();
        var cpfTrim = dto.CPF.Trim();
        var emailTrim = dto.Email?.Trim();

        var erros = new List<string>();

        if (await _funcionarioRepository.ExisteAsync(f => f.OficinaId == dto.OficinaId && f.CPF == cpfTrim))
            erros.Add("Já existe um funcionário com esse CPF nessa oficina.");

        if (!string.IsNullOrWhiteSpace(emailTrim) &&
            await _funcionarioRepository.ExisteAsync(f => f.OficinaId == dto.OficinaId && f.Email != null && f.Email.ToUpper() == emailTrim.ToUpper()))
            erros.Add("Já existe um funcionário com esse email nessa oficina.");

        if (erros.Count > 0)
            return ApiResponse<FuncionarioDto>.ComErros(erros);

        var funcionario = new Funcionario
        {
            OficinaId = dto.OficinaId,
            Nome = nomeTrim,
            CPF = cpfTrim,
            SenhaHash = _passwordHasher.Hash(dto.Senha),
            Telefone = dto.Telefone?.Trim(),
            Email = emailTrim,
            Cargo = CargoGerente,
            DataAdmissao = dto.DataAdmissao
        };

        await _funcionarioRepository.AddAsync(funcionario);
        await _funcionarioRepository.SaveChangesAsync();

        return ApiResponse<FuncionarioDto>.ComSucesso(funcionario);
    }

    public async Task<ApiResponse<FuncionarioDto>> CadastrarAsync(CadastrarFuncionarioDto dto)
    {
        var validacao = await _cadastrarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
            return ApiResponse<FuncionarioDto>.ComErros(validacao.Errors.Select(e => e.ErrorMessage));

        var oficina = await _oficinaRepository.GetByIdAsync(dto.OficinaId);
        if (oficina is null)
            return ApiResponse<FuncionarioDto>.NaoEncontrado("Oficina informada não existe.");

        var nomeTrim = dto.Nome.Trim();
        var cpfTrim = dto.CPF.Trim();
        var emailTrim = dto.Email?.Trim();

        var erros = new List<string>();

        if (await _funcionarioRepository.ExisteAsync(f => f.OficinaId == dto.OficinaId && f.Nome.ToUpper() == nomeTrim.ToUpper()))
            erros.Add("Já existe um funcionário com esse nome nessa oficina.");

        if (await _funcionarioRepository.ExisteAsync(f => f.OficinaId == dto.OficinaId && f.CPF == cpfTrim))
            erros.Add("Já existe um funcionário com esse CPF nessa oficina.");

        if (!string.IsNullOrWhiteSpace(emailTrim) &&
            await _funcionarioRepository.ExisteAsync(f => f.OficinaId == dto.OficinaId && f.Email != null && f.Email.ToUpper() == emailTrim.ToUpper()))
            erros.Add("Já existe um funcionário com esse email nessa oficina.");

        if (erros.Count > 0)
            return ApiResponse<FuncionarioDto>.ComErros(erros);

        var funcionario = new Funcionario
        {
            OficinaId = dto.OficinaId,
            Nome = nomeTrim,
            CPF = cpfTrim,
            SenhaHash = _passwordHasher.Hash(dto.Senha),
            Telefone = dto.Telefone?.Trim(),
            Email = emailTrim,
            Cargo = dto.Cargo.Trim(),
            DataAdmissao = dto.DataAdmissao
        };

        await _funcionarioRepository.AddAsync(funcionario);
        await _funcionarioRepository.SaveChangesAsync();

        return ApiResponse<FuncionarioDto>.ComSucesso(funcionario);
    }

    public async Task<ApiResponse<FuncionarioDto>> AtualizarAsync(long id, AtualizarFuncionarioDto dto)
    {
        var validacao = await _atualizarValidator.ValidateAsync(dto);
        if (!validacao.IsValid)
            return ApiResponse<FuncionarioDto>.ComErros(validacao.Errors.Select(e => e.ErrorMessage));

        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario is null)
            return ApiResponse<FuncionarioDto>.NaoEncontrado("Funcionário não encontrado.");

        var nomeTrim = dto.Nome.Trim();
        var cpfTrim = dto.CPF.Trim();
        var emailTrim = dto.Email?.Trim();
        var oficinaId = funcionario.OficinaId;

        var erros = new List<string>();

        if (await _funcionarioRepository.ExisteAsync(f => f.Id != id && f.OficinaId == oficinaId && f.Nome.ToUpper() == nomeTrim.ToUpper()))
            erros.Add("Já existe um funcionário com esse nome nessa oficina.");

        if (await _funcionarioRepository.ExisteAsync(f => f.Id != id && f.OficinaId == oficinaId && f.CPF == cpfTrim))
            erros.Add("Já existe um funcionário com esse CPF nessa oficina.");

        if (!string.IsNullOrWhiteSpace(emailTrim) &&
            await _funcionarioRepository.ExisteAsync(f => f.Id != id && f.OficinaId == oficinaId && f.Email != null && f.Email.ToUpper() == emailTrim.ToUpper()))
            erros.Add("Já existe um funcionário com esse email nessa oficina.");

        if (erros.Count > 0)
            return ApiResponse<FuncionarioDto>.ComErros(erros);

        funcionario.Nome = nomeTrim;
        funcionario.CPF = cpfTrim;
        funcionario.Telefone = dto.Telefone?.Trim();
        funcionario.Email = emailTrim;
        funcionario.Cargo = dto.Cargo.Trim();
        funcionario.DataAdmissao = dto.DataAdmissao;

        _funcionarioRepository.Update(funcionario);
        await _funcionarioRepository.SaveChangesAsync();

        return ApiResponse<FuncionarioDto>.ComSucesso(funcionario);
    }

    public async Task<ApiResponse<string>> DeletarAsync(long id)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario is null)
            return ApiResponse<string>.NaoEncontrado("Funcionário não encontrado.");

        _funcionarioRepository.Remove(funcionario);
        await _funcionarioRepository.SaveChangesAsync();

        return ApiResponse<string>.ComSucesso("Funcionário deletado com sucesso");
    }

    public async Task<ApiResponse<FuncionarioDto>> BuscarPorIdAsync(long id)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario is null)
            return ApiResponse<FuncionarioDto>.NaoEncontrado("Funcionário não encontrado.");

        return ApiResponse<FuncionarioDto>.ComSucesso(funcionario);
    }
}