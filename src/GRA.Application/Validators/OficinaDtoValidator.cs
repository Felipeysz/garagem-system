using System.Text.RegularExpressions;
using FluentValidation;
using GRA.Application.DTOs;
using GRA.Shared.Documentos;

namespace GRA.Application.Validators;

public class CadastrarOficinaDtoValidator : AbstractValidator<CadastrarOficinaDto>
{
    public CadastrarOficinaDtoValidator()
    {
        RuleFor(o => o.Nome)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(o => o.CNPJ)
            .Must(cnpj => !string.IsNullOrWhiteSpace(cnpj)).WithMessage("CNPJ é obrigatório.")
            .Must(cnpj => Regex.IsMatch(cnpj.Trim().ToUpper(), @"^[A-Z0-9]{12}\d{2}$"))
                .WithMessage("CNPJ deve ter 14 caracteres (letras/números nas 12 primeiras posições e dígitos nas 2 últimas).")
            .Must(cnpj => DocumentoValidacao.CnpjEhValido(cnpj.Trim().ToUpper()))
                .WithMessage("CNPJ inválido (dígito verificador não confere).");

        RuleFor(o => o.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                System.Net.Mail.MailAddress.TryCreate(email.Trim(), out _))
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
            .Must(cnpj => !string.IsNullOrWhiteSpace(cnpj)).WithMessage("CNPJ é obrigatório.")
            .Must(cnpj => Regex.IsMatch(cnpj.Trim().ToUpper(), @"^[A-Z0-9]{12}\d{2}$"))
                .WithMessage("CNPJ deve ter 14 caracteres (letras/números nas 12 primeiras posições e dígitos nas 2 últimas).")
            .Must(cnpj => DocumentoValidacao.CnpjEhValido(cnpj.Trim().ToUpper()))
                .WithMessage("CNPJ inválido (dígito verificador não confere).");

        RuleFor(o => o.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                System.Net.Mail.MailAddress.TryCreate(email.Trim(), out _))
                .WithMessage("Email inválido.");

        RuleFor(o => o.Endereco!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => o.Endereco is not null);
    }
}