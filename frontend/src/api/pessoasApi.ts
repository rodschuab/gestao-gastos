import { apiClient } from "./cliente";

import type { NovaPessoa, Pessoa } from "../types";

/* GET /api/pessoas — lista todas as pessoas cadastradas. */
export async function listarPessoas(): Promise<Pessoa[]> {
  const { data } = await apiClient.get<Pessoa[]>("/pessoas");
  return data;
}

/* POST /api/pessoas — cria uma nova pessoa. */
export async function criarPessoa(pessoa: NovaPessoa): Promise<Pessoa> {
  const { data } = await apiClient.post<Pessoa>("/pessoas", pessoa);
  return data;
}

/*
  DELETE /api/pessoas/{id} — remove uma pessoa.
  O backend cuida da exclusão em cascata das transações vinculadas.
 */
export async function deletarPessoa(id: string): Promise<void> {
  await apiClient.delete(`/pessoas/${id}`);
}