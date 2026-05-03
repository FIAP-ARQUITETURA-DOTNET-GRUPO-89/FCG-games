Feature: Deletar Jogo
  Como administrador do sistema
  Quero inativar jogos do catálogo
  Para controlar quais jogos estão disponíveis

  Background:
    Given estou autenticado como "admin@fgcgames.com" com senha "Abc!1234"

  Scenario: Admin inativa jogo existente retorna 204
    Given existe um jogo cadastrado com nome "Jogo Para Deletar"
    When deleto o jogo
    Then a resposta deve ter status 204

  Scenario: Deletar jogo inexistente retorna 404
    When deleto o jogo com ID "00000000-0000-0000-0000-000000000006"
    Then a resposta deve ter status 404

  Scenario: Usuário comum não pode deletar jogo retorna 403
    Given estou autenticado como "user@fgcgames.com" com senha "Abc!1234"
    And existe um jogo cadastrado com nome "Jogo Protegido"
    When deleto o jogo
    Then a resposta deve ter status 403
