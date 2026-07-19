interface Props {
  aberto: boolean;
  titulo: string;
  mensagem: string;
  aoConfirmar: () => void;
  aoCancelar: () => void;
}

/*
  Modal de confirmação genérico e reutilizável.
 */
export function ConfirmacaoModal({ aberto, titulo, mensagem, aoConfirmar, aoCancelar }: Props) {
  if (!aberto) {
    return null;
  }

  return (
    <div className="modal-fundo" onClick={aoCancelar}>
      <div className="modal-caixa" onClick={(e) => e.stopPropagation()}>
        <h3>{titulo}</h3>
        <p>{mensagem}</p>
        <div className="modal-acoes">
          <button className="botao-secundario" onClick={aoCancelar}>
            Cancelar
          </button>
          <button className="botao-perigo-solido" onClick={aoConfirmar}>
            Excluir
          </button>
        </div>
      </div>
    </div>
  );
}