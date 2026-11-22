# Wallet Management API

A RESTful API built with ASP.NET Core (.NET 10), Entity Framework Core and PostgreSQL for managing digital wallets and their transactions. The solution also contains a small console client that consumes the API and an xUnit test suite.

## Solution layout

```
WalletManagement/
├── WalletManagement.slnx
├── WalletManagement.Api/        # Web API
│   ├── Controllers/             # WalletsController, WalletQueriesController
│   ├── DTOs/                    # Request/response contracts
│   ├── Data/                    # WalletDbContext
│   ├── Middleware/              # Exception handling
│   ├── Migrations/              # EF Core migrations (schema + seed data)
│   ├── Models/                  # Wallet, Transaction
│   ├── Queries/                 # Read-only reporting queries
│   ├── Services/                # Wallet business logic
│   └── WalletManagement.Api.http  # Ready-to-run sample requests
├── WalletManagement.Client/     # Console client that consumes the API
└── WalletManagement.Tests/      # xUnit tests (in-memory EF Core)
```

## Technologies

C#, ASP.NET Core Web API (.NET 10), Entity Framework Core 10, PostgreSQL (Npgsql), xUnit, EF Core Migrations.

## Getting started

### Prerequisites

- .NET 10 SDK
- PostgreSQL
- Visual Studio 2026, VS Code, or the .NET CLI

### 1. Clone and restore

```bash
git clone https://github.com/Kelvinjaston/week5-efcore-wallet-api.git
cd week5-efcore-wallet-api
dotnet restore
```

### 2. Configure the database

Create a PostgreSQL database named `WalletManagementDb`, then store your connection string in user secrets so the password never goes into Git:

```bash
cd WalletManagement.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=WalletManagementDb;Username=postgres;Password=<your-password>"
cd ..
```

`appsettings.json` only contains a placeholder. Never commit real credentials.

### 3. Apply migrations

```bash
dotnet tool install --global dotnet-ef --version 10.0.0
dotnet ef database update --project WalletManagement.Api
```

### 4. Run the API

```bash
dotnet run --project WalletManagement.Api
```

The console prints the listening address (for example `http://localhost:5181`). In Development, the OpenAPI document is served at `/openapi/v1.json` (and Swagger UI at `/swagger` if enabled in `Program.cs`).

### 5. Run the client

With the API running:

```bash
dotnet run --project WalletManagement.Client
# or against another address:
dotnet run --project WalletManagement.Client -- http://localhost:5181
```

The client creates a wallet, deposits, withdraws (including one request that is expected to fail), then reads the wallet back and calls two query endpoints.

### 6. Run the tests

```bash
dotnet test
```

## API reference

Base URL: `http://localhost:5181` (use the address printed by your run). Routes are case-insensitive. Requests and responses are JSON; property names are camelCase. Example values below are illustrative.

On Windows PowerShell use `curl.exe` instead of `curl`, or use `WalletManagement.Api/WalletManagement.Api.http`, Swagger, or Postman.

### Wallets: `/api/Wallets`

| Method | Route | Description | Success | Errors |
|---|---|---|---|---|
| GET | `/api/Wallets` | List all wallets with their transactions | 200 | |
| GET | `/api/Wallets/{id}` | Get one wallet | 200 | 404 |
| POST | `/api/Wallets` | Create a wallet | 201 (with `Location`) | 400 |
| POST | `/api/Wallets/{id}/deposit` | Add money to a wallet | 200 | 400 |
| POST | `/api/Wallets/{id}/withdraw` | Take money from a wallet | 200 | 400 |

**Create a wallet**

```bash
curl -X POST http://localhost:5181/api/Wallets \
  -H "Content-Type: application/json" \
  -d '{ "ownerName": "Ada Lovelace", "initialBalance": 1000 }'
```

Response `201 Created`:

```json
{
  "id": 4,
  "ownerName": "Ada Lovelace",
  "balance": 1000,
  "createdAt": "2026-11-22T15:04:05.123Z",
  "transactions": [
    { "id": 7, "walletId": 4, "type": "Credit", "amount": 1000, "createdAt": "2026-11-22T15:04:05.123Z" }
  ]
}
```

Validation: `ownerName` is required (2 to 100 characters); `initialBalance` is 0 to 1,000,000,000 (default 0). A positive initial balance records a `Credit` transaction. Invalid input returns `400` in the standard ASP.NET problem-details format:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": { "OwnerName": ["The field OwnerName must be a string with a minimum length of 2 and a maximum length of 100."] }
}
```

**List wallets / get one wallet**

```bash
curl http://localhost:5181/api/Wallets
curl http://localhost:5181/api/Wallets/4
```

Both return wallet objects shaped like the response above. An unknown id returns `404 Not Found`.

**Deposit**

```bash
curl -X POST http://localhost:5181/api/Wallets/4/deposit \
  -H "Content-Type: application/json" \
  -d '{ "amount": 250 }'
```

Response `200 OK`:

```json
{ "message": "Deposit successful" }
```

`amount` must be between 0.01 and 1,000,000,000, otherwise `400` with `{"errors":{"Amount":["Amount must be greater than zero."]}}`. A deposit records a `Credit` transaction.

**Withdraw**

```bash
curl -X POST http://localhost:5181/api/Wallets/4/withdraw \
  -H "Content-Type: application/json" \
  -d '{ "amount": 100 }'
```

Response `200 OK`:

```json
{ "message": "Withdrawal successful" }
```

A withdrawal records a `Debit` transaction. If the wallet does not exist, the amount is invalid, or the balance is too low, the API returns `400 Bad Request` with the text `Withdrawal failed. Verify wallet ID, amount, or insufficient balance.` (deposit failures return a similar `Deposit failed...` message).

### Wallet queries: `/api/WalletQueries`

All query endpoints are `GET` and return `200 OK` with JSON.

| Route | Query parameters | Description |
|---|---|---|
| `/high-value` | `minBalance` (default 1000) | Wallets with a balance above the threshold |
| `/liquidity` | none | Total balance held across all wallets |
| `/most-active` | `topCount` (default 5) | Wallets with the most transactions |
| `/top-transactions` | `topCount` (default 5) | Largest transactions |
| `/dormant` | none | Dormant wallets (no recent activity) |
| `/cash-flow` | none | Cash flow per wallet (money in vs money out) |
| `/{walletId}/transactions-range` | `startDate`, `endDate` (UTC, ISO 8601) | One wallet's transactions between two dates |
| `/recent-high-balance` | `minBalance` (default 500), `days` (default 30) | High-balance wallets active in the last N days |

**Examples**

```bash
curl "http://localhost:5181/api/WalletQueries/high-value?minBalance=1000"
curl http://localhost:5181/api/WalletQueries/liquidity
curl "http://localhost:5181/api/WalletQueries/1/transactions-range?startDate=2026-01-01T00:00:00Z&endDate=2026-12-31T23:59:59Z"
```

`/liquidity` returns:

```json
{ "totalLiquidity": 1800 }
```

## Data models

**Wallet**: `Id`, `OwnerName`, `Balance`, `CreatedAt`, `Transactions` (collection).

**Transaction**: `Id`, `WalletId` (foreign key), `Wallet` (navigation), `Type` (`Credit` or `Debit`), `Amount`, `CreatedAt`.

A wallet has many transactions; each transaction belongs to one wallet.

## Notes and known limitations

- Deposit and withdraw return `400` for every failure, including an unknown wallet id (not `404`).
- Concurrent withdrawals are not protected by a concurrency token, so two simultaneous requests could overdraw a wallet.
- Use UTC timestamps (ending in `Z`) for date parameters.

## Learning objectives

- Understanding the Data Access Layer and Entity Framework Core as an ORM
- Modelling entities, relationships and PostgreSQL connectivity
- Creating and applying migrations
- Organising an API with controllers, DTOs and services
- Exposing a domain as a REST API and consuming it from a client
- Writing automated tests in a multi-project `.slnx` solution

## Future improvements

- Add a concurrency token to prevent overdrafts under load
- Return specific status codes (`404`, `409`) for different failure reasons
- Add Docker Compose support
