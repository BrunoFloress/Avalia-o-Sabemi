using Microsoft.Extensions.Configuration;
using Sabemi.webhook.Api.Services;
using Xunit;

namespace Sabemi.webhook.Api.Tests;

public class ValidacaoWebhookServiceTests
{
    private static IConfiguration CriarConfig(string apiKeyEsperada = "sabemi-secret-2026")
    {
        var dict = new Dictionary<string, string?> { { "Webhook:ApiKey", apiKeyEsperada } };
        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    [Fact]
    public void Deve_Validar_ApiKey_Correta()
    {
        var service = new ValidacaoWebhookService(CriarConfig());
        Assert.True(service.ApiKeyValida("sabemi-secret-2026"));
    }

    [Fact]
    public void Deve_Rejeitar_ApiKey_Incorreta()
    {
        var service = new ValidacaoWebhookService(CriarConfig());
        Assert.False(service.ApiKeyValida("chave-errada"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Deve_Rejeitar_ApiKey_Ausente(string? chave)
    {
        var service = new ValidacaoWebhookService(CriarConfig());
        Assert.False(service.ApiKeyValida(chave));
    }
}
