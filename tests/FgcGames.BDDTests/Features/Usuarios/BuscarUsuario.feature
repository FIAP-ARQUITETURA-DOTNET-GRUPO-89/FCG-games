Feature: Buscar Usuário
  Como administrador do sistema
  Quero buscar usuários cadastrados
  Para gerenciar os acessos da plataforma

  Background:
    Given estou autenticado como "admin@fgcgames.com" com senha "Abc!1234"

  Scenario: Buscar usuário por ID existente retorna 200
    Given existe um usuário com email "busca.id@teste.com"
    When busco o usuário pelo ID
    Then a resposta deve ter status 200
    And a resposta deve conter os dados do usuário

  Scenario: Buscar usuário por ID inexistente retorna 404
    When busco o usuário com ID "00000000-0000-0000-0000-000000000001"
    Then a resposta deve ter status 404

  Scenario: Listar todos os usuários retorna lista paginada
    When listo todos os usuários com página 1 e tamanho 10
    Then a resposta deve ter status 200
    And a resposta deve conter uma lista paginada

  Scenario: Buscar usuários por nome retorna resultados filtrados
    Given existe um usuário com nome "Carlos Teste" e email "carlos.teste@teste.com"
    When busco usuários pelo nome "Carlos"
    Then a resposta deve ter status 200
