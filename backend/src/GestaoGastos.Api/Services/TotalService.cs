using GestaoGastos.Api.Data;
using GestaoGastos.Api.DTOs;
using GestaoGastos.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestaoGastos.Api.Services;
/*
    calculos de totais: para cada pessoa, soma receitas e
    despesas e calcula o saldo; ao final geral de todas as pessoas.
*/
public class TotalService : ITotalService
{
    private readonly AppDbContext _context;

    public TotalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ConsultaTotaisDto> ObterTotaisAsync()
    {
        // pega todas as pessoas junto com suas transações em uma única consulta

        var pessoas = await _context.Pessoas
            .AsNoTracking()
            .Include(p => p.Transacoes)
            .OrderBy(p => p.Nome)
            .ToListAsync();

        var totaisPorPessoa = pessoas.Select(p => new TotalPorPessoaDto
        {
            PessoaId = p.Id,
            Nome = p.Nome,
            Idade = p.Idade,
            TotalReceitas = p.Transacoes
                .Where(t => t.Tipo == TipoTransacao.Receita)
                .Sum(t => t.Valor),
            TotalDespesas = p.Transacoes
                .Where(t => t.Tipo == TipoTransacao.Despesa)
                .Sum(t => t.Valor)
        }).ToList();

        return new ConsultaTotaisDto
        {
            Pessoas = totaisPorPessoa,
            TotalGeralReceitas = totaisPorPessoa.Sum(t => t.TotalReceitas),
            TotalGeralDespesas = totaisPorPessoa.Sum(t => t.TotalDespesas)
        };
    }
}