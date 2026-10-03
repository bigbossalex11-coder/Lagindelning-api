# Lagindelning – API

Backend för Lagindelning. Används av både webbappen och mobilappen.

## Köra lokalt

Kräver .NET 10 SDK.

```
git clone https://github.com/bigbossalex11-coder/Lagindelning-api
cd Lagindelning-api
dotnet run
```

API:t lyssnar på http://localhost:5293. Testa med http://localhost:5293/players.

Databasen (`lagindelning.db`) skapas automatiskt första gången API:t startar. Inga extra kommandon behövs.

API:t lyssnar på alla nätverkskort (`0.0.0.0`), så att mobilappen kan nå det från en telefon på samma wifi.

## Appar

- Webbapp: [Lagindelning](https://github.com/bigbossalex11-coder/Lagindelning)
- Mobilapp: [Lagindelning-app](https://github.com/bigbossalex11-coder/Lagindelning-app)

Starta API:t först, sedan apparna.

## Struktur

```
Controllers/
  PlayersController.cs   /players – hämta, skapa, ändra, ta bort, ladda upp fil
  TeamsController.cs     /teams – dela in spelare i lag
Data/
  AppDbContext.cs        kopplingen till databasen (EF Core)
Migrations/              databasens tabeller, genererade av EF Core
Models/
  Player.cs              spelarens data (record)
Repositories/
  PlayerRepository.cs    hämtar och sparar spelare i databasen
Program.cs               registrerar tjänster, skapar databasen och kopplar in controllers
```

## Endpoints

| Metod | Adress | Gör |
|---|---|---|
| GET | /players | Hämtar alla spelare |
| POST | /players | Lägger till en spelare. 400 om namnet är tomt eller ranken är ogiltig |
| PUT | /players/{id} | Ändrar rank på en spelare. 404 om spelaren saknas |
| POST | /players/{id}/file | Laddar upp en fil till en spelare |
| DELETE | /players/{id} | Tar bort en spelare. 204 om det gick, 404 om spelaren saknas |
| GET | /teams | Delar in alla spelare i slumpade eller nivåindelade lag |

### GET /teams

| Inställning | Värden | Betyder |
|---|---|---|
| teamCount | 1 eller fler | antal lag |
| mode | random | slumpar alla spelare |
| mode | level | blandar inom varje nivå och fördelar nivåerna jämnt |

Svarar 400 Bad Request om teamCount är 0 eller mindre.

## Tekniska val

**SQLite och Entity Framework Core**
Spelarna sparades först i en JSON-fil. Det räckte för en användare, men appen ska användas på riktigt: närvaro per träning med historik, flera tränare som skriver samtidigt och på sikt hosting. Det passar en databas bättre. SQLite är en databas i en enda fil, så ingen databasserver behöver installeras. EF Core gör att koden arbetar med vanliga C#-klasser medan databasen sköter lagring och id:n (autoincrement). Byte till en större databas vid hosting kräver bara en annan databasdrivrutin.

**Controllers och repository**
Efter feedback bytte jag från minimal API till controllers och repository. Varje controller sköter en resurs, och controllerna pratar bara med repositoryt genom metoder som `GetAll`, `GetById`, `Add`, `Update` och `Delete`. När JSON-filen byttes mot databasen behövde därför bara repositoryt skrivas om. Controllerna fick bara nya metodnamn. `AppDbContext` skapas en gång per anrop (scoped), så `PlayerRepository` är också scoped.

**Databasen skapas vid start**
`Program.cs` kör migrationerna när API:t startar, så att den som klonar repot bara behöver `dotnet run`. Om databasen är tom och det finns en gammal `players.json` flyttas spelarna in automatiskt en gång.

**Records med with**
`Player` bär bara data, ingen logik. Den är oföränderlig, så ändringar görs med `with`, som ger en ny kopia. Repositoryt skriver över originalet i databasen med kopians värden.

**Personuppgifter**
Spelarna är barn. Databasfilen och `players.json` versionshanteras inte, utan finns bara lokalt. Appen sparar filnamn i stället för bilder, eftersom foton vore personuppgifter.

**Tre separata repon**
API, webbapp och mobilapp är tre program som startas och driftsätts var för sig.

**Inget CSRF-skydd**
CSRF-skydd behövs för formulär som skickar cookies. API:t använder inga cookies och begränsas av CORS, så det tillför inget. Controllers med `[ApiController]` kontrollerar inte antiforgery automatiskt, så det finns inget att stänga av.

## För utveckling

Nya tabeller eller kolumner kräver en ny migration:

```
dotnet tool install --global dotnet-ef
dotnet ef migrations add NamnPåÄndringen
```

Migrationen körs automatiskt nästa gång API:t startar.