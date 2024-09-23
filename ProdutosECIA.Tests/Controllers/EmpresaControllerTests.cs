using Microsoft.AspNetCore.Mvc;
using Moq;
using ProdutosECIA.API.Controllers;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Interfaces;

namespace ProdutosECIA.Tests.Controllers;

public class EmpresaControllerTests
{
    private readonly Mock<IEmpresaService> _empresaServiceMock;
    private readonly EmpresaController _empresaController;

    public EmpresaControllerTests()
    {
        _empresaServiceMock = new Mock<IEmpresaService>();
        _empresaController = new EmpresaController(_empresaServiceMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WhenEmpresasExist()
    {
        // Arrange
        var empresas = new List<EmpresaDto>
        {
            new EmpresaDto { Id = Guid.NewGuid(), Nome = "Empresa 1" },
            new EmpresaDto { Id = Guid.NewGuid(), Nome = "Empresa 2" }
        };

        _empresaServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(empresas);

        // Act
        var result = await _empresaController.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<EmpresaDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenEmpresaDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _empresaServiceMock.Setup(service => service.GetByIdAsync(id)).ReturnsAsync((EmpresaDto)null);

        // Act
        var result = await _empresaController.Get(id);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Post_ShouldReturnCreated_WhenEmpresaIsCreated()
    {
        // Arrange
        var empresaCreateDto = new EmpresaCreateDto { Nome = "Nova Empresa" };
        var empresaDto = new EmpresaDto { Id = Guid.NewGuid(), Nome = "Nova Empresa" };

        _empresaServiceMock.Setup(service => service.CreateAsync(empresaCreateDto)).ReturnsAsync(empresaDto);

        // Act
        var result = await _empresaController.Post(empresaCreateDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("Get", createdResult.ActionName);
        Assert.Equal(empresaDto.Id, createdResult.RouteValues["id"]);
        Assert.Equal(empresaDto, createdResult.Value);
    }

    [Fact]
    public async Task Put_ShouldReturnOk_WhenEmpresaIsUpdated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var empresaUpdateDto = new EmpresaUpdateDto { Nome = "Empresa Atualizada" };
        _empresaServiceMock.Setup(service => service.UpdateAsync(id, empresaUpdateDto)).ReturnsAsync(true);

        // Act
        var result = await _empresaController.Put(id, empresaUpdateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(true, okResult.Value);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenEmpresaDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var empresaUpdateDto = new EmpresaUpdateDto { Nome = "Empresa Atualizada" };
        _empresaServiceMock.Setup(service => service.UpdateAsync(id, empresaUpdateDto)).ReturnsAsync(false);

        // Act
        var result = await _empresaController.Put(id, empresaUpdateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(false, okResult.Value);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenEmpresaIsDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _empresaServiceMock.Setup(service => service.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _empresaController.Delete(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenEmpresaDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _empresaServiceMock.Setup(service => service.DeleteAsync(id)).ReturnsAsync(false);

        // Act
        var result = await _empresaController.Delete(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
