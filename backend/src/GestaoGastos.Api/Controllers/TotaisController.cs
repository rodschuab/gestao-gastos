using GestaoGastos.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGastos.Api.Controllers;

/*
    Endpoint de consulta de totais: para cada pessoa cadastrada, exibe o total
    de receitas, despesas e o saldo (receita - despesa); ao final, exibe o
    total geral consolidado de todas as pessoas.
*/

[ApiController]
[Route("api/totais")]
public class TotaisController : ControllerBase
{
    private readonly ITotalService _totalService;

    public TotaisController(ITotalService totalService)
    {
        _totalService = totalService;
    }

    [HttpGet]
    public async Task<ActionResult<DTOs.ConsultaTotaisDto>> ObterTotais()
    {
        var totais = await _totalService.ObterTotaisAsync();
        return Ok(totais);
    }
}