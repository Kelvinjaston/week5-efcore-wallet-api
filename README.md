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

### 3. Apply migrations (optional: the API also does this on startup)

```bash
dotnet tool install --global dotnet-ef --version 10.0.0
dotnet ef database update --project WalletManagement.Api
```

### 4. Run the API

```bash
dotnet run --project WalletManagement.Api
```

The console prints the listening address (for example `http://localhost:5181`). In Development, Swagger UI is at `/swagger` and the OpenAPI document at `/swagger/v1/swagger.json`.

### 5. Run the client

With the API running:

```bash
dotnet run --project WalletManagement.Client
# or against another address:
dotnet run --project WalletManagement.Client -- http://localhost:5181
```

The client creates a wallet, deposits, withdraws (including one request that is expected to fail), then reads the wallet back and calls two query endpoints.

Run it with `race` to start the concurrent-withdrawal demo described below: `dotnet run --project WalletManagement.Client -- race`.

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

**Wallet**: `Id`, `OwnerName`, `Balance`, `CreatedAt`, `Transactions` (collection), `Version` (row version, mapped to PostgreSQL `xmin`).

**Transaction**: `Id`, `WalletId` (foreign key), `Wallet` (navigation), `Type` (`Credit` or `Debit`), `Amount`, `CreatedAt`.

A wallet has many transactions; each transaction belongs to one wallet.

## Notes and known limitations

- Deposit and withdraw return `400` for every failure, including an unknown wallet id (not `404`).
- A request that loses the concurrency check ten times in a row returns an error, and the client should retry (see the concurrency section).
- Use UTC timestamps (ending in `Z`) for date parameters.

## Learning objectives

- Understanding the Data Access Layer and Entity Framework Core as an ORM
- Modelling entities, relationships and PostgreSQL connectivity
- Creating and applying migrations
- Organising an API with controllers, DTOs and services
- Exposing a domain as a REST API and consuming it from a client
- Writing automated tests in a multi-project `.slnx` solution

## Concurrency and transactions (DDIA Chapter 7)

### The problem: lost updates

A withdrawal is a read-modify-write: read the balance, check it, subtract, save. If two requests do this at the same moment, both read the same balance, both pass the check, and the second save overwrites the first. PostgreSQL's default isolation level (Read Committed) does not stop this, because each request is a separate, valid transaction.

I wrote a demo that sends 20 simultaneous withdrawals of 30 to a wallet holding 100. Only three should succeed.

```
dotnet run --project WalletManagement.Client -- race
```

| | Without the fix | With the fix |
|---|---|---|
| Withdrawals that succeeded | 11 | 3 |
| Rejected (insufficient funds) | 9 | 17 |
| Final balance | 10.00 (should be -230 if 11 had really happened) | 10.00 |
| Debit rows in the ledger | 11 | 3 |
| Result | FAIL: balance and ledger disagree, wallet overdrawn | PASS |

### The fix: optimistic concurrency

- `Wallet.Version` is mapped to PostgreSQL's `xmin` system column, which changes every time a row is updated (`IsRowVersion()` in `WalletDbContext`).
- EF Core adds `WHERE xmin = <the value I read>` to every wallet update. If another request changed the row first, no row matches and EF throws `DbUpdateConcurrencyException`.
- `DepositAsync` and `WithdrawAsync` catch it, discard the stale copy, wait a short random time, re-read the wallet and redo the whole check, up to 10 attempts. The balance check therefore always runs against fresh data.
- The balance update and the new transaction row are saved in a single `SaveChanges` call, which EF runs in one database transaction, so either both happen or neither does (the A in ACID).

### Alternatives I considered

- `SELECT ... FOR UPDATE` (pessimistic locking) makes requests queue on the row, which is simpler but holds locks while the app works.
- A single atomic `UPDATE ... WHERE Balance >= amount` avoids the race in SQL, but it bypasses EF's change tracking and needs a separate insert for the transaction row.
- Serializable isolation also prevents it, but needs the same retry logic and costs more.

I chose optimistic concurrency because conflicts on a single wallet are rare in normal use, so most requests pay nothing.

### Faults and consistency (Chapters 8 and 9)

- The API runs against one PostgreSQL node, so there are no replicas to disagree with each other and reads always see the latest committed write.
- If a client retries a deposit after a network timeout, it could be applied twice. An idempotency key on deposit and withdraw would fix that.
- A request that loses all 10 retries currently surfaces as a server error. Returning `409 Conflict` instead is future work.

## Future improvements

- Add idempotency keys to deposit and withdraw, so a retried request is not applied twice
- Return specific status codes (`404`, `409`) for different failure reasons
- Add paging to `GET /api/Wallets`
- Replace the transaction-type strings with an enum
- Add Docker Compose support
