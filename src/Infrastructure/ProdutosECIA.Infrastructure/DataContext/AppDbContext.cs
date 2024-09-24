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
    public DbSet<EstoqueProduto> EstoqueProdutos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EstoqueProduto>()
        .HasKey(ep => ep.Id);

        modelBuilder.Entity<EstoqueProduto>()
            .HasOne(ep => ep.Produto)
            .WithMany(p => p.Estoques)
            .HasForeignKey(ep => ep.ProdutoId);

        modelBuilder.Entity<EstoqueProduto>()
            .HasOne(ep => ep.Empresa)
            .WithMany()
            .HasForeignKey(ep => ep.EmpresaId);

        base.OnModelCreating(modelBuilder);

        // Seed usuário admin
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin");        
        modelBuilder.Entity<Usuario>().HasData(new Usuario
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            PasswordHash = adminPasswordHash,
            Role = "Admin"
        });
    }
}