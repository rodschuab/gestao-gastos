using System.Net;
using System.Text.Json;
using GestaoGastos.Api.Exceptions;

namespace GestaoGastos.Api.Middleware;

// Middleware global de tratamento de exceções
// Centraliza a tradução de exceptions 
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NaoEncontradoException ex)
        {
            await EscreverResposta(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (RegraDeNegocioException ex)
        {
            await EscreverResposta(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado ao processar a requisição.");
            await EscreverResposta(context, HttpStatusCode.InternalServerError,
                "Ocorreu um erro inesperado ao processar a requisição.");
        }
    }

    private static async Task EscreverResposta(HttpContext context, HttpStatusCode statusCode, string mensagem)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var corpo = JsonSerializer.Serialize(new { mensagem });
        await context.Response.WriteAsync(corpo);
    }
}