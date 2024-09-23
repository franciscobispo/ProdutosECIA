using AutoMapper;
using ProdutosECIA.Application.DTOs;
using ProdutosECIA.Domain.Entities;

namespace ProdutosECIA.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<ProdutoCreateDto, Produto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignorar Id se for auto gerado
        CreateMap<ProdutoUpdateDto, Produto>();
        CreateMap<Produto, ProdutoDto>();

        CreateMap<EmpresaCreateDto, Empresa>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignorar Id se for auto gerado
        CreateMap<EmpresaUpdateDto, Empresa>();
        CreateMap<Empresa, EmpresaDto>();
    }
}