Feature: Buscar Jogo
  Como usuário autenticado
  Quero buscar jogos cadastrados
  Para visualizar o catálogo disponível

  Background:
    Given estou autenticado como "admin@fgcgames.com" com senha "Abc!1234"
    And existe um jogo cadastrado com nome "God of War"

  Scenario: Listar todos os jogos retorna 200
    When listo todos os jogos
    Then a resposta deve ter status 200
    And a resposta deve conter uma lista de jogos

  Scenario: Buscar jogo por ID existente retorna 200
    When busco o jogo pelo ID
    Then a resposta deve ter status 200
    And a resposta deve conter os dados do jogo

  Scenario: Buscar jogo por ID inexistente retorna 404
    When busco o jogo com ID "00000000-0000-0000-0000-000000000003"
    Then a resposta deve ter status 404

  Scenario: Usuário comum pode listar jogos retorna 200
    Given estou autenticado como "user@fgcgames.com" com senha "Abc!1234"
    When listo todos os jogos
    Then a resposta deve ter status 200

  Scenario: Requisição sem autenticação não pode listar jogos retorna 401
    Given não estou autenticado
    When listo todos os jogos
    Then a resposta deve ter status 401
