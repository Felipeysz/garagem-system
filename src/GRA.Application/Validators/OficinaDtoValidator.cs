using System.Text.RegularExpressions;
using FluentValidation;
using GRA.Application.DTOs;

namespace GRA.Application.Validators;

internal static class OficinaValidationHelper
{
    private static readonly Regex CnpjRegex = new(
        @"^[A-Z0-9]{12}\d{2}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(500));

    private static readonly int[] Pesos1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] Pesos2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    public static bool CnpjEhValido(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        var valor = cnpj.Trim().ToUpperInvariant();

        if (!CnpjRegex.IsMatch(valor))
            return false;

        var valores = valor.Select(c => (int)c - 48).ToArray();

        var soma1 = 0;
        for (var i = 0; i < 12; i++)
            soma1 += valores[i] * Pesos1[i];

        var resto1 = soma1 % 11;
        var dv1 = resto1 < 2 ? 0 : 11 - resto1;

        if (valores[12] != dv1)
            return false;

        var soma2 = 0;
        for (var i = 0; i < 13; i++)
            soma2 += valores[i] * Pesos2[i];

        var resto2 = soma2 % 11;
        var dv2 = resto2 < 2 ? 0 : 11 - resto2;

        return valores[13] == dv2;
    }

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

public abstract class OficinaDtoValidatorBase<T> : AbstractValidator<T>
{
    protected void AplicarRegrasComuns(
        Func<T, string?> nomeSelector,
        Func<T, string?> cnpjSelector,
        Func<T, string?> emailSelector,
        Func<T, EnderecoDto?> enderecoSelector)
    {
        RuleFor(o => nomeSelector(o))
            .Must(nome => !string.IsNullOrWhiteSpace(nome))
                .WithMessage("Nome é obrigatório.")
            .Must(nome => nome!.Trim().Length <= 150)
                .WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(o => cnpjSelector(o))
            .Must(OficinaValidationHelper.CnpjEhValido)
                .WithMessage("CNPJ inválido (formato incorreto ou dígito verificador não confere).");

        RuleFor(o => emailSelector(o))
            .Must(OficinaValidationHelper.EmailEhValido)
                .WithMessage("Email inválido.");

        RuleFor(o => enderecoSelector(o)!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => enderecoSelector(o) is not null);
    }
}

public class CadastrarOficinaDtoValidator : OficinaDtoValidatorBase<CadastrarOficinaDto>
{
    public CadastrarOficinaDtoValidator()
    {
        AplicarRegrasComuns(
            o => o.Nome,
            o => o.CNPJ,
            o => o.Email,
            o => o.Endereco);
    }
}

public class AtualizarOficinaDtoValidator : OficinaDtoValidatorBase<AtualizarOficinaDto>
{
    public AtualizarOficinaDtoValidator()
    {
        AplicarRegrasComuns(
            o => o.Nome,
            o => o.CNPJ,
            o => o.Email,
            o => o.Endereco);
    }
}