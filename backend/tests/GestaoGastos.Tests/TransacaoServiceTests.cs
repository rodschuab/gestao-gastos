using GestaoGastos.Api.Data;
using GestaoGastos.Api.DTOs;
using GestaoGastos.Api.Enums;
using GestaoGastos.Api.Exceptions;
using GestaoGastos.Api.Models;
using GestaoGastos.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestaoGastos.Tests;

/// <summary>
/// Testes da regra de negócio central do sistema: pessoas menores de idade
/// só podem ter despesas cadastradas, e a pessoa informada precisa existir.
/// Cada teste usa um banco EF Core InMemory isolado (nome único por teste,
/// via Guid), garantindo que um teste não interfere no outro.
/// </summary>
public class TransacaoServiceTests
{
    private static AppDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Criar_DeveLancarExcecao_QuandoPessoaNaoExiste()
    {
        // Arrange
        await using var contexto = CriarContexto();
        var service = new TransacaoService(contexto);

        var dto = new CriarTransacaoDto
        {
            Descricao = "Compra qualquer",
            Valor = 100,
            Tipo = TipoTransacao.Despesa,
            PessoaId = Guid.NewGuid() // Id que não existe no banco
        };

        // Act & Assert
        await Assert.ThrowsAsync<RegraDeNegocioException>(() => service.CriarAsync(dto));
    }

    [Fact]
    public async Task Criar_DeveLancarExcecao_QuandoMenorDeIdadeTentaCadastrarReceita()
    {
        // Arrange
        await using var contexto = CriarContexto();
        var pessoaMenorDeIdade = new Pessoa { Nome = "Criança Teste", Idade = 10 };
        contexto.Pessoas.Add(pessoaMenorDeIdade);
        await contexto.SaveChangesAsync();

        var service = new TransacaoService(contexto);
        var dto = new CriarTransacaoDto
        {
            Descricao = "Mesada",
            Valor = 50,
            Tipo = TipoTransacao.Receita,
            PessoaId = pessoaMenorDeIdade.Id
        };

        // Act & Assert
        await Assert.ThrowsAsync<RegraDeNegocioException>(() => service.CriarAsync(dto));
    }

    [Fact]
    public async Task Criar_DevePermitir_QuandoMenorDeIdadeCadastraDespesa()
    {
        // Arrange
        await using var contexto = CriarContexto();
        var pessoaMenorDeIdade = new Pessoa { Nome = "Criança Teste", Idade = 10 };
        contexto.Pessoas.Add(pessoaMenorDeIdade);
        await contexto.SaveChangesAsync();

        var service = new TransacaoService(contexto);
        var dto = new CriarTransacaoDto
        {
            Descricao = "Lanche",
            Valor = 15,
            Tipo = TipoTransacao.Despesa,
            PessoaId = pessoaMenorDeIdade.Id
        };

        // Act
        var resultado = await service.CriarAsync(dto);

        // Assert
        Assert.Equal("Lanche", resultado.Descricao);
        Assert.Equal(TipoTransacao.Despesa, resultado.Tipo);
    }

    [Fact]
    public async Task Criar_DevePermitir_QuandoMaiorDeIdadeCadastraReceita()
    {
        // Arrange
        await using var contexto = CriarContexto();
        var pessoaMaiorDeIdade = new Pessoa { Nome = "Adulto Teste", Idade = 30 };
        contexto.Pessoas.Add(pessoaMaiorDeIdade);
        await contexto.SaveChangesAsync();

        var service = new TransacaoService(contexto);
        var dto = new CriarTransacaoDto
        {
            Descricao = "Salário",
            Valor = 3000,
            Tipo = TipoTransacao.Receita,
            PessoaId = pessoaMaiorDeIdade.Id
        };

        // Act
        var resultado = await service.CriarAsync(dto);

        // Assert
        Assert.Equal(TipoTransacao.Receita, resultado.Tipo);
        Assert.Equal(3000, resultado.Valor);
    }

    [Fact]
    public async Task Criar_DevePermitir_QuandoPessoaTemExatamente18Anos()
    {
        
        await using var contexto = CriarContexto();
        var pessoaComDezoitoAnos = new Pessoa { Nome = "Recém Maior", Idade = 18 };
        contexto.Pessoas.Add(pessoaComDezoitoAnos);
        await contexto.SaveChangesAsync();

        var service = new TransacaoService(contexto);
        var dto = new CriarTransacaoDto
        {
            Descricao = "Primeiro salário",
            Valor = 1500,
            Tipo = TipoTransacao.Receita,
            PessoaId = pessoaComDezoitoAnos.Id
        };

        // Act
        var resultado = await service.CriarAsync(dto);

        // Assert
        Assert.Equal(TipoTransacao.Receita, resultado.Tipo);
    }
}