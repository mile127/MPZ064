namespace Domain.Models;

public sealed class BocaVode : Zapis
{
    public int ZapreminaBoce { get; init; }
    public BocaVode(string naziv, string opis, int zapreminaBoce) : base(naziv, opis)
    {
        if(zapreminaBoce < 50 || zapreminaBoce > 2000)
            throw new ArgumentOutOfRangeException(nameof(zapreminaBoce), "Zapremina boce mora biti izmedju 50 i 2000 ml");
        ZapreminaBoce = zapreminaBoce;
    }
    public override string TipZapisa => "Boca vode";
    public override int ZapreminaMl => ZapreminaBoce;
}