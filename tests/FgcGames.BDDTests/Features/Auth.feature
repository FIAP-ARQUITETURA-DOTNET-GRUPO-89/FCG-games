Feature: Autenticação
  Como usuário do sistema
  Quero realizar login com minhas credenciais
  Para obter acesso às funcionalidades da API

  Scenario: Login com credenciais válidas retorna token
    When faço login com email "admin@fgcgames.com" e senha "Abc!1234"
    Then a resposta deve ter status 200
    And a resposta deve conter um token JWT válido

  Scenario: Login com email inexistente retorna 401
    When faço login com email "naoexiste@fgcgames.com" e senha "Abc!1234"
    Then a resposta deve ter status 401
    And o detalhe do erro deve ser "Credenciais inválidas."

  Scenario: Login com senha incorreta retorna 401
    When faço login com email "admin@fgcgames.com" e senha "SenhaErrada!"
    Then a resposta deve ter status 401
    And o detalhe do erro deve ser "Credenciais inválidas."

  Scenario: Login com email vazio retorna 400
    When faço login com email "" e senha "Abc!1234"
    Then a resposta deve ter status 400
    And os erros de validação devem conter o campo "Email"

  Scenario: Login com senha vazia retorna 400
    When faço login com email "admin@fgcgames.com" e senha ""
    Then a resposta deve ter status 400
    And os erros de validação devem conter o campo "Senha"

  Scenario: Login sem nenhum campo retorna 400
    When faço login sem informar credenciais
    Then a resposta deve ter status 400
    And os erros de validação devem conter o campo "Email"
    And os erros de validação devem conter o campo "Senha"
