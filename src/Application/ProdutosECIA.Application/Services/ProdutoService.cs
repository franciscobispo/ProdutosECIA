using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Services.Interfaces;
using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IEstoqueProdutoRepository _estoqueProdutoRepository;
    private readonly IMapper _mapper;

    public ProdutoService(IProdutoRepository produtoRepository, IEstoqueProdutoRepository estoqueProdutoRepository, IEmpresaRepository empresaRepository, IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _estoqueProdutoRepository = estoqueProdutoRepository;
        _empresaRepository = empresaRepository;
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
        // Verificar se o Produto existe
        var produto = await _produtoRepository.GetByIdAsync(produtoId);
        if (produto == null)
        {
            throw new ArgumentException("Produto não localizado");
        }

        // Verificar se a Empresa existe
        var empresa = await _empresaRepository.GetByIdAsync(movimentacaoDto.EmpresaId);
        if (empresa == null)
        {
            throw new ArgumentException("Empresa não localizada");
        }

        // Obtém o estoque do produto para a empresa especificada
        var estoque = await _estoqueProdutoRepository.GetEstoqueProdutoAsync(produtoId, movimentacaoDto.EmpresaId);

        if (estoque == null)
        {
            // Se o estoque não existir para esse produto e empresa, criar uma nova entrada na tabela EstoqueProduto
            var novoEstoqueProduto = new EstoqueProduto
            {
                Id = Guid.NewGuid(),
                ProdutoId = produtoId,
                EmpresaId = movimentacaoDto.EmpresaId,
                Quantidade = movimentacaoDto.Quantidade
            };

            // Adicionar ao repositório de estoque
            await _estoqueProdutoRepository.AddAsync(novoEstoqueProduto);
            return true;
        }

        if (movimentacaoDto.Adicionar)
        {
            // Adiciona a quantidade solicitada ao estoque
            estoque.Quantidade += movimentacaoDto.Quantidade;
        }
        else
        {
            // Verifica se há estoque suficiente antes de remover a quantidade
            if (estoque.Quantidade < movimentacaoDto.Quantidade)
            {
                // Evitar que a quantidade fique negativa
                throw new ArgumentException("Não permitido adicionar uma quantidade que fará com que o estoqudo produto fique com valor negativo.");
            }

            estoque.Quantidade -= movimentacaoDto.Quantidade;
        }

        // Atualiza o estoque no repositório
        return await _estoqueProdutoRepository.UpdateAsync(estoque);
    }

    public async Task<bool> MovimentarProdutosEmLoteAsync(MovimentacaoLoteDto movimentacaoLoteDto)
    {
        foreach (var item in movimentacaoLoteDto.Items)
        {
            // Obtém o estoque do produto para a empresa especificada
            var estoque = await _estoqueProdutoRepository.GetEstoqueProdutoAsync(item.ProdutoId, item.EmpresaId);

            if (estoque == null)
            {
                // Se o estoque não existir para esse produto e empresa, retornar falso
                return false;
            }

            if (item.Adicionar)
            {
                // Adiciona a quantidade solicitada ao estoque
                estoque.Quantidade += item.Quantidade;
            }
            else
            {
                // Verifica se há estoque suficiente antes de remover a quantidade
                if (estoque.Quantidade < item.Quantidade)
                {
                    return false; // Evitar que a quantidade fique negativa
                }

                estoque.Quantidade -= item.Quantidade;
            }

            // Atualiza o estoque no repositório
            await _estoqueProdutoRepository.UpdateAsync(estoque);
        }

        return true;
    }

    public async Task<decimal> ObterValorTotalEstoqueAsync()
    {
        var estoques = await _estoqueProdutoRepository.GetAllAsync(query => query.Include(e => e.Produto));
        return estoques.Sum(e => e.Produto.PrecoCusto * e.Quantidade);
    }

    public async Task<int> ObterQuantidadeTotalProdutoAsync(Guid produtoId, Guid empresaId)
    {
        var estoque = await _estoqueProdutoRepository.GetEstoqueProdutoAsync(produtoId, empresaId);

        return estoque?.Quantidade ?? 0;
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

    public async Task<bool> TransferirProdutoAsync(Guid produtoId, Guid deEmpresaId, Guid paraEmpresaId, int quantidade)
    {
        // Validar que as empresas e o produto existem

        // Remover do estoque da empresa de origem
        var estoqueOrigem = await _estoqueProdutoRepository.GetByProdutoAndEmpresaAsync(produtoId, deEmpresaId);
        if (estoqueOrigem == null || estoqueOrigem.Quantidade < quantidade) return false;

        estoqueOrigem.Quantidade -= quantidade;
        await _estoqueProdutoRepository.UpdateAsync(estoqueOrigem);

        // Adicionar no estoque da empresa de destino
        var estoqueDestino = await _estoqueProdutoRepository.GetByProdutoAndEmpresaAsync(produtoId, paraEmpresaId);
        if (estoqueDestino == null)
        {
            // Se não existir, criar um novo
            estoqueDestino = new EstoqueProduto { ProdutoId = produtoId, EmpresaId = paraEmpresaId, Quantidade = quantidade };
            await _estoqueProdutoRepository.AddAsync(estoqueDestino);
        }
        else
        {
            estoqueDestino.Quantidade += quantidade;
            await _estoqueProdutoRepository.UpdateAsync(estoqueDestino);
        }

        return true;
    }
}