using GestaoGastos.Api.Enums;
using GestaoGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoGastos.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.Idade)
                .IsRequired();

            entity.HasMany(p => p.Transacoes)
                .WithOne(t => t.Pessoa)
                .HasForeignKey(t => t.PessoaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(t => t.Descricao)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(t => t.Valor)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(t => t.Tipo)
                .IsRequired()
                .HasConversion<string>(); // grava o enum como texto no banco, em vez de numero
        });
    }
}