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
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.")
            .MustAsync(async (dto, nome, ct) => !await funcionarioRepository.ExisteAsync(f =>
                f.OficinaId == dto.OficinaId && f.Nome == nome))
                .WithMessage("Já existe um funcionário com esse nome nessa oficina.");

        RuleFor(f => f.CPF)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Matches(@"^\d{11}$").WithMessage("CPF deve conter 11 dígitos numéricos.")
            .MustAsync(async (dto, cpf, ct) => !await funcionarioRepository.ExisteAsync(f =>
                f.OficinaId == dto.OficinaId && f.CPF == cpf))
                .WithMessage("Já existe um funcionário com esse CPF nessa oficina.");

        RuleFor(f => f.Telefone)
            .Matches(@"^\d{10,11}$").WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.")
            .When(f => !string.IsNullOrWhiteSpace(f.Telefone));

        RuleFor(f => f.Email)
            .Cascade(CascadeMode.Stop)
            .EmailAddress().WithMessage("Email inválido.")
            .MustAsync(async (dto, email, ct) => !await funcionarioRepository.ExisteAsync(f =>
                f.OficinaId == dto.OficinaId && f.Email == email))
                .WithMessage("Já existe um funcionário com esse email nessa oficina.")
            .When(f => !string.IsNullOrWhiteSpace(f.Email));

        RuleFor(f => f.Cargo)
            .NotEmpty().WithMessage("Cargo é obrigatório.")
            .MaximumLength(100).WithMessage("Cargo deve ter no máximo 100 caracteres.");

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
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.")
            .MustAsync(async (dto, nome, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                var oficinaId = (long)context.RootContextData["OficinaId"];
                return !await funcionarioRepository.ExisteAsync(f =>
                    f.Id != id && f.OficinaId == oficinaId && f.Nome == nome);
            }).WithMessage("Já existe um funcionário com esse nome nessa oficina.");

        RuleFor(f => f.CPF)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Matches(@"^\d{11}$").WithMessage("CPF deve conter 11 dígitos numéricos.")
            .MustAsync(async (dto, cpf, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                var oficinaId = (long)context.RootContextData["OficinaId"];
                return !await funcionarioRepository.ExisteAsync(f =>
                    f.Id != id && f.OficinaId == oficinaId && f.CPF == cpf);
            }).WithMessage("Já existe um funcionário com esse CPF nessa oficina.");

        RuleFor(f => f.Telefone)
            .Matches(@"^\d{10,11}$").WithMessage("Telefone deve conter 10 ou 11 dígitos numéricos.")
            .When(f => !string.IsNullOrWhiteSpace(f.Telefone));

        RuleFor(f => f.Email)
            .Cascade(CascadeMode.Stop)
            .EmailAddress().WithMessage("Email inválido.")
            .MustAsync(async (dto, email, context, ct) =>
            {
                var id = (long)context.RootContextData["Id"];
                var oficinaId = (long)context.RootContextData["OficinaId"];
                return !await funcionarioRepository.ExisteAsync(f =>
                    f.Id != id && f.OficinaId == oficinaId && f.Email == email);
            }).WithMessage("Já existe um funcionário com esse email nessa oficina.")
            .When(f => !string.IsNullOrWhiteSpace(f.Email));

        RuleFor(f => f.Cargo)
            .NotEmpty().WithMessage("Cargo é obrigatório.")
            .MaximumLength(100).WithMessage("Cargo deve ter no máximo 100 caracteres.");

        RuleFor(f => f.DataAdmissao)
            .NotEmpty().WithMessage("Data de admissão é obrigatória.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Data de admissão não pode ser no futuro.");
    }
}