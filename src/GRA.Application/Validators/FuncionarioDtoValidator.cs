using System.Text.RegularExpressions;
using FluentValidation;
using GRA.Application.DTOs;
using GRA.Shared.Documentos;

namespace GRA.Application.Validators;

public class CadastrarFuncionarioDtoValidator : AbstractValidator<CadastrarFuncionarioDto>
{
    public CadastrarFuncionarioDtoValidator()
    {
        RuleFor(f => f.OficinaId)
            .GreaterThan(0).WithMessage("OficinaId é obrigatório.");

        RuleFor(f => f.Nome)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(f => f.CPF)
            .Must(cpf => !string.IsNullOrWhiteSpace(cpf)).WithMessage("CPF é obrigatório.")
            .Must(cpf => Regex.IsMatch(cpf.Trim(), @"^\d{11}$")).WithMessage("CPF deve conter 11 dígitos numéricos.")
            .Must(cpf => DocumentoValidacao.CpfEhValido(cpf.Trim())).WithMessage("CPF inválido (dígito verificador não confere).");

        RuleFor(f => f.Telefone)
            .Must(telefone => string.IsNullOrWhiteSpace(telefone) ||
                Regex.IsMatch(telefone.Trim(), @"^\d{10,11}$"))
                .WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.");

        RuleFor(f => f.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
                .WithMessage("Email inválido.");

        RuleFor(f => f.Cargo)
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
    public AtualizarFuncionarioDtoValidator()
    {
        RuleFor(f => f.Nome)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(f => f.CPF)
            .Must(cpf => !string.IsNullOrWhiteSpace(cpf)).WithMessage("CPF é obrigatório.")
            .Must(cpf => Regex.IsMatch(cpf.Trim(), @"^\d{11}$")).WithMessage("CPF deve conter 11 dígitos numéricos.")
            .Must(cpf => DocumentoValidacao.CpfEhValido(cpf.Trim())).WithMessage("CPF inválido (dígito verificador não confere).");

        RuleFor(f => f.Telefone)
            .Must(telefone => string.IsNullOrWhiteSpace(telefone) ||
                Regex.IsMatch(telefone.Trim(), @"^\d{10,11}$"))
                .WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.");

        RuleFor(f => f.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
                .WithMessage("Email inválido.");

        RuleFor(f => f.Cargo)
            .Must(cargo => !string.IsNullOrWhiteSpace(cargo)).WithMessage("Cargo é obrigatório.")
            .Must(cargo => cargo.Trim().Length <= 100).WithMessage("Cargo deve ter no máximo 100 caracteres.");

        RuleFor(f => f.DataAdmissao)
            .NotEmpty().WithMessage("Data de admissão é obrigatória.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Data de admissão não pode ser no futuro.");
    }
}