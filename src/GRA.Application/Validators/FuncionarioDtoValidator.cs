using System.Text.RegularExpressions;
using FluentValidation;
using GRA.Application.DTOs;
using GRA.Domain.ValueObjects;

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
            .Must(cpf => Cpf.TryCreate(cpf, out _))
                .WithMessage("CPF inválido (formato incorreto ou dígito verificador não confere).");

        RuleFor(f => f.Senha)
            .Must(senha => !string.IsNullOrWhiteSpace(senha)).WithMessage("Senha é obrigatória.")
            .Must(senha => senha.Length >= 8).WithMessage("Senha deve ter no mínimo 8 caracteres.");

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
            .Must(cpf => Cpf.TryCreate(cpf, out _))
                .WithMessage("CPF inválido (formato incorreto ou dígito verificador não confere).");

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