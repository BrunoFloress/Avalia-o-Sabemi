using System.Threading.Channels;
using Sabemi.webhook.Api.DTOs;

namespace Sabemi.webhook.Api.Services;

public class FilaPagamentoService : IFilaPagamentoService
{
    private readonly Channel<PagamentoWebhookDto> _channel = Channel.CreateUnbounded<PagamentoWebhookDto>();

    public ChannelReader<PagamentoWebhookDto> Reader => _channel.Reader;

    public async ValueTask EnfileirarAsync(PagamentoWebhookDto evento, CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(evento, ct);
    }
}
