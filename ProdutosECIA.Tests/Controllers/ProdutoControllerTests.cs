using Microsoft.AspNetCore.Mvc;
using Moq;
using ProdutosECIA.API.Controllers;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Interfaces;

namespace ProdutosECIA.Tests.Controllers;

public class ProdutoControllerTests
{
    private readonly Mock<IProdutoService> _produtoServiceMock;
    private readonly ProdutoController _produtoController;

    public ProdutoControllerTests()
    {
        _produtoServiceMock = new Mock<IProdutoService>();
        _produtoController = new ProdutoController(_produtoServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenProdutosExist()
    {
        // Arrange
        var produtos = new List<ProdutoDto>
        {
            new ProdutoDto { Id = Guid.NewGuid(), Nome = "Produto 1", Quantidade = 10 },
            new ProdutoDto { Id = Guid.NewGuid(), Nome = "Produto 2", Quantidade = 5 }
        };

        _produtoServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(produtos);

        // Act
        var result = await _produtoController.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<ProdutoDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenProdutoExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtoDto = new ProdutoDto { Id = id, Nome = "Produto 1" };
        _produtoServiceMock.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(produtoDto);

        // Act
        var result = await _produtoController.GetById(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(produtoDto, okResult.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenProdutoDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _produtoServiceMock.Setup(service => service.GetByIdAsync(id)).ReturnsAsync((ProdutoDto)null);

        // Act
        var result = await _produtoController.GetById(id);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenProdutoIsCreated()
    {
        // Arrange
        var produtoCreateDto = new ProdutoCreateDto { Nome = "Novo Produto", Quantidade = 10 };
        var produtoDto = new ProdutoDto { Id = Guid.NewGuid(), Nome = "Novo Produto", Quantidade = 10 };

        _produtoServiceMock.Setup(service => service.CreateAsync(produtoCreateDto)).ReturnsAsync(produtoDto);

        // Act
        var result = await _produtoController.Create(produtoCreateDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("GetById", createdResult.ActionName);
        Assert.Equal(produtoDto.Id, createdResult.RouteValues["id"]);
        Assert.Equal(produtoDto, createdResult.Value);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var produtoCreateDto = new ProdutoCreateDto();
        _produtoController.ModelState.AddModelError("Nome", "O nome é obrigatório.");

        // Act
        var result = await _produtoController.Create(produtoCreateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenProdutoIsUpdated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtoUpdateDto = new ProdutoUpdateDto { Nome = "Produto Atualizado" };
        _produtoServiceMock.Setup(service => service.UpdateAsync(id, produtoUpdateDto)).ReturnsAsync(true);

        // Act
        var result = await _produtoController.Update(id, produtoUpdateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenProdutoDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtoUpdateDto = new ProdutoUpdateDto { Nome = "Produto Atualizado" };
        _produtoServiceMock.Setup(service => service.UpdateAsync(id, produtoUpdateDto)).ReturnsAsync(false);

        // Act
        var result = await _produtoController.Update(id, produtoUpdateDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenProdutoIsDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _produtoServiceMock.Setup(service => service.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _produtoController.Delete(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenProdutoDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _produtoServiceMock.Setup(service => service.DeleteAsync(id)).ReturnsAsync(false);

        // Act
        var result = await _produtoController.Delete(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task MovimentarProdutoAsync_ShouldReturnOk_WhenMovimentacaoIsSuccessful()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var movimentacaoDto = new MovimentacaoProdutoDto { Adicionar = true, Quantidade = 5 };
        _produtoServiceMock.Setup(service => service.MovimentarProdutoAsync(produtoId, movimentacaoDto)).ReturnsAsync(true);

        // Act
        var result = await _produtoController.MovimentarProdutoAsync(produtoId, movimentacaoDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Movimentação realizada com sucesso.", okResult.Value);
    }

    [Fact]
    public async Task MovimentarProdutoAsync_ShouldReturnBadRequest_WhenMovimentacaoFails()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var movimentacaoDto = new MovimentacaoProdutoDto { Adicionar = true, Quantidade = 5 };
        _produtoServiceMock.Setup(service => service.MovimentarProdutoAsync(produtoId, movimentacaoDto)).ReturnsAsync(false);

        // Act
        var result = await _produtoController.MovimentarProdutoAsync(produtoId, movimentacaoDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Erro ao movimentar o produto.", badRequestResult.Value);
    }

    [Fact]
    public async Task MovimentarProdutosEmLoteAsync_ShouldReturnOk_WhenMovimentacaoEmLoteIsSuccessful()
    {
        // Arrange
        var movimentacaoLoteDto = new MovimentacaoLoteDto { Adicionar = true, Quantidade = 5, ProdutoIds = new List<Guid> { Guid.NewGuid() } };
        _produtoServiceMock.Setup(service => service.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto)).ReturnsAsync(true);

        // Act
        var result = await _produtoController.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Movimentação em lote realizada com sucesso.", okResult.Value);
    }

    [Fact]
    public async Task MovimentarProdutosEmLoteAsync_ShouldReturnBadRequest_WhenMovimentacaoEmLoteFails()
    {
        // Arrange
        var movimentacaoLoteDto = new MovimentacaoLoteDto { Adicionar = true, Quantidade = 5, ProdutoIds = new List<Guid> { Guid.NewGuid() } };
        _produtoServiceMock.Setup(service => service.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto)).ReturnsAsync(false);

        // Act
        var result = await _produtoController.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Erro ao movimentar produtos.", badRequestResult.Value);
    }

    [Fact]
    public async Task ObterValorTotalEstoqueAsync_ShouldReturnOk_WhenCalled()
    {
        // Arrange
        var valorTotal = 1000.00m;
        _produtoServiceMock.Setup(service => service.ObterValorTotalEstoqueAsync()).ReturnsAsync(valorTotal);

        // Act
        var result = await _produtoController.ObterValorTotalEstoqueAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(valorTotal, okResult.Value);
    }

    [Fact]
    public async Task ObterQuantidadeTotalEstoqueAsync_ShouldReturnOk_WhenCalled()
    {
        // Arrange
        var quantidadeTotal = 50;
        _produtoServiceMock.Setup(service => service.ObterQuantidadeTotalEstoqueAsync()).ReturnsAsync(quantidadeTotal);

        // Act
        var result = await _produtoController.ObterQuantidadeTotalEstoqueAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(quantidadeTotal, okResult.Value);
    }

    [Fact]
    public async Task ObterCustoMedioProdutoAsync_ShouldReturnOk_WhenProdutoExists()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var custoMedio = 200.00m;
        _produtoServiceMock.Setup(service => service.ObterCustoMedioProdutoAsync(produtoId)).ReturnsAsync(custoMedio);

        // Act
        var result = await _produtoController.ObterCustoMedioProdutoAsync(produtoId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(custoMedio, okResult.Value);
    }

    [Fact]
    public async Task ObterCustoMedioProdutoAsync_ShouldReturnNotFound_WhenProdutoDoesNotExist()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        _produtoServiceMock.Setup(service => service.ObterCustoMedioProdutoAsync(produtoId)).ReturnsAsync(0);

        // Act
        var result = await _produtoController.ObterCustoMedioProdutoAsync(produtoId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Produto com o Id {produtoId} não foi encontrado.", notFoundResult.Value);
    }

    [Fact]
    public async Task ObterCustoMedioEstoqueAsync_ShouldReturnOk_WhenCalled()
    {
        // Arrange
        var custoMedio = 150.00m;
        _produtoServiceMock.Setup(service => service.ObterCustoMedioEstoqueAsync()).ReturnsAsync(custoMedio);

        // Act
        var result = await _produtoController.ObterCustoMedioEstoqueAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(custoMedio, okResult.Value);
    }
}

