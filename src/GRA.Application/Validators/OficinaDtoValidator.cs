using FluentValidation;
using GRA.Application.DTOs;
using GRA.Domain.Repositories;

namespace GRA.Application.Validators;

public class CadastrarOficinaDtoValidator : AbstractValidator<CadastrarOficinaDto>
{
    public CadastrarOficinaDtoValidator(IOficinaRepository oficinaRepository)
    {
        RuleFor(o => o.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.")
            .MustAsync(async (nome, ct) => !await oficinaRepository.ExisteAsync(o => o.Nome == nome))
                .WithMessage("Já existe uma oficina cadastrada com esse nome.");

        RuleFor(o => o.CNPJ)
            .NotEmpty().WithMessage("CNPJ é obrigatório.")
            .Matches(@"^\d{14}$").WithMessage("CNPJ deve conter 14 dígitos numéricos.")
            .MustAsync(async (cnpj, ct) => !await oficinaRepository.ExisteAsync(o => o.CNPJ == cnpj))
                .WithMessage("Já existe uma oficina cadastrada com esse CNPJ.");

        RuleFor(o => o.Email)
            .EmailAddress().WithMessage("Email inválido.")
            .MustAsync(async (email, ct) => !await oficinaRepository.ExisteAsync(o => o.Email == email))
                .WithMessage("Já existe uma oficina cadastrada com esse email.")
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
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.")
            .MustAsync(async (dto, nome, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                return !await oficinaRepository.ExisteAsync(o => o.Nome == nome && o.Id != id);
            }).WithMessage("Já existe uma oficina cadastrada com esse nome.");

        RuleFor(o => o.CNPJ)
            .NotEmpty().WithMessage("CNPJ é obrigatório.")
            .Matches(@"^\d{14}$").WithMessage("CNPJ deve conter 14 dígitos numéricos.")
            .MustAsync(async (dto, cnpj, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                return !await oficinaRepository.ExisteAsync(o => o.CNPJ == cnpj && o.Id != id);
            }).WithMessage("Já existe uma oficina cadastrada com esse CNPJ.");

        RuleFor(o => o.Email)
            .EmailAddress().WithMessage("Email inválido.")
            .MustAsync(async (dto, email, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                return !await oficinaRepository.ExisteAsync(o => o.Email == email && o.Id != id);
            }).WithMessage("Já existe uma oficina cadastrada com esse email.")
            .When(o => !string.IsNullOrWhiteSpace(o.Email));

        RuleFor(o => o.Endereco!.Value)
            .SetValidator(new EnderecoDtoValidator())
            .When(o => o.Endereco is not null);
    }
}