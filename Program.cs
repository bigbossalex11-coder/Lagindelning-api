using System.Text.Json;
using server.Models;
using server.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
     options.AddDefaultPolicy(policy =>
     policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddSingleton<PlayerRepository>();
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

var players = new List<Player>
{
    new Player(1, "adam" , "grön"),
    new Player(2, "eva" ,"gul"),
    new Player(3, "oskar" ,"röd")
};

var allowedRanks = new[] { "grön", "gul", "röd" };

if (File.Exists("players.json"))
{
    var json = File.ReadAllText("players.json");
    players = JsonSerializer.Deserialize<List<Player>>(json) ?? players;
}

void Save()
{
    var json = JsonSerializer.Serialize(players);
    File.WriteAllText("players.json", json);
}

app.UseCors();

app.Run();
