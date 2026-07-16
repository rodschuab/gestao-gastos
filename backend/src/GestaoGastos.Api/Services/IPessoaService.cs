using GestaoGastos.Api.DTOs;

namespace GestaoGastos.Api.Services;
public interface IPessoaService
{
    Task<IEnumerable<PessoaDto>> ListarAsync();
    Task<PessoaDto> ObterPorIdAsync(Guid id);
    Task<PessoaDto> CriarAsync(CriarPessoaDto dto);
    Task DeletarAsync(Guid id);
}