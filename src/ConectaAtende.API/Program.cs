using ConectaAtende.Application.Services;
using ConectaAtende.Domain.Repositories;
using ConectaAtende.Domain.Services;
using ConectaAtende.Infrastructure.Data;
using ConectaAtende.Infrastructure.Repositories;
using ConectaAtende.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context (SQLite)
builder.Services.AddDbContext<ConectaAtendeDbContext>(options =>
    options.UseSqlite("Data Source=ConectaAtende.db"));

// Repositories (Scoped - uma instância por requisição)
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

// Application Services
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<TicketService>();

// Infrastructure Services (Singleton para preservar estado entre requisições)
// Nota: UndoService e RecentContactsService usam IServiceScopeFactory para acessar repositórios Scoped
builder.Services.AddSingleton<UndoService>();
builder.Services.AddSingleton<RecentContactsService>(sp =>
    new RecentContactsService(sp.GetRequiredService<IServiceScopeFactory>(), maxCapacity: 10));

// Triage Policy (Singleton para manter política configurada)
builder.Services.AddSingleton<ITriagePolicyService, TriagePolicyService>();

var app = builder.Build();

// Garantir que o banco de dados seja criado
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ConectaAtendeDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
