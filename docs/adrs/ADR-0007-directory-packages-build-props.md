# ADR-0007 - Padronização de dependências e build com Directory.Packages.props e Directory.Build.props

## 📌 Status

Aceito

## 📅 Data

2026-04-16

## 🧭 Contexto

Durante a organização da solução, surgiu a necessidade de padronizar:

- Gerenciamento de dependências (pacotes NuGet)
- Configurações comuns de build entre múltiplos projetos
- Redução de duplicação de configurações nos arquivos `.csproj`

O projeto é composto por múltiplos projetos (API, Application, Domain, Infra, Tests, etc.), o que pode gerar:

- Inconsistência de versões de pacotes
- Duplicação de configurações
- Dificuldade de manutenção

As principais abordagens consideradas foram:

- Gerenciar dependências diretamente em cada `.csproj`
- Centralizar configurações e versões utilizando arquivos globais

## 🧠 Decisão

Foi adotado o uso de arquivos globais para padronização:

- `Directory.Packages.props` → gerenciamento centralizado de versões de pacotes
- `Directory.Build.props` → configurações compartilhadas de build

### Estrutura adotada

```text id="c9p2xz"
/
├── Directory.Packages.props
├── Directory.Build.props
├── src/
└── tests/
```

### Responsabilidades

- **Directory.Packages.props**
  - Centraliza versões de pacotes NuGet
  - Garante consistência entre projetos
  - Facilita atualização de dependências

- **Directory.Build.props**
  - Define configurações comuns de build
  - Reduz duplicação em arquivos `.csproj`
  - Padroniza comportamento entre projetos

## ⚖️ Consequências

### ✅ Positivas

- Centralização do gerenciamento de dependências
- Consistência de versões em toda a solução
- Redução de duplicação nos `.csproj`
- Facilidade de manutenção e atualização de pacotes
- Padronização de configurações de build
- Melhor governança da solução

### ❌ Negativas

- Necessidade de conhecimento adicional sobre arquivos globais
- Pode dificultar entendimento inicial para novos desenvolvedores
- Alterações globais impactam todos os projetos simultaneamente
- Debug de configurações pode ser menos direto

## 🔄 Alternativas consideradas

### 🧩 Gerenciamento individual por projeto (.csproj)

#### 🚫 Motivo da rejeição

- Duplicação de configurações
- Risco de inconsistência de versões
- Maior esforço de manutenção
- Atualizações mais complexas e propensas a erro

## 🔮 Considerações adicionais

A utilização de arquivos globais será mantida enquanto o projeto possuir múltiplos projetos e necessidade de padronização.

A estratégia adotada está alinhada com boas práticas modernas do ecossistema .NET.

## 📚 Referências (opcional)

- https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management
- https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-your-build
