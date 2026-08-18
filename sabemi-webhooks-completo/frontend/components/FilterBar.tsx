"use client";

import { FiltroPagamentos } from "@/types";

interface Props {
  filtro: FiltroPagamentos;
  onChange: (filtro: FiltroPagamentos) => void;
  onRefresh: () => void;
  carregando: boolean;
}

export function FilterBar({ filtro, onChange, onRefresh, carregando }: Props) {
  return (
    <div className="flex flex-wrap items-end gap-4 border-b border-border pb-5">
      <div className="flex flex-col gap-1.5">
        <label className="text-xs uppercase tracking-wider text-muted">Status</label>
        <select
          value={filtro.status ?? ""}
          onChange={(e) => onChange({ ...filtro, status: e.target.value || undefined })}
          className="w-44 rounded-sm border border-border bg-surfaceAlt px-3 py-2 text-sm text-ink outline-none focus:border-accent"
        >
          <option value="">Todos</option>
          <option value="Sucesso">Sucesso</option>
          <option value="Erro">Erro</option>
          <option value="Pendente">Pendente</option>
        </select>
      </div>

      <div className="flex flex-col gap-1.5">
        <label className="text-xs uppercase tracking-wider text-muted">ID do contrato</label>
        <input
          value={filtro.idContrato ?? ""}
          onChange={(e) => onChange({ ...filtro, idContrato: e.target.value || undefined })}
          placeholder="Ex: CT-00123"
          className="w-52 rounded-sm border border-border bg-surfaceAlt px-3 py-2 font-mono text-sm text-ink placeholder:text-muted/60 outline-none focus:border-accent"
        />
      </div>

      <button
        onClick={onRefresh}
        disabled={carregando}
        className="ml-auto flex items-center gap-2 rounded-sm border border-accent/40 bg-accent/10 px-4 py-2 text-sm text-accent transition hover:bg-accent/20 disabled:opacity-50"
      >
        {carregando ? "Atualizando…" : "Atualizar"}
      </button>
    </div>
  );
}
