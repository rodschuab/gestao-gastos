using GestaoGastos.Api.DTOs;

namespace GestaoGastos.Api.Services;
public interface ITransacaoService
{
    Task<IEnumerable<TransacaoDto>> ListarAsync(Guid? pessoaId);
    Task<TransacaoDto> CriarAsync(CriarTransacaoDto dto);
}