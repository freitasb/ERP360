using ERP360.Pedidos.Api.Contracts.Pedidos;
using FluentValidation;

namespace ERP360.Pedidos.Api.Validaion.Pedidos
{
    public sealed class CriarPedidoDtoValidator : AbstractValidator<CriarPedidoDto>
    {
        public CriarPedidoDtoValidator()
        {
            RuleFor(x => x.ClienteId)
                .NotEmpty().WithMessage("ClienteId é obrigatório.");

            RuleFor(x => x.Itens)
                .NotNull().WithMessage("Itens não podem ser nulos.")
                .NotEmpty().WithMessage("O pedido deve conter pelo menos um item.");

            RuleForEach(x => x.Itens)
                .SetValidator(new CriarPedidoItemDtoValidator());
        }
    }
}
