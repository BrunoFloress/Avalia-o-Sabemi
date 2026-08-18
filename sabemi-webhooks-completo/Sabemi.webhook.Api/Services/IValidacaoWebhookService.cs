namespace Sabemi.webhook.Api.Services;

public interface IValidacaoWebhookService
{
    bool ApiKeyValida(string? apiKeyRecebida);
}
