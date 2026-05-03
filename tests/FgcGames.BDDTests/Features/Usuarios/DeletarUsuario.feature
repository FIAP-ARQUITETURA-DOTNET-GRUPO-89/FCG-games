Feature: Deletar Usuário
  Como administrador do sistema
  Quero inativar usuários da plataforma
  Para controlar os acessos

  Background:
    Given estou autenticado como "admin@fgcgames.com" com senha "Abc!1234"

  Scenario: Inativar usuário existente retorna 200
    Given existe um usuário com email "deletar@teste.com"
    When deleto o usuário
    Then a resposta deve ter status 200

  Scenario: Deletar usuário inexistente retorna 404
    When deleto o usuário com ID "00000000-0000-0000-0000-000000000002"
    Then a resposta deve ter status 404
