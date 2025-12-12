using ERP360.Pedidos.Domain.Enums;
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



        public void AdicionarItem(Guid produtoId, string nome, int quantidade, Money precoUnitario)
        {
            var novo = new PedidoItem(produtoId, nome, quantidade, precoUnitario);
            _itens.Add(novo);
            // Invariante: total sempre coerente (Total é calculado on-the-fly)
        }


        public void Confirmar()
        {
            // Rascunho -> AguardandoPagamento
            TransicionarPara(StatusPedido.AguardandoPagamento, motivo: "Cliente confirmou pedido");
            if (string.IsNullOrWhiteSpace(Numero)) Numero = GerarNumero();
            _events.Add(new Events.PedidoCriado(PedidoId, ClienteId, Total));
        }


        public void MarcarPago() => TransicionarPara(StatusPedido.Pago, "Pagamento confirmado");
        public void IniciarSeparacao() => TransicionarPara(StatusPedido.EmSeparacao, "Estoque reservado e nota emitida");
        public void MarcarEnviado() => TransicionarPara(StatusPedido.Enviado, "Pedido despachado");
        public void MarcarEntregue() => TransicionarPara(StatusPedido.Entregue, "Entrega confirmada");
        public void IniciarDevolucao() => TransicionarPara(StatusPedido.EmDevolucao, "Cliente solicitou devolução");
        public void ConcluirDevolucao() => TransicionarPara(StatusPedido.Devolvido, "Devolução concluída e reembolso feito");


        public void CancelarPorFalhaPagamento() => TransicionarPara(StatusPedido.Cancelado, "Falha de pagamento");


        /// <summary>
        /// Cancelamento manual permitido apenas **antes do envio**.
        /// </summary>
        public void CancelarManual()
        {
            if (Status == StatusPedido.Enviado || Status == StatusPedido.Entregue ||
            Status == StatusPedido.EmDevolucao || Status == StatusPedido.Devolvido)
            {
                throw new InvalidOperationException("Cancelamento não permitido após envio.");
            }
            TransicionarPara(StatusPedido.Cancelado, "Cancelamento manual");
        }


        private void TransicionarPara(StatusPedido destino, string motivo)
        {
            if (!PodeTransitar(Status, destino))
                throw new InvalidOperationException($"Transição inválida: {Status} -> {destino}");


            var anterior = Status;
            Status = destino;
            DataAtualizacaoStatus = DateTimeOffset.Now;
            _events.Add(new Events.StatusPedidoAlterado(PedidoId, anterior, destino, motivo, DataAtualizacaoStatus.Value));


            if (destino == StatusPedido.Cancelado)
            {
                _events.Add(new Events.PedidoCancelado(PedidoId, motivo));
            }
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
