using Microsoft.EntityFrameworkCore;
using WebApplication2;
using WebApplication2.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<RefuelingDbContext>(
    options =>
    {
        options.UseNpgsql(configuration.GetConnectionString(nameof(RefuelingDbContext)));
    });

var app = builder.Build();

app.MapControllers();
app.UseSwagger(); 
app.UseSwaggerUI();
app.Run();