using Domain.Models;

namespace Presentation.Formatting;

public sealed class DetaljniZapisFormater : IZapisFormater
{
    public string Formatiraj(Zapis zapis)
    {
        string oznakaZapremine;
        if(zapis is CasaVode)
            oznakaZapremine = $"Zapremina case: {zapis.ZapreminaMl} ml";
        else
            oznakaZapremine = $"Zapremina boce: {zapis.ZapreminaMl} ml";
        return $"[{zapis.Status}] {zapis.TipZapisa} | {zapis.Naziv} ({oznakaZapremine}) - {zapis.Opis} | Datum: {zapis.KreiranoUtc:yyyy-MM-dd HH:mm} | ID: {zapis.Id}";
    }
}