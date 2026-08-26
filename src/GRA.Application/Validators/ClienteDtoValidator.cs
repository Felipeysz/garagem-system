using System.Text.RegularExpressions;
using FluentValidation;
using GRA.Application.DTOs;
using GRA.Shared.Documentos;

namespace GRA.Application.Validators;

public class CadastrarClienteDtoValidator : AbstractValidator<CadastrarClienteDto>
{
    public CadastrarClienteDtoValidator()
    {
        RuleFor(c => c.Nome)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(c => c.CPF)
            .Must(cpf => !string.IsNullOrWhiteSpace(cpf)).WithMessage("CPF é obrigatório.")
            .Must(cpf => Regex.IsMatch(cpf.Trim(), @"^\d{11}$")).WithMessage("CPF deve conter 11 dígitos numéricos.")
            .Must(cpf => DocumentoValidacao.CpfEhValido(cpf.Trim())).WithMessage("CPF inválido (dígito verificador não confere).");

        RuleFor(c => c.Senha)
            .Must(senha => !string.IsNullOrWhiteSpace(senha)).WithMessage("Senha é obrigatória.")
            .Must(senha => senha.Length >= 8).WithMessage("Senha deve ter no mínimo 8 caracteres.");

        RuleFor(c => c.Telefone)
            .Must(telefone => string.IsNullOrWhiteSpace(telefone) ||
                Regex.IsMatch(telefone.Trim(), @"^\d{10,11}$"))
                .WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.");

        RuleFor(c => c.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                System.Net.Mail.MailAddress.TryCreate(email.Trim(), out _))
                .WithMessage("Email inválido.");
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
            .Must(cpf => !string.IsNullOrWhiteSpace(cpf)).WithMessage("CPF é obrigatório.")
            .Must(cpf => Regex.IsMatch(cpf.Trim(), @"^\d{11}$")).WithMessage("CPF deve conter 11 dígitos numéricos.")
            .Must(cpf => DocumentoValidacao.CpfEhValido(cpf.Trim())).WithMessage("CPF inválido (dígito verificador não confere).");

        RuleFor(c => c.Telefone)
            .Must(telefone => string.IsNullOrWhiteSpace(telefone) ||
                Regex.IsMatch(telefone.Trim(), @"^\d{10,11}$"))
                .WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.");

        RuleFor(c => c.Email)
            .Must(email => string.IsNullOrWhiteSpace(email) ||
                System.Net.Mail.MailAddress.TryCreate(email.Trim(), out _))
                .WithMessage("Email inválido.");
    }
}