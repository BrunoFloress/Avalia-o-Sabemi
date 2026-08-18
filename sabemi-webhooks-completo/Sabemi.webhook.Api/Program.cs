using Microsoft.EntityFrameworkCore;
using Sabemi.webhook.Api.Background;
using Sabemi.webhook.Api.Data;
using Sabemi.webhook.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IFilaPagamentoService, FilaPagamentoService>();
builder.Services.AddScoped<IValidacaoWebhookService, ValidacaoWebhookService>();
builder.Services.AddHostedService<ProcessadorPagamentoWorker>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", p => p
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
