export interface Pessoa {
  id: string;
  nome: string;
  idade: number;
}

export type NovaPessoa = Omit<Pessoa, "id">;

export type TipoTransacao = "Receita" | "Despesa";

export interface Transacao {
  id: string;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  pessoaId: string;
  pessoaNome?: string;
}

export interface NovaTransacao {
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  pessoaId: string;
}

export interface TotalPorPessoa {
  pessoaId: string;
  nome: string;
  idade: number;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
}

export interface ConsultaTotais {
  pessoas: TotalPorPessoa[];
  totalGeralReceitas: number;
  totalGeralDespesas: number;
  saldoLiquidoGeral: number;
}

export interface ErroApi {
  mensagem: string;
}