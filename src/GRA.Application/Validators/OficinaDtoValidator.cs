using FluentValidation;
using GRA.Application.DTOs;

namespace GRA.Application.Validators;

public class CadastrarOficinaDtoValidator : AbstractValidator<CadastrarOficinaDto>
{
    public CadastrarOficinaDtoValidator()
    {
        RuleFor(o => o.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(o => o.CNPJ)
            .NotEmpty().WithMessage("CNPJ é obrigatório.")
            .Matches(@"^\d{14}$").WithMessage("CNPJ deve conter 14 dígitos numéricos.");

        RuleFor(o => o.Email)
            .EmailAddress().WithMessage("Email inválido.")
            .When(o => !string.IsNullOrWhiteSpace(o.Email));

        RuleFor(o => o.Endereco!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => o.Endereco is not null);
    }
}

public class AtualizarOficinaDtoValidator : AbstractValidator<AtualizarOficinaDto>
{
    public AtualizarOficinaDtoValidator()
    {
        RuleFor(o => o.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(o => o.CNPJ)
            .NotEmpty().WithMessage("CNPJ é obrigatório.")
            .Matches(@"^\d{14}$").WithMessage("CNPJ deve conter 14 dígitos numéricos.");

        RuleFor(o => o.Email)
            .EmailAddress().WithMessage("Email inválido.")
            .When(o => !string.IsNullOrWhiteSpace(o.Email));

        RuleFor(o => o.Endereco!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => o.Endereco is not null);
    }
}