# Lagindelning - API

Lagindelning-api är backend som är kopplat till webbappen och mobilappen.


## Köra lokalt

```
git clone https://github.com/bigbossalex11-coder/Lagindelning-api
cd Lagindelning-api
dotnet run

```

API lyssnar på http://localhost:5293 : testa http://localhost:5293/players
API lyssnar på alla nätverkskort (0.0.0.0), så att mobilappen kan nå det från telefon på samma wifi.
## Struktur

```
Controllers/
  PlayersController.cs   /players – hämta, skapa, ändra, ta bort, ladda upp fil
  TeamsController.cs     /teams – dela in spelare i lag
Models/
  Player.cs              spelarens data (record)
Repositories/
  PlayerRepository.cs    spelarlistan, läser och sparar players.json
Program.cs               registrerar tjänster och kopplar in controllers
```

## Endpoints

| Metod | Adress | Gör |
|---|---|---|
| GET | /players | Hämtar alla spelare i listan |
| POST | /players | Lägger till en spelare i listan |
| PUT | /players/{id} | Ändrar rank på en spelare i listan |
| POST | /players/{id}/file | Lägger till en fil på en spelare i listan |
| DELETE | /players/{id} | Tar bort en spelare från listan |
| GET | /teams | Delar upp alla spelare i slumpade eller nivåindelade lag |


### GET /teams

| Inställning | Värden | Betyder |
|---|---|---|
| teamCount | 1 eller fler | antal lag |
| mode | random | slumpar alla spelare |
| mode | level | nivåindelar spelare |

Svarar 400 Bad Request om teamCount är 0 eller mindre.

## Webbapp

Frontend finns i [Lagindelning](https://github.com/bigbossalex11-coder/Lagindelning). Starta API:t först, sedan webappen.

## Tekniska val

Controllers och repository
Efter feedback från läraren bytte jag från minimal API till controllers och
repository för att strukturera kodbasen enligt branschstandard. När applikationen
växte med både /players och /teams blev det tydligare med en controller per resurs.
Controllers skapas på nytt vid varje anrop, så spelarlistan kan inte ligga där.
PlayerRepository registreras därför som singleton och delas av alla controllers
via dependency injection. Controllerna pratar bara med repositoryt, så när
JSON-filen byts mot SQLite behöver bara PlayerRepository ändras, inte controllerna.


Records med with och Player bär bara data, ingen logik. Oföränderlig, 
så ändringar görs med with som ger en ny kopia i stället för att skriva över.

JSON-fil i stället för databas
Listan ligger i minnet och skrivs till players.json vid varje ändring
och läses in vid start. Persistens utan extra beroenden. 
SQLite med EF Core är nästa steg och kräver ingen ändring i klienten.

Tre separata repon. Tre program, API, webbapp och mobilapp, som startas och driftsätts var för sig.

Filnamn i stället för bilder Spelarna är barn, så foton vore personuppgifter.
Appen sparar filer och visar filnamnet, men lagrar inga bilder.

Inget CSRF-skydd
CSRF-skydd behövs för formulär som skickar cookies. API:t använder inga cookies
och begränsas av CORS, så det tillför inget. Controllers med [ApiController]
kontrollerar inte antiforgery automatiskt, så det finns inget att stänga av.