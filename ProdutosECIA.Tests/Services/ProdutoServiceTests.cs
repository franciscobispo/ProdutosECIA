using AutoMapper;
using FluentAssertions;
using Moq;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Interfaces;
using ProdutosECIA.Application.Services;
using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Tests.Services;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly IProdutoService _produtoService;
    private readonly Mock<IMapper> _mapperMock;

    public ProdutoServiceTests()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _mapperMock = new Mock<IMapper>();
        _produtoService = new ProdutoService(_produtoRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllProdutos_ShouldReturnListOfProdutos_WhenProdutosExist()
    {
        // Arrange
        var produtos = new List<Produto>
        {
            new Produto { Nome = "Produto 1", PrecoCusto = 10, PrecoVenda = 15, Quantidade = 50 },
            new Produto { Nome = "Produto 2", PrecoCusto = 20, PrecoVenda = 30, Quantidade = 70 }
        };

        var produtosDto = produtos.Select(p => new ProdutoDto { Nome = p.Nome, PrecoCusto = p.PrecoCusto, PrecoVenda = p.PrecoVenda, Quantidade = p.Quantidade });

        _produtoRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(produtos);
        _mapperMock.Setup(m => m.Map<IEnumerable<ProdutoDto>>(produtos)).Returns(produtosDto);

        // Act
        var result = await _produtoService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _produtoRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<ProdutoDto>>(produtos), Times.Once);
    }

    [Fact]
    public async Task GetAllProdutos_ShouldReturnEmptyList_WhenNoProdutosExist()
    {
        // Arrange
        _produtoRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Produto>());

        // Act
        var result = await _produtoService.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
        _produtoRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProdutoById_ShouldReturnProduto_WhenProdutoExists()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId, Nome = "Produto Teste", PrecoCusto = 10, PrecoVenda = 15, Quantidade = 100 };
        var produtoDto = new ProdutoDto { Id = produtoId, Nome = "Produto Teste", PrecoCusto = 10, PrecoVenda = 15, Quantidade = 100 };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId)).ReturnsAsync(produto);
        _mapperMock.Setup(m => m.Map<ProdutoDto>(produto)).Returns(produtoDto);

        // Act
        var result = await _produtoService.GetByIdAsync(produtoId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(produtoId, result.Id);
        Assert.Equal(produto.Nome, result.Nome);
        _produtoRepositoryMock.Verify(repo => repo.GetByIdAsync(produtoId), Times.Once);
        _mapperMock.Verify(m => m.Map<ProdutoDto>(produto), Times.Once);
    }

    [Fact]
    public async Task GetProdutoById_ShouldReturnNull_WhenProdutoDoesNotExist()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId)).ReturnsAsync((Produto)null);

        // Act
        var result = await _produtoService.GetByIdAsync(produtoId);

        // Assert
        result.Should().BeNull();
        _produtoRepositoryMock.Verify(repo => repo.GetByIdAsync(produtoId), Times.Once);
    }

    [Fact]
    public async Task CreateProduto_ShouldAddProdutoSuccessfully()
    {
        // Arrange
        var produtoCreateDto = new ProdutoCreateDto
        {
            Nome = "Novo Produto",
            PrecoCusto = 10,
            PrecoVenda = 15,
            Quantidade = 50,
            EmpresaId = Guid.NewGuid()
        };

        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = produtoCreateDto.Nome,
            PrecoCusto = produtoCreateDto.PrecoCusto,
            PrecoVenda = produtoCreateDto.PrecoVenda,
            Quantidade = produtoCreateDto.Quantidade,
            EmpresaId = produtoCreateDto.EmpresaId
        };

        // Simulando o mapeamento de ProdutoCreateDto para Produto
        _mapperMock.Setup(m => m.Map<Produto>(produtoCreateDto)).Returns(produto);

        // Simulando a adição ao repositório
        _produtoRepositoryMock.Setup(repo => repo.AddAsync(produto)).Returns(Task.CompletedTask);

        // Act
        await _produtoService.CreateAsync(produtoCreateDto);

        // Assert
        _produtoRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Produto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduto_WhenProdutoExists()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId, Nome = "Produto Antigo" };
        var produtoUpdateDto = new ProdutoUpdateDto { Nome = "Produto Atualizado" };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId)).ReturnsAsync(produto);
        _produtoRepositoryMock.Setup(repo => repo.UpdateAsync(produto)).ReturnsAsync(true);

        // Act
        var result = await _produtoService.UpdateAsync(produtoId, produtoUpdateDto);

        // Assert
        Assert.True(result);
        _produtoRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Produto>()), Times.Once);
        _mapperMock.Verify(m => m.Map(produtoUpdateDto, produto), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenProdutoDoesNotExist()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var produtoUpdateDto = new ProdutoUpdateDto { Nome = "Produto Atualizado" };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId)).ReturnsAsync((Produto)null);

        // Act
        var result = await _produtoService.UpdateAsync(produtoId, produtoUpdateDto);

        // Assert
        Assert.False(result);
        _produtoRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenProdutoDeleted()
    {
        // Arrange
        var produtoId = Guid.NewGuid();

        _produtoRepositoryMock.Setup(repo => repo.DeleteAsync(produtoId)).ReturnsAsync(true);

        // Act
        var result = await _produtoService.DeleteAsync(produtoId);

        // Assert
        Assert.True(result);
        _produtoRepositoryMock.Verify(repo => repo.DeleteAsync(produtoId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProdutoNotDeleted()
    {
        // Arrange
        var produtoId = Guid.NewGuid();

        _produtoRepositoryMock.Setup(repo => repo.DeleteAsync(produtoId)).ReturnsAsync(false);

        // Act
        var result = await _produtoService.DeleteAsync(produtoId);

        // Assert
        Assert.False(result);
        _produtoRepositoryMock.Verify(repo => repo.DeleteAsync(produtoId), Times.Once);
    }

    [Fact]
    public async Task MovimentarProdutoAsync_ShouldReturnFalse_WhenProdutoDoesNotExist()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var movimentacaoDto = new MovimentacaoProdutoDto { Adicionar = true, Quantidade = 10 };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId)).ReturnsAsync((Produto)null);

        // Act
        var result = await _produtoService.MovimentarProdutoAsync(produtoId, movimentacaoDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task MovimentarProdutoAsync_ShouldAddQuantidade_WhenAdicionarIsTrue()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId, Quantidade = 5 };
        var movimentacaoDto = new MovimentacaoProdutoDto { Adicionar = true, Quantidade = 10 };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId)).ReturnsAsync(produto);
        _produtoRepositoryMock.Setup(repo => repo.UpdateAsync(produto)).ReturnsAsync(true);

        // Act
        var result = await _produtoService.MovimentarProdutoAsync(produtoId, movimentacaoDto);

        // Assert
        Assert.True(result);
        Assert.Equal(15, produto.Quantidade); // Verifica se a quantidade foi atualizada
    }

    [Fact]
    public async Task MovimentarProdutoAsync_ShouldReturnFalse_WhenRemovingQuantityCausesNegativeStock()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId, Quantidade = 5 };
        var movimentacaoDto = new MovimentacaoProdutoDto { Adicionar = false, Quantidade = 10 };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId)).ReturnsAsync(produto);

        // Act
        var result = await _produtoService.MovimentarProdutoAsync(produtoId, movimentacaoDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task MovimentarProdutoAsync_ShouldRemoveQuantidade_WhenAdicionarIsFalse()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId, Quantidade = 10 };
        var movimentacaoDto = new MovimentacaoProdutoDto { Adicionar = false, Quantidade = 5 };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId)).ReturnsAsync(produto);
        _produtoRepositoryMock.Setup(repo => repo.UpdateAsync(produto)).ReturnsAsync(true);

        // Act
        var result = await _produtoService.MovimentarProdutoAsync(produtoId, movimentacaoDto);

        // Assert
        Assert.True(result);
        Assert.Equal(5, produto.Quantidade); // Verifica se a quantidade foi atualizada
    }

    [Fact]
    public async Task MovimentarProdutosEmLoteAsync_ShouldReturnFalse_WhenAnyProdutoDoesNotExist()
    {
        // Arrange
        var movimentacaoLoteDto = new MovimentacaoLoteDto
        {
            ProdutoIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            Quantidade = 10,
            Adicionar = true
        };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(movimentacaoLoteDto.ProdutoIds[0]))
                              .ReturnsAsync((Produto)null); // Primeiro produto não existe

        // Act
        var result = await _produtoService.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task MovimentarProdutosEmLoteAsync_ShouldUpdateQuantities_WhenAdicionarIsTrue()
    {
        // Arrange
        var produtoId1 = Guid.NewGuid();
        var produtoId2 = Guid.NewGuid();
        var produto1 = new Produto { Id = produtoId1, Quantidade = 5 };
        var produto2 = new Produto { Id = produtoId2, Quantidade = 3 };

        var movimentacaoLoteDto = new MovimentacaoLoteDto
        {
            ProdutoIds = new List<Guid> { produtoId1, produtoId2 },
            Quantidade = 10,
            Adicionar = true
        };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId1)).ReturnsAsync(produto1);
        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId2)).ReturnsAsync(produto2);
        _produtoRepositoryMock.Setup(repo => repo.UpdateAsync(produto1)).ReturnsAsync(true);
        _produtoRepositoryMock.Setup(repo => repo.UpdateAsync(produto2)).ReturnsAsync(true);

        // Act
        var result = await _produtoService.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto);

        // Assert
        Assert.True(result);
        Assert.Equal(15, produto1.Quantidade);
        Assert.Equal(13, produto2.Quantidade);
    }

    [Fact]
    public async Task MovimentarProdutosEmLoteAsync_ShouldReturnFalse_WhenRemovingQuantityCausesNegativeStock()
    {
        // Arrange
        var produtoId1 = Guid.NewGuid();
        var produto1 = new Produto { Id = produtoId1, Quantidade = 5 };

        var movimentacaoLoteDto = new MovimentacaoLoteDto
        {
            ProdutoIds = new List<Guid> { produtoId1 },
            Quantidade = 10,
            Adicionar = false
        };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId1)).ReturnsAsync(produto1);

        // Act
        var result = await _produtoService.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ObterValorTotalEstoqueAsync_ShouldReturnZero_WhenNoProdutosExist()
    {
        // Arrange
        _produtoRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Produto>());

        // Act
        var result = await _produtoService.ObterValorTotalEstoqueAsync();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ObterValorTotalEstoqueAsync_ShouldReturnTotalValue_WhenProdutosExist()
    {
        // Arrange
        var produtos = new List<Produto>
        {
            new Produto { PrecoCusto = 10, Quantidade = 2 }, // Total: 20
            new Produto { PrecoCusto = 20, Quantidade = 1 }  // Total: 20
        };

        _produtoRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(produtos);

        // Act
        var result = await _produtoService.ObterValorTotalEstoqueAsync();

        // Assert
        Assert.Equal(40, result);
    }

    [Fact]
    public async Task ObterQuantidadeTotalEstoqueAsync_ShouldReturnZero_WhenNoProdutosExist()
    {
        // Arrange
        _produtoRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Produto>());

        // Act
        var result = await _produtoService.ObterQuantidadeTotalEstoqueAsync();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ObterQuantidadeTotalEstoqueAsync_ShouldReturnTotalQuantity_WhenProdutosExist()
    {
        // Arrange
        var produtos = new List<Produto>
        {
            new Produto { Quantidade = 2 },
            new Produto { Quantidade = 3 }
        };

        _produtoRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(produtos);

        // Act
        var result = await _produtoService.ObterQuantidadeTotalEstoqueAsync();

        // Assert
        Assert.Equal(5, result);
    }

    [Fact]
    public async Task ObterCustoMedioProdutoAsync_ShouldReturnZero_WhenProdutoDoesNotExist()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId)).ReturnsAsync((Produto)null);

        // Act
        var result = await _produtoService.ObterCustoMedioProdutoAsync(produtoId);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ObterCustoMedioProdutoAsync_ShouldReturnPrecoCusto_WhenProdutoExists()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId, PrecoCusto = 15.50m };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId)).ReturnsAsync(produto);

        // Act
        var result = await _produtoService.ObterCustoMedioProdutoAsync(produtoId);

        // Assert
        Assert.Equal(15.50m, result);
    }

    [Fact]
    public async Task ObterCustoMedioEstoqueAsync_ShouldReturnZero_WhenNoProdutosExist()
    {
        // Arrange
        _produtoRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Produto>());

        // Act
        var result = await _produtoService.ObterCustoMedioEstoqueAsync();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ObterCustoMedioEstoqueAsync_ShouldReturnAverageCost_WhenProdutosExist()
    {
        // Arrange
        var produtos = new List<Produto>
        {
            new Produto { PrecoCusto = 10, Quantidade = 1 },
            new Produto { PrecoCusto = 20, Quantidade = 1 }
        };

        _produtoRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(produtos);

        // Act
        var result = await _produtoService.ObterCustoMedioEstoqueAsync();

        // Assert
        Assert.Equal(15, result); // Média: (10 + 20) / 2
    }
}
