namespace ProdutosECIA.Application.DTOs;

public class MovimentacaoLoteDto
{
    public List<Guid> ProdutoIds { get; set; }
    public int Quantidade { get; set; }
    public bool Adicionar { get; set; }
}