using FluentValidation;
using GRA.Application.DTOs;

namespace GRA.Application.Validators;

public class CadastrarFuncionarioDtoValidator : AbstractValidator<CadastrarFuncionarioDto>
{
    public CadastrarFuncionarioDtoValidator()
    {
        RuleFor(f => f.OficinaId)
            .GreaterThan(0).WithMessage("OficinaId é obrigatório.");

        RuleFor(f => f.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(f => f.CPF)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Matches(@"^\d{11}$").WithMessage("CPF deve conter 11 dígitos numéricos.");

        RuleFor(f => f.Telefone)
            .Matches(@"^\d{10,11}$").WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.")
            .When(f => !string.IsNullOrWhiteSpace(f.Telefone));

        RuleFor(f => f.Email)
            .EmailAddress().WithMessage("Email inválido.")
            .When(f => !string.IsNullOrWhiteSpace(f.Email));

        RuleFor(f => f.Cargo)
            .NotEmpty().WithMessage("Cargo é obrigatório.")
            .MaximumLength(100).WithMessage("Cargo deve ter no máximo 100 caracteres.");

        RuleFor(f => f.DataAdmissao)
            .NotEmpty().WithMessage("Data de admissão é obrigatória.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Data de admissão não pode ser no futuro.");
    }
}

public class AtualizarFuncionarioDtoValidator : AbstractValidator<AtualizarFuncionarioDto>
{
    public AtualizarFuncionarioDtoValidator()
    {
        RuleFor(f => f.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(f => f.CPF)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Matches(@"^\d{11}$").WithMessage("CPF deve conter 11 dígitos numéricos.");

        RuleFor(f => f.Telefone)
            .Matches(@"^\d{10,11}$").WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.")
            .When(f => !string.IsNullOrWhiteSpace(f.Telefone));

        RuleFor(f => f.Email)
            .EmailAddress().WithMessage("Email inválido.")
            .When(f => !string.IsNullOrWhiteSpace(f.Email));

        RuleFor(f => f.Cargo)
            .NotEmpty().WithMessage("Cargo é obrigatório.")
            .MaximumLength(100).WithMessage("Cargo deve ter no máximo 100 caracteres.");

        RuleFor(f => f.DataAdmissao)
            .NotEmpty().WithMessage("Data de admissão é obrigatória.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Data de admissão não pode ser no futuro.");
    }
}