using Microsoft.EntityFrameworkCore;
using ProdutosECIA.Domain.Entities;

namespace ProdutosECIA.Infrastructure.DataContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Empresa> Empresas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurações adicionais, constraints, etc.
        modelBuilder.Entity<Produto>()
            .HasOne(p => p.Empresa)
            .WithMany(e => e.Produtos)
            .HasForeignKey(p => p.EmpresaId);
    }
}