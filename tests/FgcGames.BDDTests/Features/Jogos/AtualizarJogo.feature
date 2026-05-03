Feature: Atualizar Jogo
  Como administrador do sistema
  Quero atualizar os dados dos jogos
  Para manter o catálogo correto

  Background:
    Given estou autenticado como "admin@fgcgames.com" com senha "Abc!1234"
    And existe um jogo cadastrado com nome "Dark Souls"

  Scenario: Admin atualiza jogo com dados válidos retorna 204
    When atualizo o jogo com nome "Dark Souls Remastered", descrição "Versão remasterizada", preço 149.90, lançamento "2018-05-25" e classificação 16
    Then a resposta deve ter status 204

  Scenario: Atualizar jogo inexistente retorna 404
    When atualizo o jogo com ID "00000000-0000-0000-0000-000000000004" com nome "Teste"
    Then a resposta deve ter status 404

  Scenario: Usuário comum não pode atualizar jogo retorna 403
    Given estou autenticado como "user@fgcgames.com" com senha "Abc!1234"
    When atualizo o jogo com nome "Novo Nome", descrição "Desc", preço 99.90, lançamento "2020-01-01" e classificação 0
    Then a resposta deve ter status 403

  Scenario: Admin altera preço do jogo retorna 204
    When altero o preço do jogo para 89.90
    Then a resposta deve ter status 204

  Scenario: Alterar preço de jogo inexistente retorna 404
    When altero o preço do jogo com ID "00000000-0000-0000-0000-000000000005" para 50.00
    Then a resposta deve ter status 404
