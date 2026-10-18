using Microsoft.EntityFrameworkCore;
using WalletManagement.Api.Data;
using WalletManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
// Register Services
builder.Services.AddScoped<IWalletService, WalletService>();

// Register Entity Framework Core with PostgreSQL.
builder.Services.AddDbContext<WalletDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();