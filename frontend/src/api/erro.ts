import axios from "axios";
import type { ErroApi } from "../types";

export function extrairMensagemErro(erro: unknown, mensagemPadrao: string): string {
  if (axios.isAxiosError(erro)) {
    const dados = erro.response?.data as ErroApi | { errors?: Record<string, string[]> } | undefined;

    if (dados && "mensagem" in dados && dados.mensagem) {
      return dados.mensagem;
    }

    if (dados && "errors" in dados && dados.errors) {
      const primeiraChave = Object.keys(dados.errors)[0];
      return dados.errors[primeiraChave]?.[0] ?? mensagemPadrao;
    }
  }

  return mensagemPadrao;
}