using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ProdutosECIA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await _authService.LoginAsync(loginDto);

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "NOTA: Somente usuários administradores podem registrar novos usuários.")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        await _authService.RegisterAsync(registerDto);
        return Ok(new { Message = "User registered successfully" });
    }
}