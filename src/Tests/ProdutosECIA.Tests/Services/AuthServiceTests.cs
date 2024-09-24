using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Services;
using ProdutosECIA.Application.Settings;
using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace ProdutosECIA.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IOptions<AuthSettings>> _authSettingsMock;
    private readonly AuthService _authService;
    private readonly AuthSettings _authSettings;

    public AuthServiceTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _authSettingsMock = new Mock<IOptions<AuthSettings>>();
        _authSettings = new AuthSettings { Secret = "FBCVt8XbM2aPzR4cN7yLqW1uZ5vXs9QhTEST" };
        _authSettingsMock.Setup(a => a.Value).Returns(_authSettings);

        _authService = new AuthService(_usuarioRepositoryMock.Object, _authSettingsMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password123" };
        var usuario = new Usuario
        {
            Username = loginDto.Username,
            PasswordHash = HashPassword("password123"),
            Role = "User"
        };

        _usuarioRepositoryMock.Setup(x => x.GetByUsernameAsync(loginDto.Username)).ReturnsAsync(usuario);

        // Act
        var token = await _authService.LoginAsync(loginDto);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        jwtToken.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == loginDto.Username);
        jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "User");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "invaliduser", Password = "password123" };

        _usuarioRepositoryMock.Setup(x => x.GetByUsernameAsync(loginDto.Username)).ReturnsAsync((Usuario)null);

        // Act
        var token = await _authService.LoginAsync(loginDto);

        // Assert
        token.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsInvalid()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "wrongpassword" };
        var usuario = new Usuario
        {
            Username = loginDto.Username,
            PasswordHash = HashPassword("password123"),
            Role = "User"
        };

        _usuarioRepositoryMock.Setup(x => x.GetByUsernameAsync(loginDto.Username)).ReturnsAsync(usuario);

        // Act
        var token = await _authService.LoginAsync(loginDto);

        // Assert
        token.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WithHashedPassword()
    {
        // Arrange
        var registerDto = new RegisterDto { Username = "newuser", Password = "password123" };

        _usuarioRepositoryMock.Setup(x => x.CreateUserAsync(It.IsAny<Usuario>()))
            .Returns(Task.CompletedTask);

        // Act
        await _authService.RegisterAsync(registerDto);

        // Assert
        _usuarioRepositoryMock.Verify(x => x.CreateUserAsync(It.Is<Usuario>(u =>
            u.Username == registerDto.Username &&
            u.Role == "User" &&
            !string.IsNullOrEmpty(u.PasswordHash)
        )), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCallRepository_ToCreateUser()
    {
        // Arrange
        var registerDto = new RegisterDto { Username = "newuser", Password = "password123" };

        _usuarioRepositoryMock.Setup(x => x.CreateUserAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask).Verifiable();

        // Act
        await _authService.RegisterAsync(registerDto);

        // Assert
        _usuarioRepositoryMock.Verify(x => x.CreateUserAsync(It.IsAny<Usuario>()), Times.Once);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
