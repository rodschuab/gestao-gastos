import { apiClient } from "./cliente";
import type { ConsultaTotais } from "../types";

/* GET /api/totais — totais de receitas/despesas/saldo por pessoa + total geral */
export async function consultarTotais(): Promise<ConsultaTotais> {
  const { data } = await apiClient.get<ConsultaTotais>("/totais");
  return data;
}