namespace Sabemi.webhook.Api.Models;

public class EventoLog
{
    public int Id { get; set; }
    public string IdTransacao { get; set; } = default!;
    public string PayloadBruto { get; set; } = default!; // salvo como JSON serializado (string)
    public DateTime RecebidoEm { get; set; } = DateTime.UtcNow;
    public bool Processado { get; set; } = false;
    public string StatusProcessamento { get; set; } = "pendente"; // pendente | sucesso | erro
    public string? MensagemErro { get; set; }
}
