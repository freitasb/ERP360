# ERP360

Projeto backend em .NET 8 organizado por bounded contexts, desenvolvido para consolidar práticas de arquitetura que aparecem em sistemas corporativos reais: separação de responsabilidades, integração assíncrona entre serviços, persistência relacional e testes automatizados.

O contexto de negócio é um ERP simplificado com fluxo de pedidos e controle de estoque. A escolha foi intencional: trabalho com sistemas corporativos e quis praticar decisões arquiteturais dentro de um domínio que já conheço bem.

---

## Stack

| Camada | Tecnologia |
|---|---|
| Backend | .NET 8, ASP.NET Core |
| Arquitetura | Clean Architecture, DDD, CQRS |
| Mediação | MediatR |
| Persistência | EF Core, SQL Server |
| Mensageria | RabbitMQ, MassTransit |
| Testes | xUnit, Moq |
| Infraestrutura | Docker |

---

## Estrutura da solução

```
src/
├── ERP360.Pedidos.Api
├── ERP360.Pedidos.Application
├── ERP360.Pedidos.Domain
├── ERP360.Pedidos.Infrastructure
├── ERP360.Estoque.Api
├── ERP360.Estoque.Application
├── ERP360.Estoque.Domain
├── ERP360.Estoque.Infrastructure
└── ERP360.Contracts

tests/
├── ERP360.Pedidos.Domain.Tests
└── ERP360.Pedidos.Application.Tests
```

Cada contexto tem suas próprias camadas. `ERP360.Contracts` centraliza os contratos de integração compartilhados entre publisher e consumer.

---

## Fluxo principal implementado

```
[POST /pedidos]
  → Pedidos.Api
  → Pedidos.Application (CriarPedidoCommandHandler)
  → Pedidos.Domain (agregado Pedido)
  → Pedidos.Infrastructure (EF Core → SQL Server)

[POST /pedidos/{id}/confirmar-pagamento]
  → Pedidos.Application (ConfirmarPagamentoCommandHandler)
  → Pedidos.Domain (transição de status)
  → Pedidos.Infrastructure (RabbitMQ → publica PedidoPago)

[Consumer]
  → Estoque.Api (PedidoPagoConsumer)
  → Estoque.Application (ReservarEstoqueDoPedidoCommandHandler)
  → Estoque.Domain (EstoqueItem)
  → Estoque.Infrastructure (EF Core → SQL Server)
```

---

## Decisões arquiteturais relevantes

**Bounded contexts separados** — Pedidos e Estoque têm APIs, domínios e bancos independentes. A comunicação entre eles é exclusivamente via evento.

**CQRS com MediatR** — commands e queries são tratados por handlers especializados. O Controller não acessa repositório diretamente.

**Contracts como fronteira formal** — `ERP360.Contracts` evita que Estoque referencie a Application de Pedidos. Publisher e consumer dependem apenas do contrato compartilhado.

**Result pattern** — falhas esperadas de negócio são tratadas por resultado controlado, sem exceções desnecessárias no fluxo normal.

**Domain sem dependência de infraestrutura** — o agregado `Pedido` não conhece EF Core, RabbitMQ nem ASP.NET Core.

**Observabilidade inicial** — `CorrelationIdMiddleware` e health check no contexto de Pedidos.

---

## Testes

Os testes estão separados por tipo de validação:

- `Domain.Tests` — regras puras do domínio, sem mock
- `Application.Tests` — handlers com Moq, simulando repositório e event bus

---

## Como rodar localmente

**Pré-requisitos:** .NET 8 SDK, SQL Server local, Docker

**1. Subir o RabbitMQ**
```bash
docker-compose up -d
```

**2. Aplicar migrations**
```bash
# Pedidos
dotnet ef database update --project src/ERP360.Pedidos.Infrastructure --startup-project src/ERP360.Pedidos.Api

# Estoque
dotnet ef database update --project src/ERP360.Estoque.Infrastructure --startup-project src/ERP360.Estoque.Api
```

**3. Rodar os serviços**
```bash
dotnet run --project src/ERP360.Pedidos.Api
dotnet run --project src/ERP360.Estoque.Api
```

A API de Pedidos expõe Swagger em `https://localhost:{porta}/swagger`.

---

## Documentação técnica

A pasta [`docs/`](./docs) contém a documentação completa do projeto, organizada em seções:

- Visão geral e requisitos
- Modelagem conceitual e casos de uso
- Arquitetura da solução
- Modelo físico de dados
- Qualidade, testes e infraestrutura
- Histórico de evolução e decisões

---

## Status do projeto

O fluxo principal entre Pedidos e Estoque está implementado e funcional. Algumas frentes ainda estão em evolução: consolidação do ambiente Docker, endurecimento do contexto de Estoque e limpeza de componentes provisórios. O histórico de decisões e pendências está registrado em [`docs/10-historico-de-evolucao-e-decisoes-importantes.md`](./docs/10-historico-de-evolucao-e-decisoes-importantes.md).
