using ERP360.Pedidos.Api.Contracts.Pedidos.CriarPedido;
using FluentValidation;

namespace ERP360.Pedidos.Api.Validaion.Pedidos
{
    public sealed class CriarPedidoItemDtoValidator : AbstractValidator<CriarPedidoItemDto>
    {
        public CriarPedidoItemDtoValidator()
        {
            RuleFor(x => x.ProdutoId)
                .NotEmpty().WithMessage("ProdutoId é obrigatório.");

            RuleFor(x => x.NomeProduto)
                .NotEmpty().WithMessage("Nome do produto é obrigatório.")
                .MaximumLength(200).WithMessage("Nome do produto deve ter no máximo 200 caracteres.");

            RuleFor(x => x.Quantidade)
                .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero.");

            RuleFor(x => x.PrecoUnitario)
                .GreaterThan(0m).WithMessage("Preço unitário deve ser maior que zero.");
        }
    }
}
