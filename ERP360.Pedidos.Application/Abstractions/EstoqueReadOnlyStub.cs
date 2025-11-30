using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Abstractions
{
    public sealed class EstoqueReadOnlyStub : IEstoqueReadOnlyService
    {
        public Task<bool> CheckDisponibilidadeAsync(IEnumerable<ItemSolicitado> itens, CancellationToken ct = default)
        {
            // Regra didática: se a soma das quantidades <= 50, consideramos disponível; senão, indisponível.
            var total = itens.Sum(i => i.Quantidade);
            return Task.FromResult(total <= 50);
        }
    }
}
