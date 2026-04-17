# 🧩 ADR-0005 - Adoção do .NET Aspire como orquestrador da aplicação

## 📌 Status

Aceito

## 📅 Data

2026-04-16

## 🧭 Contexto

Durante a definição da estratégia de execução e orquestração da aplicação, surgiu a necessidade de gerenciar múltiplos componentes do sistema de forma integrada, incluindo:

- API
- Banco de dados
- Dependências externas
- Configurações de ambiente

Além disso, o projeto demanda:

- Facilidade de execução local
- Padronização de ambiente de desenvolvimento
- Suporte a testes de integração
- Observabilidade e configuração centralizada

As principais abordagens consideradas foram:

- Execução manual dos serviços (sem orquestrador)
- Uso de Docker Compose
- Uso do .NET Aspire

## 🧠 Decisão

Foi adotado o uso do .NET Aspire como orquestrador da aplicação.

A orquestração é realizada através do projeto:

```text id="f2k9mv"
FgcGames.AppHost/
```

O Aspire é responsável por:

- Configurar e iniciar os serviços da aplicação
- Gerenciar dependências (ex: banco de dados)
- Centralizar configurações de ambiente
- Facilitar a execução local do sistema

## ⚖️ Consequências

### ✅ Positivas

- Execução simplificada do ambiente completo com um único comando
- Padronização do ambiente de desenvolvimento
- Integração nativa com o ecossistema .NET
- Melhor organização de serviços e dependências
- Facilita a observabilidade e configuração

### ✅ Positiva adicional (destaque)

- Facilita significativamente a implementação de testes de integração, permitindo subir o ambiente necessário de forma controlada e consistente

### ❌ Negativas

- Dependência de uma tecnologia relativamente nova
- Curva de aprendizado inicial
- Menor quantidade de material e comunidade comparado a alternativas mais consolidadas
- Possível lock-in ao ecossistema .NET

## 🔄 Alternativas consideradas

### 🐳 Docker Compose

#### 🚫 Motivo da rejeição

- Configuração mais verbosa e descentralizada
- Menor integração com o ecossistema .NET
- Maior esforço de manutenção dos arquivos de orquestração

### ⚙️ Execução manual dos serviços

#### 🚫 Motivo da rejeição

- Alto esforço para setup local
- Maior risco de inconsistência entre ambientes
- Dificuldade para reproduzir cenários de teste
- Baixa escalabilidade da abordagem

## 🔮 Considerações adicionais

O uso do .NET Aspire será mantido enquanto proporcionar ganhos de produtividade e simplicidade no gerenciamento do ambiente.

Caso o projeto evolua para cenários mais complexos de infraestrutura ou necessite maior portabilidade entre ambientes, a adoção de ferramentas como Docker Compose ou Kubernetes poderá ser reavaliada.

## 📚 Referências

- https://learn.microsoft.com/en-us/dotnet/aspire/
