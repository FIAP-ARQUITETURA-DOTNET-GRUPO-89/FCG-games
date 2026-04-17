# ADR-0006 - Adoção do xUnit como framework de testes

## 📌 Status

Aceito

## 📅 Data

2026-04-16

## 🧭 Contexto

Durante a definição da estratégia de testes automatizados, foi necessário escolher um framework de testes para suportar:

- Testes unitários
- Testes de integração

O projeto possui múltiplas camadas (Domain, Application, Infra, API), e a abordagem de testes deve garantir:

- Facilidade de escrita e leitura dos testes
- Boa integração com ferramentas de mocking e assertions
- Suporte a testes assíncronos
- Manutenção e escalabilidade da suíte de testes

As principais opções consideradas foram:

- xUnit
- NUnit
- MSTest

## 🧠 Decisão

Foi adotado o xUnit como framework principal para testes automatizados.

A estrutura de testes está organizada em:

```text id="h2k9sd"
tests/
├── FgcGames.UnitTests/
└── FgcGames.IntegrationTests/
```

O xUnit será utilizado para:

- Definição de testes unitários e de integração
- Execução de testes assíncronos
- Integração com ferramentas de mocking (ex: Moq)
- Execução em pipelines de CI

## ⚖️ Consequências

### ✅ Positivas

- Forte integração com o ecossistema .NET moderno
- Suporte nativo a testes assíncronos
- Estrutura simples e objetiva
- Ampla adoção na comunidade
- Boa integração com ferramentas de mocking e assertions
- Execução eficiente em pipelines de CI/CD

### ❌ Negativas

- Menor familiaridade para desenvolvedores acostumados com NUnit ou MSTest
- Necessidade de entender conceitos específicos (ex: Fixtures, Collections)
- Menor suporte a alguns padrões mais tradicionais de testes (ex: setup/teardown clássico)

## 🔄 Alternativas consideradas

### 🧩 NUnit

#### 🚫 Motivo da rejeição

- Abordagem mais próxima de frameworks tradicionais
- Menor alinhamento com o ecossistema moderno do .NET
- Sintaxe mais verbosa em alguns cenários

### 🧩 MSTest

#### 🚫 Motivo da rejeição

- Menor flexibilidade comparado ao xUnit
- Menor adoção em projetos modernos
- Estrutura mais limitada para cenários avançados

## 🔮 Considerações adicionais

O xUnit será mantido como framework principal de testes enquanto atender às necessidades do projeto.

A estratégia de testes poderá evoluir com a adoção de ferramentas complementares, como bibliotecas de mocking, assertions mais expressivas e melhorias na cobertura de testes.

Além disso, a integração com o ambiente orquestrado pelo .NET Aspire facilita a execução de testes de integração de forma consistente e reproduzível.

## 📚 Referências

- https://xunit.net/
- https://learn.microsoft.com/en-us/dotnet/core/testing/
