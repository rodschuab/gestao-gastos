using GestaoGastos.Api.DTOs;

namespace GestaoGastos.Api.Services;
public interface ITotalService
{
    Task<ConsultaTotaisDto> ObterTotaisAsync();
}