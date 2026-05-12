# AGENT.md

## Objetivo deste arquivo

Este arquivo define as regras que o agente de IA deve seguir ao trabalhar neste projeto usando **SDD — Spec-Driven Development**.

O agente deve sempre priorizar clareza, rastreabilidade, aprovação humana e validação antes de alterar o código.

---

# Regras Gerais do Agente

## 1. O agente deve seguir SDD

Antes de implementar qualquer alteração, o agente deve entender a especificação, identificar ambiguidades, propor um plano e aguardar aprovação explícita.

O agente não deve tratar uma tarefa como apenas “programar algo”. Toda tarefa deve passar por um fluxo de decisão, planejamento, implementação, validação e documentação.

---

## 2. Fluxo obrigatório de trabalho

O agente deve seguir obrigatoriamente estas fases:

```text
PHASE 1 → Clarification
PHASE 2 → Planning
PHASE 3 → Approval
PHASE 4 → Implementation
PHASE 5 → Validation
PHASE 6 → Documentation
```

Nenhuma fase deve ser pulada, exceto quando a tarefa for exclusivamente de leitura, explicação ou revisão sem alteração de arquivos.

---

## Pre-Execution Gate

Antes de iniciar qualquer task, o agente deve verificar:

- Existe spec associada?
- A task está descrita em `tasks.md`?
- Existem critérios de aceite definidos?
- A task está aprovada/priorizada?

Se qualquer item estiver ausente, o agente deve parar e informar:

```md
## Execution Blocker

Não posso iniciar esta task pois:

- [Motivo]

Preciso que isso seja resolvido antes de continuar.
```

---

# PHASE 1 → Clarification

## Objetivo

Identificar dúvidas, ambiguidades, riscos e informações ausentes antes de planejar ou alterar qualquer arquivo.

## O agente deve verificar:

- A tarefa está clara?
- Existem regras de negócio ausentes?
- Existe alguma decisão técnica em aberto?
- Existe algum impacto em banco de dados, API, UI, autenticação, segurança ou arquitetura?
- Existem arquivos de especificação relacionados?
- A tarefa depende de outra tarefa anterior?
- Existem testes ou critérios de aceite já definidos?

## Quando houver ambiguidade relevante

O agente deve parar e perguntar antes de continuar.

Formato obrigatório:

```md
## Clarification Gate

Antes de continuar, preciso esclarecer os seguintes pontos:

1. [Pergunta objetiva]
2. [Pergunta objetiva]
3. [Pergunta objetiva]

Não farei alterações até que esses pontos estejam claros.
```

## Quando houver pequenas dúvidas não bloqueantes

O agente pode assumir algo, mas deve declarar a suposição no plano.

Formato:

```md
## Assumptions

- Vou assumir que [...]
- Vou assumir que [...]
```

---

# PHASE 2 → Planning

## Objetivo

Criar um plano claro antes da implementação.

O plano deve explicar exatamente o que será alterado, por quê, onde e como será validado.

Formato obrigatório:

```md
## Implementation Plan

### Goal
[Objetivo da tarefa]

### Files likely to change
- `path/to/file.cs` — motivo da alteração
- `path/to/another-file.cs` — motivo da alteração

### Proposed changes
1. [Alteração proposta]
2. [Alteração proposta]
3. [Alteração proposta]

### Tests and validation
- [Teste unitário/integrado/manual esperado]
- [Comando ou validação necessária]

### Risks
- [Risco técnico ou de negócio]

### Assumptions
- [Suposição, se houver]
```

---

# PHASE 3 → Approval

## Objetivo

O agente deve aguardar autorização explícita antes de alterar arquivos.

## Regra obrigatória

O agente não pode implementar nenhuma alteração de código, banco de dados, migration, configuração ou documentação persistente sem aprovação explícita.

A aprovação deve ser uma mensagem clara do usuário, como:

```text
Aprovado
Pode implementar
Pode seguir
Execute o plano
```

## Formato obrigatório antes de parar

```md
## Approval Gate

Revise o plano acima.

Só continuarei para a implementação após sua aprovação explícita.
```

---

# PHASE 4 → Implementation

## Objetivo

Implementar somente o que foi aprovado.

## Regras de implementação

- Alterar apenas os arquivos necessários.
- Não fazer refatorações fora do escopo da tarefa.
- Não corrigir problemas não relacionados sem avisar antes.
- Não alterar arquitetura sem justificar no plano.
- Não remover código existente sem explicar o motivo.
- Não criar abstrações desnecessárias.
- Manter o padrão de código já usado no projeto.
- Seguir os nomes, camadas e convenções existentes.
- Não adicionar dependências externas sem aprovação.
- Não executar comandos destrutivos sem aprovação explícita.

## Se surgir um problema durante a implementação

O agente deve parar e informar:

```md
## Implementation Blocker

Encontrei um bloqueio durante a implementação:

- Problema: [...]
- Impacto: [...]
- Opções possíveis:
  1. [...]
  2. [...]

Preciso de aprovação para seguir com uma das opções.
```

---

# PHASE 5 → Validation

## Objetivo

Validar que a alteração funciona e não quebrou o comportamento esperado.

## O agente deve validar com:

- Testes automatizados, quando existirem.
- Testes novos, quando a tarefa exigir.
- Build do projeto, quando possível.
- Validação manual descrita, quando testes automatizados não forem possíveis.
- Conferência dos critérios de aceite da task.

## Test Rule

O agente deve seguir as diretrizes definidas em `docs/testing.md`.

## Regra sobre testes

Testes reais devem ser criados no formato adequado do projeto, por exemplo:

- Testes unitários em projetos de teste.
- Testes de integração quando necessário.
- Testes de API quando aplicável.
- Testes manuais documentados quando automação ainda não for viável.

Arquivos `.md` podem documentar cenários de teste e critérios de aceite, mas não substituem testes automatizados quando eles forem necessários.

## Formato obrigatório da validação

```md
## Validation Report

### Commands executed
- `dotnet build`
- `dotnet test`

### Results
- [Resultado do build]
- [Resultado dos testes]

### Manual validation
- [O que foi validado manualmente]

### Notes
- [Limitações, pendências ou observações]

### Post-validation feedback
- Status: [Sucesso / Falha / Parcial]
- Confiança: [Alta / Média / Baixa]
- Cobertura da task: [% estimado]
- Pendências:
  - [...]
```

Se o agente não conseguir executar algum teste, deve informar claramente:

```md
Não consegui executar `[comando]` porque [...].
```

O agente não deve dizer que algo foi testado se não foi realmente testado.

---

# PHASE 6 → Documentation

## Objetivo

Manter a documentação do projeto alinhada com a implementação.

## O agente deve atualizar, quando aplicável:

- `tasks.md`
- arquivos de especificação
- documentação técnica
- comentários relevantes no código
- exemplos de uso
- documentação de endpoints
- notas de migration
- critérios de aceite

## Regra

A documentação deve explicar o que mudou e por quê, mas sem duplicar código desnecessariamente.

Formato sugerido:

```md
## Documentation Update

### Updated files
- `tasks.md` — status da task atualizado
- `docs/example.md` — comportamento documentado

### Summary
[Resumo curto da alteração]
```

---

# Definition of Done

Uma tarefa só pode ser considerada concluída quando:

- A ambiguidade foi resolvida ou documentada como suposição.
- O plano foi apresentado.
- A aprovação foi recebida.
- A implementação seguiu o plano aprovado.
- A validação foi executada ou a impossibilidade foi explicada.
- A documentação foi atualizada quando necessário.
- O agente informou claramente o que foi alterado.

---

# Regras para banco de dados e migrations

Quando a tarefa envolver banco de dados, o agente deve:

- Explicar o impacto da migration antes de criá-la.
- Informar quais tabelas, colunas, índices ou relacionamentos serão alterados.
- Avaliar se haverá impacto em dados existentes.
- Criar migration apenas após aprovação.
- Sugerir validações para conferir o schema gerado.
- Nunca apagar dados sem aprovação explícita.

Formato esperado no plano:

```md
### Database impact
- Table: `[NomeDaTabela]`
- Change: `[Descrição da alteração]`
- Migration: `[Nome sugerido]`
- Data risk: `[baixo/médio/alto]`
```

---

# Regras para API

Quando a tarefa envolver API, o agente deve:

- Indicar endpoint, método HTTP e contrato esperado.
- Explicar request e response.
- Respeitar versionamento, se existir.
- Manter padrão de erro do projeto.
- Validar entrada de dados.
- Atualizar documentação de endpoints quando necessário.

Formato esperado:

```md
### API contract
- Method: `POST`
- Route: `/api/v1/example`
- Request: `[DTO esperado]`
- Response: `[DTO esperado]`
- Errors: `[erros previstos]`
```

---

# Regras para UI

Quando a tarefa envolver interface, o agente deve:

- Usar obrigatoriamente as imagens e o código em `Documentation/visual-reference` como referência visual e estrutural para qualquer task relacionada a UI.
- Conferir os arquivos de referência relevantes antes de implementar a interface, incluindo `.html`, `.png` e `.jpg` disponíveis nessa pasta.
- Explicar o fluxo de tela.
- Indicar componentes afetados.
- Respeitar padrões visuais existentes.
- Considerar estados de loading, erro, vazio e sucesso.
- Validar o comportamento esperado do usuário.

---

# Regras de segurança

O agente não deve:

- Expor secrets, tokens, connection strings ou senhas.
- Criar credenciais hardcoded.
- Reduzir validações de segurança sem aprovação.
- Remover autenticação ou autorização sem justificativa e aprovação.
- Executar comandos destrutivos sem confirmação explícita.

---

# Regras de comunicação

O agente deve ser claro e direto.

Ao explicar uma alteração, deve priorizar:

1. O que será feito.
2. Por que será feito.
3. Quais arquivos serão afetados.
4. Como será validado.
5. Quais riscos existem.

O agente deve evitar respostas vagas como:

```text
Vou melhorar o código.
```

E preferir respostas específicas como:

```text
Vou adicionar uma migration para criar a tabela `Users`, configurar a entidade `User`, mapear o DbSet no `AppDbContext` e criar testes para validar o schema esperado.
```

---

# Comportamento esperado para tasks

Ao receber uma task, o agente deve responder primeiro com Clarification ou Planning.

Exemplo:

```md
## PHASE 1 → Clarification

A tarefa está clara o suficiente para planejamento.

## PHASE 2 → Planning

[Plano detalhado]

## PHASE 3 → Approval

Revise o plano acima.
Só continuarei para a implementação após sua aprovação explícita.
```

Se a tarefa estiver ambígua:

```md
## PHASE 1 → Clarification

Antes de planejar, preciso esclarecer:

1. A entidade deve se chamar `User` ou `Usuario`?
2. A autenticação será feita agora ou em uma task futura?
3. O campo de e-mail deve ser único no banco?

Não farei alterações até que esses pontos estejam claros.
```

---

# Instrução final ao agente

Sempre trate o usuário como o aprovador final das decisões de produto, arquitetura e implementação.

O agente pode sugerir caminhos técnicos, mas não deve assumir decisões importantes sem aprovação.

O objetivo é desenvolver com segurança, clareza e rastreabilidade, seguindo o fluxo de SDD definido neste arquivo.
