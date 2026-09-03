using System.Text.RegularExpressions;
using FluentValidation;
using GRA.Application.DTOs;

namespace GRA.Application.Validators;

internal static class ClienteValidationHelper
{
    private static readonly Regex CpfRegex = new(
        @"^\d{11}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(500));

    private static readonly Regex TelefoneRegex = new(
        @"^\d{10,11}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(500));

    private static readonly int[] Pesos1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] Pesos2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

    public static bool CpfEhValido(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var valor = cpf.Trim();

        if (!CpfRegex.IsMatch(valor))
            return false;

        if (valor.Distinct().Count() == 1)
            return false;

        var digitos = valor.Select(c => c - '0').ToArray();

        var soma1 = 0;
        for (var i = 0; i < 9; i++)
            soma1 += digitos[i] * Pesos1[i];

        var resto1 = soma1 % 11;
        var dv1 = resto1 < 2 ? 0 : 11 - resto1;

        if (digitos[9] != dv1)
            return false;

        var soma2 = 0;
        for (var i = 0; i < 10; i++)
            soma2 += digitos[i] * Pesos2[i];

        var resto2 = soma2 % 11;
        var dv2 = resto2 < 2 ? 0 : 11 - resto2;

        return digitos[10] == dv2;
    }

    public static bool TelefoneEhValido(string? telefone) =>
        string.IsNullOrWhiteSpace(telefone) || TelefoneRegex.IsMatch(telefone.Trim());

    public static bool EmailEhValido(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return true; // e-mail opcional

        return System.Net.Mail.MailAddress.TryCreate(email.Trim(), out var mail)
            && mail.Address == email.Trim();
    }

    public static bool NomeEhValido(string? nome) =>
        !string.IsNullOrWhiteSpace(nome) && nome.Trim().Length <= 150;
}

public abstract class ClienteDtoValidatorBase<T> : AbstractValidator<T>
{
    protected void AplicarRegrasComuns(
        Func<T, string?> nomeSelector,
        Func<T, string?> cpfSelector,
        Func<T, string?> telefoneSelector,
        Func<T, string?> emailSelector)
    {
        RuleFor(c => nomeSelector(c))
            .Must(nome => !string.IsNullOrWhiteSpace(nome))
                .WithMessage("Nome é obrigatório.")
            .Must(nome => nome!.Trim().Length <= 150)
                .WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(c => cpfSelector(c))
            .Must(ClienteValidationHelper.CpfEhValido)
                .WithMessage("CPF inválido (formato incorreto ou dígito verificador não confere).");

        RuleFor(c => telefoneSelector(c))
            .Must(ClienteValidationHelper.TelefoneEhValido)
                .WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.");

        RuleFor(c => emailSelector(c))
            .Must(ClienteValidationHelper.EmailEhValido)
                .WithMessage("Email inválido.");
    }
}

public class CadastrarClienteDtoValidator : ClienteDtoValidatorBase<CadastrarClienteDto>
{
    public CadastrarClienteDtoValidator()
    {
        AplicarRegrasComuns(
            c => c.Nome,
            c => c.CPF,
            c => c.Telefone,
            c => c.Email);

        RuleFor(c => c.Senha)
            .Must(senha => !string.IsNullOrWhiteSpace(senha))
                .WithMessage("Senha é obrigatória.")
            .Must(senha => senha!.Length >= 8)
                .WithMessage("Senha deve ter no mínimo 8 caracteres.");
    }
}

public class AtualizarClienteDtoValidator : ClienteDtoValidatorBase<AtualizarClienteDto>
{
    public AtualizarClienteDtoValidator()
    {
        AplicarRegrasComuns(
            c => c.Nome,
            c => c.CPF,
            c => c.Telefone,
            c => c.Email);
    }
}