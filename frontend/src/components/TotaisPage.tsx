import { useEffect, useState } from "react";
import type { ConsultaTotais } from "../types";
import { consultarTotais } from "../api/totaisApi";
import { extrairMensagemErro } from "../api/erro";

function formatarMoeda(valor: number): string {
  return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

export function TotaisPage() {
  const [totais, setTotais] = useState<ConsultaTotais | null>(null);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  useEffect(() => {
    async function carregar() {
      setCarregando(true);
      try {
        const dados = await consultarTotais();
        setTotais(dados);
      } catch (e) {
        setErro(extrairMensagemErro(e, "Não foi possível carregar os totais."));
      } finally {
        setCarregando(false);
      }
    }
    carregar();
  }, []);

  if (carregando) {
    return <p>Carregando totais...</p>;
  }

  if (erro) {
    return <p className="mensagem-erro">{erro}</p>;
  }

  if (!totais || totais.pessoas.length === 0) {
    return <p>Nenhuma pessoa cadastrada ainda.</p>;
  }

  return (
    <section className="pagina">
      <div className="cartao">
        <h2>Totais por pessoa</h2>
        <table className="tabela">
          <thead>
            <tr>
              <th>Pessoa</th>
              <th>Receitas</th>
              <th>Despesas</th>
              <th>Saldo</th>
            </tr>
          </thead>
          <tbody>
            {totais.pessoas.map((p) => (
              <tr key={p.pessoaId}>
                <td>{p.nome}</td>
                <td className="valor-positivo">{formatarMoeda(p.totalReceitas)}</td>
                <td className="valor-negativo">{formatarMoeda(p.totalDespesas)}</td>
                <td className={p.saldo >= 0 ? "valor-positivo" : "valor-negativo"}>
                  {formatarMoeda(p.saldo)}
                </td>
              </tr>
            ))}
          </tbody>
          <tfoot>
            <tr>
              <td><strong>Total geral</strong></td>
              <td className="valor-positivo">
                <strong>{formatarMoeda(totais.totalGeralReceitas)}</strong>
              </td>
              <td className="valor-negativo">
                <strong>{formatarMoeda(totais.totalGeralDespesas)}</strong>
              </td>
              <td className={totais.saldoLiquidoGeral >= 0 ? "valor-positivo" : "valor-negativo"}>
                <strong>{formatarMoeda(totais.saldoLiquidoGeral)}</strong>
              </td>
            </tr>
          </tfoot>
        </table>
      </div>
    </section>
  );
}