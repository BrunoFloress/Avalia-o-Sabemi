import { FiltroPagamentos, StatusContrato } from "@/types";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

export async function buscarPagamentos(
  filtro: FiltroPagamentos
): Promise<StatusContrato[]> {
  const params = new URLSearchParams();
  if (filtro.status) params.set("status", filtro.status);
  if (filtro.idContrato) params.set("idContrato", filtro.idContrato);

  const res = await fetch(`${API_BASE_URL}/webhooks/status?${params.toString()}`, {
    cache: "no-store",
  });

  if (!res.ok) {
    throw new Error(`Falha ao buscar pagamentos (HTTP ${res.status})`);
  }

  return res.json();
}
