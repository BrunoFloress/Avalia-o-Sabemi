namespace Sabemi.webhook.Api.Models;

public class StatusContrato
{
    public string IdContrato { get; set; } = default!; // chave primária
    public string UltimoIdTransacao { get; set; } = default!;
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; }
    public string Status { get; set; } = default!; // Sucesso | Erro
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}
