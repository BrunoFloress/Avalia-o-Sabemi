export type StatusPagamento = "Sucesso" | "Erro" | "Pendente";

export interface StatusContrato {
  idContrato: string;
  ultimoIdTransacao: string;
  valor: number;
  dataPagamento: string; // ISO
  status: StatusPagamento;
  atualizadoEm: string; // ISO
}

export interface FiltroPagamentos {
  status?: string;
  idContrato?: string;
}
