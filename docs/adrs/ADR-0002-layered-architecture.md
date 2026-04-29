# ADR-0002 - Adoção de Arquitetura Horizontal (Layered / N-Tier)

## 📌 Status

Aceito

## 📅 Data

2026-04-16

## 🧭 Contexto

Durante a definição da arquitetura do sistema, foi necessário escolher um modelo de organização estrutural que guiasse todo o projeto.

O sistema é composto por múltiplas camadas (API, Application, Domain, Infra, etc.), e a decisão deveria garantir:

- Clareza na separação de responsabilidades
- Facilidade de manutenção
- Padronização estrutural
- Facilidade de onboarding

Além disso, foi necessário definir como a camada de Application seria organizada internamente.

As principais abordagens consideradas foram:

- Arquitetura Horizontal (Layered / N-Tier)
- Arquitetura Vertical Slice (Feature-based)

## 🧠 Decisão

Foi adotada a Arquitetura Horizontal (Layered / N-Tier) em todo o sistema.

A organização será baseada na separação por responsabilidade técnica.

### 🗂️ Estrutura adotada

```text
Application/
├── Commands/
├── Queries/
├── Handlers/
├── Validators/
└── Responses/
```

Cada diretório agrupa componentes do mesmo tipo, promovendo padronização e previsibilidade.

## ⚖️ Consequências

### ✅ Positivas

- Forte separação de responsabilidades
- Estrutura amplamente conhecida no ecossistema .NET
- Facilidade de onboarding
- Alta previsibilidade na organização
- Facilidade de reaproveitamento de componentes
- Consistência estrutural no projeto

### ❌ Negativas

- Código de uma mesma funcionalidade fica distribuído
- Navegação por feature exige percorrer múltiplas pastas
- Risco de aumento de acoplamento entre camadas sem disciplina arquitetural

## 🔄 Alternativas consideradas

### 🧩 Arquitetura Vertical Slice (Feature-based)

Estrutura alternativa:

```text
Application/
└── TaskItemExamples/
    └── Create/
        ├── Command.cs
        ├── Handler.cs
        ├── Validator.cs
        └── Response.cs
```

#### 🚫 Motivo da rejeição

- Menor padronização global no contexto atual do projeto
- Maior risco de inconsistência entre features
- Dificuldade de reaproveitamento de componentes
- Possível duplicação de código
- Curva de aprendizado maior para o time

## 🔮 Considerações adicionais

A arquitetura Vertical Slice poderá ser reavaliada futuramente, inclusive em um modelo híbrido, caso o domínio evolua para maior complexidade ou necessidade de isolamento por feature.

## 📚 Referências

- https://learn.microsoft.com/en-us/dotnet/architecture/
- https://martinfowler.com/bliki/LayeredArchitecture.html
- https://jimmybogard.com/vertical-slice-architecture/
