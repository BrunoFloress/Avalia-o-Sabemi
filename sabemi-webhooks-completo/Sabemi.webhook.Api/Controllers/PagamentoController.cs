using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sabemi.webhook.Api.Data;
using Sabemi.webhook.Api.DTOs;
using Sabemi.webhook.Api.Models;
using Sabemi.webhook.Api.Services;

namespace Sabemi.webhook.Api.Controllers;

[ApiController]
[Route("webhooks")]
public class PagamentoController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IFilaPagamentoService _fila;
    private readonly IValidacaoWebhookService _validacao;

    public PagamentoController(AppDbContext db, IFilaPagamentoService fila, IValidacaoWebhookService validacao)
    {
        _db = db;
        _fila = fila;
        _validacao = validacao;
    }

    [HttpPost("pagamento")]
    public async Task<IActionResult> Receber(
        [FromBody] PagamentoWebhookDto dto,
        [FromHeader(Name = "X-Api-Key")] string? apiKey)
    {
        if (!_validacao.ApiKeyValida(apiKey))
            return Unauthorized(new { erro = "ApiKey inválida ou ausente" });

        var jaExiste = await _db.EventosLog.AnyAsync(e => e.IdTransacao == dto.IdTransacao);
        if (jaExiste)
            return Ok(new { mensagem = "Evento já recebido anteriormente (idempotência)" });

        var log = new EventoLog
        {
            IdTransacao = dto.IdTransacao,
            PayloadBruto = JsonSerializer.Serialize(dto),
            Processado = false,
            StatusProcessamento = "pendente"
        };

        _db.EventosLog.Add(log);
        await _db.SaveChangesAsync();

        await _fila.EnfileirarAsync(dto);

        return Accepted(new { mensagem = "Recebido, processamento em andamento" });
    }

    [HttpGet("status")]
    public async Task<IActionResult> Listar([FromQuery] string? status, [FromQuery] string? idContrato)
    {
        var query = _db.StatusContratos.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(s => s.Status == status);

        if (!string.IsNullOrEmpty(idContrato))
            query = query.Where(s => s.IdContrato.Contains(idContrato));

        var resultado = await query
            .OrderByDescending(s => s.AtualizadoEm)
            .Select(s => new StatusContratoResponseDto
            {
                IdContrato = s.IdContrato,
                UltimoIdTransacao = s.UltimoIdTransacao,
                Valor = s.Valor,
                DataPagamento = s.DataPagamento,
                Status = s.Status,
                AtualizadoEm = s.AtualizadoEm
            })
            .ToListAsync();

        return Ok(resultado);
    }
}
