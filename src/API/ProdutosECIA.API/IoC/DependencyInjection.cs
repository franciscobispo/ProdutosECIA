using FluentValidation;
using FluentValidation.AspNetCore;
using ProdutosECIA.Application.Interfaces;
using ProdutosECIA.Application.Validators;
using ProdutosECIA.Infrastructure.Repositories.Interfaces;

namespace ProdutosECIA.API.IoC;

public static class DependencyInjection
{
    public static void AddValidators(this IServiceCollection services)
    {  
        // Registra TODOS os Validators do FluentValidation dinamicamente, em qualquer assembly onde um Validator esteja presente
        services.AddControllers()
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<EmpresaCreateDtoValidator>());
        services.AddValidatorsFromAssemblyContaining<EmpresaCreateDtoValidator>();
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrar automaticamente todos os Services
        services.Scan(scan => scan
            .FromAssemblyOf<IEmpresaService>()  // Use qualquer interface de serviço como referência
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Registrar automaticamente todos os Repositories
        services.Scan(scan => scan
            .FromAssemblyOf<IEmpresaRepository>()  // Use qualquer interface de repositório como referência
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

}