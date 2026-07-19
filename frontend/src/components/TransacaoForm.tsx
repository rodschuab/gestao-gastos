import { useMemo, useState, type FormEvent } from "react";
import type { NovaTransacao, Pessoa, TipoTransacao } from "../types";

interface Props {
  pessoas: Pessoa[];
  aoCadastrar: (transacao: NovaTransacao) => Promise<void>;
}

export function TransacaoForm({ pessoas, aoCadastrar }: Props) {
  const [descricao, setDescricao] = useState("");
  const [valor, setValor] = useState("");
  const [tipo, setTipo] = useState<TipoTransacao>("Despesa");
  const [pessoaId, setPessoaId] = useState("");
  const [enviando, setEnviando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  const pessoaSelecionada = useMemo(
    () => pessoas.find((p) => p.id === pessoaId),
    [pessoas, pessoaId]
  );
  const pessoaMenorDeIdade = (pessoaSelecionada?.idade ?? 18) < 18;

  function handleSelecionarPessoa(id: string) {
    setPessoaId(id);
    const pessoa = pessoas.find((p) => p.id === id);
    if (pessoa && pessoa.idade < 18) {
      setTipo("Despesa");
    }
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setErro(null);

    if (!pessoaId) {
      setErro("Selecione a pessoa da transação.");
      return;
    }
    if (!descricao.trim()) {
      setErro("Informe a descrição da transação.");
      return;
    }
    const valorNumerico = Number(valor);
    if (!valor || valorNumerico <= 0) {
      setErro("Informe um valor maior que zero.");
      return;
    }

    setEnviando(true);
    try {
      await aoCadastrar({
        descricao: descricao.trim(),
        valor: valorNumerico,
        tipo,
        pessoaId,
      });
      setDescricao("");
      setValor("");
      setTipo("Despesa");
    } catch (e) {
      setErro(e instanceof Error ? e.message : "Não foi possível cadastrar a transação.");
    } finally {
      setEnviando(false);
    }
  }

  return (
    <form className="cartao formulario" onSubmit={handleSubmit}>
      <h2>Cadastrar transação</h2>

      <div className="campo">
        <label htmlFor="pessoa">Pessoa</label>
        <select
          id="pessoa"
          value={pessoaId}
          onChange={(e) => handleSelecionarPessoa(e.target.value)}
        >
          <option value="">Selecione...</option>
          {pessoas.map((pessoa) => (
            <option key={pessoa.id} value={pessoa.id}>
              {pessoa.nome} ({pessoa.idade} anos)
            </option>
          ))}
        </select>
        {pessoaMenorDeIdade && (
          <p className="mensagem-aviso">
            Pessoa menor de idade: apenas despesas podem ser cadastradas.
          </p>
        )}
      </div>

      <div className="campo">
        <label htmlFor="descricao">Descrição</label>
        <input
          id="descricao"
          type="text"
          value={descricao}
          onChange={(e) => setDescricao(e.target.value)}
          placeholder="Ex: Supermercado, Salário..."
        />
      </div>

      <div className="campo">
        <label htmlFor="valor">Valor (R$)</label>
        <input
          id="valor"
          type="number"
          min={0.01}
          step="0.01"
          value={valor}
          onChange={(e) => setValor(e.target.value)}
          placeholder="Ex: 150.00"
        />
      </div>

      <div className="campo">
        <label htmlFor="tipo">Tipo</label>
        <select
          id="tipo"
          value={tipo}
          onChange={(e) => setTipo(e.target.value as TipoTransacao)}
          disabled={pessoaMenorDeIdade}
        >
          <option value="Despesa">Despesa</option>
          <option value="Receita" disabled={pessoaMenorDeIdade}>
            Receita
          </option>
        </select>
      </div>

      {erro && <p className="mensagem-erro">{erro}</p>}

      <button type="submit" disabled={enviando}>
        {enviando ? "Salvando..." : "Cadastrar"}
      </button>
    </form>
  );
}