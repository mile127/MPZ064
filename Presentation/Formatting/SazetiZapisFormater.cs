using Domain.Models;

namespace Presentation.Formatting;

public sealed class SazetiZapisFormater : IZapisFormater
{
    public string Formatiraj(Zapis zapis)
    {
        return $"[{zapis.Status}] {zapis.TipZapisa} | {zapis.Naziv} | ID: {zapis.Id}";
    }
}