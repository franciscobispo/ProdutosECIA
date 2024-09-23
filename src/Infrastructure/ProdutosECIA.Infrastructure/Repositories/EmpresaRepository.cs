using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.DataContext;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Infrastructure.Repositories;

public class EmpresaRepository : GenericRepository<Empresa>, IEmpresaRepository
{
    public EmpresaRepository(AppDbContext context) : base(context) { }
}