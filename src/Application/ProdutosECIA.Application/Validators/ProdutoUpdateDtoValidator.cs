using FluentValidation;
using ProdutosECIA.Application.DTOs;

namespace ProdutosECIA.Application.Validators;

public class ProdutoUpdateDtoValidator : AbstractValidator<ProdutoUpdateDto>
{
    public ProdutoUpdateDtoValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do produto deve ter no máximo 100 caracteres.");

        RuleFor(p => p.PrecoCusto)
            .GreaterThan(0).WithMessage("O preço de custo deve ser maior que zero.");

        RuleFor(p => p.PrecoVenda)
            .GreaterThan(0).WithMessage("O preço de venda deve ser maior que zero.");

        RuleFor(p => p.EmpresaId)
            .NotEmpty().WithMessage("O ID da empresa é obrigatório.");
    }
}