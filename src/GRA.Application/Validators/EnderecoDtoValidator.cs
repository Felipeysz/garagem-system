using FluentValidation;
using GRA.Application.DTOs;

namespace GRA.Application.Validators;

public class EnderecoDtoValidator : AbstractValidator<EnderecoDto>
{
    public EnderecoDtoValidator()
    {
        RuleFor(e => e.Logradouro).NotEmpty().WithMessage("Logradouro é obrigatório.");
        RuleFor(e => e.Numero).NotEmpty().WithMessage("Número é obrigatório.");
        RuleFor(e => e.Bairro).NotEmpty().WithMessage("Bairro é obrigatório.");
        RuleFor(e => e.Cidade).NotEmpty().WithMessage("Cidade é obrigatória.");

        RuleFor(e => e.Estado)
            .NotEmpty().WithMessage("Estado é obrigatório.")
            .Length(2).WithMessage("Estado deve ser a sigla da UF (2 letras).");

        RuleFor(e => e.CEP)
            .NotEmpty().WithMessage("CEP é obrigatório.")
            .Matches(@"^\d{8}$").WithMessage("CEP deve conter 8 dígitos numéricos.");
    }
}