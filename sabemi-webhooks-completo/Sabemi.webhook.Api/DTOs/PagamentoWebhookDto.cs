using System.ComponentModel.DataAnnotations;

namespace Sabemi.webhook.Api.DTOs;

public class PagamentoWebhookDto
{
    [Required]
    public string IdTransacao { get; set; } = default!;

    [Required]
    public string IdContrato { get; set; } = default!;

    [Required]
    public decimal Valor { get; set; }

    [Required]
    public DateTime DataPagamento { get; set; }

    [Required]
    public string Status { get; set; } = default!;
}
