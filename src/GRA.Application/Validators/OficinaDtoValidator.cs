using System.Text.RegularExpressions;
using FluentValidation;
using GRA.Application.DTOs;

namespace GRA.Application.Validators;

public class CadastrarOficinaDtoValidator : AbstractValidator<CadastrarOficinaDto>
{
    public CadastrarOficinaDtoValidator()
    {
        RuleFor(o => o.Nome)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(o => o.CNPJ)
            .Must(CnpjEhValido)
                .WithMessage("CNPJ inválido (formato incorreto ou dígito verificador não confere).");

        RuleFor(o => o.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
                .WithMessage("Email inválido.");

        RuleFor(o => o.Endereco!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => o.Endereco is not null);
    }

    private static bool CnpjEhValido(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        var valor = cnpj.Trim().ToUpperInvariant();

        if (!Regex.IsMatch(valor, @"^[A-Z0-9]{12}\d{2}$"))
            return false;

        int[] pesos1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] pesos2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var valores = valor.Select(c => (int)c - 48).ToArray();

        var soma1 = 0;
        for (var i = 0; i < 12; i++)
            soma1 += valores[i] * pesos1[i];

        var resto1 = soma1 % 11;
        var dv1 = resto1 < 2 ? 0 : 11 - resto1;

        if (valores[12] != dv1)
            return false;

        var soma2 = 0;
        for (var i = 0; i < 13; i++)
            soma2 += valores[i] * pesos2[i];

        var resto2 = soma2 % 11;
        var dv2 = resto2 < 2 ? 0 : 11 - resto2;

        return valores[13] == dv2;
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
            .Must(CnpjEhValido)
                .WithMessage("CNPJ inválido (formato incorreto ou dígito verificador não confere).");

        RuleFor(o => o.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
                .WithMessage("Email inválido.");

        RuleFor(o => o.Endereco!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => o.Endereco is not null);
    }

    private static bool CnpjEhValido(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        var valor = cnpj.Trim().ToUpperInvariant();

        if (!Regex.IsMatch(valor, @"^[A-Z0-9]{12}\d{2}$"))
            return false;

        int[] pesos1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] pesos2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var valores = valor.Select(c => (int)c - 48).ToArray();

        var soma1 = 0;
        for (var i = 0; i < 12; i++)
            soma1 += valores[i] * pesos1[i];

        var resto1 = soma1 % 11;
        var dv1 = resto1 < 2 ? 0 : 11 - resto1;

        if (valores[12] != dv1)
            return false;

        var soma2 = 0;
        for (var i = 0; i < 13; i++)
            soma2 += valores[i] * pesos2[i];

        var resto2 = soma2 % 11;
        var dv2 = resto2 < 2 ? 0 : 11 - resto2;

        return valores[13] == dv2;
    }
}