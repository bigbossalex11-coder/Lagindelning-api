var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

app.MapGet("/players", () => players);

app.Run();

record Player(int Id, string Name, string Rank);

