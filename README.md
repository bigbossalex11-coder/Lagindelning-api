# Lagindelning - API

Lagindelning-api är backend som är kopplat till lagindelning webappen.


## Köra lokalt

```
git clone https://github.com/bigbossalex11-coder/Lagindelning-api
cd Lagindelning-api
dotnet run

```

API lyssnar på http://localhost:5293 : testa http://localhost:5293/players

## Endpoints

```
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
```
## Webapp

Frontend finns i [Lagindelning](https://github.com/bigbossalex11-coder/Lagindelning). Starta API:t först, sedan webappen.

## Tekniska val

Minimal API i stället för controllers** Fyra endpoints och en datatyp. 
Controllers och service-lager hade lagt till filer utan att göra något tydligare. 
Vid fler resurser blir uppdelningen motiverad.

Records med with och Player bär bara data, ingen logik. Oföränderlig, 
så ändringar görs med with som ger en ny kopia i stället för att skriva över.

JSON-fil i stället för databas
Listan ligger i minnet och skrivs till players.json vid varje ändring
och läses in vid start. Persistens utan extra beroenden. 
SQLite med EF Core är nästa steg och kräver ingen ändring i klienten.

Två separata repon. Två program som startas och driftsätts var för sig. 
Uppgiften kräver dessutom två repolänkar.

Filnamn i stället för bilder Spelarna är barn, så foton vore personuppgifter.
Appen sparar filer och visar filnamnet, men lagrar inga bilder.

DisableAntiforgery CSRF-skyddet är gjort för formulär med cookies.
API:t använder inga cookies och begränsas av CORS, så token tillför inget.