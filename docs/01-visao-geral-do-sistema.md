# 01. Visão Geral do Sistema

## 1.1 Objetivo desta seção

Esta seção apresenta a visão geral do ERP360 no estado atual do projeto. O objetivo aqui é estabelecer uma leitura inicial clara do sistema, definindo:

- o que é o ERP360

- qual é seu objetivo

- qual é seu escopo atual

- quais módulos compõem a solução neste estágio

- quais atores participam dos fluxos principais

- quais são os macrofluxos já implementados

Esta seção funciona como ponto de entrada da documentação. Ele não substitui as seções de requisitos, arquitetura, modelagem ou histórico. Seu papel é oferecer uma visão inicial consolidada do sistema e do seu recorte atual.

## 1.2 Visão geral do ERP360

O ERP360 é uma solução modular em .NET 8 organizada para representar, de forma técnica e estruturada, um cenário de ERP com separação de contextos, persistência real, integração entre módulos e evolução arquitetural controlada.

No estado atual, o sistema está centrado no fluxo de Pedidos, com integração ao contexto de Estoque após a confirmação de pagamento. Esse recorte foi implementado como base funcional do projeto e já permite observar uma cadeia de execução que passa por:

- entrada HTTP

- aplicação de casos de uso

- regras de domínio

- persistência em banco relacional

- publicação de evento

- consumo por outro contexto

- atualização do estado de estoque

A solução foi organizada para que cada parte do fluxo tenha responsabilidade clara, com separação entre:

- borda de entrada

- orquestração de casos de uso

- regras centrais de negócio

- persistência e integração técnica

## 1.3 Objetivo do sistema

O objetivo do ERP360, no seu recorte atual, é representar um fluxo empresarial modular em que o pedido deixa de ser apenas um registro estático e passa a participar de um ciclo de vida com regras explícitas, persistência real e efeitos em outro contexto do sistema.

Esse objetivo se materializa em três frentes principais:

**Organização do fluxo de negócio**

O sistema registra pedidos, controla seus itens, mantém seu ciclo de status e trata o pagamento como evento relevante do processo.

**Integração entre contextos**

A confirmação de pagamento no módulo de Pedidos aciona o módulo de Estoque por meio de mensageria, permitindo que a reserva de produtos aconteça em um contexto separado.

**Base de evolução da solução**

A estrutura atual já permite crescimento por novos módulos, novos contratos e maior maturidade operacional sem exigir reestruturação completa do núcleo implementado.

## 1.4 Escopo atual

O escopo atual do ERP360 está concentrado em dois módulos já implementados de fato:

**Módulo de Pedidos**

É o núcleo principal da solução neste estágio. Ele já contempla:

- criação de pedido

- registro de itens

- consulta por identificador

- atualização de status operacional

- confirmação de pagamento

- publicação do evento PedidoPago

**Módulo de Estoque**

É o contexto que reage ao pagamento confirmado. Ele já contempla:

- recebimento do evento PedidoPago

- interpretação dos itens do pedido

- validação de disponibilidade

- reserva de quantidade

- persistência do novo saldo

**Escopo técnico já implementado**

Além do recorte funcional, o projeto já possui:

- APIs separadas por contexto

- camadas Api, Application, Domain e Infrastructure

- persistência com EF Core e SQL Server

- mensageria com RabbitMQ e MassTransit

- projeto de contratos compartilhados

- testes unitários em partes centrais da solução

- health check e instrumentação inicial de correlação

**Fora do escopo atual**

Ainda não fazem parte do núcleo implementado:

- autenticação e autorização completas

- catálogo físico de Clientes e Produtos

- pipeline CI/CD consolidado

- observabilidade distribuída completa

- módulos adicionais além do recorte atual

## 1.5 Módulos principais da solução

### 1.5.1 Pedidos

O módulo de Pedidos concentra o ciclo principal do sistema. É nele que o pedido nasce, recebe itens, assume status ao longo do processo e produz o fato de integração que será consumido por Estoque.

**Responsabilidades centrais**

- manter os dados principais do pedido

- manter seus itens

- controlar seu ciclo de vida

- confirmar pagamento

- publicar PedidoPago

### 1.5.2 Estoque

O módulo de Estoque responde pela disponibilidade e reserva dos itens após o pagamento.

**Responsabilidades centrais**

- receber o evento de pagamento confirmado

- localizar itens de estoque

- validar saldo

- reservar quantidade

- persistir o novo estado

### 1.5.3 Contracts

O projeto ERP360.Contracts formaliza os contratos compartilhados de integração entre os contextos.

**Responsabilidades centrais**

- centralizar mensagens de integração

- evitar duplicidade de contratos

- sustentar a comunicação entre publisher e consumer

No estado atual, o principal contrato implementado é:

- PedidoPago

- com seus respectivos ItemSolicitado

### 1.5.4 Testes

A solução já possui projetos dedicados de teste para partes importantes do domínio e da aplicação, principalmente no fluxo de Pedidos.

Embora não sejam módulos de negócio, eles já compõem a estrutura principal da solução por participarem da validação do comportamento do sistema.

## 1.6 Atores envolvidos

Para a visão geral do ERP360, faz sentido distinguir atores de negócio e participantes sistêmicos.

### 1.6.1 Atores de negócio

**Operador Comercial / Atendimento**

Representa quem registra e consulta pedidos no sistema.

**Operador Financeiro**

Representa quem aciona a confirmação de pagamento no fluxo atual.

**Operação de Estoque / Logística**

Representa a área interessada na reserva e disponibilidade dos itens após o pagamento.

### 1.6.2 Participantes sistêmicos principais

**API de Pedidos**

Recebe as chamadas HTTP do contexto de Pedidos.

**Aplicação de Pedidos**

Executa os casos de uso do módulo de Pedidos.

**Banco de Pedidos**

Mantém o estado persistido do pedido e de seus itens.

**RabbitMQ**

Transporta o evento de integração entre os contextos.

**Consumer de Estoque**

Recebe PedidoPago e inicia a reserva.

**Banco de Estoque**

Mantém o estado persistido da disponibilidade dos produtos.

## 1.7 Macrofluxos do sistema

Os macrofluxos abaixo representam o comportamento principal já implementado no ERP360.

### 1.7.1 Criar pedido

O fluxo começa na API de Pedidos, recebe os dados do pedido, cria o agregado, adiciona os itens, confirma a etapa inicial do ciclo e persiste o pedido em banco.

### 1.7.2 Consultar pedido

O sistema recebe um identificador, executa a query correspondente, consulta o repositório e devolve uma representação adequada para leitura.

### 1.7.3 Confirmar pagamento

O pagamento confirmado leva o pedido ao estado Pago, persiste a alteração e inicia a publicação do evento de integração.

### 1.7.4 Atualizar status operacional

O pedido pode seguir seu ciclo operacional por meio de mudança controlada de status, respeitando as regras do domínio.

### 1.7.5 Publicar PedidoPago

Após a confirmação de pagamento, o contexto de Pedidos publica o contrato PedidoPago no barramento.

### 1.7.6 Reservar estoque

O contexto de Estoque consome o evento, interpreta os itens recebidos, valida disponibilidade e persiste a reserva.

## 1.8 Leitura consolidada do sistema

No estado atual, o ERP360 já apresenta um fluxo completo entre dois contextos de negócio:

- o pedido é criado e persistido

- seu ciclo de vida é controlado no contexto de Pedidos

- o pagamento confirmado gera evento

- o contexto de Estoque reage e reserva os itens

Esse recorte já é suficiente para sustentar a documentação técnica da solução em termos de:

- requisitos

- modelagem

- comportamento

- arquitetura

- persistência

- infraestrutura

- histórico de evolução

## 1.9 Referências internas úteis

Para a formalização do comportamento esperado do sistema, ver Seção 02 — Requisitos do Sistema.

Para o detalhamento dos fluxos e estados, ver Seção 05 — Diagramas Comportamentais.

Para a estrutura macro da solução, ver Seção 06 — Arquitetura da Solução.

Para o histórico de evolução, decisões e pendências consolidadas, ver Seção 10 — Histórico de Evolução e Decisões Importantes.

## 1.10 Fechamento da seção

O ERP360, no seu estágio atual, já possui um recorte funcional e técnico claro: um fluxo de pedidos integrado ao contexto de estoque, estruturado em camadas, persistido em banco relacional e apoiado por mensageria para a comunicação entre módulos.

Esta seção define a visão inicial do sistema. Os detalhes funcionais, comportamentais, arquiteturais e físicos são aprofundados nas seções seguintes.

---

[Voltar ao índice](./README.md)
