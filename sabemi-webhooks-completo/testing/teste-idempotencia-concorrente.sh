#!/usr/bin/env bash
# Simula o cenário real de risco: o banco parceiro reenviando a MESMA notificação
# várias vezes em paralelo por timeout/retry de rede.
# Uso: ./teste-idempotencia-concorrente.sh

set -e

BASE_URL="${BASE_URL:-http://localhost:5000}"
API_KEY="${API_KEY:-sabemi-secret-2026}"
ID_TRANSACAO="TX-CONCORRENTE-$(date +%s)"

echo "Disparando 10 requisições simultâneas com idTransacao=$ID_TRANSACAO"
echo "---------------------------------------------------------------"

for i in $(seq 1 10); do
  curl -s -o /dev/null -w "Tentativa $i -> HTTP %{http_code} (%{time_total}s)\n" \
    -X POST "$BASE_URL/webhooks/pagamento" \
    -H "Content-Type: application/json" \
    -H "X-Api-Key: $API_KEY" \
    -d "{
      \"idTransacao\": \"$ID_TRANSACAO\",
      \"idContrato\": \"CT-CONCORRENCIA\",
      \"valor\": 42.00,
      \"dataPagamento\": \"2026-08-18T12:00:00Z\",
      \"status\": \"Sucesso\"
    }" &
done

wait

echo "---------------------------------------------------------------"
echo "Verifique no banco: deve haver APENAS 1 linha em eventos_log"
echo "para idTransacao = $ID_TRANSACAO"
echo ""
echo "Query de conferência:"
echo "  SELECT COUNT(*) FROM eventos_log WHERE id_transacao = '$ID_TRANSACAO';"
echo "  -- resultado esperado: 1"
