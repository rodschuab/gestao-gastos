namespace GestaoGastos.Api.DTOs;
public class TotalPorPessoaDto
{
    public Guid PessoaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }

    /// <summary>Saldo = Receitas - Despesas.</summary>
    public decimal Saldo => TotalReceitas - TotalDespesas;
}

/*
   DTO de resposta da consulta de totais: lista de totais por pessoa
   + o total geral consolidado de todas as pessoas.
*/
public class ConsultaTotaisDto
{
    public List<TotalPorPessoaDto> Pessoas { get; set; } = new();

    public decimal TotalGeralReceitas { get; set; }
    public decimal TotalGeralDespesas { get; set; }

    /// <summary>Saldo líquido geral = Total geral de receitas - Total geral de despesas.</summary>
    public decimal SaldoLiquidoGeral => TotalGeralReceitas - TotalGeralDespesas;
}