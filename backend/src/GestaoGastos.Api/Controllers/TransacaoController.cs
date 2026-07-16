using GestaoGastos.Api.DTOs;
using GestaoGastos.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGastos.Api.Controllers;

// Endpoints de gerenciamento de Transações: criação e listagem.

[ApiController]
[Route("api/transacoes")]
public class TransacoesController : ControllerBase
{
    private readonly ITransacaoService _transacaoService;

    public TransacoesController(ITransacaoService transacaoService)
    {
        _transacaoService = transacaoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransacaoDto>>> Listar([FromQuery] Guid? pessoaId)
    {
        var transacoes = await _transacaoService.ListarAsync(pessoaId);
        return Ok(transacoes);
    }

    [HttpPost]
    public async Task<ActionResult<TransacaoDto>> Criar([FromBody] CriarTransacaoDto dto)
    {
        var transacao = await _transacaoService.CriarAsync(dto);
        return CreatedAtAction(nameof(Listar), new { pessoaId = transacao.PessoaId }, transacao);
    }
}