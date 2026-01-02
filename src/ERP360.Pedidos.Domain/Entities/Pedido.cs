using ERP360.Pedidos.Domain.Common;
using ERP360.Pedidos.Domain.Enums;
using ERP360.Pedidos.Domain.Events;
using ERP360.Pedidos.Domain.Primitives;
using ERP360.Pedidos.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Domain.Entities
{
    public sealed class Pedido
    {
        private readonly List<PedidoItem> _itens = new();
        private readonly List<IDomainEvent> _events = new();


        public Guid PedidoId { get; private set; }
        public Guid ClienteId { get; private set; }
        public string Numero { get; private set; } = string.Empty; // Pode ser preenchido ao confirmar
        public StatusPedido Status { get; private set; }
        public DateTimeOffset DataCriacao { get; private set; }
        public DateTimeOffset? DataAtualizacaoStatus { get; private set; }


        public IReadOnlyCollection<PedidoItem> Itens => _itens.AsReadOnly();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();


        public Money Total => _itens.Aggregate(Money.Zero(), (acc, item) => acc.Add(item.Subtotal));


        private Pedido() { /* EF/Serialização */ }


        private Pedido(Guid clienteId)
        {
            if (clienteId == Guid.Empty) throw new ArgumentException("ClienteId inválido.");
            PedidoId = Guid.NewGuid();
            ClienteId = clienteId;
            Status = StatusPedido.Rascunho;
            DataCriacao = DateTimeOffset.Now;
        }


        public static Pedido CriarRascunho(Guid clienteId)
        {
            return new Pedido(clienteId);
        }

        public DomainResult AlterarStatus(StatusPedido destino)
        {
            if (destino == StatusPedido.Cancelado)
                return CancelarManual();

            return TransicionarPara(destino, "Alteração de status");
        }


        public void AdicionarItem(Guid produtoId, string nome, int quantidade, Money precoUnitario)
        {
            var novo = new PedidoItem(PedidoId, produtoId, nome, quantidade, precoUnitario);
            _itens.Add(novo);
            // Invariante: total sempre coerente (Total é calculado on-the-fly)
        }


        public DomainResult Confirmar()
        {
            // Rascunho -> AguardandoPagamento
            var rascunhoCriado = TransicionarPara(StatusPedido.AguardandoPagamento, motivo: "Cliente confirmou pedido");
            if (!rascunhoCriado.IsSuccess) return rascunhoCriado;
            if (string.IsNullOrWhiteSpace(Numero)) Numero = GerarNumero();
            _events.Add(new Events.PedidoCriado(PedidoId, ClienteId, Total));
            return DomainResult.Success();
        }


        public DomainResult MarcarPago() => TransicionarPara(StatusPedido.Pago, "Pagamento confirmado");
        public DomainResult IniciarSeparacao() => TransicionarPara(StatusPedido.EmSeparacao, "Estoque reservado e nota emitida");
        public DomainResult MarcarEnviado() => TransicionarPara(StatusPedido.Enviado, "Pedido despachado");
        public DomainResult MarcarEntregue() => TransicionarPara(StatusPedido.Entregue, "Entrega confirmada");
        public DomainResult IniciarDevolucao() => TransicionarPara(StatusPedido.EmDevolucao, "Cliente solicitou devolução");
        public DomainResult ConcluirDevolucao() => TransicionarPara(StatusPedido.Devolvido, "Devolução concluída e reembolso feito");


        public void CancelarPorFalhaPagamento() => TransicionarPara(StatusPedido.Cancelado, "Falha de pagamento");


        /// <summary>
        /// Cancelamento manual permitido apenas **antes do envio**.
        /// </summary>
        public DomainResult CancelarManual()
        {
            if (Status == StatusPedido.Enviado || Status == StatusPedido.Entregue ||
            Status == StatusPedido.EmDevolucao || Status == StatusPedido.Devolvido)
            {
                return DomainResult.Failure("Cancelamento não permitido após envio.");
            }
            return TransicionarPara(StatusPedido.Cancelado, "Cancelamento manual");
        }


        private DomainResult TransicionarPara(StatusPedido destino, string motivo)
        {
            if (!PodeTransitar(Status, destino))
                return DomainResult.Failure($"Transição inválida: {Status} -> {destino}");//throw new InvalidOperationException($"Transição inválida: {Status} -> {destino}");


            var anterior = Status;
            Status = destino;
            DataAtualizacaoStatus = DateTimeOffset.Now;
            _events.Add(new Events.StatusPedidoAlterado(PedidoId, anterior, destino, motivo, DataAtualizacaoStatus.Value));


            if (destino == StatusPedido.Cancelado)
            {
                _events.Add(new Events.PedidoCancelado(PedidoId, motivo));
            }
            return DomainResult.Success();
        }

        private static bool PodeTransitar(StatusPedido de, StatusPedido para) => (de, para) switch
        {
            (StatusPedido.Rascunho, StatusPedido.AguardandoPagamento) => true,
            (StatusPedido.AguardandoPagamento, StatusPedido.Pago) => true,
            (StatusPedido.Pago, StatusPedido.EmSeparacao) => true,
            (StatusPedido.EmSeparacao, StatusPedido.Enviado) => true,
            (StatusPedido.Enviado, StatusPedido.Entregue) => true,
            (StatusPedido.Enviado, StatusPedido.Devolvido) => true, // falha na entrega
            (StatusPedido.Entregue, StatusPedido.EmDevolucao) => true,
            (StatusPedido.EmDevolucao, StatusPedido.Devolvido) => true,


            // Cancelamento manual antes do envio
            (StatusPedido.Rascunho, StatusPedido.Cancelado) => true,
            (StatusPedido.AguardandoPagamento, StatusPedido.Cancelado) => true,
            (StatusPedido.Pago, StatusPedido.Cancelado) => true,


            _ => false
        };

        private static string GerarNumero() => $"P-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
    }
}
