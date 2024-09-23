# Produtos e CIA - API de Controle de Estoque

## Sum�rio
1. [Descri��o do Projeto](#descri��o-do-projeto)
2. [Arquitetura](#arquitetura)
3. [Tecnologias Utilizadas](#tecnologias-utilizadas)
4. [Instala��o e Configura��o](#instala��o-e-configura��o)
5. [Autentica��o](#autentica��o)
6. [Rotas da API](#rotas-da-api)
7. [Contribui��es](#contribui��es)
8. [Licen�a](#licen�a)

## Descri��o do Projeto
API RESTful desenvolvida para gerenciar o controle de estoque de produtos de diferentes empresas, com funcionalidades de movimenta��o de produtos, controle de estoque em lote, autentica��o JWT, entre outras. O projeto foi desenvolvido utilizando o padr�o DDD (Domain-Driven Design), garantindo uma separa��o clara de responsabilidades e um c�digo escal�vel.

## Arquitetura
O projeto est� estruturado seguindo o padr�o de Domain-Driven Design (DDD) e possui as seguintes camadas:

- **Domain**: Cont�m as entidades e interfaces de reposit�rio, representando o n�cleo do sistema.
- **Application**: Cont�m os servi�os que coordenam as opera��es de neg�cio, al�m dos DTOs e mapeamentos (AutoMapper).
- **Infrastructure**: Implementa os reposit�rios e a persist�ncia de dados usando Entity Framework Core e PostgreSQL.
- **API**: Camada de interface com o usu�rio, que exp�e os endpoints e controla o fluxo das requisi��es HTTP.
- **Tests**: Cont�m a implementa��o de testes unit�rios para a API. Utilizamos o xUnit como framework de testes, juntamente com Moq para simula��o de depend�ncias, a fim de garantir que os servi�os e controladores funcionem conforme o esperado.

### Diagrama de Camadas

+------------------+ +---------------------+ | Presentation | --> | Application Layer | +------------------+ +---------------------+ | v +--------------------+ +--------------------+ | Domain Layer | --> | Infrastructure | +--------------------+ +--------------------+

## Tecnologias Utilizadas
- **.NET 8**: Framework principal para a constru��o da API.
- **Entity Framework Core 7**: ORM utilizado para o mapeamento e persist�ncia de dados.
- **PostgreSQL**: Banco de dados relacional utilizado para armazenar as informa��es.
- **AutoMapper**: Ferramenta de mapeamento de objetos para transformar entidades em DTOs.
- **FluentValidation**: Biblioteca para valida��o de dados de entrada nos DTOs.
- **JWT (JSON Web Token)**: Autentica��o e prote��o de rotas.
- **Swagger**: Ferramenta para documenta��o da API e teste das rotas.
- **xUnit**: Framework para realizar testes unit�rios.
- **Moq**: Utilizado nos testes unit�rios, para simula��o de depend�ncias.

