import { apiClient } from "./cliente";
import type { NovaTransacao, Transacao } from "../types";

/* GET /api/transacoes — lista todas as transações  */
export async function listarTransacoes(pessoaId?: string): Promise<Transacao[]> {
  const { data } = await apiClient.get<Transacao[]>("/transacoes", {
    params: pessoaId ? { pessoaId } : undefined,
  });
  return data;
}

 /* POST /api/transacoes cria uma nova transação*/
 
export async function criarTransacao(transacao: NovaTransacao): Promise<Transacao> {
  const { data } = await apiClient.post<Transacao>("/transacoes", transacao);
  return data;
}