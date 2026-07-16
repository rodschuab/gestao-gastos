using GestaoGastos.Api.DTOs;
using GestaoGastos.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGastos.Api.Controllers;

// Endpoints de gerenciamento de Pessoas: criação, deleção e listagem.

[ApiController]
[Route("api/pessoas")]
public class PessoasController : ControllerBase
{
    private readonly IPessoaService _pessoaService;

    public PessoasController(IPessoaService pessoaService)
    {
        _pessoaService = pessoaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PessoaDto>>> Listar()
    {
        var pessoas = await _pessoaService.ListarAsync();
        return Ok(pessoas);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PessoaDto>> ObterPorId(Guid id)
    {
        var pessoa = await _pessoaService.ObterPorIdAsync(id);
        return Ok(pessoa);
    }

    [HttpPost]
    public async Task<ActionResult<PessoaDto>> Criar([FromBody] CriarPessoaDto dto)
    {
        var pessoa = await _pessoaService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = pessoa.Id }, pessoa);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        await _pessoaService.DeletarAsync(id);
        return NoContent();
    }
}