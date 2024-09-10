using Microsoft.EntityFrameworkCore;
using WebApplication2;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RefuelingDbContext>(
    options =>
    {
        options.UseNpgsql(configuration.GetConnectionString(nameof(RefuelingDbContext)));
    });

var app = builder.Build();

app.UseSwagger(); 
app.UseSwaggerUI();