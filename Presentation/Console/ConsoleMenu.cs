using Domain.Models;
using Presentation.Composition;
using Presentation.Interfaces;

namespace Presentation.Console;

// Meni treba dopuniti operacijama F1-F6 iz dodeljenog zadatka.
public sealed class ConsoleMenu
{
    private readonly IConsoleIO _io;
    private readonly AppContainer _container;

    public ConsoleMenu(IConsoleIO io, AppContainer container)
    {
        _io = io;
        _container = container;
    }

    public void Run()
    {
        bool radi = true;

        while(radi)
        {
            _io.WriteLine();
            _io.WriteLine("---EVIDENCIJA UNOSA VODE---");
            _io.WriteLine("1. Evidentiraj casu vode");
            _io.WriteLine("2. Evidentiraj bocu vode");
            _io.WriteLine("3. Prikazi sve zapise i dnevni zbir");
            _io.WriteLine("4. Pretrazi zapise po datumu");
            _io.WriteLine("5. Potvrdi zapis");
            _io.WriteLine("6. Ponisti zapis");
            _io.WriteLine("0. Izlaz");
            _io.Write("Izbor: ");

            string izbor = _io.ReadLine().Trim();
            _io.WriteLine();

            switch(izbor)
            {
                case "1":
                    EvidentirajCasu();
                    break;
                case "2":
                    EvidentirajBocu();
                    break;
                case "3":
                    PrikaziSveIDnevniZbir();
                    break;
                case "4":
                    PretraziZapise();
                    break;
                case "5":
                    PotvrdiZapis();
                    break;
                case "6":
                    PonistiZapis();
                    break;
                case "0":
                    radi = false;
                    break;
                default:
                    _io.WriteLine("Niste uneli dobar broj");
                    break;
            }
        }
    }

    private void EvidentirajCasu()
    {
        _io.Write("Naziv: ");
        string naziv = _io.ReadLine();

        _io.Write("Opis: ");
        string opis = _io.ReadLine();

        _io.Write("Zapremina case: ");
        if(!int.TryParse(_io.ReadLine(), out int zapremina))
        {
            _io.WriteLine("Greska: Zapremina mora biti ceo broj.");
            return;
        }

        var rezultat = _container.Servis.DodajCasuVode(naziv, opis, zapremina);
        if(rezultat.IsSuccess && rezultat.Value is not null)
            _io.WriteLine($"Casa vode je uspesno evidentirana. ID: {rezultat.Value.Id}");
        else
            _io.WriteLine($"Greska: {rezultat.Error?.Message}");
    }

    private void EvidentirajBocu()
    {
        _io.Write("Naziv: ");
        string naziv = _io.ReadLine();

        _io.Write("Opis: ");
        string opis = _io.ReadLine();

        _io.Write("Zapremina boce: ");
        if(!int.TryParse(_io.ReadLine(), out int zapremina))
        {
            _io.WriteLine("Greska: Zapremina mora biti ceo broj.");
            return;
        }

        var rezultat = _container.Servis.DodajBocuVode(naziv, opis, zapremina);
        if(rezultat.IsSuccess && rezultat.Value is not null)
            _io.WriteLine($"Boca vode je uspesno evidentirana. ID: {rezultat.Value.Id}");
        else
            _io.WriteLine($"Greska: {rezultat.Error?.Message}");
    }

    private void PrikaziSveIDnevniZbir()
    {
        var sviRezultat = _container.Servis.VratiSveZapise();
        if(!sviRezultat.IsSuccess || sviRezultat.Value is null)
        {
            _io.WriteLine($"Greska: {sviRezultat.Error?.Message}");
            return;
        }

        if(sviRezultat.Value.Count == 0)
        {
            _io.WriteLine("Nema evidentiranih zapisa");
        }
        else
        {
            foreach(var zapis in sviRezultat.Value)
            {
                IspisiZapis(zapis);
            }
        }
        var danas = DateOnly.FromDateTime(DateTime.UtcNow);
        var zbirRezultat = _container.Servis.IzracunajDnevniZbirMl(danas);
        if(zbirRezultat.IsSuccess)
            _io.WriteLine($"Dnevni zbir za danas ({danas:dd.MM.yyyy}: {zbirRezultat.Value} ml)");
        else
            _io.WriteLine($"Greska pri racunanju zbira: {zbirRezultat.Error?.Message}");
    }

    private void PretraziZapise()
    {
        _io.Write("Unesite datum (GGGG-MM-DD): ");
        string unos = _io.ReadLine().Trim();

        if(!DateOnly.TryParseExact(unos, "yyyy-MM-dd", out DateOnly datum))
        {
            _io.WriteLine("Greska: Neispravan format datuma. Koristite oblik GGGG-MM-DD");
            return;
        }

        var rezultat = _container.Servis.PretraziPoDatumu(datum);
        if(!rezultat.IsSuccess || rezultat.Value is null)
        {
            _io.WriteLine($"Greska: {rezultat.Error?.Message}");
            return;
        }

        if(rezultat.Value.Count == 0)
        {
            _io.WriteLine($"Nema zapisa za datum {datum:yyyy-MM-dd}.");
            return;
        }

        foreach(var zapis in rezultat.Value)
        {
            IspisiZapis(zapis);
        }
    }

    private void PotvrdiZapis()
    {
        _io.Write("Unesite ID zapisa za potvrdu: ");
        if(!Guid.TryParse(_io.ReadLine().Trim(), out Guid id))
        {
            _io.WriteLine("Greska: Neispravan format ID-a");
            return;
        }

        var rezultat = _container.Servis.PotvrdiZapis(id);
        if(rezultat.IsSuccess)
            _io.WriteLine("Zapis je uspesno potvrdjen");
        else
            _io.WriteLine($"Greska: {rezultat.Error?.Message}");
    }

    private void PonistiZapis()
    {
        _io.Write("Unesite ID zapisa za ponistavanje: ");
        if(!Guid.TryParse(_io.ReadLine().Trim(), out Guid id))
        {
            _io.WriteLine("Greska: Neispravan format ID-a");
            return;
        }
        var rezultat = _container.Servis.PonistiZapis(id);
        if(rezultat.IsSuccess)
            _io.WriteLine("Zapis je uspesno ponisten");
        else
            _io.WriteLine($"Greska: {rezultat.Error?.Message}");
    }
    private void IspisiZapis(Zapis zapis)
    {
        _io.WriteLine($"[{zapis.Status}] {zapis.TipZapisa} | {zapis.Naziv} ({zapis.ZapreminaMl} ml) - {zapis.Opis} | ID: {zapis.Id}");
    }
}
