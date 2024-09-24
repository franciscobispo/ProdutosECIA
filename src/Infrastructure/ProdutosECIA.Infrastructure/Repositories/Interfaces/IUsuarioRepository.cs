using ProdutosECIA.Domain.Entities;

namespace ProdutosECIA.Infrastructure.Repositories.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario> GetByUsernameAsync(string username);
    Task CreateUserAsync(Usuario usuario);
}