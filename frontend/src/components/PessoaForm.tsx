import { useState, type FormEvent } from "react";
import type { NovaPessoa } from "../types";

interface Props {
  aoCadastrar: (pessoa: NovaPessoa) => Promise<void>;
}

export function PessoaForm({ aoCadastrar }: Props) {
  const [nome, setNome] = useState("");
  const [idade, setIdade] = useState("");
  const [enviando, setEnviando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setErro(null);

    if (!nome.trim()) {
      setErro("Informe o nome da pessoa.");
      return;
    }
    if (idade === "" || Number(idade) < 0) {
      setErro("Informe uma idade válida.");
      return;
    }

    setEnviando(true);
    try {
      await aoCadastrar({ nome: nome.trim(), idade: Number(idade) });
      setNome("");
      setIdade("");
    } catch {
      setErro("Não foi possível cadastrar a pessoa. Tente novamente.");
    } finally {
      setEnviando(false);
    }
  }

  return (
    <form className="cartao formulario" onSubmit={handleSubmit}>
      <h2>Cadastrar pessoa</h2>

      <div className="campo">
        <label htmlFor="nome">Nome</label>
        <input
          id="nome"
          type="text"
          value={nome}
          onChange={(e) => setNome(e.target.value)}
          placeholder="Ex: Maria Silva"
        />
      </div>

      <div className="campo">
        <label htmlFor="idade">Idade</label>
        <input
          id="idade"
          type="number"
          min={0}
          value={idade}
          onChange={(e) => setIdade(e.target.value)}
          placeholder="Ex: 30"
        />
      </div>

      {erro && <p className="mensagem-erro">{erro}</p>}

      <button type="submit" disabled={enviando}>
        {enviando ? "Salvando..." : "Cadastrar"}
      </button>
    </form>
  );
}