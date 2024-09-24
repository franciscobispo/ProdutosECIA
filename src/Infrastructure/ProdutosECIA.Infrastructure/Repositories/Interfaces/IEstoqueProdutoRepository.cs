using ProdutosECIA.Domain.Entities;

namespace ProdutosECIA.Infrastructure.Repositories.Interfaces;

public interface IEstoqueProdutoRepository : IGenericRepository<EstoqueProduto>
{
    Task<EstoqueProduto> GetByProdutoAndEmpresaAsync(Guid produtoId, Guid empresaId);
    Task<IEnumerable<EstoqueProduto>> GetAllByEmpresaAsync(Guid empresaId);
    Task<IEnumerable<EstoqueProduto>> GetAllByProdutoAsync(Guid produtoId);
    Task<EstoqueProduto> GetEstoqueProdutoAsync(Guid produtoId, Guid empresaId);
}