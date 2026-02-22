using ConectaAtende.Application.Services;
using ConectaAtende.Domain.Repositories;
using ConectaAtende.Domain.Services;
using ConectaAtende.Infrastructure.Repositories;
using ConectaAtende.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositories (Singleton para manter dados em memória entre requisições)
builder.Services.AddSingleton<IContactRepository, InMemoryContactRepository>();
builder.Services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();

// Application Services
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<TicketService>();

// Infrastructure Services (Singleton para preservar estado entre requisições)
builder.Services.AddSingleton<UndoService>(sp =>
    new UndoService(sp.GetRequiredService<IContactRepository>()));
builder.Services.AddSingleton<RecentContactsService>(sp =>
    new RecentContactsService(sp.GetRequiredService<IContactRepository>(), maxCapacity: 10));

// Triage Policy (Singleton para manter política configurada)
builder.Services.AddSingleton<ITriagePolicyService, TriagePolicyService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
