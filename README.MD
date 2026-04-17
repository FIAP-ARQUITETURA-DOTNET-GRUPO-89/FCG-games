![FIAP](https://img.shields.io/badge/Instituição-FIAP-red)
![Pós-Graduação](https://img.shields.io/badge/Tipo-Pós--Graduação-blue)
![Arquitetura .NET](https://img.shields.io/badge/Área-Arquitetura%20.NET-purple)
![Progresso](https://img.shields.io/badge/Progresso-Fase%201%20de%205-green)

# 🎮 FIAP Cloud Games (FCG) - Tech Challenge

A **FIAP Cloud Games** é uma plataforma para venda de jogos digitais e gestão de servidores online. Este projeto faz parte do **Tech Challenge (Fase 1)** da pós-graduação da FIAP, focando inicialmente em um MVP para gerenciamento de usuários e jogos.

## 📑 Sumário

- [📂 Estrutura de Documentação](#-estrutura-de-documentação)
- [🛠️ Requisitos Mínimos](#️-requisitos-mínimos)
- [🚀 Como Executar](#-como-executar)
- [🧪 Testes](#-testes)
- [👥 Grupo (G89)](#-grupo-g89)
- [✨ Sobre este repositório](#-sobre-este-repositório)

## 📂 Estrutura de Documentação

Para entender as decisões técnicas e o fluxo do projeto, explore a pasta [`/docs`](./docs):

- 📊 **[Diagramas](./docs/diagrams):** Event Storming e Domain Storytelling.
- 🧠 **[Linguagem ubíqua](./docs/linguagem-ubiqua):** Linguagem ubíqua e conceitos do domínio
- 🎨 **[Miro Board](https://miro.com/app/board/uXjVGuph9Sk=/?share_link_id=200277775380):** Board interativo com a dinâmica de Event Storming.
- 📜 **[ADRs](./docs/adrs):** Registros de decisões de arquitetura.
- 🚀 **[Postman](./docs/api-collection):** Coleção de endpoints e ambientes para testes.

## 🛠️ Requisitos Mínimos

Antes de começar, certifique-se de ter instalado:

- **SDK .NET 10:** https://dotnet.microsoft.com/download/dotnet/10.0
- **Container Runtime:** Docker, Podman ou Rancher Desktop
- **Workload Aspire:**
  ```bash
  dotnet workload install aspire
  ```

## 🚀 Como Executar

### 1. Clonar o repositório

```bash
git clone https://github.com/FIAP-ARQUITETURA-DOTNET-GRUPO-89/FCG-games.git
cd FCG-games
```

### 2. Subir a aplicação

```bash
dotnet run --project src/FgcGames.AppHost
```

### 3. Acessar o dashboard

👉 [https://localhost:17014](https://localhost:17014)

No dashboard do Aspire você poderá:

- Visualizar a API e o banco de dados em execução
- Acessar o Swagger pelos endpoints
- Consultar logs centralizados
- Acompanhar traces das requisições
- Acessar o PgAdmin

## 🧪 Testes

O projeto possui cobertura com **testes unitários** e **testes de integração**.

### ▶️ Executar todos os testes

```bash
dotnet test
```

### 🧩 Tipos de testes

- **Unitários:** Validação de regras de negócio, handlers, validators e domínio
- **Integração:** Testam os endpoints da API utilizando o **.NET Aspire** para subir o ambiente completo

### ⚙️ Observações sobre testes de integração

- Utilizam o `AppHost` como base para orquestração
- Sobem a aplicação completa (API + banco)
- Requerem suporte a containers (Docker, Podman ou Rancher Desktop)

## 👥 Grupo

- **[Camila Melo Machado](https://github.com/camilamachado)**
- **[Iago Valverde](https://github.com/iagovalverde)**
- **[Isabella Honorato](https://github.com/isahonorato)**
- **[Jonatas Damião](https://github.com/joncodes-stack)**
- **[Thaina Silva](https://github.com/ThainaSilva100)**

## ✨ Sobre este repositório

> Este projeto foi desenvolvido para fins acadêmicos como parte do Tech Challenge FIAP 2026.
