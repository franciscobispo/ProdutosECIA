namespace ProdutosECIA.Application.DTOs;

public class MovimentacaoLoteDto
{
    //public List<Guid> ProdutoIds { get; set; }
    //public int Quantidade { get; set; }
    //public bool Adicionar { get; set; }
    public List<MovimentacaoLoteItemDto> Items { get; set; }
}

public class MovimentacaoLoteItemDto
{
    public Guid ProdutoId { get; set; }
    public Guid EmpresaId { get; set; }
    public int Quantidade { get; set; }
    public bool Adicionar { get; set; }
}