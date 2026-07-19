import { useState } from "react";
import { PessoasPage } from "./components/PessoasPage";
import { TransacoesPage } from "./components/TransacoesPage";
import { TotaisPage } from "./components/TotaisPage";

type Aba = "pessoas" | "transacoes" | "totais";

const ABAS: { chave: Aba; rotulo: string }[] = [
  { chave: "pessoas", rotulo: "Pessoas" },
  { chave: "transacoes", rotulo: "Transações" },
  { chave: "totais", rotulo: "Totais" },
];

function App() {
  const [abaAtiva, setAbaAtiva] = useState<Aba>("pessoas");

  return (
    <div className="app">
      <header className="cabecalho">
        <h1>Controle de Gastos Residenciais</h1>
        <nav className="abas">
          {ABAS.map((aba) => (
            <button
              key={aba.chave}
              className={aba.chave === abaAtiva ? "aba aba-ativa" : "aba"}
              onClick={() => setAbaAtiva(aba.chave)}
            >
              {aba.rotulo}
            </button>
          ))}
        </nav>
      </header>

      <main>
        {abaAtiva === "pessoas" && <PessoasPage />}
        {abaAtiva === "transacoes" && <TransacoesPage />}
        {abaAtiva === "totais" && <TotaisPage />}
      </main>
    </div>
  );
}

export default App;