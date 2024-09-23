using FluentValidation;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Domain.Utils;

namespace ProdutosECIA.Application.Validators;

public class EmpresaUpdateDtoValidator : AbstractValidator<EmpresaUpdateDto>
{
    public EmpresaUpdateDtoValidator()
    {
        RuleFor(e => e.Nome)
            .NotEmpty().WithMessage("O nome da empresa é obrigatório.")
            .Length(2, 100).WithMessage("O nome deve ter entre 2 e 100 caracteres.");

        RuleFor(e => e.CNPJ)
            .NotEmpty().WithMessage("O CNPJ da empresa é obrigatório.")
            .Must(IsValidCnpj).WithMessage("O CNPJ informado não é válido.");
    }

    private bool IsValidCnpj(string cnpj)
    {
        return FieldValidators.CnpjIsValid(cnpj);
    }
}