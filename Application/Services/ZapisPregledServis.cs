using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

namespace Application.Services;

public sealed class ZapisPregledServis
{
    private readonly IZapisiCitanjeRepozitorijum _citanjeRepozitorijum;

    public ZapisPregledServis(IZapisiCitanjeRepozitorijum citanjeRepozitorijum)
    {
        _citanjeRepozitorijum = citanjeRepozitorijum ?? throw new ArgumentNullException(nameof(citanjeRepozitorijum));
    }

    public Result<IReadOnlyList<Zapis>> VratiSveZapise() => _citanjeRepozitorijum.Svi();

    public Result<IReadOnlyList<Zapis>> PretraziPoDatumu(DateOnly datum)
    {
        var sviRezultat = _citanjeRepozitorijum.Svi();
        if(!sviRezultat.IsSuccess || sviRezultat.Value is null)
            return Result<IReadOnlyList<Zapis>>.Failure(sviRezultat.Error?.Code ?? "repo.greska", sviRezultat.Error?.Message ?? "Greska pri ucitavanju zapisa");
        var pronadjeni = new List<Zapis>();
        foreach(var zapis in sviRezultat.Value)
        {
            var datumZapisa = DateOnly.FromDateTime(zapis.KreiranoUtc.UtcDateTime);
            if(datumZapisa == datum)
            {
                pronadjeni.Add(zapis);
            }
        }
        return Result<IReadOnlyList<Zapis>>.Success(pronadjeni);
    }

    public Result<int> IzracunajDnevniZbirMl(DateOnly datum)
    {
        var sviRezultat = _citanjeRepozitorijum.Svi();
        if(!sviRezultat.IsSuccess || sviRezultat.Value is null)
        {
            var greska = sviRezultat.Error ?? new AppError("greska", "Nije moguce ucitati zapise");
            return Result<int>.Failure(greska.Code, greska.Message); 
        }
        int zbir = 0;
        foreach(var zapis in sviRezultat.Value)
        {
            var datumZapisa = DateOnly.FromDateTime(zapis.KreiranoUtc.UtcDateTime);
            if(zapis.Status != StatusZapisa.Ponisten && datumZapisa == datum)
            {
                zbir += zapis.ZapreminaMl;
            }
        }
        return Result<int>.Success(zbir);
    }
}
