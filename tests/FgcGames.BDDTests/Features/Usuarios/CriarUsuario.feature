Feature: Criar Usuário
  Como visitante do sistema
  Quero criar uma conta de usuário
  Para acessar as funcionalidades da plataforma

  Scenario: Criar usuário com dados válidos retorna 201
    When crio um usuário com nome "João Silva", email "joao.silva@teste.com", data de nascimento "1995-06-15" e senha "Abc!1234"
    Then a resposta deve ter status 201
    And a resposta deve conter um ID de usuário válido

  Scenario: Criar usuário com email já cadastrado retorna 400
    Given existe um usuário com email "email.existente@teste.com"
    When crio um usuário com nome "Outro Nome", email "email.existente@teste.com", data de nascimento "1990-01-01" e senha "Abc!1234"
    Then a resposta deve ter status 400

  Scenario: Criar usuário com senha fraca retorna 400
    When crio um usuário com nome "João Silva", email "joao.fraco@teste.com", data de nascimento "1995-06-15" e senha "123"
    Then a resposta deve ter status 400
