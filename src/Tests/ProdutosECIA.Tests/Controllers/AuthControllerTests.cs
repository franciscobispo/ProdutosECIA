using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProdutosECIA.API.Controllers;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Services.Interfaces;

namespace ProdutosECIA.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenValidCredentials()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "admin", Password = "password" };
        var token = "validToken";
        _authServiceMock.Setup(x => x.LoginAsync(loginDto)).ReturnsAsync(token);

        // Act
        var result = await _authController.Login(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(new { Token = token });
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "admin", Password = "wrongPassword" };
        _authServiceMock.Setup(x => x.LoginAsync(loginDto)).ReturnsAsync(string.Empty);

        // Act
        var result = await _authController.Login(loginDto);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenCalledByAdmin()
    {
        // Arrange
        var registerDto = new RegisterDto { Username = "newUser", Password = "password" };
        _authServiceMock.Setup(x => x.RegisterAsync(registerDto)).Returns(Task.CompletedTask);

        // Act
        var result = await _authController.Register(registerDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(new { Message = "User registered successfully" });
    }
}