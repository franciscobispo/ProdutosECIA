namespace ProdutosECIA.Domain.Entities;

public class EstoqueProduto
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public Guid EmpresaId { get; set; }
    public int Quantidade { get; set; }

    public Produto Produto { get; set; }
    public Empresa Empresa { get; set; }
}