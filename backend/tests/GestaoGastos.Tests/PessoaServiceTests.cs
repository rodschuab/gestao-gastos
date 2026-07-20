using GestaoGastos.Api.Data;
using GestaoGastos.Api.DTOs;
using GestaoGastos.Api.Exceptions;
using GestaoGastos.Api.Models;
using GestaoGastos.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestaoGastos.Tests;

public class PessoaServiceTests
{
    private static AppDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Criar_DeveGerarIdAutomaticamente()
    {
        await using var contexto = CriarContexto();
        var service = new PessoaService(contexto);

        var resultado = await service.CriarAsync(new CriarPessoaDto { Nome = "Maria", Idade = 40 });

        Assert.NotEqual(Guid.Empty, resultado.Id);
        Assert.Equal("Maria", resultado.Nome);
    }

    [Fact]
    public async Task ObterPorId_DeveLancarExcecao_QuandoPessoaNaoExiste()
    {
        await using var contexto = CriarContexto();
        var service = new PessoaService(contexto);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => service.ObterPorIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Deletar_DeveRemoverTransacoesEmCascata()
    {
        // Arrange
        await using var contexto = CriarContexto();
        var pessoa = new Pessoa { Nome = "Teste Cascade", Idade = 30 };
        contexto.Pessoas.Add(pessoa);
        await contexto.SaveChangesAsync();

        contexto.Transacoes.Add(new Transacao
        {
            Descricao = "Transação da pessoa",
            Valor = 100,
            Tipo = Api.Enums.TipoTransacao.Despesa,
            PessoaId = pessoa.Id
        });
        await contexto.SaveChangesAsync();

        var service = new PessoaService(contexto);

        // Act
        await service.DeletarAsync(pessoa.Id);

        // Assert
        var transacoesRestantes = await contexto.Transacoes
            .Where(t => t.PessoaId == pessoa.Id)
            .ToListAsync();
        Assert.Empty(transacoesRestantes);
    }
}