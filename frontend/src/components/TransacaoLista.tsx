import type { Transacao } from "../types";

interface Props {
  transacoes: Transacao[];
  carregando: boolean;
}

function formatarMoeda(valor: number): string {
  return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

export function TransacaoLista({ transacoes, carregando }: Props) {
  if (carregando) {
    return <p>Carregando transações...</p>;
  }

  if (transacoes.length === 0) {
    return <p>Nenhuma transação cadastrada ainda.</p>;
  }

  return (
    <table className="tabela">
      <thead>
        <tr>
          <th>Descrição</th>
          <th>Pessoa</th>
          <th>Tipo</th>
          <th>Valor</th>
        </tr>
      </thead>
      <tbody>
        {transacoes.map((t) => (
          <tr key={t.id}>
            <td>{t.descricao}</td>
            <td>{t.pessoaNome}</td>
            <td>
              <span className={t.tipo === "Receita" ? "etiqueta-receita" : "etiqueta-despesa"}>
                {t.tipo}
              </span>
            </td>
            <td>{formatarMoeda(t.valor)}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}