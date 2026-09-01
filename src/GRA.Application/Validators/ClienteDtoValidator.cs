using System.Text.RegularExpressions;
using FluentValidation;
using GRA.Application.DTOs;

namespace GRA.Application.Validators;

public class CadastrarClienteDtoValidator : AbstractValidator<CadastrarClienteDto>
{
    public CadastrarClienteDtoValidator()
    {
        RuleFor(c => c.Nome)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(c => c.CPF)
            .Must(CpfEhValido)
                .WithMessage("CPF inválido (formato incorreto ou dígito verificador não confere).");

        RuleFor(c => c.Senha)
            .Must(senha => !string.IsNullOrWhiteSpace(senha)).WithMessage("Senha é obrigatória.")
            .Must(senha => senha.Length >= 8).WithMessage("Senha deve ter no mínimo 8 caracteres.");

        RuleFor(c => c.Telefone)
            .Must(telefone => string.IsNullOrWhiteSpace(telefone) ||
                Regex.IsMatch(telefone.Trim(), @"^\d{10,11}$"))
                .WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.");

        RuleFor(c => c.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
                .WithMessage("Email inválido.");
    }

    private static bool CpfEhValido(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var valor = cpf.Trim();

        if (!Regex.IsMatch(valor, @"^\d{11}$"))
            return false;

        if (valor.Distinct().Count() == 1)
            return false;

        int[] pesos1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] pesos2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        var digitos = valor.Select(c => c - '0').ToArray();

        var soma1 = 0;
        for (var i = 0; i < 9; i++)
            soma1 += digitos[i] * pesos1[i];

        var resto1 = soma1 % 11;
        var dv1 = resto1 < 2 ? 0 : 11 - resto1;

        if (digitos[9] != dv1)
            return false;

        var soma2 = 0;
        for (var i = 0; i < 10; i++)
            soma2 += digitos[i] * pesos2[i];

        var resto2 = soma2 % 11;
        var dv2 = resto2 < 2 ? 0 : 11 - resto2;

        return digitos[10] == dv2;
    }
}

public class AtualizarClienteDtoValidator : AbstractValidator<AtualizarClienteDto>
{
    public AtualizarClienteDtoValidator()
    {
        RuleFor(c => c.Nome)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(c => c.CPF)
            .Must(CpfEhValido)
                .WithMessage("CPF inválido (formato incorreto ou dígito verificador não confere).");

        RuleFor(c => c.Telefone)
            .Must(telefone => string.IsNullOrWhiteSpace(telefone) ||
                Regex.IsMatch(telefone.Trim(), @"^\d{10,11}$"))
                .WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.");

        RuleFor(c => c.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
                .WithMessage("Email inválido.");
    }

    private static bool CpfEhValido(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var valor = cpf.Trim();

        if (!Regex.IsMatch(valor, @"^\d{11}$"))
            return false;

        if (valor.Distinct().Count() == 1)
            return false;

        int[] pesos1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] pesos2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        var digitos = valor.Select(c => c - '0').ToArray();

        var soma1 = 0;
        for (var i = 0; i < 9; i++)
            soma1 += digitos[i] * pesos1[i];

        var resto1 = soma1 % 11;
        var dv1 = resto1 < 2 ? 0 : 11 - resto1;

        if (digitos[9] != dv1)
            return false;

        var soma2 = 0;
        for (var i = 0; i < 10; i++)
            soma2 += digitos[i] * pesos2[i];

        var resto2 = soma2 % 11;
        var dv2 = resto2 < 2 ? 0 : 11 - resto2;

        return digitos[10] == dv2;
    }
}