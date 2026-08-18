using System.Threading.Channels;
using Sabemi.webhook.Api.DTOs;

namespace Sabemi.webhook.Api.Services;

public interface IFilaPagamentoService
{
    ChannelReader<PagamentoWebhookDto> Reader { get; }
    ValueTask EnfileirarAsync(PagamentoWebhookDto evento, CancellationToken ct = default);
}
