using Microsoft.EntityFrameworkCore;
using WalletManagement.Api.Data;
using WalletManagement.Api.Middleware;
using WalletManagement.Api.Queries;
using WalletManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Adds classic Swagger support

// Register Services
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IWalletQueryService, WalletQueryService>();

// Register Entity Framework Core with PostgreSQL.
builder.Services.AddDbContext<WalletDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Automatically apply database creation/migrations on startup so tables exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // This enables the /swagger UI page
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();