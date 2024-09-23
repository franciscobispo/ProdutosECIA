using AutoMapper;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Application.Interfaces;
using ProdutosECIA.Domain.Entities;
using ProdutosECIA.Infrastructure.Repositories;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.Application.Services;

public class EmpresaService : IEmpresaService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IMapper _mapper;

    public EmpresaService(IEmpresaRepository empresaRepository, IMapper mapper)
    {
        _empresaRepository = empresaRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmpresaDto>> GetAllAsync()
    {
        var empresas = await _empresaRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<EmpresaDto>>(empresas);
    }

    public async Task<EmpresaDto> GetByIdAsync(Guid id)
    {
        var empresa = await _empresaRepository.GetByIdAsync(id);
        return _mapper.Map<EmpresaDto>(empresa);
    }

    public async Task<EmpresaDto> CreateAsync(EmpresaCreateDto empresaCreateDto)
    {
        var empresa = _mapper.Map<Empresa>(empresaCreateDto);
        await _empresaRepository.AddAsync(empresa);
        return _mapper.Map<EmpresaDto>(empresa);
    }

    public async Task<bool> UpdateAsync(Guid id, EmpresaUpdateDto empresaUpdateDto)
    {
        var empresa = await _empresaRepository.GetByIdAsync(id);
        if (empresa == null)
        {
            return false;
        }
        _mapper.Map(empresaUpdateDto, empresa);
        return await _empresaRepository.UpdateAsync(empresa);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _empresaRepository.DeleteAsync(id);
    }
}