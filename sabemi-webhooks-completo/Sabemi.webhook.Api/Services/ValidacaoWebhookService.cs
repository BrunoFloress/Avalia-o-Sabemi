namespace Sabemi.webhook.Api.Services;

public class ValidacaoWebhookService : IValidacaoWebhookService
{
    private readonly IConfiguration _config;

    public ValidacaoWebhookService(IConfiguration config)
    {
        _config = config;
    }

    public bool ApiKeyValida(string? apiKeyRecebida)
    {
        var apiKeyEsperada = _config["Webhook:ApiKey"];
        return !string.IsNullOrEmpty(apiKeyRecebida) && apiKeyRecebida == apiKeyEsperada;
    }
}
