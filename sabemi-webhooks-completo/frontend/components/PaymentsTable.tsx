import { StatusContrato } from "@/types";
import { StatusBadge } from "./StatusBadge";

function formatarMoeda(valor: number) {
  return new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(valor);
}

function formatarData(iso: string) {
  return new Date(iso).toLocaleString("pt-BR");
}

export function PaymentsTable({ dados }: { dados: StatusContrato[] }) {
  if (dados.length === 0) {
    return (
      <div className="flex flex-col items-center gap-2 rounded-sm border border-dashed border-border py-16 text-center">
        <p className="text-sm text-ink">Nenhum evento encontrado</p>
        <p className="text-xs text-muted">Ajuste os filtros ou aguarde a próxima notificação do banco.</p>
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-sm border border-border">
      <table className="w-full border-collapse text-sm">
        <thead>
          <tr className="border-b border-border bg-surfaceAlt text-left text-xs uppercase tracking-wider text-muted">
            <th className="px-4 py-3 font-medium">Contrato</th>
            <th className="px-4 py-3 font-medium">Última transação</th>
            <th className="px-4 py-3 font-medium">Valor</th>
            <th className="px-4 py-3 font-medium">Data pagamento</th>
            <th className="px-4 py-3 font-medium">Status</th>
            <th className="px-4 py-3 font-medium">Atualizado em</th>
          </tr>
        </thead>
        <tbody>
          {dados.map((item) => {
            const comErro = item.status === "Erro";
            return (
              <tr
                key={item.idContrato}
                className={`border-b border-border last:border-0 ${
                  comErro ? "bg-error/5" : "hover:bg-surfaceAlt"
                }`}
              >
                <td className="px-4 py-3 font-mono text-ink">
                  <span className="flex items-center gap-2">
                    {comErro && <span aria-hidden className="text-error">▲</span>}
                    {item.idContrato}
                  </span>
                </td>
                <td className="px-4 py-3 font-mono text-muted">{item.ultimoIdTransacao}</td>
                <td className="px-4 py-3 font-mono text-ink">{formatarMoeda(item.valor)}</td>
                <td className="px-4 py-3 text-muted">{formatarData(item.dataPagamento)}</td>
                <td className="px-4 py-3">
                  <StatusBadge status={item.status} />
                  {comErro && (
                    <p className="mt-1 text-xs text-error/80">
                      Falha na validação do evento — verifique o log bruto.
                    </p>
                  )}
                </td>
                <td className="px-4 py-3 text-muted">{formatarData(item.atualizadoEm)}</td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}
