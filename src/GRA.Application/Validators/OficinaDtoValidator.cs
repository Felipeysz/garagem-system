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
            .Must(cnpj => !string.IsNullOrWhiteSpace(cnpj)).WithMessage("CNPJ é obrigatório.")
            .Must(cnpj => Regex.IsMatch(cnpj.Trim().ToUpper(), @"^[A-Z0-9]{12}\d{2}$"))
                .WithMessage("CNPJ deve ter 14 caracteres (letras/números nas 12 primeiras posições e dígitos nas 2 últimas).")
            .Must(cnpj => CnpjEhValido(cnpj.Trim().ToUpper()))
                .WithMessage("CNPJ inválido (dígito verificador não confere).");

        RuleFor(o => o.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
                .WithMessage("Email inválido.");

        RuleFor(o => o.Endereco!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => o.Endereco is not null);
    }

    private static bool CnpjEhValido(string cnpj)
    {
        int[] pesos1 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3];
        int[] pesos2 = [7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3];

        var valores = cnpj.Select(c => (int)c - 48).ToArray();

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
            .Must(cnpj => !string.IsNullOrWhiteSpace(cnpj)).WithMessage("CNPJ é obrigatório.")
            .Must(cnpj => Regex.IsMatch(cnpj.Trim().ToUpper(), @"^[A-Z0-9]{12}\d{2}$"))
                .WithMessage("CNPJ deve ter 14 caracteres (letras/números nas 12 primeiras posições e dígitos nas 2 últimas).")
            .Must(cnpj => CnpjEhValido(cnpj.Trim().ToUpper()))
                .WithMessage("CNPJ inválido (dígito verificador não confere).");

        RuleFor(o => o.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
                .WithMessage("Email inválido.");

        RuleFor(o => o.Endereco!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => o.Endereco is not null);
    }

    private static bool CnpjEhValido(string cnpj)
    {
        int[] pesos1 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3];
        int[] pesos2 = [7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3];

        var valores = cnpj.Select(c => (int)c - 48).ToArray();

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