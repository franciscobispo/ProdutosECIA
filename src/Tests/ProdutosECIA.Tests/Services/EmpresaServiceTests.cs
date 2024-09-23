using AutoMapper;
using Moq;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Interfaces;
using ProdutosECIA.Application.Services;
using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Tests.Services;

public class EmpresaServiceTests
{
    private readonly Mock<IEmpresaRepository> _empresaRepositoryMock;
    private readonly IEmpresaService _empresaService;
    private readonly Mock<IMapper> _mapperMock;

    public EmpresaServiceTests()
    {
        _empresaRepositoryMock = new Mock<IEmpresaRepository>();
        _mapperMock = new Mock<IMapper>();
        _empresaService = new EmpresaService(_empresaRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllEmpresas_ShouldReturnListOfEmpresas_WhenProdutosExist()
    {
        // Arrange
        var empresas = new List<Empresa>
        {
            new Empresa { Id = Guid.NewGuid(), Nome = "Empresa 1", CNPJ = "78.402.669/0001-05" },
            new Empresa { Id = Guid.NewGuid(), Nome = "Empresa 2", CNPJ = "81.913.761/0001-36" }
        };

        var empresasDto = empresas.Select(e => new EmpresaDto { Nome = e.Nome, CNPJ = e.CNPJ });

        _empresaRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(empresas);
        _mapperMock.Setup(m => m.Map<IEnumerable<EmpresaDto>>(empresas)).Returns(empresasDto);

        // Act
        var result = await _empresaService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _empresaRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<EmpresaDto>>(empresas), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEmpresa_WhenEmpresaExists()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var empresa = new Empresa { Id = empresaId, Nome = "Empresa Teste", CNPJ = "78.402.669/0001-05" };
        var empresaDto = new EmpresaDto { Id = empresaId, Nome = empresa.Nome, CNPJ = empresa.CNPJ };

        _empresaRepositoryMock.Setup(repo => repo.GetByIdAsync(empresaId)).ReturnsAsync(empresa);
        _mapperMock.Setup(m => m.Map<EmpresaDto>(empresa)).Returns(empresaDto);

        // Act
        var result = await _empresaService.GetByIdAsync(empresaId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(empresaId, result.Id);
        Assert.Equal(empresa.Nome, result.Nome);
        _empresaRepositoryMock.Verify(repo => repo.GetByIdAsync(empresaId), Times.Once);
        _mapperMock.Verify(m => m.Map<EmpresaDto>(empresa), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEmpresaDoesNotExist()
    {
        // Arrange
        var empresaId = Guid.NewGuid();

        _empresaRepositoryMock.Setup(repo => repo.GetByIdAsync(empresaId)).ReturnsAsync((Empresa)null);

        // Act
        var result = await _empresaService.GetByIdAsync(empresaId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnEmpresaDto_WhenEmpresaIsCreated()
    {
        // Arrange
        var empresaCreateDto = new EmpresaCreateDto { Nome = "Nova Empresa" };
        var empresa = new Empresa { Id = Guid.NewGuid(), Nome = "Nova Empresa" };

        _mapperMock.Setup(m => m.Map<Empresa>(empresaCreateDto)).Returns(empresa);
        _empresaRepositoryMock.Setup(repo => repo.AddAsync(empresa)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<EmpresaDto>(empresa)).Returns(new EmpresaDto { Id = empresa.Id, Nome = empresa.Nome });

        // Act
        var result = await _empresaService.CreateAsync(empresaCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(empresa.Id, result.Id);
        _empresaRepositoryMock.Verify(repo => repo.AddAsync(empresa), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEmpresa_WhenEmpresaExists()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var empresa = new Empresa { Id = empresaId, Nome = "Empresa Antiga" };
        var empresaUpdateDto = new EmpresaUpdateDto { Nome = "Empresa Atualizada" };

        _empresaRepositoryMock.Setup(repo => repo.GetByIdAsync(empresaId)).ReturnsAsync(empresa);
        _empresaRepositoryMock.Setup(repo => repo.UpdateAsync(empresa)).ReturnsAsync(true);

        // Act
        var result = await _empresaService.UpdateAsync(empresaId, empresaUpdateDto);

        // Assert
        Assert.True(result);
        _mapperMock.Verify(m => m.Map(empresaUpdateDto, empresa), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenEmpresaDoesNotExist()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var empresaUpdateDto = new EmpresaUpdateDto { Nome = "Empresa Atualizada" };

        _empresaRepositoryMock.Setup(repo => repo.GetByIdAsync(empresaId)).ReturnsAsync((Empresa)null);

        // Act
        var result = await _empresaService.UpdateAsync(empresaId, empresaUpdateDto);

        // Assert
        Assert.False(result);
        _empresaRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Empresa>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenEmpresaDeleted()
    {
        // Arrange
        var empresaId = Guid.NewGuid();

        _empresaRepositoryMock.Setup(repo => repo.DeleteAsync(empresaId)).ReturnsAsync(true);

        // Act
        var result = await _empresaService.DeleteAsync(empresaId);

        // Assert
        Assert.True(result);
        _empresaRepositoryMock.Verify(repo => repo.DeleteAsync(empresaId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenEmpresaNotDeleted()
    {
        // Arrange
        var empresaId = Guid.NewGuid();

        _empresaRepositoryMock.Setup(repo => repo.DeleteAsync(empresaId)).ReturnsAsync(false);

        // Act
        var result = await _empresaService.DeleteAsync(empresaId);

        // Assert
        Assert.False(result);
        _empresaRepositoryMock.Verify(repo => repo.DeleteAsync(empresaId), Times.Once);
    }
}
