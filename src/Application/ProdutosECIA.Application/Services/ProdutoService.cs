using AutoMapper;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Interfaces;
using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;

    public ProdutoService(IProdutoRepository produtoRepository, IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProdutoDto>> GetAllAsync()
    {
        var produtos = await _produtoRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
    }

    public async Task<ProdutoDto?> GetByIdAsync(Guid id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        return _mapper.Map<ProdutoDto>(produto);
    }

    public async Task<ProdutoDto> CreateAsync(ProdutoCreateDto produtoCreateDto)
    {
        var produto = _mapper.Map<Produto>(produtoCreateDto);
        await _produtoRepository.AddAsync(produto);

        return _mapper.Map<ProdutoDto>(produto);
    }

    public async Task<bool> UpdateAsync(Guid id, ProdutoUpdateDto produtoUpdateDto)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null)
        {
            return false;
        }

        _mapper.Map(produtoUpdateDto, produto);
        return await _produtoRepository.UpdateAsync(produto);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _produtoRepository.DeleteAsync(id);
    }

    public async Task<bool> MovimentarProdutoAsync(Guid produtoId, MovimentacaoProdutoDto movimentacaoDto)
    {
        var produto = await _produtoRepository.GetByIdAsync(produtoId);
        if (produto == null) return false;

        if (movimentacaoDto.Adicionar)
        {
            produto.Quantidade += movimentacaoDto.Quantidade;
        }
        else
        {
            if (produto.Quantidade < movimentacaoDto.Quantidade)
            {
                return false; // Evitar que a quantidade fique negativa
            }
            produto.Quantidade -= movimentacaoDto.Quantidade;
        }

        return await _produtoRepository.UpdateAsync(produto);
    }

    public async Task<bool> MovimentarProdutosEmLoteAsync(MovimentacaoLoteDto movimentacaoLoteDto)
    {
        foreach (var produtoId in movimentacaoLoteDto.ProdutoIds)
        {
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            if (produto == null) return false;

            if (movimentacaoLoteDto.Adicionar)
            {
                produto.Quantidade += movimentacaoLoteDto.Quantidade;
            }
            else
            {
                if (produto.Quantidade < movimentacaoLoteDto.Quantidade)
                {
                    return false; // Evitar que a quantidade fique negativa
                }
                produto.Quantidade -= movimentacaoLoteDto.Quantidade;
            }

            await _produtoRepository.UpdateAsync(produto);
        }

        return true;
    }

    public async Task<decimal> ObterValorTotalEstoqueAsync()
    {
        var produtos = await _produtoRepository.GetAllAsync();
        if (produtos == null || !produtos.Any()) return 0;

        return produtos.Sum(p => p.PrecoCusto * p.Quantidade);
    }

    public async Task<int> ObterQuantidadeTotalEstoqueAsync()
    {
        var produtos = await _produtoRepository.GetAllAsync();
        if (produtos == null || !produtos.Any()) return 0;

        return produtos.Sum(p => p.Quantidade);
    }

    public async Task<decimal> ObterCustoMedioProdutoAsync(Guid produtoId)
    {
        var produto = await _produtoRepository.GetByIdAsync(produtoId);
        if (produto == null) return 0;

        return produto.PrecoCusto;
    }

    public async Task<decimal> ObterCustoMedioEstoqueAsync()
    {
        var produtos = await _produtoRepository.GetAllAsync();
        if (produtos == null || !produtos.Any()) return 0;

        return produtos.Average(p => p.PrecoCusto);
    }
}