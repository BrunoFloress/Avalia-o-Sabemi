"use client";

import { useCallback, useEffect, useState } from "react";
import { FilterBar } from "@/components/FilterBar";
import { PaymentsTable } from "@/components/PaymentsTable";
import { buscarPagamentos } from "@/lib/api";
import { FiltroPagamentos, StatusContrato } from "@/types";

const INTERVALO_REFRESH_MS = 5000;

export default function DashboardPage() {
  const [filtro, setFiltro] = useState<FiltroPagamentos>({});
  const [dados, setDados] = useState<StatusContrato[]>([]);
  const [carregando, setCarregando] = useState(false);
  const [erroConexao, setErroConexao] = useState<string | null>(null);
  const [ultimaAtualizacao, setUltimaAtualizacao] = useState<Date | null>(null);

  const carregar = useCallback(async () => {
    setCarregando(true);
    try {
      const resultado = await buscarPagamentos(filtro);
      setDados(resultado);
      setErroConexao(null);
      setUltimaAtualizacao(new Date());
    } catch (err) {
      setErroConexao(err instanceof Error ? err.message : "Erro desconhecido ao conectar à API");
    } finally {
      setCarregando(false);
    }
  }, [filtro]);

  useEffect(() => {
    carregar();
    const intervalo = setInterval(carregar, INTERVALO_REFRESH_MS);
    return () => clearInterval(intervalo);
  }, [carregar]);

  const totalErros = dados.filter((d) => d.status === "Erro").length;

  return (
    <main className="mx-auto max-w-6xl px-6 py-10">
      <header className="mb-8 flex items-end justify-between">
        <div>
          <p className="mb-1 font-mono text-xs uppercase tracking-[0.2em] text-accent">
            Sabemi · Webhooks
          </p>
          <h1 className="text-2xl font-semibold text-ink">Notificações de pagamento</h1>
          <p className="mt-1 text-sm text-muted">
            Eventos recebidos do banco parceiro, conciliados em tempo real.
          </p>
        </div>
        {ultimaAtualizacao && (
          <p className="font-mono text-xs text-muted">
            Última atualização: {ultimaAtualizacao.toLocaleTimeString("pt-BR")}
          </p>
        )}
      </header>

      {erroConexao && (
        <div className="mb-6 rounded-sm border border-error/40 bg-error/10 px-4 py-3 text-sm text-error">
          Não foi possível conectar à API ({erroConexao}). Verifique se o backend está em execução.
        </div>
      )}

      {totalErros > 0 && (
        <div className="mb-6 rounded-sm border border-error/40 bg-error/10 px-4 py-3 text-sm text-error">
          {totalErros} evento{totalErros > 1 ? "s" : ""} com falha de validação — revise antes de encerrar o dia.
        </div>
      )}

      <div className="mb-6">
        <FilterBar filtro={filtro} onChange={setFiltro} onRefresh={carregar} carregando={carregando} />
      </div>

      <PaymentsTable dados={dados} />
    </main>
  );
}
