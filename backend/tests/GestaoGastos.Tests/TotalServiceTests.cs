using GestaoGastos.Api.Data;
using GestaoGastos.Api.Enums;
using GestaoGastos.Api.Models;
using GestaoGastos.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestaoGastos.Tests;

/// <summary>
/// Testes do cálculo de totais: receitas, despesas e saldo por pessoa,
/// e a consolidação do total geral.
/// </summary>
public class TotalServiceTests
{
    private static AppDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task ObterTotais_DeveCalcularSaldoCorretamente_ParaUmaUnicaPessoa()
    {
        // Arrange
        await using var contexto = CriarContexto();
        var pessoa = new Pessoa { Nome = "Rodrigo", Idade = 26 };
        contexto.Pessoas.Add(pessoa);
        await contexto.SaveChangesAsync();

        contexto.Transacoes.AddRange(
            new Transacao { Descricao = "Salário", Valor = 100, Tipo = TipoTransacao.Receita, PessoaId = pessoa.Id },
            new Transacao { Descricao = "Mercado", Valor = 30, Tipo = TipoTransacao.Despesa, PessoaId = pessoa.Id }
        );
        await contexto.SaveChangesAsync();

        var service = new TotalService(contexto);

        // Act
        var resultado = await service.ObterTotaisAsync();

        // Assert
        var totalPessoa = Assert.Single(resultado.Pessoas);
        Assert.Equal(100, totalPessoa.TotalReceitas);
        Assert.Equal(30, totalPessoa.TotalDespesas);
        Assert.Equal(70, totalPessoa.Saldo);
    }

    [Fact]
    public async Task ObterTotais_DeveConsolidarTotalGeral_DeVariasPessoas()
    {
        // Arrange: mesmo cenário validado manualmente durante o desenvolvimento
        // (Pedro, menor de idade, só despesa; Rodrigo, maior, só receita).
        await using var contexto = CriarContexto();
        var pedro = new Pessoa { Nome = "Pedro", Idade = 10 };
        var rodrigo = new Pessoa { Nome = "Rodrigo", Idade = 26 };
        contexto.Pessoas.AddRange(pedro, rodrigo);
        await contexto.SaveChangesAsync();

        contexto.Transacoes.AddRange(
            new Transacao { Descricao = "Lanche", Valor = 20, Tipo = TipoTransacao.Despesa, PessoaId = pedro.Id },
            new Transacao { Descricao = "Salário", Valor = 20, Tipo = TipoTransacao.Receita, PessoaId = rodrigo.Id }
        );
        await contexto.SaveChangesAsync();

        var service = new TotalService(contexto);

        // Act
        var resultado = await service.ObterTotaisAsync();

        // Assert
        Assert.Equal(20, resultado.TotalGeralReceitas);
        Assert.Equal(20, resultado.TotalGeralDespesas);
        Assert.Equal(0, resultado.SaldoLiquidoGeral);
    }

    [Fact]
    public async Task ObterTotais_DeveRetornarListaVazia_QuandoNaoHaPessoasCadastradas()
    {
        // Arrange
        await using var contexto = CriarContexto();
        var service = new TotalService(contexto);

        // Act
        var resultado = await service.ObterTotaisAsync();

        // Assert
        Assert.Empty(resultado.Pessoas);
        Assert.Equal(0, resultado.TotalGeralReceitas);
        Assert.Equal(0, resultado.TotalGeralDespesas);
    }

    [Fact]
    public async Task ObterTotais_DeveRetornarZerado_ParaPessoaSemTransacoes()
    {
        // Arrange
        await using var contexto = CriarContexto();
        var pessoa = new Pessoa { Nome = "Sem Movimentação", Idade = 40 };
        contexto.Pessoas.Add(pessoa);
        await contexto.SaveChangesAsync();

        var service = new TotalService(contexto);

        // Act
        var resultado = await service.ObterTotaisAsync();

        // Assert
        var totalPessoa = Assert.Single(resultado.Pessoas);
        Assert.Equal(0, totalPessoa.TotalReceitas);
        Assert.Equal(0, totalPessoa.TotalDespesas);
        Assert.Equal(0, totalPessoa.Saldo);
    }
}