using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerOperation(Summary = "Cria/Adiciona/Remove quantidade no estoque de um produto de uma empresa.")]
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
    [SwaggerOperation(Summary = "Adiciona/Remove em lote a quantidade no estoque de um produto de uma empresa.")]
    public async Task<IActionResult> MovimentarProdutosEmLoteAsync([FromBody] MovimentacaoLoteDto movimentacaoLoteDto)
    {
        var resultado = await _produtoService.MovimentarProdutosEmLoteAsync(movimentacaoLoteDto);
        if (!resultado)
        {
            return BadRequest("Erro ao movimentar produtos.");
        }
        return Ok("Movimentação em lote realizada com sucesso.");
    }

    [HttpGet("estoque/valor-total")]
    [SwaggerOperation(Summary = "Consulta o valor total do estoque ((PrecoCusto * e.Quantidade) de todos os Produtos cadastrados).")]
    public async Task<IActionResult> ObterValorTotalEstoqueAsync()
    {
        var valorTotal = await _produtoService.ObterValorTotalEstoqueAsync();
        return Ok(valorTotal);
    }

    [HttpGet("{produtoId}/empresa/{empresaId}/quantidade-total")]
    [SwaggerOperation(Summary = "Consulta a quantidade total de produtos no estoque de uma Empresa.")]
    public async Task<IActionResult> ObterQuantidadeTotalProdutoAsync(Guid produtoId, Guid empresaId)
    {
        var quantidadeTotal = await _produtoService.ObterQuantidadeTotalProdutoAsync(produtoId, empresaId);

        if (quantidadeTotal == 0)
        {
            return NotFound($"Estoque não encontrado para o Produto com o Id {produtoId} na Empresa {empresaId}.");
        }

        return Ok(quantidadeTotal);
    }

    [HttpGet("{produtoId}/custo-medio")]
    [SwaggerOperation(Summary = "Consulta o custo médio de um produto específico.")]
    public async Task<IActionResult> ObterCustoMedioProdutoAsync(Guid produtoId)
    {
        var custoMedio = await _produtoService.ObterCustoMedioProdutoAsync(produtoId);

        if (custoMedio == 0)
        {
            return NotFound($"Produto com o Id {produtoId} não foi encontrado.");
        }

        return Ok(custoMedio);
    }

    [HttpGet("estoque/custo-medio")]
    [SwaggerOperation(Summary = "Consulta o custo médio do estoque.")]
    public async Task<IActionResult> ObterCustoMedioEstoqueAsync()
    {
        var custoMedio = await _produtoService.ObterCustoMedioEstoqueAsync();
        return Ok(custoMedio);
    }

    [HttpPost("{produtoId}/transferir")]
    [SwaggerOperation(Summary = "Transfere de uma empresa para outra, uma quantidade de um produto específico.")]
    public async Task<IActionResult> TransferirProdutoAsync(Guid produtoId, Guid deEmpresaId, Guid paraEmpresaId, int quantidade)
    {
        var resultado = await _produtoService.TransferirProdutoAsync(produtoId, deEmpresaId, paraEmpresaId, quantidade);
        if (!resultado)
        {
            return BadRequest("Erro ao transferir o produto.");
        }
        return Ok("Transferência realizada com sucesso.");
    }

}