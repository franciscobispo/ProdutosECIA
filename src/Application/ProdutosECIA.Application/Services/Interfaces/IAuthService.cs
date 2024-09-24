using ProdutosECIA.Application.DTOs;

namespace ProdutosECIA.Application.Services.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(LoginDto loginDto);
    Task RegisterAsync(RegisterDto registerDto);
}