import { useState } from "react";
import type { Pessoa } from "../types";
import { ConfirmacaoModal } from "./ConfirmacaoModal";

interface Props {
  pessoas: Pessoa[];
  carregando: boolean;
  aoExcluir: (id: string) => void;
}

export function PessoaLista({ pessoas, carregando, aoExcluir }: Props) {
  const [pessoaParaExcluir, setPessoaParaExcluir] = useState<Pessoa | null>(null);

  function confirmarExclusao() {
    if (pessoaParaExcluir) {
      aoExcluir(pessoaParaExcluir.id);
      setPessoaParaExcluir(null);
    }
  }

  if (carregando) {
    return <p>Carregando pessoas...</p>;
  }

  if (pessoas.length === 0) {
    return <p>Nenhuma pessoa cadastrada ainda.</p>;
  }

  return (
    <>
      <table className="tabela">
        <thead>
          <tr>
            <th>Nome</th>
            <th>Idade</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {pessoas.map((pessoa) => (
            <tr key={pessoa.id}>
              <td>{pessoa.nome}</td>
              <td>
                {pessoa.idade}
                {pessoa.idade < 18 && " (menor de idade)"}
              </td>
              <td>
                <button className="botao-perigo" onClick={() => setPessoaParaExcluir(pessoa)}>
                  Excluir
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <ConfirmacaoModal
        aberto={pessoaParaExcluir !== null}
        titulo="Excluir pessoa"
        mensagem={
          pessoaParaExcluir
            ? `Excluir "${pessoaParaExcluir.nome}"? Todas as transações dessa pessoa também serão excluídas.`
            : ""
        }
        aoConfirmar={confirmarExclusao}
        aoCancelar={() => setPessoaParaExcluir(null)}
      />
    </>
  );
}