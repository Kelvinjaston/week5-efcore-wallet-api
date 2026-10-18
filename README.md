Wallet Management API
A RESTful API built with ASP.NET Core Web API, Entity Framework Core, and PostgreSQL to practise database access, data modelling, migrations, and wallet-related operations in .NET.

Overview
The Wallet Management API models digital wallets and their financial transactions. It demonstrates how an ASP.NET Core application can interact with a relational database using Entity Framework Core.
This project was built as part of my Week 5 backend development learning activities, focusing on data access in .NET.

Technologies Used
- C# – Programming language
- ASP.NET Core Web API (.NET 10) – API development
- Entity Framework Core 10 – Object-relational mapping (ORM)
- PostgreSQL – Relational database
- Npgsql – PostgreSQL provider for Entity Framework Core
- DTOs – Data transfer between API endpoints and clients
- EF Core Migrations – Database schema management

Features
- Wallet and transaction data models
- Relationship between wallets and their transactions
- PostgreSQL database integration
- Entity Framework Core database context (WalletDbContext)
- Database schema migrations
- Seed-data migration
- DTOs for wallet and transaction operations
- Service layer for wallet-related business logic
- REST API endpoints for wallet operations

Project Structure
WalletManagement.Api/
├── Controllers/
│   └── WalletsController.cs
├── DTOs/
│   ├── CreateWalletDto.cs
│   ├── TransactionRequestDto.cs
│   ├── TransactionResponseDto.cs
│   └── WalletResponseDto.cs
├── Data/
│   └── WalletDbContext.cs
├── Migrations/
│   ├── InitialCreate
│   ├── AddSeedData
│   └── WalletDbContextModelSnapshot.cs
├── Models/
│   ├── Wallet.cs
│   └── Transaction.cs
├── Services/
│   ├── IWalletService.cs
│   └── WalletService.cs
├── Program.cs
├── appsettings.json
└── WalletManagement.Api.csproj

Data Models

Wallet
Represents a wallet in the system.
- Id – Unique wallet identifier
- OwnerName – Name of the wallet owner
- Balance – Wallet balance
- CreatedAt – Date the wallet was created
- Transactions – Collection of transactions associated with the wallet

Transaction
Represents a transaction associated with a wallet.
- Id – Unique transaction identifier
- WalletId – Foreign key referencing the wallet
- Wallet – Navigation property to the associated wallet
- Type – Transaction type
- Amount – Transaction amount
- CreatedAt – Date the transaction was created

A wallet can have multiple transactions, and each transaction belongs to one wallet.

Getting Started

Prerequisites
Make sure you have the following installed:
- .NET 10 SDK
- PostgreSQL
- An API testing tool such as Swagger UI or Postman

1. Clone the Repository
git clone https://github.com/Kelvinjaston/week5-efcore-wallet-api.git
cd week5-efcore-wallet-api

2. Install Dependencies
Restore the project dependencies:
dotnet restore

3. Configure the Database
Create a PostgreSQL database named WalletManagementDb.
Configure the DefaultConnection connection string in your local configuration with your PostgreSQL host, database name, username, and password.
Security note: Do not commit real database passwords or other secrets to GitHub. Use .NET User Secrets or environment variables for local credentials.

4. Apply Database Migrations
Install the EF Core command-line tool if necessary:
dotnet tool install --global dotnet-ef --version 10.0.0
Apply the existing migrations:
dotnet ef database update
Run this command from the directory containing WalletManagement.Api.csproj.

5. Run the API
dotnet run
Use the local URL printed in the terminal. In Development mode, the OpenAPI document is available at /openapi/v1.json. Swagger UI is available only if it has been configured separately in the project.

API Testing
You can test the available endpoints using Postman or another HTTP client.
Review Controllers/WalletsController.cs to see the implemented wallet routes and the request/response DTOs to understand the expected data formats.

Learning Objectives
This project helped me practise:
- Understanding the Data Access Layer (DAL)
- Working with Entity Framework Core as an ORM
- Modelling entities and configuring relationships
- Configuring PostgreSQL connectivity
- Creating and applying database migrations
- Organising API code with controllers, DTOs, and services
- Understanding how database-backed APIs are structured in .NET

Future Improvements
- Add comprehensive unit and integration tests
- Expand transaction handling and validation
- Improve error handling and API documentation
- Add more advanced wallet and transaction queries
