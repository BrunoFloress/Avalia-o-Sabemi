namespace Sabemi.webhook.Api.DTOs;

public class StatusContratoResponseDto
{
    public string IdContrato { get; set; } = default!;
    public string UltimoIdTransacao { get; set; } = default!;
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; }
    public string Status { get; set; } = default!;
    public DateTime AtualizadoEm { get; set; }
}
