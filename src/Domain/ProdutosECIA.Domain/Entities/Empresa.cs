namespace ProdutosECIA.Domain.Entities;

public class Empresa
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string CNPJ { get; set; }

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}