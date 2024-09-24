using Microsoft.EntityFrameworkCore;
using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.DataContext;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Infrastructure.Repositories;

public class EstoqueProdutoRepository : GenericRepository<EstoqueProduto>, IEstoqueProdutoRepository
{
    private readonly AppDbContext _context;

    public EstoqueProdutoRepository(AppDbContext context) : base(context) {
        _context = context;
    }

    public async Task<EstoqueProduto> GetByProdutoAndEmpresaAsync(Guid produtoId, Guid empresaId)
    {
        return await _context.EstoqueProdutos
            .FirstOrDefaultAsync(e => e.ProdutoId == produtoId && e.EmpresaId == empresaId);
    }

    public async Task<IEnumerable<EstoqueProduto>> GetAllByEmpresaAsync(Guid empresaId)
    {
        return await _context.EstoqueProdutos
            .Where(e => e.EmpresaId == empresaId)
            .ToListAsync();
    }

    public async Task<IEnumerable<EstoqueProduto>> GetAllByProdutoAsync(Guid produtoId)
    {
        return await _context.EstoqueProdutos
            .Where(e => e.ProdutoId == produtoId)
            .ToListAsync();
    }

    public async Task<EstoqueProduto> GetEstoqueProdutoAsync(Guid produtoId, Guid empresaId)
    {
        return await _context.EstoqueProdutos
            .FirstOrDefaultAsync(e => e.ProdutoId == produtoId && e.EmpresaId == empresaId);
    }
}