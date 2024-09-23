namespace ProdutosECIA.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public decimal PrecoCusto { get; set; }
    public decimal PrecoVenda { get; set; }
    public int Quantidade { get; set; }
    public Guid EmpresaId { get; set; }

    public Empresa Empresa { get; set; }
}