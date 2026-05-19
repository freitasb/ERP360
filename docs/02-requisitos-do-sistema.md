# 02. Requisitos do Sistema

## 2.1 Objetivo desta seção

Esta seção formaliza os requisitos do ERP360 no estado atual do projeto. Seu papel é registrar, de forma clara e rastreável:

- o que o sistema deve fazer

- como ele deve se comportar tecnicamente

- quais regras de negócio governam seus fluxos

- quais premissas e restrições delimitam o recorte atual da solução

Esta seção funciona como a base formal da documentação. Ele não detalha a modelagem conceitual, o comportamento dos fluxos nem a implementação, mas estabelece o que essas partes precisam respeitar.

## 2.2 Critério de rastreabilidade adotado

Para manter esta seção útil ao longo da evolução do projeto, os requisitos foram organizados com identificadores fixos.

**Prefixos**

- RF = Requisito Funcional

- RNF = Requisito Não Funcional

- RN = Regra de Negócio

- PRE = Premissa

- RES = Restrição

**Referência de módulos**

- PED = Módulo de Pedidos

- EST = Módulo de Estoque

- INT = Integração entre módulos

- PLAT = Estrutura transversal da solução

**Referência de macrofluxos**

- MF1 = Criação de pedido

- MF2 = Consulta de pedido

- MF3 = Confirmação de pagamento

- MF4 = Atualização de status operacional

- MF5 = Publicação do evento PedidoPago

- MF6 = Consumo do evento e reserva de estoque

## 2.3 Requisitos funcionais

### 2.3.1 Módulo de Pedidos

RF-PED-001 — Registrar pedido
O sistema deve permitir o registro de um pedido com identificação do cliente e uma coleção de itens.
Rastreabilidade: PED | MF1

RF-PED-002 — Registrar itens do pedido
O sistema deve permitir que cada pedido seja criado com um ou mais itens contendo, no escopo atual, identificação do produto, nome do produto, quantidade e preço unitário.
Rastreabilidade: PED | MF1

RF-PED-003 — Gerar identidade do pedido
O sistema deve atribuir ao pedido uma identidade própria e um número funcional de rastreamento.
Rastreabilidade: PED | MF1

RF-PED-004 — Persistir pedido e itens
O sistema deve persistir o pedido e seus itens em banco relacional, preservando sua associação.
Rastreabilidade: PED | MF1

RF-PED-005 — Consultar pedido por identificador
O sistema deve permitir a consulta de um pedido específico por seu identificador.
Rastreabilidade: PED | MF2

RF-PED-006 — Retornar dados relevantes da consulta
Na consulta do pedido, o sistema deve retornar seus dados centrais, seu status e seus itens.
Rastreabilidade: PED | MF2

RF-PED-007 — Confirmar pagamento do pedido
O sistema deve permitir a execução do caso de uso de confirmação de pagamento para um pedido existente.
Rastreabilidade: PED | MF3

RF-PED-008 — Confirmar pagamento como caminho exclusivo para o status Pago
Ao confirmar o pagamento com sucesso, o sistema deve atualizar o status do pedido para Pago, sendo este o único caminho permitido para que o pedido alcance esse estado.
Rastreabilidade: PED | MF3

RF-PED-009 — Atualizar status do pedido por fluxo controlado, exceto pagamento
O sistema deve permitir a atualização do status do pedido dentro das transições admitidas pelo domínio para os estados operacionais do ciclo, excluindo a transição para Pago, que pertence exclusivamente ao caso de uso de confirmação de pagamento.
Rastreabilidade: PED | MF4

RF-PED-010 — Registrar informações temporais do ciclo do pedido
O sistema deve manter informações temporais que permitam identificar quando o pedido foi criado e quando houve mudança relevante em seu status.
Rastreabilidade: PED | MF1, MF3, MF4

### 2.3.2 Integração entre Pedidos e Estoque

RF-INT-001 — Publicar evento de pedido pago exclusivamente após confirmação de pagamento
Ao concluir com sucesso a confirmação de pagamento, o sistema deve publicar o evento de integração PedidoPago, contendo os dados necessários para o processamento no módulo de Estoque. Esse evento não deve ser disparado por uma atualização genérica de status.
Rastreabilidade: INT | MF3, MF5

RF-INT-002 — Compartilhar contrato de integração estável
O sistema deve utilizar um contrato explícito e compartilhado para o evento PedidoPago e seus itens, padronizando a comunicação entre os contextos.
Rastreabilidade: INT | MF5

RF-INT-003 — Consumir evento no contexto de Estoque
O módulo de Estoque deve receber o evento PedidoPago e iniciar o processamento interno correspondente à reserva dos itens.
Rastreabilidade: EST, INT | MF6

### 2.3.3 Módulo de Estoque

RF-EST-001 — Processar reserva a partir de pedido pago
O sistema deve processar a reserva de estoque com base nos itens recebidos no evento de pedido pago.
Rastreabilidade: EST | MF6

RF-EST-002 — Localizar itens de estoque envolvidos na reserva
O módulo de Estoque deve localizar os itens correspondentes aos produtos informados no evento recebido.
Rastreabilidade: EST | MF6

RF-EST-003 — Atualizar quantidade disponível após reserva
Ao reservar itens com sucesso, o sistema deve reduzir a disponibilidade dos produtos envolvidos conforme a quantidade solicitada.
Rastreabilidade: EST | MF6

RF-EST-004 — Persistir alteração de estoque
O sistema deve persistir em banco relacional o novo estado dos itens de estoque após a reserva.
Rastreabilidade: EST | MF6

### 2.3.4 Plataforma e operação

RF-PLAT-001 — Expor endpoints HTTP para os casos de uso atuais
O sistema deve expor endpoints HTTP para os fluxos já implementados, especialmente criação, consulta, confirmação de pagamento e atualização de status operacional.
Rastreabilidade: PLAT | MF1, MF2, MF3, MF4

RF-PLAT-002 — Expor health check
O sistema deve disponibilizar um endpoint de verificação de saúde para indicar disponibilidade básica da aplicação.
Rastreabilidade: PLAT

## 2.4 Requisitos não funcionais

### 2.4.1 Estrutura e arquitetura

RNF-PLAT-001 — Organização em camadas
A solução deve ser organizada em camadas com responsabilidades distintas para API, Application, Domain e Infrastructure.
Rastreabilidade: PLAT

RNF-PLAT-002 — Separação por módulos
A solução deve manter separação explícita entre os contextos de Pedidos e Estoque, permitindo evolução controlada de cada módulo.
Rastreabilidade: PLAT

RNF-PLAT-003 — Dependências direcionadas com clareza
As dependências entre projetos devem respeitar a direção arquitetural definida, preservando o domínio desacoplado de detalhes de infraestrutura.
Rastreabilidade: PLAT

RNF-PLAT-004 — Contratos de integração centralizados
Os contratos usados na comunicação entre contextos devem permanecer centralizados em um projeto compartilhado próprio.
Rastreabilidade: INT, PLAT

### 2.4.2 Persistência e dados

RNF-PLAT-005 — Persistência real em banco relacional
Os dados principais dos módulos devem ser persistidos em banco relacional real, com uso de EF Core como mecanismo de acesso e mapeamento.
Rastreabilidade: PED, EST

RNF-PLAT-006 — Mapeamento consistente entre domínio e banco
A camada de infraestrutura deve mapear entidades e relacionamentos do domínio de forma estável, preservando identidade, associação entre agregados e consistência dos dados.
Rastreabilidade: PED, EST

### 2.4.3 Integração e comunicação

RNF-INT-001 — Integração assíncrona entre contextos
A comunicação entre confirmação de pagamento e reserva de estoque deve ocorrer de forma assíncrona por mensageria.
Rastreabilidade: INT | MF5, MF6

RNF-INT-002 — Acoplamento reduzido entre módulos
O módulo de Pedidos deve comunicar o fato relevante de negócio sem incorporar internamente a lógica de reserva de estoque.
Rastreabilidade: PED, EST, INT

RNF-INT-003 — Consumidor preparado para falhas de negócio esperadas
O processamento do consumidor deve tratar falhas de negócio previsíveis com registro adequado, evitando comprometer a estabilidade do fluxo técnico.
Rastreabilidade: EST, INT | MF6

### 2.4.4 Qualidade e manutenção

RNF-PLAT-007 — Testabilidade dos casos de uso críticos
Os fluxos principais do sistema devem ser organizados de modo a permitir testes unitários sobre domínio e aplicação.
Rastreabilidade: PED, EST, PLAT

RNF-PLAT-008 — Casos de uso explícitos
As ações principais do sistema devem ser implementadas como casos de uso explícitos, com comandos, queries e handlers próprios.
Rastreabilidade: PED | MF1, MF2, MF3, MF4

RNF-PLAT-009 — Responsabilidade concentrada no ponto correto
As regras centrais do pedido devem permanecer no domínio, e a orquestração dos fluxos deve permanecer na camada de aplicação.
Rastreabilidade: PED

RNF-PLAT-010 — Clareza de borda HTTP
A camada API deve se concentrar em receber requisições, validar contratos de entrada e delegar a execução para a aplicação.
Rastreabilidade: PLAT

### 2.4.5 Observabilidade e operação

RNF-PLAT-011 — Rastreabilidade básica do fluxo
O sistema deve possuir meios básicos de rastrear a execução dos fluxos principais entre entrada HTTP, processamento interno e integração entre módulos.
Rastreabilidade: PLAT, INT

RNF-PLAT-012 — Disponibilidade verificável
A aplicação deve permitir verificação operacional básica de disponibilidade por meio do health check.
Rastreabilidade: PLAT

## 2.5 Regras de negócio

### 2.5.1 Regras do pedido

RN-PED-001 — Todo pedido deve possuir identificação de cliente
A criação do pedido exige vinculação a um cliente.
Rastreabilidade: PED | MF1

RN-PED-002 — Todo pedido deve possuir ao menos um item
O pedido só pode ser criado com uma coleção válida de itens.
Rastreabilidade: PED | MF1

RN-PED-003 — Quantidade de item deve ser válida
A quantidade informada para cada item do pedido deve ser compatível com uma operação comercial válida.
Rastreabilidade: PED | MF1

RN-PED-004 — Preço unitário deve ser válido
O preço unitário informado para cada item deve representar um valor comercial aceitável dentro do domínio atual.
Rastreabilidade: PED | MF1

RN-PED-005 — O pedido possui ciclo de status controlado
O pedido deve seguir um ciclo de vida definido por estados de negócio, com transições controladas pelo domínio.
Rastreabilidade: PED | MF3, MF4

RN-PED-006 — Somente a confirmação de pagamento pode levar o pedido para Pago
O estado Pago só pode ser alcançado pelo caso de uso de confirmação de pagamento, por meio da operação específica do domínio associada a esse fato de negócio.
Rastreabilidade: PED | MF3

RN-PED-007 — Mudança de status deve respeitar a ordem admitida e a natureza da transição
O sistema deve impedir transições incoerentes entre estados do pedido e deve recusar o uso do fluxo genérico de atualização de status para marcar pagamento, já que essa transição pertence exclusivamente ao caso de uso de confirmação de pagamento.
Rastreabilidade: PED | MF3, MF4

RN-PED-008 — Alterações de status devem ser registradas temporalmente
Mudanças relevantes no ciclo do pedido devem atualizar a marcação temporal correspondente.
Rastreabilidade: PED | MF3, MF4

### 2.5.2 Regras da integração

RN-INT-001 — O evento PedidoPago depende exclusivamente do fluxo de confirmação de pagamento
A publicação do evento PedidoPago só pode ocorrer quando o caso de uso de confirmação de pagamento tiver sido concluído com sucesso. Uma mudança genérica de status não deve disparar esse evento.
Rastreabilidade: INT | MF3, MF5

RN-INT-002 — O evento deve conter os dados necessários para o estoque
O evento publicado deve transportar informações suficientes para que o módulo de Estoque identifique os produtos e quantidades a reservar.
Rastreabilidade: INT | MF5, MF6

### 2.5.3 Regras do estoque

RN-EST-001 — Reserva de estoque depende de identificação do item
Para reservar estoque, o sistema precisa localizar o item correspondente ao produto informado no evento.
Rastreabilidade: EST | MF6

RN-EST-002 — Reserva depende de disponibilidade suficiente
A reserva só pode ser concluída quando houver quantidade disponível compatível com a quantidade solicitada.
Rastreabilidade: EST | MF6

RN-EST-003 — Reserva reduz a disponibilidade
Uma reserva concluída com sucesso reduz a quantidade disponível do item em estoque.
Rastreabilidade: EST | MF6

RN-EST-004 — Falhas de negócio do estoque devem ser tratadas dentro do contexto
Condições como item inexistente ou quantidade insuficiente devem ser tratadas pelo contexto de Estoque conforme sua regra interna de processamento.
Rastreabilidade: EST | MF6

## 2.6 Premissas

PRE-001 — O ERP360 está documentado a partir do estado atual implementado
Esta seção considera o sistema no ponto em que os módulos de Pedidos e Estoque já estão estruturados e integrados no fluxo principal.

PRE-002 — O fluxo central do projeto parte de Pedidos
O módulo de Pedidos é o ponto de origem do ciclo principal já implementado, e o módulo de Estoque participa como contexto acionado pela integração.

PRE-003 — O domínio foi modelado com foco em um recorte funcional representativo
O projeto cobre um fluxo empresarial relevante sem tentar abranger todos os módulos e subprocessos de um ERP completo nesta etapa.

PRE-004 — A persistência relacional faz parte da solução
O uso de banco relacional real integra o desenho técnico do sistema e participa da definição do comportamento atual.

PRE-005 — A integração entre contextos faz parte do comportamento funcional do sistema
A comunicação entre Pedidos e Estoque não é acessória; ela compõe o fluxo principal do ERP360.

PRE-006 — O projeto admite evolução incremental
Novos módulos, integrações e requisitos poderão ser incorporados sem invalidar a estrutura-base já definida.

## 2.7 Restrições

RES-001 — O escopo atual está concentrado em Pedidos e Estoque
Outros módulos típicos de ERP ainda não compõem o núcleo implementado desta versão da solução.
Rastreabilidade: PED, EST

RES-002 — A borda principal da aplicação está em APIs REST com Controllers
Os casos de uso do escopo atual são expostos prioritariamente por endpoints HTTP organizados em controllers.
Rastreabilidade: PLAT

RES-003 — A integração atual relevante entre módulos ocorre a partir do evento PedidoPago
A comunicação assíncrona modelada no estado atual está centrada no disparo do fluxo de reserva após pagamento confirmado.
Rastreabilidade: INT

RES-004 — O modelo atual trabalha com dados comerciais simplificados para o recorte escolhido
O foco está em consolidar ciclo de pedido, integração e reserva, sem ampliar neste momento para complexidades como políticas avançadas de preço, faturamento ou múltiplas moedas.
Rastreabilidade: PED

RES-005 — O sistema ainda não cobre toda a camada operacional de segurança corporativa
Aspectos como autenticação, autorização detalhada e políticas completas de acesso não são parte central do escopo já consolidado.
Rastreabilidade: PLAT

RES-006 — A consistência entre módulos segue a estratégia do fluxo assíncrono adotado
Entre a confirmação de pagamento em Pedidos e o efeito correspondente em Estoque, o sistema opera segundo a lógica de processamento por evento.
Rastreabilidade: INT

RES-007 — A cobertura de testes está concentrada nos pontos de maior relevância do fluxo atual
Os testes existentes acompanham a maturidade atual da solução e priorizam domínio e casos de uso críticos.
Rastreabilidade: PLAT

RES-008 — O status Pago não pertence ao fluxo genérico de atualização de status
No escopo atual do ERP360, a transição para Pago está reservada ao caso de uso especializado de confirmação de pagamento e não deve ser executada por atualização genérica de status.
Rastreabilidade: PED | MF3, MF4

## 2.8 Referências internas úteis

Para a modelagem conceitual das entidades, estados e responsabilidades envolvidas nestes requisitos, ver Seção 04 — Modelagem Conceitual.

Para o detalhamento do comportamento dos fluxos e da matriz de estados do pedido, ver Seção 05 — Diagramas Comportamentais.

Para a leitura arquitetural das camadas, contratos e bounded contexts que sustentam estes requisitos, ver Seção 06 — Arquitetura da Solução.

Para o histórico das decisões que endureceram o tratamento de pagamento e integração, ver Seção 10 — Histórico de Evolução e Decisões Importantes.

## 2.9 Fechamento da seção

Esta seção estabelece a base formal do ERP360 em quatro dimensões complementares:

- o que o sistema faz

- como ele deve se comportar tecnicamente

- quais regras de negócio governam os fluxos

- quais limites definem o recorte atual da solução

A partir daqui, as seções seguintes podem aprofundar:

- modelagem

- comportamento

- arquitetura

- implementação

- persistência

- infraestrutura

sem perder o vínculo com a definição formal do sistema.

---

[Voltar ao índice](./README.md)
