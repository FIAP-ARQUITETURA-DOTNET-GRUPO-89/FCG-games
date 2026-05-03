Feature: Atualizar Usuário
  Como administrador do sistema
  Quero atualizar os dados dos usuários
  Para manter o cadastro atualizado

  Background:
    Given estou autenticado como "admin@fgcgames.com" com senha "Abc!1234"
    And existe um usuário com email "atualizar@teste.com"

  Scenario: Atualizar perfil com dados válidos retorna 200
    When atualizo o nome do usuário para "Nome Atualizado" e data de nascimento "1990-05-20"
    Then a resposta deve ter status 200

  Scenario: Atualizar role do usuário retorna 200
    When atualizo a role do usuário para "Admin"
    Then a resposta deve ter status 200

  Scenario: Atualizar senha com nova senha válida retorna 200
    When atualizo a senha do usuário para "NovaSenha!1"
    Then a resposta deve ter status 200
