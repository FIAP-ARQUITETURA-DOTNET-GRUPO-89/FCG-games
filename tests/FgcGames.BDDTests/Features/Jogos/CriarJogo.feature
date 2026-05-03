Feature: Criar Jogo
  Como administrador do sistema
  Quero cadastrar novos jogos
  Para disponibilizá-los na plataforma

  Background:
    Given estou autenticado como "admin@fgcgames.com" com senha "Abc!1234"

  Scenario: Admin cria jogo com dados válidos retorna 201
    When crio um jogo com nome "The Witcher 3", descrição "RPG de mundo aberto", preço 199.90, lançamento "2015-05-19" e classificação 18
    Then a resposta deve ter status 201
    And a resposta deve conter um ID de jogo válido

  Scenario: Usuário comum não pode criar jogo retorna 403
    Given estou autenticado como "user@fgcgames.com" com senha "Abc!1234"
    When crio um jogo com nome "Jogo Teste", descrição "Desc", preço 99.90, lançamento "2020-01-01" e classificação 0
    Then a resposta deve ter status 403

  Scenario: Requisição sem autenticação não pode criar jogo retorna 401
    Given não estou autenticado
    When crio um jogo com nome "Jogo Teste", descrição "Desc", preço 99.90, lançamento "2020-01-01" e classificação 0
    Then a resposta deve ter status 401
