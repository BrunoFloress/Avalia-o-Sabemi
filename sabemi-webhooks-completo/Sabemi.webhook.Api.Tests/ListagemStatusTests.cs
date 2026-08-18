using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sabemi.webhook.Api.Controllers;
using Sabemi.webhook.Api.Data;
using Sabemi.webhook.Api.Models;
using Sabemi.webhook.Api.Services;
using Xunit;

namespace Sabemi.webhook.Api.Tests;

public class ListagemStatusTests
{
    private static AppDbContext CriarDbComDados(string nomeBanco)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;
        var db = new AppDbContext(options);

        db.StatusContratos.AddRange(
            new StatusContrato { IdContrato = "CT-001", Status = "Sucesso", Valor = 100, UltimoIdTransacao = "TX-1", DataPagamento = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow },
            new StatusContrato { IdContrato = "CT-002", Status = "Erro", Valor = 250, UltimoIdTransacao = "TX-2", DataPagamento = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow },
            new StatusContrato { IdContrato = "CT-003", Status = "Sucesso", Valor = 75, UltimoIdTransacao = "TX-3", DataPagamento = DateTime.UtcNow, AtualizadoEm = DateTime.UtcNow }
        );
        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task Deve_Filtrar_Por_Status_Erro()
    {
        await using var db = CriarDbComDados(nameof(Deve_Filtrar_Por_Status_Erro));
        var controller = new PagamentoController(db, Mock.Of<IFilaPagamentoService>(), Mock.Of<IValidacaoWebhookService>());

        var resultado = await controller.Listar(status: "Erro", idContrato: null) as OkObjectResult;

        var lista = Assert.IsAssignableFrom<IEnumerable<object>>(resultado!.Value);
        Assert.Single(lista);
    }

    [Fact]
    public async Task Deve_Filtrar_Por_Id_Contrato_Parcial()
    {
        await using var db = CriarDbComDados(nameof(Deve_Filtrar_Por_Id_Contrato_Parcial));
        var controller = new PagamentoController(db, Mock.Of<IFilaPagamentoService>(), Mock.Of<IValidacaoWebhookService>());

        var resultado = await controller.Listar(status: null, idContrato: "CT-00") as OkObjectResult;

        var lista = Assert.IsAssignableFrom<IEnumerable<object>>(resultado!.Value);
        Assert.Equal(3, lista.Count());
    }
}
