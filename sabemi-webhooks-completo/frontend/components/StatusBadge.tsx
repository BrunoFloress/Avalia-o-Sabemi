import { StatusPagamento } from "@/types";

const estilos: Record<StatusPagamento, string> = {
  Sucesso: "border-success/40 text-success bg-success/10",
  Erro: "border-error/40 text-error bg-error/10",
  Pendente: "border-pending/40 text-pending bg-pending/10",
};

export function StatusBadge({ status }: { status: StatusPagamento }) {
  return (
    <span
      className={`inline-flex items-center gap-1.5 rounded-sm border px-2 py-0.5 font-mono text-xs uppercase tracking-wider ${estilos[status] ?? estilos.Pendente}`}
    >
      <span className="h-1.5 w-1.5 rounded-full bg-current" />
      {status}
    </span>
  );
}
