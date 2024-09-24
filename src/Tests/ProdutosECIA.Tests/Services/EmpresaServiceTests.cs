using AutoMapper;
using FluentAssertions;
using Moq;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Services;
using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Tests.Services;

public class EmpresaServiceTests
{
    private readonly Mock<IEmpresaRepository> _empresaRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly EmpresaService _empresaService;

    public EmpresaServiceTests()
    {
        _empresaRepositoryMock = new Mock<IEmpresaRepository>();
        _mapperMock = new Mock<IMapper>();
        _empresaService = new EmpresaService(_empresaRepositoryMock.Object, _mapperMock.Object);
    }


    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEmpresas()
    {
        // Arrange
        var empresas = new List<Empresa> { new Empresa(), new Empresa() };

        _empresaRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<Func<IQueryable<Empresa>, IQueryable<Empresa>>>()))
            .ReturnsAsync(empresas);

        var empresasDto = new List<EmpresaDto> { new EmpresaDto(), new EmpresaDto() };
        _mapperMock.Setup(m => m.Map<IEnumerable<EmpresaDto>>(empresas)).Returns(empresasDto);

        // Act
        var result = await _empresaService.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(empresasDto);

        _empresaRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<Func<IQueryable<Empresa>, IQueryable<Empresa>>>()), Times.Once);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnEmpresa_WhenEmpresaExists()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var empresa = new Empresa { Id = empresaId };
        var empresaDto = new EmpresaDto { Id = empresaId };

        _empresaRepositoryMock.Setup(repo => repo.GetByIdAsync(empresaId)).ReturnsAsync(empresa);
        _mapperMock.Setup(m => m.Map<EmpresaDto>(empresa)).Returns(empresaDto);

        // Act
        var result = await _empresaService.GetByIdAsync(empresaId);

        // Assert
        result.Should().BeEquivalentTo(empresaDto);
        _empresaRepositoryMock.Verify(repo => repo.GetByIdAsync(empresaId), Times.Once);
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
        result.Should().BeNull();
        _empresaRepositoryMock.Verify(repo => repo.GetByIdAsync(empresaId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedEmpresaDto()
    {
        // Arrange
        var empresaCreateDto = new EmpresaCreateDto { Nome = "Empresa Teste" };
        var empresa = new Empresa { Nome = "Empresa Teste" };
        var empresaDto = new EmpresaDto { Nome = "Empresa Teste" };

        _mapperMock.Setup(m => m.Map<Empresa>(empresaCreateDto)).Returns(empresa);
        _empresaRepositoryMock.Setup(repo => repo.AddAsync(empresa)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<EmpresaDto>(empresa)).Returns(empresaDto);

        // Act
        var result = await _empresaService.CreateAsync(empresaCreateDto);

        // Assert
        result.Should().BeEquivalentTo(empresaDto);
        _empresaRepositoryMock.Verify(repo => repo.AddAsync(empresa), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenEmpresaExists()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var empresaUpdateDto = new EmpresaUpdateDto { Nome = "Empresa Atualizada" };
        var empresa = new Empresa { Id = empresaId };

        _empresaRepositoryMock.Setup(repo => repo.GetByIdAsync(empresaId)).ReturnsAsync(empresa);
        _empresaRepositoryMock.Setup(repo => repo.UpdateAsync(empresa)).ReturnsAsync(true);

        // Act
        var result = await _empresaService.UpdateAsync(empresaId, empresaUpdateDto);

        // Assert
        result.Should().BeTrue();
        _empresaRepositoryMock.Verify(repo => repo.UpdateAsync(empresa), Times.Once);
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
        result.Should().BeFalse();
        _empresaRepositoryMock.Verify(repo => repo.GetByIdAsync(empresaId), Times.Once);
        _empresaRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Empresa>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenEmpresaIsDeleted()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        _empresaRepositoryMock.Setup(repo => repo.DeleteAsync(empresaId)).ReturnsAsync(true);

        // Act
        var result = await _empresaService.DeleteAsync(empresaId);

        // Assert
        result.Should().BeTrue();
        _empresaRepositoryMock.Verify(repo => repo.DeleteAsync(empresaId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenEmpresaDoesNotExist()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        _empresaRepositoryMock.Setup(repo => repo.DeleteAsync(empresaId)).ReturnsAsync(false);

        // Act
        var result = await _empresaService.DeleteAsync(empresaId);

        // Assert
        result.Should().BeFalse();
        _empresaRepositoryMock.Verify(repo => repo.DeleteAsync(empresaId), Times.Once);
    }
}
