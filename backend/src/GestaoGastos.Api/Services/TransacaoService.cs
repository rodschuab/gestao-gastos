using GestaoGastos.Api.Data;
using GestaoGastos.Api.DTOs;
using GestaoGastos.Api.Enums;
using GestaoGastos.Api.Exceptions;
using GestaoGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoGastos.Api.Services;
public class TransacaoService : ITransacaoService
{
    private readonly AppDbContext _context;

    public TransacaoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TransacaoDto>> ListarAsync(Guid? pessoaId)
    {
        var query = _context.Transacoes
            .AsNoTracking()
            .Include(t => t.Pessoa)
            .AsQueryable();

        if (pessoaId.HasValue)
        {
            query = query.Where(t => t.PessoaId == pessoaId.Value);
        }

        return await query
            .OrderByDescending(t => t.Id)
            .Select(t => new TransacaoDto
            {
                Id = t.Id,
                Descricao = t.Descricao,
                Valor = t.Valor,
                Tipo = t.Tipo,
                PessoaId = t.PessoaId,
                PessoaNome = t.Pessoa!.Nome
            })
            .ToListAsync();
    }

    /*
      a pessoa informada precisa existir no cadastro de pessoas
      se a pessoa for menor de idade , apenas transações do
      tipo Despesa podem ser cadastradas para ela. 
      
    */
    public async Task<TransacaoDto> CriarAsync(CriarTransacaoDto dto)
    {
        var pessoa = await _context.Pessoas.FindAsync(dto.PessoaId);

        if (pessoa is null)
        {
            throw new RegraDeNegocioException("A pessoa informada não existe no cadastro.");
        }

        if (pessoa.EhMenorDeIdade && dto.Tipo == TipoTransacao.Receita)
        {
            throw new RegraDeNegocioException(
                "Pessoas menores de idade (menores de 18 anos) só podem ter despesas cadastradas.");
        }

        var transacao = new Transacao
        {
            Descricao = dto.Descricao.Trim(),
            Valor = dto.Valor,
            Tipo = dto.Tipo,
            PessoaId = dto.PessoaId
        };

        _context.Transacoes.Add(transacao);
        await _context.SaveChangesAsync();

        return new TransacaoDto
        {
            Id = transacao.Id,
            Descricao = transacao.Descricao,
            Valor = transacao.Valor,
            Tipo = transacao.Tipo,
            PessoaId = transacao.PessoaId,
            PessoaNome = pessoa.Nome
        };
    }
}