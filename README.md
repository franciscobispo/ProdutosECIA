# Produtos e CIA - API de Controle de Estoque

## Sumário
1. [Descrição do Projeto](#descrição-do-projeto)
2. [Arquitetura](#arquitetura)
3. [Tecnologias Utilizadas](#tecnologias-utilizadas)
4. [Instalação e Configuração](#instalação-e-configuração)
5. [Autenticação](#autenticação)
6. [Rotas da API](#rotas-da-api)
7. [Contribuições](#contribuições)
8. [Licença](#licença)

## Descrição do Projeto
API RESTful desenvolvida para gerenciar o controle de estoque de produtos de diferentes empresas, com funcionalidades de movimentação de produtos, controle de estoque em lote, autenticação JWT, entre outras. O projeto foi desenvolvido utilizando o padrão DDD (Domain-Driven Design), garantindo uma separação clara de responsabilidades e um código escalável.

## Arquitetura
O projeto está estruturado seguindo o padrão de Domain-Driven Design (DDD) e possui as seguintes camadas:

- **Domain**: Contém as entidades e interfaces de repositório, representando o núcleo do sistema.
- **Application**: Contém os serviços que coordenam as operações de negócio, além dos DTOs e mapeamentos (AutoMapper).
- **Infrastructure**: Implementa os repositórios e a persistência de dados usando Entity Framework Core e PostgreSQL.
- **API**: Camada de interface com o usuário, que expõe os endpoints e controla o fluxo das requisições HTTP.
- **Tests**: Contém a implementação de testes unitários para a API. Utilizamos o xUnit como framework de testes, juntamente com Moq para simulação de dependências, a fim de garantir que os serviços e controladores funcionem conforme o esperado.

## Diagrama de Camadas

```plaintext
+------------------+
|   Presentation   |
|  (Controllers)   |
+------------------+
         |
         v
+------------------+
|   Application    |
|   (Services)     |
+------------------+
         |
         v
+------------------+
|      Domain      |
|    (Entities)    |
+------------------+
         |
         v
+------------------+
|  Infrastructure  |
|  (Repositories)  |
+------------------+


+------------------+
|      Tests       |
+------------------+
```

## Tecnologias Utilizadas
- **.NET 8**: Framework principal para a construção da API.
- **Entity Framework Core 7**: ORM utilizado para o mapeamento e persistência de dados.
- **PostgreSQL**: Banco de dados relacional utilizado para armazenar as informações.
- **AutoMapper**: Ferramenta de mapeamento de objetos para transformar entidades em DTOs.
- **FluentValidation**: Biblioteca para validação de dados de entrada nos DTOs.
- **JWT (JSON Web Token)**: Autenticação e proteção de rotas.
- **Swagger**: Ferramenta para documentação da API e teste das rotas.
- **xUnit**: Framework para realizar testes unitários.
- **Moq**: Utilizado nos testes unitários, para simulação de dependências.

