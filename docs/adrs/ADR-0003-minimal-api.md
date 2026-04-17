# ADR-0003 - Adoção de Minimal APIs para exposição da aplicação

## 📌 Status

Aceito

## 📅 Data

2026-04-16

## 🧭 Contexto

Durante o desenvolvimento da camada de API, foi necessário definir a abordagem para exposição dos endpoints HTTP.

As principais opções no ecossistema .NET incluem:

- Controllers (API tradicional baseada em MVC)
- Minimal APIs

A decisão deveria considerar:

- Simplicidade na definição de endpoints
- Redução de boilerplate
- Facilidade de leitura e manutenção
- Integração com o restante da arquitetura (Application, Domain, etc.)

O projeto utiliza uma arquitetura em camadas, onde a API atua apenas como ponto de entrada, delegando responsabilidades para a camada de Application.

## 🧠 Decisão

Foi adotado o uso de Minimal APIs para definição dos endpoints HTTP.

Os endpoints são organizados por responsabilidade em arquivos específicos:

```text id="g1n2xk"
Endpoints/
├── AuthEndpoints.cs
├── JogoEndpoints.cs
└── UsuarioEndpoints.cs
```

Cada endpoint é responsável por:

- Receber a requisição HTTP
- Validar entrada (via filters/validators)
- Delegar o processamento para a camada de Application
- Retornar a resposta apropriada

## ⚖️ Consequências

### ✅ Positivas

- Redução significativa de boilerplate em comparação com Controllers
- Maior proximidade com o modelo funcional (handlers simples e diretos)
- Facilidade de leitura e entendimento dos endpoints
- Melhor controle sobre o pipeline HTTP (filters, middlewares)
- Mais flexibilidade na organização dos endpoints

### ❌ Negativas

- Menor padronização “out-of-the-box” comparado ao uso de Controllers
- Pode gerar desorganização se não houver convenções bem definidas
- Menor familiaridade para desenvolvedores acostumados com MVC tradicional
- Necessidade de disciplina para manter separação de responsabilidades

## 🔄 Alternativas consideradas

### 🧩 Controllers (ASP.NET Core MVC)

Exemplo conceitual:

```text id="u9r8xp"
Controllers/
└── JogoController.cs
```

#### 🚫 Motivo da rejeição

- Maior quantidade de boilerplate
- Estrutura mais verbosa para cenários simples
- Menor flexibilidade na definição de endpoints
- Abstrações desnecessárias para o contexto do projeto

## 🔮 Considerações adicionais

O uso de Minimal APIs será mantido enquanto a simplicidade e clareza forem preservadas.

Caso o sistema evolua para cenários com maior complexidade (ex: versionamento avançado, múltiplos contratos, crescimento significativo de endpoints), a adoção de Controllers poderá ser reavaliada.

## 📚 Referências

- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis
- https://learn.microsoft.com/en-us/aspnet/core/web-api/
