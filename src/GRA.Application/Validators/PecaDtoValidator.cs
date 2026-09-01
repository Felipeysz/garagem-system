using FluentValidation;
using GRA.Application.DTOs;

namespace GRA.Application.Validators;

public class CadastrarPecaDtoValidator : AbstractValidator<CadastrarPecaDto>
{
    public CadastrarPecaDtoValidator()
    {
        RuleFor(p => p.OficinaId)
            .GreaterThan(0).WithMessage("OficinaId é obrigatório.");

        RuleFor(p => p.Nome)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(p => p.CodigoInterno)
            .Must(codigo => string.IsNullOrWhiteSpace(codigo) || codigo.Trim().Length <= 50)
                .WithMessage("Código interno deve ter no máximo 50 caracteres.");

        RuleFor(p => p.UnidadeMedida)
            .Must(unidade => string.IsNullOrWhiteSpace(unidade) || unidade.Trim().Length <= 20)
                .WithMessage("Unidade de medida deve ter no máximo 20 caracteres.");

        RuleFor(p => p.PrecoVenda)
            .Must(preco => preco is null || preco >= 0)
                .WithMessage("Preço de venda não pode ser negativo.");

        RuleFor(p => p.EstoqueMinimo)
            .GreaterThanOrEqualTo(0).WithMessage("Estoque mínimo não pode ser negativo.");
    }
}

public class AtualizarPecaDtoValidator : AbstractValidator<AtualizarPecaDto>
{
    public AtualizarPecaDtoValidator()
    {
        RuleFor(p => p.Nome)
            .Must(nome => !string.IsNullOrWhiteSpace(nome)).WithMessage("Nome é obrigatório.")
            .Must(nome => nome.Trim().Length <= 150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(p => p.CodigoInterno)
            .Must(codigo => string.IsNullOrWhiteSpace(codigo) || codigo.Trim().Length <= 50)
                .WithMessage("Código interno deve ter no máximo 50 caracteres.");

        RuleFor(p => p.UnidadeMedida)
            .Must(unidade => string.IsNullOrWhiteSpace(unidade) || unidade.Trim().Length <= 20)
                .WithMessage("Unidade de medida deve ter no máximo 20 caracteres.");

        RuleFor(p => p.PrecoVenda)
            .Must(preco => preco is null || preco >= 0)
                .WithMessage("Preço de venda não pode ser negativo.");

        RuleFor(p => p.EstoqueMinimo)
            .GreaterThanOrEqualTo(0).WithMessage("Estoque mínimo não pode ser negativo.");
    }
}