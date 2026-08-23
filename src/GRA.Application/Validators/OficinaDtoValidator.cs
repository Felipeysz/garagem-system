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
            .Must(cnpj => System.Text.RegularExpressions.Regex.IsMatch(cnpj.Trim(), @"^\d{14}$"))
                .WithMessage("CNPJ deve conter 14 dígitos numéricos.")
            .MustAsync(async (cnpj, ct) =>
            {
                var cnpjTrim = cnpj.Trim();
                return !await oficinaRepository.ExisteAsync(o => o.Ativo && o.CNPJ == cnpjTrim);
            }).WithMessage("Já existe uma oficina ativa cadastrada com esse CNPJ.");

        RuleFor(o => o.Email)
            .Cascade(CascadeMode.Stop)
            .Must(email => string.IsNullOrWhiteSpace(email) || new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
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
            .Must(cnpj => System.Text.RegularExpressions.Regex.IsMatch(cnpj.Trim(), @"^\d{14}$"))
                .WithMessage("CNPJ deve conter 14 dígitos numéricos.")
            .MustAsync(async (dto, cnpj, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                var cnpjTrim = cnpj.Trim();
                return !await oficinaRepository.ExisteAsync(o => o.Ativo && o.CNPJ == cnpjTrim && o.Id != id);
            }).WithMessage("Já existe uma oficina ativa cadastrada com esse CNPJ.");

        RuleFor(o => o.Email)
            .Cascade(CascadeMode.Stop)
            .Must(email => string.IsNullOrWhiteSpace(email) || new System.Net.Mail.MailAddress(email.Trim()).Address == email.Trim())
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