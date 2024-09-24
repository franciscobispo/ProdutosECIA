namespace ProdutosECIA.Application.DTOs;

public class ProdutoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public decimal PrecoCusto { get; set; }
    public decimal PrecoVenda { get; set; }
    //public int Quantidade { get; set; }
}