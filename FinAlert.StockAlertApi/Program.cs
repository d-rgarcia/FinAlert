using FinAlert.Identity.Core.Domain;
using FinAlert.Identity.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using FinAlert.AlertStore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

string alertsConnectionString = builder.Configuration.GetConnectionString("AlertsDb") ??
    throw new ArgumentNullException("Alerts connection string is required");

builder.Services.AddAlertServices(alertsConnectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();