Wallet Management API & Test Suite
A RESTful API built with ASP.NET Core Web API, Entity Framework Core, and PostgreSQL to practise database access, data modelling, migrations, wallet-related operations, and containerization, complete with an automated unit test suite.

Overview
The Wallet Management API models digital wallets and their financial transactions. It demonstrates how an ASP.NET Core application can interact with a relational database using Entity Framework Core, handle exceptions with global middleware, and run containerized using Docker Compose.
This workspace is structured as a unified solution (.slnx) managing both the core Web API and its corresponding test suite side-by-side. It was built as part of my backend development learning activities, focusing on data access, architectural organization, containerization, and testing in .NET 10.

Technologies Used
- C# – Programming language
- ASP.NET Core Web API (.NET 10) – API development
- Entity Framework Core 10 – Object-relational mapping (ORM)
- PostgreSQL (15-alpine) – Relational database
- Npgsql – PostgreSQL provider for Entity Framework Core
- Docker & Docker Compose – Containerization and service orchestration
- xUnit / .NET Testing – Automated unit and query testing
- DTOs – Data transfer between API endpoints and clients
- EF Core Migrations – Database schema management
- Modern .slnx Format – Unified Visual Studio solution layout

Features
- Wallet and transaction data models with explicit relationships
- PostgreSQL database integration with column case-sensitivity handling
- Entity Framework Core database context (WalletDbContext)
- Database schema and seed-data migrations
- Clean separation of concerns via DTOs, service, and query layers
- REST API endpoints for wallet commands and query operations
- Global ExceptionHandlingMiddleware for standardized error handling
- UTC-based transaction auditing
- Dockerfile and docker-compose.yml for seamless multi-container deployment
- Automated test project (WalletManagement.Tests) validating core query and business logic

Project Structure
repos/
├── WalletManagement.Api/          # Web API project
├── WalletManagement.Tests/        # Unit & query test suite
├── docker-compose.yml             # Multi-container orchestration config
├── WalletManagement.Api.slnx      # Unified Visual Studio Solution
└── .gitignore                     # Root workspace Git ignore rules

WalletManagement.Api/
├── Controllers/
│   ├── WalletsController.cs
│   └── WalletQueriesController.cs
├── DTOs/
│   ├── CreateWalletDto.cs
│   ├── TransactionRequestDto.cs
│   ├── TransactionResponseDto.cs
│   └── WalletResponseDto.cs
├── Data/
│   └── WalletDbContext.cs
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
├── Migrations/
│   ├── InitialCreate
│   ├── AddSeedData
│   ├── FixColumnCaseSensitivity
│   └── WalletDbContextModelSnapshot.cs
├── Models/
│   ├── Wallet.cs
│   └── Transaction.cs
├── Queries/
│   ├── IWalletQueryService.cs
│   └── WalletQueryService.cs
├── Services/
│   ├── IWalletService.cs
│   └── WalletService.cs
├── Dockerfile
├── Program.cs
├── appsettings.json
└── WalletManagement.Api.csproj

Data Models

Wallet
Represents a wallet in the system.
- Id – Unique wallet identifier
- OwnerName – Name of the wallet owner
- Balance – Wallet balance
- CreatedAt – Date the wallet was created (UTC)
- Transactions – Collection of transactions associated with the wallet

Transaction
Represents a transaction associated with a wallet.
- Id – Unique transaction identifier
- WalletId – Foreign key referencing the wallet
- Wallet – Navigation property to the associated wallet
- Type – Transaction type
- Amount – Transaction amount
- CreatedAt – Date the transaction was created (UTC)

A wallet can have multiple transactions, and each transaction belongs to one wallet.

Getting Started

Prerequisites
Make sure you have the following installed:
- .NET 10 SDK
- PostgreSQL (or Docker Desktop)
- Visual Studio 2026, VS Code, or .NET CLI

Option A: Running with Docker Compose (Recommended)
1. Clone the Repository
git clone https://github.com/Kelvinjaston/week5-efcore-wallet-api.git
cd week5-efcore-wallet-api

2. Build and Start Containers
docker-compose up --build

Option B: Running Locally
1. Clone the Repository
git clone https://github.com/Kelvinjaston/week5-efcore-wallet-api.git
cd week5-efcore-wallet-api

2. Install Dependencies
Restore the project dependencies across the solution:
dotnet restore WalletManagement.Api.slnx

3. Configure the Database
Create a PostgreSQL database named WalletManagementDb.
Configure the DefaultConnection connection string in your local configuration with your PostgreSQL host, database name, username, and password.
Security note: Do not commit real database passwords or other secrets to GitHub. Use .NET User Secrets or environment variables for local credentials.

4. Apply Database Migrations
Install the EF Core command-line tool if necessary:
dotnet tool install --global dotnet-ef --version 10.0.0
Apply the existing migrations:
dotnet ef database update --project WalletManagement.Api
Run this command from the directory containing WalletManagement.Api.csproj.

5. Run the API
dotnet run --project WalletManagement.Api
Use the local URL printed in the terminal. In Development mode, the OpenAPI document is available at /openapi/v1.json.

Running Tests
To run the complete test suite across the unified solution:
dotnet test WalletManagement.Api.slnx

Or run tests specifically for the test project:
dotnet test WalletManagement.Tests/WalletManagement.Tests.csproj

API Testing
You can test the available endpoints using Postman or another HTTP client.
Review Controllers/WalletsController.cs and Controllers/WalletQueriesController.cs to see the implemented routes and DTOs.

Learning Objectives
This project helped me practise:
- Understanding the Data Access Layer (DAL) and clean architecture
- Working with Entity Framework Core as an ORM and managing migrations
- Modelling entities, relationships, and PostgreSQL column case sensitivity
- Resolving Npgsql timestamp discrepancies by standardizing on DateTime.UtcNow
- Implementing global exception handling middleware and segregated query services
- Containerizing applications using Dockerfiles and Docker Compose orchestration
- Setting up a modern multi-project .slnx solution layout and writing automated tests

Future Improvements
- Expand transaction validation pipelines and security features
- Implement JWT authentication and role-based access control (RBAC)
- Set up CI/CD workflows using GitHub Actions
