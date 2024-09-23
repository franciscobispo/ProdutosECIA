using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Interfaces;

namespace ProdutosECIA.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutoController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    // GET: api/produto
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAll()
    {
        var produtos = await _produtoService.GetAllAsync();
        return Ok(produtos);
    }

    // GET: api/produto/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoDto>> GetById(Guid id)
    {
        var produto = await _produtoService.GetByIdAsync(id);
        if (produto == null)
        {
            return NotFound();
        }
        return Ok(produto);
    }

    // POST: api/produto
    [HttpPost]
    public async Task<ActionResult<ProdutoDto>> Create([FromBody] ProdutoCreateDto produtoCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdProduto = await _produtoService.CreateAsync(produtoCreateDto);
        return CreatedAtAction(nameof(GetById), new { id = createdProduto.Id }, createdProduto);
    }

    // PUT: api/produto/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProdutoUpdateDto produtoUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var isUpdated = await _produtoService.UpdateAsync(id, produtoUpdateDto);
        if (!isUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/produto/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var isDeleted = await _produtoService.DeleteAsync(id);
        if (!isDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{produtoId}/movimentar")]
    public async Task<IActionResult> MovimentarProdutoAsync(Guid produtoId, [FromBody] MovimentacaoProdutoDto movimentacaoDto)
    {
        var resultado = await _produtoService.MovimentarProdutoAsync(produtoId, movimentacaoDto);
        if (!resultado)
        {
            return BadRequest("Erro ao movimentar o produto.");
        }
        return Ok("Movimentação realizada com sucesso.");
    }

    [HttpPost("movimentar/lote")]
    public async Task<IActionResult> MovimentarProdutosEmLoteAsync([FromBody] MovimentacaoLoteDto movimentacaoLoteDto)
    {
        var resultado = await _produtoService.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto);
        if (!resultado)
        {
            return BadRequest("Erro ao movimentar produtos.");
        }
        return Ok("Movimentação em lote realizada com sucesso.");
    }

    // Consulta o valor total do estoque
    [HttpGet("estoque/valor-total")]
    public async Task<IActionResult> ObterValorTotalEstoqueAsync()
    {
        var valorTotal = await _produtoService.ObterValorTotalEstoqueAsync();
        return Ok(valorTotal);
    }

    // Consulta a quantidade total de produtos no estoque
    [HttpGet("estoque/quantidade-total")]
    public async Task<IActionResult> ObterQuantidadeTotalEstoqueAsync()
    {
        var quantidadeTotal = await _produtoService.ObterQuantidadeTotalEstoqueAsync();
        return Ok(quantidadeTotal);
    }

    // Consulta o custo médio de um produto específico
    [HttpGet("{produtoId}/custo-medio")]
    public async Task<IActionResult> ObterCustoMedioProdutoAsync(Guid produtoId)
    {
        var custoMedio = await _produtoService.ObterCustoMedioProdutoAsync(produtoId);

        if (custoMedio == 0)
        {
            return NotFound($"Produto com o Id {produtoId} não foi encontrado.");
        }

        return Ok(custoMedio);
    }

    // Consulta o custo médio do estoque
    [HttpGet("estoque/custo-medio")]
    public async Task<IActionResult> ObterCustoMedioEstoqueAsync()
    {
        var custoMedio = await _produtoService.ObterCustoMedioEstoqueAsync();
        return Ok(custoMedio);
    }

}