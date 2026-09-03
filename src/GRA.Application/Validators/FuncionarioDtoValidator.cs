using FluentValidation;
using GRA.Application.DTOs;

namespace GRA.Application.Validators;

public abstract class FuncionarioDtoValidatorBase<T> : AbstractValidator<T>
{
    protected void AplicarRegrasComuns(
        Func<T, string?> nomeSelector,
        Func<T, string?> cpfSelector,
        Func<T, string?> telefoneSelector,
        Func<T, string?> emailSelector,
        Func<T, string?> cargoSelector,
        Func<T, DateOnly> dataAdmissaoSelector)
    {
        RuleFor(f => nomeSelector(f))
            .Must(nome => !string.IsNullOrWhiteSpace(nome))
                .WithMessage("Nome é obrigatório.")
            .Must(nome => nome!.Trim().Length <= 150)
                .WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(f => cpfSelector(f))
            .Must(ClienteValidationHelper.CpfEhValido)
                .WithMessage("CPF inválido (formato incorreto ou dígito verificador não confere).");

        RuleFor(f => telefoneSelector(f))
            .Must(ClienteValidationHelper.TelefoneEhValido)
                .WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.");

        RuleFor(f => emailSelector(f))
            .Must(ClienteValidationHelper.EmailEhValido)
                .WithMessage("Email inválido.");

        RuleFor(f => cargoSelector(f))
            .Must(cargo => !string.IsNullOrWhiteSpace(cargo))
                .WithMessage("Cargo é obrigatório.")
            .Must(cargo => cargo!.Trim().Length <= 100)
                .WithMessage("Cargo deve ter no máximo 100 caracteres.");

        RuleFor(f => dataAdmissaoSelector(f))
            .NotEmpty().WithMessage("Data de admissão é obrigatória.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Data de admissão não pode ser no futuro.");
    }
}

public class CadastrarFuncionarioDtoValidator : FuncionarioDtoValidatorBase<CadastrarFuncionarioDto>
{
    public CadastrarFuncionarioDtoValidator()
    {
        RuleFor(f => f.OficinaId)
            .GreaterThan(0).WithMessage("OficinaId é obrigatório.");

        AplicarRegrasComuns(
            f => f.Nome,
            f => f.CPF,
            f => f.Telefone,
            f => f.Email,
            f => f.Cargo,
            f => f.DataAdmissao);

        RuleFor(f => f.Senha)
            .Must(senha => !string.IsNullOrWhiteSpace(senha))
                .WithMessage("Senha é obrigatória.")
            .Must(senha => senha!.Length >= 8)
                .WithMessage("Senha deve ter no mínimo 8 caracteres.");
    }
}

public class AtualizarFuncionarioDtoValidator : FuncionarioDtoValidatorBase<AtualizarFuncionarioDto>
{
    public AtualizarFuncionarioDtoValidator()
    {
        AplicarRegrasComuns(
            f => f.Nome,
            f => f.CPF,
            f => f.Telefone,
            f => f.Email,
            f => f.Cargo,
            f => f.DataAdmissao);
    }
}