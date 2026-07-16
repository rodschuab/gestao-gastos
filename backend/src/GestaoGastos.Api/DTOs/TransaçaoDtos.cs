using System.ComponentModel.DataAnnotations;
using GestaoGastos.Api.Enums;

namespace GestaoGastos.Api.DTOs;
public class CriarTransacaoDto
{
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [MaxLength(250, ErrorMessage = "A descrição deve ter no máximo 250 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "O tipo da transação é obrigatório (Receita ou Despesa).")]
    public TipoTransacao Tipo { get; set; }

    [Required(ErrorMessage = "O identificador da pessoa é obrigatório.")]
    public Guid PessoaId { get; set; }
}
public class TransacaoDto
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public TipoTransacao Tipo { get; set; }
    public Guid PessoaId { get; set; }
    public string? PessoaNome { get; set; }
}