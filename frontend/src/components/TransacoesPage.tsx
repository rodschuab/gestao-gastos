import { useEffect, useState } from "react";
import type { NovaTransacao, Pessoa, Transacao } from "../types";
import { criarTransacao, listarTransacoes } from "../api/transacoesApi";
import { listarPessoas } from "../api/pessoasApi";
import { extrairMensagemErro } from "../api/erro";
import { TransacaoForm } from "./TransacaoForm";
import { TransacaoLista } from "./TransacaoLista";

export function TransacoesPage() {
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  async function carregar() {
    setCarregando(true);
    try {
      const [dadosTransacoes, dadosPessoas] = await Promise.all([
        listarTransacoes(),
        listarPessoas(),
      ]);
      setTransacoes(dadosTransacoes);
      setPessoas(dadosPessoas);
    } catch (e) {
      setErro(extrairMensagemErro(e, "Não foi possível carregar os dados."));
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    carregar();
  }, []);

  async function handleCadastrar(transacao: NovaTransacao) {
    setErro(null);
    try {
      await criarTransacao(transacao);
      await carregar();
    } catch (e) {
      throw new Error(extrairMensagemErro(e, "Não foi possível cadastrar a transação."));
    }
  }

  return (
    <section className="pagina">
      {pessoas.length === 0 && !carregando ? (
        <p className="mensagem-aviso">
          Cadastre ao menos uma pessoa antes de lançar transações.
        </p>
      ) : (
        <TransacaoForm pessoas={pessoas} aoCadastrar={handleCadastrar} />
      )}

      <div className="cartao">
        <h2>Transações cadastradas</h2>
        {erro && <p className="mensagem-erro">{erro}</p>}
        <TransacaoLista transacoes={transacoes} carregando={carregando} />
      </div>
    </section>
  );
}