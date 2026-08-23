using System.Text.RegularExpressions;
using FluentValidation;
using GRA.Application.DTOs;
using GRA.Domain.Repositories;

namespace GRA.Application.Validators;

public class CadastrarFuncionarioDtoValidator : AbstractValidator<CadastrarFuncionarioDto>
{
    public CadastrarFuncionarioDtoValidator(IFuncionarioRepository funcionarioRepository)
    {
        RuleFor(f => f.OficinaId)
            .GreaterThan(0).WithMessage("OficinaId é obrigatório.");

        RuleFor(f => f.Nome)
            .Cascade(CascadeMode.Stop)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.")
            .MustAsync(async (dto, nome, ct) =>
            {
                var nomeTrim = nome.Trim();
                return !await funcionarioRepository.ExisteAsync(f =>
                    f.OficinaId == dto.OficinaId && f.Nome.ToUpper() == nomeTrim.ToUpper());
            }).WithMessage("Já existe um funcionário com esse nome nessa oficina.");

        RuleFor(f => f.CPF)
            .Cascade(CascadeMode.Stop)
            .Must(cpf => !string.IsNullOrWhiteSpace(cpf)).WithMessage("CPF é obrigatório.")
            .Must(cpf => Regex.IsMatch(cpf.Trim(), @"^\d{11}$"))
                .WithMessage("CPF deve conter 11 dígitos numéricos.")
            .Must(cpf => CpfValidador.EhValido(cpf.Trim()))
                .WithMessage("CPF inválido (dígito verificador não confere).")
            .MustAsync(async (dto, cpf, ct) =>
            {
                var cpfTrim = cpf.Trim();
                return !await funcionarioRepository.ExisteAsync(f =>
                    f.OficinaId == dto.OficinaId && f.CPF == cpfTrim);
            }).WithMessage("Já existe um funcionário com esse CPF nessa oficina.");

        RuleFor(f => f.Telefone)
            .Must(telefone => string.IsNullOrWhiteSpace(telefone) ||
                Regex.IsMatch(telefone.Trim(), @"^\d{10,11}$"))
                .WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.")
            .When(f => !string.IsNullOrWhiteSpace(f.Telefone));

        RuleFor(f => f.Email)
            .Cascade(CascadeMode.Stop)
            .Must(email => string.IsNullOrWhiteSpace(email) || EmailValidador.EhValido(email.Trim()))
                .WithMessage("Email inválido.")
            .MustAsync(async (dto, email, ct) =>
            {
                var emailTrim = email!.Trim();
                return !await funcionarioRepository.ExisteAsync(f =>
                    f.OficinaId == dto.OficinaId && f.Email != null && f.Email.ToUpper() == emailTrim.ToUpper());
            }).WithMessage("Já existe um funcionário com esse email nessa oficina.")
            .When(f => !string.IsNullOrWhiteSpace(f.Email));

        RuleFor(f => f.Cargo)
            .Cascade(CascadeMode.Stop)
            .Must(cargo => !string.IsNullOrWhiteSpace(cargo)).WithMessage("Cargo é obrigatório.")
            .Must(cargo => cargo.Trim().Length <= 100).WithMessage("Cargo deve ter no máximo 100 caracteres.");

        RuleFor(f => f.DataAdmissao)
            .NotEmpty().WithMessage("Data de admissão é obrigatória.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Data de admissão não pode ser no futuro.");
    }
}

public class AtualizarFuncionarioDtoValidator : AbstractValidator<AtualizarFuncionarioDto>
{
    public AtualizarFuncionarioDtoValidator(IFuncionarioRepository funcionarioRepository)
    {
        RuleFor(f => f.Nome)
            .Cascade(CascadeMode.Stop)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.")
            .MustAsync(async (dto, nome, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                var oficinaId = (long)context.RootContextData["OficinaId"];
                var nomeTrim = nome.Trim();
                return !await funcionarioRepository.ExisteAsync(f =>
                    f.Id != id && f.OficinaId == oficinaId && f.Nome.ToUpper() == nomeTrim.ToUpper());
            }).WithMessage("Já existe um funcionário com esse nome nessa oficina.");

        RuleFor(f => f.CPF)
            .Cascade(CascadeMode.Stop)
            .Must(cpf => !string.IsNullOrWhiteSpace(cpf)).WithMessage("CPF é obrigatório.")
            .Must(cpf => Regex.IsMatch(cpf.Trim(), @"^\d{11}$"))
                .WithMessage("CPF deve conter 11 dígitos numéricos.")
            .Must(cpf => CpfValidador.EhValido(cpf.Trim()))
                .WithMessage("CPF inválido (dígito verificador não confere).")
            .MustAsync(async (dto, cpf, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                var oficinaId = (long)context.RootContextData["OficinaId"];
                var cpfTrim = cpf.Trim();
                return !await funcionarioRepository.ExisteAsync(f =>
                    f.Id != id && f.OficinaId == oficinaId && f.CPF == cpfTrim);
            }).WithMessage("Já existe um funcionário com esse CPF nessa oficina.");

        RuleFor(f => f.Telefone)
            .Must(telefone => string.IsNullOrWhiteSpace(telefone) ||
                Regex.IsMatch(telefone.Trim(), @"^\d{10,11}$"))
                .WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.")
            .When(f => !string.IsNullOrWhiteSpace(f.Telefone));

        RuleFor(f => f.Email)
            .Cascade(CascadeMode.Stop)
            .Must(email => string.IsNullOrWhiteSpace(email) || EmailValidador.EhValido(email.Trim()))
                .WithMessage("Email inválido.")
            .MustAsync(async (dto, email, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                var oficinaId = (long)context.RootContextData["OficinaId"];
                var emailTrim = email!.Trim();
                return !await funcionarioRepository.ExisteAsync(f =>
                    f.Id != id && f.OficinaId == oficinaId && f.Email != null && f.Email.ToUpper() == emailTrim.ToUpper());
            }).WithMessage("Já existe um funcionário com esse email nessa oficina.")
            .When(f => !string.IsNullOrWhiteSpace(f.Email));

        RuleFor(f => f.Cargo)
            .Cascade(CascadeMode.Stop)
            .Must(cargo => !string.IsNullOrWhiteSpace(cargo)).WithMessage("Cargo é obrigatório.")
            .Must(cargo => cargo.Trim().Length <= 100).WithMessage("Cargo deve ter no máximo 100 caracteres.");

        RuleFor(f => f.DataAdmissao)
            .NotEmpty().WithMessage("Data de admissão é obrigatória.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Data de admissão não pode ser no futuro.");
    }
}

file static class CpfValidador
{
    public static bool EhValido(string cpf)
    {
        // Rejeita sequências com todos os dígitos iguais (000.000.000-00, 111.111.111-11 etc.)
        // — passam despercebidas pelo cálculo do dígito verificador mas são inválidas na prática.
        if (cpf.Distinct().Count() == 1)
            return false;

        var valores = cpf.Select(c => (int)c - 48).ToArray();

        int[] pesos1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        var soma1 = 0;
        for (var i = 0; i < 9; i++)
            soma1 += valores[i] * pesos1[i];

        var resto1 = soma1 % 11;
        var dv1 = resto1 < 2 ? 0 : 11 - resto1;

        if (valores[9] != dv1)
            return false;

        int[] pesos2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];
        var soma2 = 0;
        for (var i = 0; i < 10; i++)
            soma2 += valores[i] * pesos2[i];

        var resto2 = soma2 % 11;
        var dv2 = resto2 < 2 ? 0 : 11 - resto2;

        return valores[10] == dv2;
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