using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Services.Interfaces;

namespace ProdutosECIA.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmpresaController : ControllerBase
{
    private readonly IEmpresaService _empresaService;

    public EmpresaController(IEmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmpresaDto>>> Get()
    {
        return Ok(await _empresaService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmpresaDto>> Get(Guid id)
    {
        var empresa = await _empresaService.GetByIdAsync(id);
        if (empresa == null)
            return NotFound();
        return Ok(empresa);
    }

    [HttpPost]
    public async Task<ActionResult<EmpresaDto>> Post([FromBody] EmpresaCreateDto empresaDto)
    {
        var empresa = await _empresaService.CreateAsync(empresaDto);
        return CreatedAtAction(nameof(Get), new { id = empresa.Id }, empresa);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<bool>> Put(Guid id, [FromBody] EmpresaUpdateDto empresaUpdateDto)
    {
        var empresa = await _empresaService.UpdateAsync(id, empresaUpdateDto);
        if (empresa == null)
            return NotFound();
        return Ok(empresa);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _empresaService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}