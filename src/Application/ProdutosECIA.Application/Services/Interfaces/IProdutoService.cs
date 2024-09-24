using ProdutosECIA.Application.DTOs;

namespace ProdutosECIA.Application.Services.Interfaces;

public interface IProdutoService
{
    Task<IEnumerable<ProdutoDto>> GetAllAsync();
    Task<ProdutoDto?> GetByIdAsync(Guid id);
    Task<ProdutoDto> CreateAsync(ProdutoCreateDto produtoCreateDto);
    Task<bool> UpdateAsync(Guid id, ProdutoUpdateDto produtoUpdateDto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> MovimentarProdutoAsync(Guid produtoId, MovimentacaoProdutoDto movimentacaoDto);
    Task<bool> MovimentarProdutosEmLoteAsync(MovimentacaoLoteDto movimentacaoLoteDto);
    Task<decimal> ObterValorTotalEstoqueAsync();
    Task<int> ObterQuantidadeTotalProdutoAsync(Guid produtoId, Guid empresaId);
    Task<decimal> ObterCustoMedioProdutoAsync(Guid produtoId);
    Task<decimal> ObterCustoMedioEstoqueAsync();
    Task<bool> TransferirProdutoAsync(Guid produtoId, Guid deEmpresaId, Guid paraEmpresaId, int quantidade);
}