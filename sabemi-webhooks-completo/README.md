# Sabemi · Webhooks de Pagamento

Projeto completo: backend (.NET 8), frontend (Next.js) e ambiente de testes,
já organizados na estrutura correta para abrir direto no VS Code.

## Estrutura

```
sabemi-webhooks-completo/
├── sabemi-webhooks.code-workspace   ← abra ESTE arquivo no VS Code
├── Sabemi.webhook.sln               ← solução .NET (backend + testes)
├── Sabemi.webhook.Api/              ← backend
│   ├── Background/
│   │   └── ProcessadorPagamentoWorker.cs
│   ├── Controllers/
│   │   └── PagamentoController.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── DTOs/
│   │   ├── PagamentoWebhookDto.cs
│   │   └── StatusContratoResponseDto.cs
│   ├── Models/
│   │   ├── EventoLog.cs
│   │   └── StatusContrato.cs
│   ├── Services/
│   │   ├── IFilaPagamentoService.cs
│   │   ├── FilaPagamentoService.cs
│   │   ├── IValidacaoWebhookService.cs
│   │   └── ValidacaoWebhookService.cs
│   ├── Properties/launchSettings.json  ← fixa a porta em 5000
│   ├── Program.cs
│   ├── appsettings.json
│   └── Sabemi.webhook.Api.csproj
├── Sabemi.webhook.Api.Tests/        ← testes automatizados (xUnit)
├── frontend/                        ← dashboard Next.js
└── testing/                         ← testes manuais (.http, docker-compose, script)
```

## Passo a passo (na ordem)

### 1. Abrir o projeto
Abra o arquivo **`sabemi-webhooks.code-workspace`** no VS Code (`File > Open Workspace from File...`).
Isso já mostra backend, testes e frontend organizados como pastas separadas, sem misturar.

### 2. Subir o banco
```bash
docker run --name sabemi-db -e POSTGRES_PASSWORD=senha -p 5432:5432 -d postgres:16
```

### 3. Restaurar e migrar o backend
```bash
cd Sabemi.webhook.Api
dotnet restore
dotnet tool install --global dotnet-ef   # se ainda não tiver
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Rodar o backend
```bash
dotnet run
```
Sobe fixo em `http://localhost:5000` (definido em `Properties/launchSettings.json`,
já casando com o frontend e os testes — não precisa ajustar porta em nenhum outro lugar).

### 5. Validar o backend isoladamente
Abra `testing/webhook-requests.http` (extensão **REST Client**) e clique em
"Send Request" no primeiro bloco. Deve retornar `202 Accepted`.

### 6. Rodar os testes automatizados
```bash
cd ../Sabemi.webhook.Api.Tests
dotnet test
```

### 7. Rodar o frontend
```bash
cd ../frontend
cp .env.local.example .env.local
npm install
npm run dev
```
Abra `http://localhost:3000`.

### 8. Teste de concorrência (opcional, mas recomendado)
```bash
cd ../testing
chmod +x teste-idempotencia-concorrente.sh
./teste-idempotencia-concorrente.sh
```
Dispara 10 requisições simultâneas com o mesmo `idTransacao` para provar que a
idempotência resiste a reenvio em paralelo (cenário real de retry de rede do banco).

## Erros comuns

- **`dotnet test` não acha o projeto principal** → confira se `Sabemi.webhook.Api.Tests`
  está no mesmo nível de `Sabemi.webhook.Api` (irmãs, não uma dentro da outra).
- **Frontend não conecta (`Failed to fetch`)** → confirme que o backend está rodando em
  `http://localhost:5000` e que `frontend/.env.local` aponta para essa mesma URL.
- **CORS bloqueado** → o `Program.cs` libera só `http://localhost:3000`; se o Next subir
  em outra porta, ajuste `WithOrigins(...)` nesse arquivo.
