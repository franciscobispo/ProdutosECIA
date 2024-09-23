namespace ProdutosECIA.Application.DTOs;

public class ProdutoUpdateDto
{
    public string Nome { get; set; }
    public decimal PrecoCusto { get; set; }
    public decimal PrecoVenda { get; set; }
    public int Quantidade { get; set; }
    public Guid EmpresaId { get; set; }
}