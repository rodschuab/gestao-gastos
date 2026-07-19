import { useEffect, useState } from "react";
import type { NovaPessoa, Pessoa } from "../types";
import { criarPessoa, deletarPessoa, listarPessoas } from "../api/pessoasApi";
import { extrairMensagemErro } from "../api/erro";
import { PessoaForm } from "./PessoaForm";
import { PessoaLista } from "./PessoaLista";

export function PessoasPage() {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  async function carregar() {
    setCarregando(true);
    try {
      const dados = await listarPessoas();
      setPessoas(dados);
    } catch (e) {
      setErro(extrairMensagemErro(e, "Não foi possível carregar as pessoas."));
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    carregar();
  }, []);

  async function handleCadastrar(pessoa: NovaPessoa) {
    setErro(null);
    try {
      await criarPessoa(pessoa);
      await carregar();
    } catch (e) {
      setErro(extrairMensagemErro(e, "Não foi possível cadastrar a pessoa."));
      throw e;
    }
  }

  async function handleExcluir(id: string) {
    setErro(null);
    try {
      await deletarPessoa(id);
      await carregar();
    } catch (e) {
      setErro(extrairMensagemErro(e, "Não foi possível excluir a pessoa."));
    }
  }

  return (
    <section className="pagina">
      <PessoaForm aoCadastrar={handleCadastrar} />

      <div className="cartao">
        <h2>Pessoas cadastradas</h2>
        {erro && <p className="mensagem-erro">{erro}</p>}
        <PessoaLista pessoas={pessoas} carregando={carregando} aoExcluir={handleExcluir} />
      </div>
    </section>
  );
}