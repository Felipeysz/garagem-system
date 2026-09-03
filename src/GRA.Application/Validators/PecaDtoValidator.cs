using FluentValidation;
using GRA.Application.DTOs;

namespace GRA.Application.Validators;

public abstract class PecaDtoValidatorBase<T> : AbstractValidator<T>
{
    protected void AplicarRegrasComuns(
        Func<T, string?> nomeSelector,
        Func<T, string?> codigoInternoSelector,
        Func<T, string?> unidadeMedidaSelector,
        Func<T, decimal?> precoVendaSelector,
        Func<T, int> estoqueMinimoSelector)
    {
        RuleFor(p => nomeSelector(p))
            .Must(nome => !string.IsNullOrWhiteSpace(nome))
                .WithMessage("Nome é obrigatório.")
            .Must(nome => nome!.Trim().Length <= 150)
                .WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(p => codigoInternoSelector(p))
            .Must(codigo => string.IsNullOrWhiteSpace(codigo) || codigo.Trim().Length <= 50)
                .WithMessage("Código interno deve ter no máximo 50 caracteres.");

        RuleFor(p => unidadeMedidaSelector(p))
            .Must(unidade => string.IsNullOrWhiteSpace(unidade) || unidade.Trim().Length <= 20)
                .WithMessage("Unidade de medida deve ter no máximo 20 caracteres.");

        RuleFor(p => precoVendaSelector(p))
            .Must(preco => preco is null || preco >= 0)
                .WithMessage("Preço de venda não pode ser negativo.");

        RuleFor(p => estoqueMinimoSelector(p))
            .GreaterThanOrEqualTo(0).WithMessage("Estoque mínimo não pode ser negativo.");
    }
}

public class CadastrarPecaDtoValidator : PecaDtoValidatorBase<CadastrarPecaDto>
{
    public CadastrarPecaDtoValidator()
    {
        RuleFor(p => p.OficinaId)
            .GreaterThan(0).WithMessage("OficinaId é obrigatório.");

        AplicarRegrasComuns(
            p => p.Nome,
            p => p.CodigoInterno,
            p => p.UnidadeMedida,
            p => p.PrecoVenda,
            p => p.EstoqueMinimo);
    }
}

public class AtualizarPecaDtoValidator : PecaDtoValidatorBase<AtualizarPecaDto>
{
    public AtualizarPecaDtoValidator()
    {
        AplicarRegrasComuns(
            p => p.Nome,
            p => p.CodigoInterno,
            p => p.UnidadeMedida,
            p => p.PrecoVenda,
            p => p.EstoqueMinimo);
    }
}