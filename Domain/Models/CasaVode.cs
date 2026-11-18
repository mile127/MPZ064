namespace Domain.Models;

public sealed class CasaVode : Zapis
{
    public int ZapreminaCase { get; init; }
    public CasaVode(string naziv, string opis, int zapreminaCase) : base(naziv, opis)
    {
        if(zapreminaCase < 50 || zapreminaCase > 2000)
            throw new ArgumentOutOfRangeException(nameof(zapreminaCase), "Zapremina case mora biti izmedju 50 i 2000 ml");
        ZapreminaCase = zapreminaCase;
    }
    public override string TipZapisa => "Casa vode";
    public override int ZapreminaMl => ZapreminaCase;
}