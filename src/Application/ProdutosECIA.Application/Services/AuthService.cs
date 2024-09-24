using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Services.Interfaces;
using ProdutosECIA.Application.Settings;
using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProdutosECIA.Application.Services;

//public class AuthService : IAuthService
//{
//    private readonly IUsuarioRepository _usuarioRepository;
//    private readonly AuthSettings _authSettings;

//    public AuthService(IUsuarioRepository usuarioRepository, IOptions<AuthSettings> authSettings)
//    {
//        _usuarioRepository = usuarioRepository;
//        _authSettings = authSettings.Value;
//    }

//    public async Task<string> LoginAsync(LoginDto loginDto)
//    {
//        var usuario = await _usuarioRepository.GetByUsernameAsync(loginDto.Username);

//        if (usuario == null || !VerifyPassword(loginDto.Password, usuario.PasswordHash))
//        {
//            return null;
//        }

//        var tokenHandler = new JwtSecurityTokenHandler();
//        var key = Encoding.ASCII.GetBytes(_authSettings.Secret);
//        var tokenDescriptor = new SecurityTokenDescriptor
//        {
//            Subject = new ClaimsIdentity(new Claim[]
//            {
//                new Claim(ClaimTypes.Name, usuario.Username),
//                new Claim(ClaimTypes.Role, usuario.Role)
//            }),
//            Expires = DateTime.UtcNow.AddHours(1),
//            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//        };
//        var token = tokenHandler.CreateToken(tokenDescriptor);
//        return tokenHandler.WriteToken(token);
//    }

//    public async Task RegisterAsync(RegisterDto registerDto)
//    {
//        var passwordHash = HashPassword(registerDto.Password);
//        var usuario = new Usuario
//        {
//            Username = registerDto.Username,
//            PasswordHash = passwordHash,
//            Role = "User"
//        };

//        await _usuarioRepository.CreateUserAsync(usuario);
//    }

//    private string HashPassword(string password)
//    {
//        return BCrypt.Net.BCrypt.HashPassword(password);
//    }

//    private bool VerifyPassword(string password, string passwordHash)
//    {
//        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
//    }
//}

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly AuthSettings _authSettings;

    public AuthService(IUsuarioRepository usuarioRepository, IOptions<AuthSettings> authSettings)
    {
        _usuarioRepository = usuarioRepository;
        _authSettings = authSettings.Value;
    }

    public async Task<string> LoginAsync(LoginDto loginDto)
    {
        var usuario = await _usuarioRepository.GetByUsernameAsync(loginDto.Username);

        if (usuario == null || !VerifyPassword(loginDto.Password, usuario.PasswordHash))
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_authSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, usuario.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task RegisterAsync(RegisterDto registerDto)
    {
        var passwordHash = HashPassword(registerDto.Password);
        var usuario = new Usuario
        {
            Username = registerDto.Username,
            PasswordHash = passwordHash,
            Role = "User"
        };

        await _usuarioRepository.CreateUserAsync(usuario);
    }

    // Use BCrypt to hash the password
    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Use BCrypt to verify the password
    private bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
