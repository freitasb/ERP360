using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Contracts.Pedidos
{
    public sealed record ItemSolicitado(Guid ProdutoId, int Quantidade);
}
