using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sabemi.webhook.Api.Controllers;
using Sabemi.webhook.Api.Data;
using Sabemi.webhook.Api.DTOs;
using Sabemi.webhook.Api.Services;
using Xunit;

namespace Sabemi.webhook.Api.Tests;

public class IdempotenciaTests
{
    private static AppDbContext CriarDbEmMemoria(string nomeBanco)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;
        return new AppDbContext(options);
    }

    private static PagamentoWebhookDto CriarDto(string idTransacao = "TX-001") => new()
    {
        IdTransacao = idTransacao,
        IdContrato = "CT-123",
        Valor = 199.90m,
        DataPagamento = DateTime.UtcNow,
        Status = "Sucesso"
    };

    [Fact]
    public async Task Deve_Aceitar_Primeira_Notificacao_De_Uma_Transacao()
    {
        // Arrange
        await using var db = CriarDbEmMemoria(nameof(Deve_Aceitar_Primeira_Notificacao_De_Uma_Transacao));
        var filaMock = new Mock<IFilaPagamentoService>();
        var validacaoMock = new Mock<IValidacaoWebhookService>();
        validacaoMock.Setup(v => v.ApiKeyValida(It.IsAny<string>())).Returns(true);

        var controller = new PagamentoController(db, filaMock.Object, validacaoMock.Object);

        // Act
        var resultado = await controller.Receber(CriarDto(), "chave-valida");

        // Assert
        Assert.IsType<AcceptedResult>(resultado);
        Assert.Equal(1, await db.EventosLog.CountAsync());
        filaMock.Verify(f => f.EnfileirarAsync(It.IsAny<PagamentoWebhookDto>(), default), Times.Once);
    }

    [Fact]
    public async Task Nao_Deve_Reprocessar_Mesma_Transacao_Enviada_Duas_Vezes()
    {
        // Arrange — simula o banco reenviando a mesma notificação por erro de rede
        await using var db = CriarDbEmMemoria(nameof(Nao_Deve_Reprocessar_Mesma_Transacao_Enviada_Duas_Vezes));
        var filaMock = new Mock<IFilaPagamentoService>();
        var validacaoMock = new Mock<IValidacaoWebhookService>();
        validacaoMock.Setup(v => v.ApiKeyValida(It.IsAny<string>())).Returns(true);

        var controller = new PagamentoController(db, filaMock.Object, validacaoMock.Object);
        var dto = CriarDto("TX-DUPLICADA");

        // Act — mesma transação chega duas vezes
        await controller.Receber(dto, "chave-valida");
        var segundaResposta = await controller.Receber(dto, "chave-valida");

        // Assert
        Assert.IsType<OkObjectResult>(segundaResposta); // não é erro, mas também não reprocessa
        Assert.Equal(1, await db.EventosLog.CountAsync()); // só um registro no log
        filaMock.Verify(f => f.EnfileirarAsync(It.IsAny<PagamentoWebhookDto>(), default), Times.Once); // só enfileirou 1x
    }

    [Fact]
    public async Task Deve_Rejeitar_Requisicao_Sem_ApiKey_Valida()
    {
        await using var db = CriarDbEmMemoria(nameof(Deve_Rejeitar_Requisicao_Sem_ApiKey_Valida));
        var filaMock = new Mock<IFilaPagamentoService>();
        var validacaoMock = new Mock<IValidacaoWebhookService>();
        validacaoMock.Setup(v => v.ApiKeyValida(It.IsAny<string>())).Returns(false);

        var controller = new PagamentoController(db, filaMock.Object, validacaoMock.Object);

        var resultado = await controller.Receber(CriarDto(), "chave-invalida");

        Assert.IsType<UnauthorizedObjectResult>(resultado);
        Assert.Equal(0, await db.EventosLog.CountAsync());
        filaMock.Verify(f => f.EnfileirarAsync(It.IsAny<PagamentoWebhookDto>(), default), Times.Never);
    }
}
