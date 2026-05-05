# Linguagem Ubíqua

Este documento define a Linguagem Ubíqua do ecossistema, estabelecendo a base conceitual do nosso Domain-Driven Design (DDD).

---

## 👥 1. Usuário
*Identidade, acesso e ciclo de vida do indivíduo na plataforma.*

### Conceitos e Termos
| Termo | Definição |
| :--- | :--- |
| **Usuário** | Pessoa física cadastrada na plataforma para consumir jogos ou gerenciar o sistema (Admin). |
| **Credenciais** | Conjunto de e-mail e senha utilizados para autenticação. |
| **Registro/Cadastro** | O ato de um usuário solicitar entrada na plataforma. |
| **Confirmação de registro** | Ato de validar o e-mail do usuário para ativar sua conta. |
| **Login** | Processo de validação de identidade para acesso às áreas restritas. |
| **Exclusão de Conta** | Ação realizada pelo próprio Usuário quando ele deseja encerrar seu vínculo com a plataforma. Esta ação dispara a revogação imediata de suas credenciais e inicia o processo de encerramento de seus dados de negócio. |

### Ações de Domínio
* `RegistrarNovoUsuario`: Inicia o processo de criação de identidade no sistema.
* `ConfirmarIdentidade`: Valida o token/e-mail para ativar o acesso.
* `Autenticar`: Valida credenciais e gera o token de acesso (JWT).
* `AlterarSenha`: Atualiza a credencial de segurança.
* `SolicitarExclusaoDeConta`: O usuário manifesta a vontade de encerrar seu vínculo (gatilho para revogar acesso e inativar conta).

---

## 💳 2. Conta
*Financeiro e perfil administrativo.*

### Conceitos e Termos
| Termo | Definição |
| :--- | :--- |
| **Conta** | Entidade que mantém o rastro financeiro (saldo) e o nível de acesso (perfil) do usuário. |
| **Saldo** | Valor monetário disponível para a aquisição de novos títulos. |
| **Depósito** | Ação de adicionar fundos à conta através de um Gateway de Pagamento. |
| **Admin (Administrador)** | Perfil de usuário com permissões elevadas para gerenciar o catálogo e outros usuários. |
| **Inativação de Conta** | Ação administrativa realizada por um Admin. Consiste em suspender temporariamente ou permanentemente as atividades de uma conta (compras, acesso à biblioteca, uso de saldo). Geralmente motivada por violação de termos, suspeita de fraude ou auditoria. |
| **Reativação de Conta** | Ação exclusiva do Admin para devolver à conta o status de "Ativa", restaurando todas as funcionalidades do usuário. |

### Ações de Domínio
* `SolicitarDeposito`: Inicia a comunicação com o Gateway para adicionar saldo.
* `ConfirmarAporteFinanceiro`: Atualiza o saldo após confirmação do Gateway.
* `DebitarSaldo`: Deduz valor da conta para compra de jogos.
* `InativarConta`: *(Ação do Admin)* Suspende as operações da conta por regras de negócio.
* `ReativarConta`: *(Ação do Admin)* Restaura as operações de uma conta suspensa.
* `DefinirPerfilAdmin`: Eleva os privilégios de uma conta.

---

## 🕹️ 3. Jogo e Catálogo
*Ciclo de vida do produto, desde a criação até a disponibilidade para venda.*

### Conceitos e Termos
| Termo | Definição |
| :--- | :--- |
| **Jogo** | O produto digital oferecido pela plataforma. |
| **Rascunho de Jogo** | Estado inicial de um jogo criado por um Admin que ainda não passou por revisão ou validação. |
| **Validação/Aprovação** | Processo de curadoria onde um segundo Admin (ou o sistema) revisa os dados do jogo para publicação. |
| **Catálogo** | Lista de jogos publicados e disponíveis para navegação e compra. |
| **Classificação Etária** | Atributo do jogo que restringe a visibilidade ou compra conforme a idade do usuário. |

### Ações de Domínio
* `CriarRascunhoDeJogo`: Salva os dados iniciais de um jogo sem publicá-lo.
* `ValidarDadosDoJogo`: Revisa informações técnicas e classificação etária.
* `AprovarPublicacao`: *(Ação do Admin)* Autoriza o jogo a entrar na vitrine.
* `PublicarNoCatalogo`: Torna o jogo visível para os usuários.
* `SincronizarLoja`: Atualiza a vitrine após alterações no catálogo.

---

## 🛒 4. Transação
*Processo de aquisição.*

### Conceitos e Termos
| Termo | Definição |
| :--- | :--- |
| **Carrinho de Compras** | Lista temporária de jogos que o usuário deseja adquirir. |
| **Transação** | O processo de checkout que envolve validação de saldo ou comunicação com Gateway e conclusão da compra. |
| **Licença** | O direito de uso concedido ao usuário após uma transação confirmada (vinculação do jogo à conta). |
| **Gateway de Pagamento** | Sistema externo responsável por processar pagamentos (Cartão, Pix, etc). |

### Ações de Domínio
* `AdicionarAoCarrinho`: Reserva a intenção de compra de um item.
* `IniciarCheckout`: Dispara a validação de regras para finalizar a compra.
* `ValidarSaldoDisponivel`: Verifica se há fundos para a transação.
* `ProcessarPagamentoExterno`: Interage com o Gateway para pagamentos fora do saldo.
* `VincularLicenca`: Cria o direito de uso do jogo para o usuário após sucesso financeiro.
* `EstornarTransacao`: Devolve o saldo caso a entrega do produto (licença) falhe.

---

## 📚 5. Biblioteca
*Consumo do produto adquirido.*

### Conceitos e Termos
| Termo | Definição |
| :--- | :--- |
| **Biblioteca** | Espaço pessoal do usuário onde residem todos os jogos que ele possui licença. |
| **Sessão de Jogo** | O período de tempo em que um jogo permanece aberto. |
| **Tempo de Jogo** | Registro acumulado de horas/minutos que o usuário dedicou a um título específico. |
| **Inicializador (Launcher)** | Componente de software responsável por baixar, instalar e executar o jogo localmente. |

### Ações de Domínio
* `SincronizarBiblioteca`: Atualiza a lista de jogos com base nas licenças ativas.
* `InstalarJogo`: Prepara os arquivos via Inicializador.
* `IniciarSessaoDeJogo`: Abre o jogo e começa a cronometrar o uso.
* `EncerrarSessaoDeJogo`: Fecha o jogo e salva o progresso/tempo.
* `RegistrarTempoDeUso`: Acumula as horas jogadas na ficha do jogo.
* `InativarAcessoAoJogo`: Bloqueia a execução (usado em caso de inativação de conta).
