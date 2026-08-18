using GRA.Application.Interfaces;
using GRA.Application.Services;
using GRA.Infra.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfraPersistence();
builder.Services.AddScoped<IOficinaAppService, OficinaAppService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();