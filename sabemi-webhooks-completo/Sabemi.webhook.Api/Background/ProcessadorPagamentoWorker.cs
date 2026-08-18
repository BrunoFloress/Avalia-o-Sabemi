using Microsoft.EntityFrameworkCore;
using Sabemi.webhook.Api.Data;
using Sabemi.webhook.Api.Models;
using Sabemi.webhook.Api.Services;

namespace Sabemi.webhook.Api.Background;

public class ProcessadorPagamentoWorker : BackgroundService
{
    private readonly IFilaPagamentoService _fila;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ProcessadorPagamentoWorker> _logger;

    public ProcessadorPagamentoWorker(
        IFilaPagamentoService fila,
        IServiceScopeFactory scopeFactory,
        ILogger<ProcessadorPagamentoWorker> logger)
    {
        _fila = fila;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var evento in _fila.Reader.ReadAllAsync(stoppingToken))
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                // simula processamento pesado da regra de negócio
                await Task.Delay(2000, stoppingToken);

                var statusContrato = await db.StatusContratos
                    .FirstOrDefaultAsync(s => s.IdContrato == evento.IdContrato, stoppingToken)
                    ?? new StatusContrato { IdContrato = evento.IdContrato };

                statusContrato.UltimoIdTransacao = evento.IdTransacao;
                statusContrato.Valor = evento.Valor;
                statusContrato.DataPagamento = evento.DataPagamento;
                statusContrato.Status = evento.Status;
                statusContrato.AtualizadoEm = DateTime.UtcNow;

                if (db.Entry(statusContrato).State == EntityState.Detached)
                    db.StatusContratos.Add(statusContrato);

                var log = await db.EventosLog
                    .FirstAsync(l => l.IdTransacao == evento.IdTransacao, stoppingToken);
                log.Processado = true;
                log.StatusProcessamento = "sucesso";

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar transação {IdTransacao}", evento.IdTransacao);

                var log = await db.EventosLog
                    .FirstOrDefaultAsync(l => l.IdTransacao == evento.IdTransacao, stoppingToken);
                if (log != null)
                {
                    log.Processado = true;
                    log.StatusProcessamento = "erro";
                    log.MensagemErro = ex.Message;
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
        }
    }
}
