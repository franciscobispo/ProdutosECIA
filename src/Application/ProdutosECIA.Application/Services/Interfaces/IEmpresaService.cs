using ProdutosECIA.Application.DTOs;

namespace ProdutosECIA.Application.Services.Interfaces;

public interface IEmpresaService
{
    Task<IEnumerable<EmpresaDto>> GetAllAsync();
    Task<EmpresaDto> GetByIdAsync(Guid id);
    Task<EmpresaDto> CreateAsync(EmpresaCreateDto empresaCreateDto);
    Task<bool> UpdateAsync(Guid id, EmpresaUpdateDto empresaUpdateDto);
    Task<bool> DeleteAsync(Guid id);
}