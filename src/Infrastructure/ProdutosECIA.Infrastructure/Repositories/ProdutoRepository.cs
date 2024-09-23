using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.DataContext;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Infrastructure.Repositories;

public class ProdutoRepository : GenericRepository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context) { }
}