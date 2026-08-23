using System.Text.RegularExpressions;
using FluentValidation;
using GRA.Application.DTOs;
using GRA.Domain.Repositories;

namespace GRA.Application.Validators;

public class CadastrarOficinaDtoValidator : AbstractValidator<CadastrarOficinaDto>
{
    public CadastrarOficinaDtoValidator(IOficinaRepository oficinaRepository)
    {
        RuleFor(o => o.Nome)
            .Cascade(CascadeMode.Stop)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.")
            .MustAsync(async (nome, ct) =>
            {
                var nomeTrim = nome.Trim();
                return !await oficinaRepository.ExisteAsync(o => o.Ativo && o.Nome.ToUpper() == nomeTrim.ToUpper());
            }).WithMessage("Já existe uma oficina ativa cadastrada com esse nome.");

        RuleFor(o => o.CNPJ)
            .Cascade(CascadeMode.Stop)
            .Must(cnpj => !string.IsNullOrWhiteSpace(cnpj)).WithMessage("CNPJ é obrigatório.")
            .Must(cnpj => Regex.IsMatch(cnpj.Trim().ToUpper(), @"^[A-Z0-9]{12}\d{2}$"))
                .WithMessage("CNPJ deve ter 14 caracteres (letras/números nas 12 primeiras posições e dígitos nas 2 últimas).")
            .Must(cnpj => CnpjValidador.EhValido(cnpj.Trim().ToUpper()))
                .WithMessage("CNPJ inválido (dígito verificador não confere).")
            .MustAsync(async (cnpj, ct) =>
            {
                var cnpjTrim = cnpj.Trim().ToUpper();
                return !await oficinaRepository.ExisteAsync(o => o.Ativo && o.CNPJ == cnpjTrim);
            }).WithMessage("Já existe uma oficina ativa cadastrada com esse CNPJ.");

        RuleFor(o => o.Email)
            .Cascade(CascadeMode.Stop)
            .Must(email => string.IsNullOrWhiteSpace(email) || EmailValidador.EhValido(email.Trim()))
                .WithMessage("Email inválido.")
            .MustAsync(async (email, ct) =>
            {
                var emailTrim = email!.Trim();
                return !await oficinaRepository.ExisteAsync(o => o.Ativo && o.Email != null && o.Email.ToUpper() == emailTrim.ToUpper());
            }).WithMessage("Já existe uma oficina ativa cadastrada com esse email.")
            .When(o => !string.IsNullOrWhiteSpace(o.Email));

        RuleFor(o => o.Endereco!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => o.Endereco is not null);
    }
}

public class AtualizarOficinaDtoValidator : AbstractValidator<AtualizarOficinaDto>
{
    public AtualizarOficinaDtoValidator(IOficinaRepository oficinaRepository)
    {
        RuleFor(o => o.Nome)
            .Cascade(CascadeMode.Stop)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.")
            .MustAsync(async (dto, nome, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                var nomeTrim = nome.Trim();
                return !await oficinaRepository.ExisteAsync(o => o.Ativo && o.Nome.ToUpper() == nomeTrim.ToUpper() && o.Id != id);
            }).WithMessage("Já existe uma oficina ativa cadastrada com esse nome.");

        RuleFor(o => o.CNPJ)
            .Cascade(CascadeMode.Stop)
            .Must(cnpj => !string.IsNullOrWhiteSpace(cnpj)).WithMessage("CNPJ é obrigatório.")
            .Must(cnpj => Regex.IsMatch(cnpj.Trim().ToUpper(), @"^[A-Z0-9]{12}\d{2}$"))
                .WithMessage("CNPJ deve ter 14 caracteres (letras/números nas 12 primeiras posições e dígitos nas 2 últimas).")
            .Must(cnpj => CnpjValidador.EhValido(cnpj.Trim().ToUpper()))
                .WithMessage("CNPJ inválido (dígito verificador não confere).")
            .MustAsync(async (dto, cnpj, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                var cnpjTrim = cnpj.Trim().ToUpper();
                return !await oficinaRepository.ExisteAsync(o => o.Ativo && o.CNPJ == cnpjTrim && o.Id != id);
            }).WithMessage("Já existe uma oficina ativa cadastrada com esse CNPJ.");

        RuleFor(o => o.Email)
            .Cascade(CascadeMode.Stop)
            .Must(email => string.IsNullOrWhiteSpace(email) || EmailValidador.EhValido(email.Trim()))
                .WithMessage("Email inválido.")
            .MustAsync(async (dto, email, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                var emailTrim = email!.Trim();
                return !await oficinaRepository.ExisteAsync(o => o.Ativo && o.Email != null && o.Email.ToUpper() == emailTrim.ToUpper() && o.Id != id);
            }).WithMessage("Já existe uma oficina ativa cadastrada com esse email.")
            .When(o => !string.IsNullOrWhiteSpace(o.Email));

        RuleFor(o => o.Endereco!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => o.Endereco is not null);
    }
}

file static class CnpjValidador
{
    public static bool EhValido(string cnpj)
    {
        int[] pesos1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] pesos2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

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

file static class EmailValidador
{
    public static bool EhValido(string email)
    {
        try
        {
            return new System.Net.Mail.MailAddress(email).Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}