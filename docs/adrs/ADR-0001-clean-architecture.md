# ADR-0001 - Adoção de Clean Architecture como padrão arquitetural

## 📌 Status

Aceito

## 📅 Data

2026-04-16

## 🧭 Contexto

Durante a definição da arquitetura do sistema, foi necessário escolher um estilo arquitetural que orientasse a separação de responsabilidades, dependências entre camadas e organização geral da solução.

O projeto possui múltiplos projetos e camadas, incluindo:

- API (entrada HTTP)
- Application (casos de uso)
- Domain (regras de negócio)
- Infra (persistência e integrações)
- IoC (configuração de dependências)
- Shared (componentes reutilizáveis)

A decisão deveria garantir:

- Independência do domínio em relação a frameworks
- Baixo acoplamento entre camadas
- Facilidade de testes
- Manutenção e evolução a longo prazo

As principais abordagens consideradas foram:

- Clean Architecture
- Arquitetura em Camadas tradicional (Layered sem regras explícitas de dependência)
- Arquitetura Monolítica sem separação clara de responsabilidades

## 🧠 Decisão

Foi adotado o padrão de Clean Architecture como base para organização do sistema.

A estrutura segue o princípio de dependência de dentro para fora:

```text id="k3d9qp"
[ API ] → [ Application ] → [ Domain ]
               ↓
            [ Infra ]
```

Regras aplicadas:

- Domain não depende de nenhuma outra camada
- Application depende apenas do Domain
- Infra implementa interfaces definidas no Domain
- API depende da Application
- IoC é responsável por realizar a composição das dependências

Essa organização garante que as regras de negócio permaneçam isoladas de detalhes de infraestrutura e frameworks.

## ⚖️ Consequências

### ✅ Positivas

- Domínio isolado e independente de frameworks
- Maior testabilidade (principalmente Domain e Application)
- Baixo acoplamento entre camadas
- Facilidade de evolução e manutenção
- Clareza na direção das dependências
- Melhor alinhamento com princípios de DDD

### ❌ Negativas

- Aumento da complexidade inicial do projeto
- Maior número de projetos e abstrações
- Necessidade de maior disciplina arquitetural
- Possível sensação de “overengineering” em cenários simples
- Curva de aprendizado maior para desenvolvedores iniciantes

## 🔄 Alternativas consideradas

### 🧩 Arquitetura em Camadas tradicional (sem regras rígidas)

#### 🚫 Motivo da rejeição

- Permite dependências incorretas entre camadas
- Maior risco de acoplamento ao longo do tempo
- Domínio pode acabar dependendo de infraestrutura
- Dificulta manutenção em projetos maiores

### 🧩 Arquitetura Monolítica simples

#### 🚫 Motivo da rejeição

- Baixa separação de responsabilidades
- Alto acoplamento
- Dificuldade de testes isolados
- Escalabilidade limitada

## 🔮 Considerações adicionais

A Clean Architecture será mantida como base do sistema, mas poderá ser flexibilizada pragmaticamente quando necessário, evitando complexidade desnecessária.

Em cenários específicos, simplificações podem ser aplicadas, desde que não comprometam o isolamento do domínio e a direção das dependências.

## 📚 Referências

- https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html
- https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures
