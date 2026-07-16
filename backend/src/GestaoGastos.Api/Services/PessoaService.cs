using GestaoGastos.Api.Data;
using GestaoGastos.Api.DTOs;
using GestaoGastos.Api.Exceptions;
using GestaoGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoGastos.Api.Services;
public class PessoaService : IPessoaService
{
    private readonly AppDbContext _context;

    public PessoaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PessoaDto>> ListarAsync()
    {
        return await _context.Pessoas
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .Select(p => new PessoaDto { Id = p.Id, Nome = p.Nome, Idade = p.Idade })
            .ToListAsync();
    }

    public async Task<PessoaDto> ObterPorIdAsync(Guid id)
    {
        var pessoa = await _context.Pessoas
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PessoaDto { Id = p.Id, Nome = p.Nome, Idade = p.Idade })
            .FirstOrDefaultAsync();

        if (pessoa is null)
        {
            throw new NaoEncontradoException("Pessoa não encontrada.");
        }

        return pessoa;
    }

    public async Task<PessoaDto> CriarAsync(CriarPessoaDto dto)
    {
        var pessoa = new Pessoa
        {
            Nome = dto.Nome.Trim(),
            Idade = dto.Idade
        };

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        return new PessoaDto { Id = pessoa.Id, Nome = pessoa.Nome, Idade = pessoa.Idade };
    }

    public async Task DeletarAsync(Guid id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa is null)
        {
            throw new NaoEncontradoException("Pessoa não encontrada.");
        }
        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();
    }
}