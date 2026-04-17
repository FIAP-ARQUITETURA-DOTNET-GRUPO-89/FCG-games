# ADR-0004 - Adoção do Entity Framework com PostgreSQL para persistência de dados

## 📌 Status

Aceito

## 📅 Data

2026-04-16

## 🧭 Contexto

Durante a definição da estratégia de persistência de dados, foi necessário escolher:

- Um ORM (Object-Relational Mapper)
- Um tipo de banco de dados

O sistema possui necessidade de:

- Persistência estruturada de dados
- Garantia de integridade e consistência
- Facilidade de evolução do modelo
- Integração com o ecossistema .NET

As principais opções consideradas foram:

- Entity Framework + banco relacional
- Dapper + banco relacional
- Bancos NoSQL (ex: documentos)

## 🧠 Decisão

Foi adotado o uso do Entity Framework como ORM, utilizando banco de dados relacional PostgreSQL.

A camada de infraestrutura será responsável pela persistência, utilizando:

- DbContext para gerenciamento de acesso a dados
- Mapeamentos via Fluent API (EntityTypeConfiguration)
- Migrations para controle de versão do banco

Exemplo estrutural:

```text id="d9x1kp"
Infra/
├── Database/
│   └── FgcGamesContext.cs
├── EntityTypeConfigurations/
│   └── TaskItemExampleEntityConfiguration.cs
├── Migrations/
└── Repositories/
```

A comunicação com o domínio será realizada através de interfaces de repositório definidas na camada de Domain.

## ⚖️ Consequências

### ✅ Positivas

- Alta integração com o ecossistema .NET
- Redução de boilerplate para operações de persistência
- Suporte a migrations para versionamento do banco
- Forte suporte a modelagem relacional
- Facilidade de manutenção e evolução do schema
- Comunidade ampla e madura

### ❌ Negativas

- Menor controle fino sobre queries em comparação com SQL puro ou Dapper
- Possível overhead de performance em cenários específicos
- Necessidade de atenção com tracking e consultas complexas
- Curva de aprendizado para uso eficiente (ex: performance tuning)

## 🔄 Alternativas consideradas

### 🧩 Dapper (micro-ORM)

#### 🚫 Motivo da rejeição

- Maior quantidade de código manual
- Ausência de gerenciamento de migrations
- Menor abstração para modelagem de domínio
- Aumento da responsabilidade do desenvolvedor sobre queries

### 🧩 Banco NoSQL (ex: documentos)

#### 🚫 Motivo da rejeição

- Modelo menos adequado para relações fortes entre entidades
- Maior complexidade para garantir consistência
- Desnecessário para o contexto atual do sistema
- Curva de aprendizado adicional

## 🔮 Considerações adicionais

O uso de Entity Framework com PostgreSQL será mantido enquanto atender aos requisitos de performance e complexidade do sistema.

Em cenários específicos, poderá ser adotada uma abordagem híbrida, utilizando Dapper para consultas críticas de alta performance.

A escolha do banco relacional poderá ser reavaliada caso o domínio evolua para necessidades que favoreçam modelos NoSQL.

## 📚 Referências

- https://learn.microsoft.com/en-us/ef/core/
- https://www.postgresql.org/docs/
