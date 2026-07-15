using GestaoGastos.Api.Data;
using GestaoGastos.Api.DTOs;
using GestaoGastos.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoGastos.Api.Controllers;

[ApiController]
[Route("api/pessoas")]
public class PessoasController : ControllerBase
{
    private readonly AppDbContext _context;

    public PessoasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PessoaDto>>> Listar()
    {
        var pessoas = await _context.Pessoas
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .Select(p => new PessoaDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Idade = p.Idade
            })
            .ToListAsync();

        return Ok(pessoas);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PessoaDto>> ObterPorId(Guid id)
    {
        var pessoa = await _context.Pessoas
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PessoaDto { Id = p.Id, Nome = p.Nome, Idade = p.Idade })
            .FirstOrDefaultAsync();

        if (pessoa is null)
        {
            return NotFound(new { mensagem = "Pessoa não encontrada." });
        }

        return Ok(pessoa);
    }

    [HttpPost]
    public async Task<ActionResult<PessoaDto>> Criar([FromBody] CriarPessoaDto dto)
    {
        var pessoa = new Pessoa
        {
            Nome = dto.Nome.Trim(),
            Idade = dto.Idade
        };

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        var resultado = new PessoaDto { Id = pessoa.Id, Nome = pessoa.Nome, Idade = pessoa.Idade };

        return CreatedAtAction(nameof(ObterPorId), new { id = pessoa.Id }, resultado);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa is null)
        {
            return NotFound(new { mensagem = "Pessoa não encontrada." });
        }

        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}