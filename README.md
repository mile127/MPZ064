# Prakticni zadatak MPZ064 - Evidencija unosa vode

Resenje prakticnog zadatka MPZ064 za evidenciju dnevnog unosa vode ('CasaVode', 'BocaVode'), realizovano u .NET 8 okruzenju uz postovanje Clean Architecture pravila i SOLID principa.

## Pokretanje

Потребан је .NET 8 SDK.

```bash
# prevodjenje resenja
dotnet restore
dotnet build

# pokretanje testova
dotnet test

# pokretanje u memorijskom rezimu
dotnet run --project Presentation -- in-memory

# pokretanje sa trajnim cuvanjem u JSON datoteci (podaci.json)
dotnet run --project Presentation -- json
```
## Izbor rezima rada i zavisnosti
- in-memory - podaci se cuvaju u radnoj memoriji tokom trajanja programa (InMemoryZapisiRepozitorijum).
- json - podaci se polimorfno serijalizuju i trajno cuvaju u datoteci podaci.json (JsonZapisiRepozitorijum).
- Sastavljanje svih zavisnosti i izbor repozitorijuma i logera (FileLoggerServis / ConsoleLoggerServis) centralizovano je na jednom mestu u klasi AppContainer (Presentation/Composition/AppContainer.cs). 

## Organizacija resenja

- Shared - zajednicki tipovi Result, Result<T> i AppError za upravljanje ishodima operacija bez oslanjanja na izuzetke u redovnom toku.
- Domain — domenski modeli (Zapis, CasaVode, BocaVode, StatusZapisa) i poslovna pravila validacije zapremine (50-2000 ml) i promene statusa.
- Application — poslovni servisi(ZapisServis, ZapisPregledServis) i razdvojeni ugovori (IZapisiRepozitorijum, IZapisiCitanjeRepozitorijum, IZapisiUpisRepozitorijum, ILoggerServis).
- Database — implementacije skladistenja podataka (InMemoryZapisiRepozitorijum i JsonZapisiRepozitorijum sa polimorfnom JSON serijalizacijom).
- Infrastructure — tehnicki servisi za evidenciju dogadjaja (FileLoggerServis, ConsoleLoggerServis).
- Presentation — konzolni korisnicki interfejs (ConsoleMenu, SystemConsoleIO), formateri prikaza (SazetiZapisFormater, DetaljniZapisFormater) i AppContainer.
- Tests — NUnit testovi (TE1-TE9 i provera smera zavisnosti slojeva).

## Dokumentacija
- [Korisnicke price(US-01 - US-06)](docs/user-stories.md)
- [Arhitektura resenja i SOLID principi](docs/architecture.md)

## Studentski podaci

- Број и назив задатка: MPZ064 - Evidencija unosa vode
- Јавни GitHub репозиторијум: https://github.com/mile127/MPZ064
- Pull Request 1: __________________________________
- Pull Request 2: __________________________________