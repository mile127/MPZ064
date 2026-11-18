using System.Text.Json.Serialization;
namespace Domain.Models;
// Zajednički osnovni tip iz koga se mogu izvesti modeli konkretnog zadatka.
// Polja, nazive i nasledjivanje prilagoditi dodeljenoj specifikaciji.
[JsonDerivedType(typeof(CasaVode), typeDiscriminator: "casa")]
[JsonDerivedType(typeof(BocaVode), typeDiscriminator: "boca")]
public abstract class Zapis
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Naziv { get; init; } = string.Empty;
    public string Opis { get; init; } = string.Empty;
    public DateTimeOffset KreiranoUtc { get; init; } = DateTimeOffset.UtcNow;
    public StatusZapisa Status { get; set; } = StatusZapisa.Evidentiran;
    public abstract string TipZapisa { get; }
    public abstract int ZapreminaMl { get; }
    protected Zapis(string naziv, string opis)
    {
        if(string.IsNullOrWhiteSpace(naziv))
            throw new ArgumentException("Naziv zapisa ne sme biti prazan", nameof(naziv));
        if(string.IsNullOrWhiteSpace(opis))
            throw new ArgumentException("Opis zapisa ne sme biti prazan", nameof(opis));
        Naziv = naziv.Trim();
        Opis = opis.Trim();
    }
    public void Potvrdi()
    {
        if(Status == StatusZapisa.Ponisten)
            throw new InvalidOperationException("Ponisten zapis nije moguce potvrditi");
        if(ZapreminaMl <= 0)
            throw new InvalidOperationException("Kolicina tecnosti mora biti veca od nule");
        Status = StatusZapisa.Potvrdjen;
    }
    public void Ponisti()
    {
        if(Status == StatusZapisa.Ponisten)
            throw new InvalidOperationException("Zapis je vec ponisten");
        Status = StatusZapisa.Ponisten;
    }
}
