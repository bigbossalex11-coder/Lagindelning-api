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
app.UseCors();

app.MapGet("/players", () => players);

app.MapPut("/players/{id}", (int id, Player updated) =>
{
    var existing = players.FirstOrDefault(p => p.Id == id);
    if (existing is null)
        return Results.NotFound();

    var changed = existing with { Rank = updated.Rank };

    players[players.IndexOf(existing)] = changed;
    return Results.Ok(changed);
});

app.MapPost("players", (Player newPlayer) =>
{
    var nextId = players.Max(p => p.Id) + 1;
    var created = newPlayer with { Id = nextId };
    players.Add(created);

    return Results.Created($"/players/{created.Id}", created);

});

app.MapPost("/players/{id}/file",(int id, IFormFile file) => {

    var existing = players.FirstOrDefault(p => p.Id == id);
    if (existing is null)
        return Results.NotFound();

    Directory.CreateDirectory("uploads");
    var patch = Path.Combine("uploads", file.FileName);
    using var stream = File.Create(patch);
    file.CopyTo(stream);
    var changed = existing with { FileName = file.FileName };
    players[players.IndexOf(existing)] = changed;
    return Results.Ok(changed);
}).DisableAntiforgery(); ;

app.Run();

record Player(int Id, string Name, string Rank, string? FileName = null);

