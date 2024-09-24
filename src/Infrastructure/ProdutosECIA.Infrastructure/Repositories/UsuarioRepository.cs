using Microsoft.EntityFrameworkCore;
using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.DataContext;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario> GetByUsernameAsync(string username)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task CreateUserAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }
}