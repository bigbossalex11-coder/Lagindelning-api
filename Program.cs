using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
     options.AddDefaultPolicy(policy =>
     policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

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

app.MapGet("/players", () => players);

app.MapPut("/players/{id}", (int id, Player updated) =>
{
    var existing = players.FirstOrDefault(p => p.Id == id);
    if (existing is null)
        return Results.NotFound();
    if (!allowedRanks.Contains(updated.Rank))
        return Results.BadRequest("Ranken måste vara grön, gul eller röd");

    var changed = existing with { Rank = updated.Rank };

    players[players.IndexOf(existing)] = changed;
    Save();
    return Results.Ok(changed);
});

app.MapPost("/players", (Player newPlayer) =>
{
    if (string.IsNullOrWhiteSpace(newPlayer.Name))
        return Results.BadRequest("Namn saknas");
    if (!allowedRanks.Contains(newPlayer.Rank))
        return Results.BadRequest("Ranken måste vara grön, gul eller röd");

    var nextId = players.Count == 0 ? 1 : players.Max(p => p.Id) + 1;
    var created = newPlayer with { Id = nextId };
    players.Add(created);
    Save();

    return Results.Created($"/players/{created.Id}", created);

});

app.MapPost("/players/{id}/file",(int id, IFormFile file) => {

    var existing = players.FirstOrDefault(p => p.Id == id);
    if (existing is null)
        return Results.NotFound();

    Directory.CreateDirectory("uploads");
    var savedName = $"{id}_{file.FileName}";
    var path = Path.Combine("uploads", savedName);
    using var stream = File.Create(path);
    file.CopyTo(stream);
    var changed = existing with { FileName = savedName };
    players[players.IndexOf(existing)] = changed;
    Save();
    return Results.Ok(changed);
}).DisableAntiforgery();


app.MapDelete("/players/{id}", (int id) =>
{
    var existing = players.FirstOrDefault(p => p.Id == id);
    if (existing is null)
        return Results.NotFound();

    players.Remove(existing);
    Save();
    return Results.NoContent();
});

app.MapGet("/teams", (int teamCount, string mode) =>{
    if (teamCount <= 0)
        return Results.BadRequest("0 och negativa tal är ej tillåtna");
    var shuffled = players.OrderBy(p => Random.Shared.Next()).ToList();
    if (mode == "level")
    {
        shuffled = shuffled.OrderBy(p => p.Rank).ToList();
    }
    var teams = new List<List<Player>>();
    for (int i = 0; i < teamCount; i++)
    {
        teams.Add(new List<Player>());
    }

    for (int i = 0; i < shuffled.Count; i++)
        {
        teams[i % teamCount].Add(shuffled[i]);
    }
    return Results.Ok(teams);
});

app.Run();

record Player(int Id, string Name, string Rank, string? FileName = null);