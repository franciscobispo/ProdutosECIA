namespace ProdutosECIA.Application.DTOs;

public class ProdutoCreateDto
{
    public string Nome { get; set; }
    public decimal PrecoCusto { get; set; }
    public decimal PrecoVenda { get; set; }
    public int Quantidade { get; set; }
    public Guid EmpresaId { get; set; }
}