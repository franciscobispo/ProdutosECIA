using AutoMapper;
using FluentAssertions;
using Moq;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Services;
using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Tests.Services;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly Mock<IEstoqueProdutoRepository> _estoqueProdutoRepositoryMock;
    private readonly Mock<IEmpresaRepository> _empresaRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProdutoService _produtoService;

    public ProdutoServiceTests()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _estoqueProdutoRepositoryMock = new Mock<IEstoqueProdutoRepository>();
        _empresaRepositoryMock = new Mock<IEmpresaRepository>();
        _mapperMock = new Mock<IMapper>();

        _produtoService = new ProdutoService(
            _produtoRepositoryMock.Object,
            _estoqueProdutoRepositoryMock.Object,
            _empresaRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProdutos()
    {
        // Arrange
        var produtos = new List<Produto> { new Produto(), new Produto() };

        _produtoRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<Func<IQueryable<Produto>, IQueryable<Produto>>>()))
            .ReturnsAsync(produtos);

        var produtoDtos = new List<ProdutoDto> { new ProdutoDto(), new ProdutoDto() };
        _mapperMock.Setup(m => m.Map<IEnumerable<ProdutoDto>>(produtos)).Returns(produtoDtos);

        // Act
        var result = await _produtoService.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(produtoDtos);
        _produtoRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<Func<IQueryable<Produto>, IQueryable<Produto>>>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduto_WhenProdutoExists()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId };
        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId))
            .ReturnsAsync(produto);

        var produtoDto = new ProdutoDto();
        _mapperMock.Setup(m => m.Map<ProdutoDto>(produto)).Returns(produtoDto);

        // Act
        var result = await _produtoService.GetByIdAsync(produtoId);

        // Assert
        result.Should().BeEquivalentTo(produtoDto);
        _produtoRepositoryMock.Verify(repo => repo.GetByIdAsync(produtoId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnProdutoDto()
    {
        // Arrange
        var produtoCreateDto = new ProdutoCreateDto();
        var produto = new Produto();
        _mapperMock.Setup(m => m.Map<Produto>(produtoCreateDto)).Returns(produto);

        var produtoDto = new ProdutoDto();
        _mapperMock.Setup(m => m.Map<ProdutoDto>(produto)).Returns(produtoDto);

        // Act
        var result = await _produtoService.CreateAsync(produtoCreateDto);

        // Assert
        result.Should().BeEquivalentTo(produtoDto);
        _produtoRepositoryMock.Verify(repo => repo.AddAsync(produto), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduto_WhenProdutoExists()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var produtoExistente = new Produto { Id = produtoId, Nome = "Produto Original" };
        var produtoUpdateDto = new ProdutoUpdateDto { Nome = "Produto Atualizado" };

        _produtoRepositoryMock
            .Setup(repo => repo.GetByIdAsync(produtoId))
            .ReturnsAsync(produtoExistente);

        _produtoRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Produto>()))
            .ReturnsAsync(true);

        // Act
        var result = await _produtoService.UpdateAsync(produtoId, produtoUpdateDto);

        // Assert
        result.Should().BeTrue();
        _produtoRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Produto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenProdutoDoesNotExist()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId))
            .ReturnsAsync((Produto)null);

        var produtoUpdateDto = new ProdutoUpdateDto();

        // Act
        var result = await _produtoService.UpdateAsync(produtoId, produtoUpdateDto);

        // Assert
        result.Should().BeFalse();
        _produtoRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProduto_WhenProdutoExists()
    {
        // Arrange
        var produtoId = Guid.NewGuid();

        _produtoRepositoryMock.Setup(repo => repo.DeleteAsync(produtoId))
            .ReturnsAsync(true);

        // Act
        var result = await _produtoService.DeleteAsync(produtoId);

        // Assert
        result.Should().BeTrue();
        _produtoRepositoryMock.Verify(repo => repo.DeleteAsync(produtoId), Times.Once);
    }

    [Fact]
    public async Task MovimentarProdutoAsync_ShouldAddQuantidade_WhenEstoqueExistsAndAdicionarIsTrue()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var empresaId = Guid.NewGuid();
        var estoqueExistente = new EstoqueProduto { ProdutoId = produtoId, EmpresaId = empresaId, Quantidade = 10 };
        var movimentacaoDto = new MovimentacaoProdutoDto { EmpresaId = empresaId, Quantidade = 5, Adicionar = true };

        _produtoRepositoryMock
            .Setup(repo => repo.GetByIdAsync(produtoId))
            .ReturnsAsync(new Produto { Id = produtoId });

        _empresaRepositoryMock
            .Setup(repo => repo.GetByIdAsync(empresaId))
            .ReturnsAsync(new Empresa { Id = empresaId });

        _estoqueProdutoRepositoryMock
            .Setup(repo => repo.GetEstoqueProdutoAsync(produtoId, empresaId))
            .ReturnsAsync(estoqueExistente);

        _estoqueProdutoRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<EstoqueProduto>()))
            .ReturnsAsync(true);

        // Act
        var result = await _produtoService.MovimentarProdutoAsync(produtoId, movimentacaoDto);

        // Assert
        result.Should().BeTrue();
        _estoqueProdutoRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<EstoqueProduto>(e => e.Quantidade == 15)), Times.Once);
    }

    [Fact]
    public async Task MovimentarProdutoAsync_ShouldThrowException_WhenProdutoDoesNotExist()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var movimentacaoDto = new MovimentacaoProdutoDto();

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId))
            .ReturnsAsync((Produto)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _produtoService.MovimentarProdutoAsync(produtoId, movimentacaoDto));
    }

    [Fact]
    public async Task MovimentarProdutoAsync_ShouldThrowException_WhenEmpresaDoesNotExist()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var empresaId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId };
        var movimentacaoDto = new MovimentacaoProdutoDto { EmpresaId = empresaId };

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoId))
            .ReturnsAsync(produto);
        _empresaRepositoryMock.Setup(repo => repo.GetByIdAsync(empresaId))
            .ReturnsAsync((Empresa)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _produtoService.MovimentarProdutoAsync(produtoId, movimentacaoDto));
    }
}
