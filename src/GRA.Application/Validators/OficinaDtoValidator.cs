using FluentValidation;
using GRA.Application.DTOs;
using GRA.Domain.ValueObjects;

namespace GRA.Application.Validators;

public class CadastrarOficinaDtoValidator : AbstractValidator<CadastrarOficinaDto>
{
    public CadastrarOficinaDtoValidator()
    {
        RuleFor(o => o.Nome)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(o => o.CNPJ)
            .Must(cnpj => Cnpj.TryCreate(cnpj, out _))
                .WithMessage("CNPJ inválido (formato incorreto ou dígito verificador não confere).");

        RuleFor(o => o.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
                .WithMessage("Email inválido.");

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
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(o => o.CNPJ)
            .Must(cnpj => Cnpj.TryCreate(cnpj, out _))
                .WithMessage("CNPJ inválido (formato incorreto ou dígito verificador não confere).");

        RuleFor(o => o.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
                .WithMessage("Email inválido.");

        RuleFor(o => o.Endereco!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => o.Endereco is not null);
    }
}