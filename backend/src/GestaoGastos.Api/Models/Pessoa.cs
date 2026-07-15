namespace GestaoGastos.Api.Models;
public class Pessoa
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    public int Idade { get; set; }

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();

    public bool EhMenorDeIdade => Idade < 18;
}