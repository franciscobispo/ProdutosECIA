using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProdutosECIA.API.Controllers;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Services.Interfaces;

namespace ProdutosECIA.Tests
{
    public class ProdutoControllerTests
    {
        private readonly Mock<IProdutoService> _mockProdutoService;
        private readonly ProdutoController _produtoController;

        public ProdutoControllerTests()
        {
            _mockProdutoService = new Mock<IProdutoService>();
            _produtoController = new ProdutoController(_mockProdutoService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WhenProductsExist()
        {
            // Arrange
            var produtos = new List<ProdutoDto>
            {
                new ProdutoDto { Id = Guid.NewGuid(), Nome = "Produto A" },
                new ProdutoDto { Id = Guid.NewGuid(), Nome = "Produto B" }
            };
            _mockProdutoService.Setup(service => service.GetAllAsync())
                .ReturnsAsync(produtos);

            // Act
            var result = await _produtoController.GetAll();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(produtos);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkResult_WhenProductExists()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new ProdutoDto { Id = produtoId, Nome = "Produto A" };
            _mockProdutoService.Setup(service => service.GetByIdAsync(produtoId))
                .ReturnsAsync(produto);

            // Act
            var result = await _produtoController.GetById(produtoId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(produto);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            _mockProdutoService.Setup(service => service.GetByIdAsync(produtoId))
                .ReturnsAsync((ProdutoDto)null);

            // Act
            var result = await _produtoController.GetById(produtoId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtActionResult_WhenProductIsCreated()
        {
            // Arrange
            var produtoCreateDto = new ProdutoCreateDto { Nome = "Produto A" };
            var createdProduto = new ProdutoDto { Id = Guid.NewGuid(), Nome = "Produto A" };
            _mockProdutoService.Setup(service => service.CreateAsync(produtoCreateDto))
                .ReturnsAsync(createdProduto);

            // Act
            var result = await _produtoController.Create(produtoCreateDto);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdAtResult = result.Result as CreatedAtActionResult;
            createdAtResult.ActionName.Should().Be("GetById");
            createdAtResult.RouteValues["id"].Should().Be(createdProduto.Id);
            createdAtResult.Value.Should().BeEquivalentTo(createdProduto);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _produtoController.ModelState.AddModelError("Nome", "Nome é obrigatório");

            var produtoCreateDto = new ProdutoCreateDto();

            // Act
            var result = await _produtoController.Create(produtoCreateDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            var serializableError = badRequestResult.Value as SerializableError;
            serializableError.Should().ContainKey("Nome");
            serializableError["Nome"].Should().BeOfType<string[]>().Which.Should().Contain("Nome é obrigatório");
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenProductIsUpdated()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produtoUpdateDto = new ProdutoUpdateDto { Nome = "Produto Atualizado" };

            _mockProdutoService.Setup(service => service.UpdateAsync(produtoId, produtoUpdateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _produtoController.Update(produtoId, produtoUpdateDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produtoUpdateDto = new ProdutoUpdateDto { Nome = "Produto Atualizado" };

            _mockProdutoService.Setup(service => service.UpdateAsync(produtoId, produtoUpdateDto))
                .ReturnsAsync(false);

            // Act
            var result = await _produtoController.Update(produtoId, produtoUpdateDto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenProductIsDeleted()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            _mockProdutoService.Setup(service => service.DeleteAsync(produtoId))
                .ReturnsAsync(true);

            // Act
            var result = await _produtoController.Delete(produtoId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            _mockProdutoService.Setup(service => service.DeleteAsync(produtoId))
                .ReturnsAsync(false);

            // Act
            var result = await _produtoController.Delete(produtoId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task MovimentarProduto_ShouldReturnOk_WhenMovimentacaoIsSuccessful()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var movimentacaoDto = new MovimentacaoProdutoDto { Quantidade = 10 };

            _mockProdutoService.Setup(service => service.MovimentarProdutoAsync(produtoId, movimentacaoDto))
                .ReturnsAsync(true);

            // Act
            var result = await _produtoController.MovimentarProdutoAsync(produtoId, movimentacaoDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task MovimentarProduto_ShouldReturnBadRequest_WhenMovimentacaoFails()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var movimentacaoDto = new MovimentacaoProdutoDto { Quantidade = 10 };

            _mockProdutoService.Setup(service => service.MovimentarProdutoAsync(produtoId, movimentacaoDto))
                .ReturnsAsync(false);

            // Act
            var result = await _produtoController.MovimentarProdutoAsync(produtoId, movimentacaoDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Erro ao movimentar o produto.");
        }

        [Fact]
        public async Task MovimentarProdutosEmLoteAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var movimentacaoLoteDto = new MovimentacaoLoteDto { };
            _mockProdutoService.Setup(x => x.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto)).ReturnsAsync(true);

            // Act
            var result = await _produtoController.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be("Movimentação em lote realizada com sucesso.");
        }

        [Fact]
        public async Task MovimentarProdutosEmLoteAsync_ShouldReturnBadRequest_WhenFails()
        {
            // Arrange
            var movimentacaoLoteDto = new MovimentacaoLoteDto { };
            _mockProdutoService.Setup(x => x.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto)).ReturnsAsync(false);

            // Act
            var result = await _produtoController.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Erro ao movimentar produtos.");
        }

        [Fact]
        public async Task ObterValorTotalEstoqueAsync_ShouldReturnOk_WithValorTotal()
        {
            // Arrange
            var valorTotal = 1000m;
            _mockProdutoService.Setup(x => x.ObterValorTotalEstoqueAsync()).ReturnsAsync(valorTotal);

            // Act
            var result = await _produtoController.ObterValorTotalEstoqueAsync();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(valorTotal);
        }

        [Fact]
        public async Task ObterQuantidadeTotalProdutoAsync_ShouldReturnOk_WhenQuantidadeIsGreaterThanZero()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var empresaId = Guid.NewGuid();
            var quantidadeTotal = 10;
            _mockProdutoService.Setup(x => x.ObterQuantidadeTotalProdutoAsync(produtoId, empresaId)).ReturnsAsync(quantidadeTotal);

            // Act
            var result = await _produtoController.ObterQuantidadeTotalProdutoAsync(produtoId, empresaId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(quantidadeTotal);
        }

        [Fact]
        public async Task ObterQuantidadeTotalProdutoAsync_ShouldReturnNotFound_WhenQuantidadeIsZero()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var empresaId = Guid.NewGuid();
            _mockProdutoService.Setup(x => x.ObterQuantidadeTotalProdutoAsync(produtoId, empresaId)).ReturnsAsync(0);

            // Act
            var result = await _produtoController.ObterQuantidadeTotalProdutoAsync(produtoId, empresaId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be($"Estoque não encontrado para o Produto com o Id {produtoId} na Empresa {empresaId}.");
        }

        [Fact]
        public async Task ObterCustoMedioProdutoAsync_ShouldReturnOk_WithCustoMedio()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var custoMedio = 20.5m;
            _mockProdutoService.Setup(x => x.ObterCustoMedioProdutoAsync(produtoId)).ReturnsAsync(custoMedio);

            // Act
            var result = await _produtoController.ObterCustoMedioProdutoAsync(produtoId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(custoMedio);
        }

        [Fact]
        public async Task ObterCustoMedioProdutoAsync_ShouldReturnNotFound_WhenCustoMedioIsZero()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            _mockProdutoService.Setup(x => x.ObterCustoMedioProdutoAsync(produtoId)).ReturnsAsync(0m);

            // Act
            var result = await _produtoController.ObterCustoMedioProdutoAsync(produtoId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be($"Produto com o Id {produtoId} não foi encontrado.");
        }

        [Fact]
        public async Task ObterCustoMedioEstoqueAsync_ShouldReturnOk_WithCustoMedioEstoque()
        {
            // Arrange
            var custoMedioEstoque = 15m;
            _mockProdutoService.Setup(x => x.ObterCustoMedioEstoqueAsync()).ReturnsAsync(custoMedioEstoque);

            // Act
            var result = await _produtoController.ObterCustoMedioEstoqueAsync();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(custoMedioEstoque);
        }

        [Fact]
        public async Task TransferirProdutoAsync_ShouldReturnOk_WhenTransferIsSuccessful()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var deEmpresaId = Guid.NewGuid();
            var paraEmpresaId = Guid.NewGuid();
            var quantidade = 5;
            _mockProdutoService.Setup(x => x.TransferirProdutoAsync(produtoId, deEmpresaId, paraEmpresaId, quantidade)).ReturnsAsync(true);

            // Act
            var result = await _produtoController.TransferirProdutoAsync(produtoId, deEmpresaId, paraEmpresaId, quantidade);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be("Transferência realizada com sucesso.");
        }

        [Fact]
        public async Task TransferirProdutoAsync_ShouldReturnBadRequest_WhenTransferFails()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var deEmpresaId = Guid.NewGuid();
            var paraEmpresaId = Guid.NewGuid();
            var quantidade = 5;
            _mockProdutoService.Setup(x => x.TransferirProdutoAsync(produtoId, deEmpresaId, paraEmpresaId, quantidade)).ReturnsAsync(false);

            // Act
            var result = await _produtoController.TransferirProdutoAsync(produtoId, deEmpresaId, paraEmpresaId, quantidade);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Erro ao transferir o produto.");
        }
    }
}