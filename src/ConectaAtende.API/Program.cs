using ConectaAtende.Application.Services;
using ConectaAtende.Domain.Repositories;
using ConectaAtende.Domain.Services;
using ConectaAtende.Infrastructure.Repositories;
using ConectaAtende.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositories
builder.Services.AddSingleton<IContactRepository, InMemoryContactRepository>();
builder.Services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();

// Services
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<UndoService>();
builder.Services.AddSingleton<RecentContactsService>(sp => 
    new RecentContactsService(sp.GetRequiredService<IContactRepository>(), maxCapacity: 10));

// Triage Policy
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
