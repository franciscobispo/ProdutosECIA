namespace ProdutosECIA.Application.DTOs;

public class MovimentacaoProdutoDto
{
    public Guid EmpresaId { get; set; }
    public int Quantidade { get; set; }
    public bool Adicionar { get; set; } // Se for true, adiciona; se for false, remove
}