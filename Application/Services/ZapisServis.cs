using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;
using Shared;

namespace Application.Services;

// Polazna struktura servisa; poslovne operacije nisu implementirane.
public sealed class ZapisServis
{
    private readonly IZapisiRepozitorijum _repozitorijum;
    private readonly ILoggerServis _logger;

    public ZapisServis(IZapisiRepozitorijum repozitorijum, ILoggerServis logger)
    {
        _repozitorijum = repozitorijum ?? throw new ArgumentNullException(nameof(repozitorijum));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Result Dodaj(Zapis zapis)
    {
        // TODO: definisati poslovna pravila i pozvati repozitorijum.
        // Primer pokazuje gde pripada use case, ne daje rešenje zadatka.
        if (zapis is null)
            return Result.Failure("zapis.null", "Zapis ne sme biti null");

        var rezultat = _repozitorijum.Sacuvaj(zapis);
        if(rezultat.IsFailure)
        {
            _logger.Loguj($"Greska pri cuvanju: {rezultat.Error?.Message}");
            return rezultat;
        }

        _logger.Loguj($"Evidentiran zapis: {zapis.TipZapisa} ({zapis.ZapreminaMl} ml) - {zapis.Naziv}");
        return Result.Success();
    }
    public Result<Zapis> DodajCasuVode(string naziv, string opis, int zapreminaCase)
    {
        try
        {
            var zapis = new CasaVode(naziv, opis, zapreminaCase);
            var rezultat = Dodaj(zapis);
            if(rezultat.IsFailure)
            {
                var greska = rezultat.Error ?? new AppError("greska", "Greska pri cuvanju zapisa");
                return Result<Zapis>.Failure(greska.Code, greska.Message);
            }

            return Result<Zapis>.Success(zapis);
        }
        catch(ArgumentException ex)
        {
            _logger.Loguj($"Neuspesno evidentiranje: {ex.Message}");
            return Result<Zapis>.Failure("validacija.greska", ex.Message);
        }
    }
    public Result<Zapis> DodajBocuVode(string naziv, string opis, int zapreminaBoce)
    {
        try
        {
            var zapis = new BocaVode(naziv, opis, zapreminaBoce);
            var rezultat = Dodaj(zapis);
            if(rezultat.IsFailure)
            {
                var greska = rezultat.Error ?? new AppError("greska", "Greska pri cuvanju zapisa");
                return Result<Zapis>.Failure(greska.Code, greska.Message);
            }
            return Result<Zapis>.Success(zapis);
        }
        catch(ArgumentException ex)
        {
            _logger.Loguj($"Neuspesno evidentiranje: {ex.Message}");
            return Result<Zapis>.Failure("validacija.greska", ex.Message);
        }
    }
    public Result<IReadOnlyList<Zapis>> VratiSveZapise() => _repozitorijum.Svi();

    public Result<IReadOnlyList<Zapis>> PretraziPoDatumu(DateOnly datum)
    {
        var sviRezultat = _repozitorijum.Svi();
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
        var sviRezultat = _repozitorijum.Svi();
        if(!sviRezultat.IsSuccess || sviRezultat.Value is null)
        {
            var greska = sviRezultat.Error ?? new AppError("greska", "Nije moguce ucitati zapise.");
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
    public Result PotvrdiZapis(Guid id)
    {
        var pronadji = _repozitorijum.Pronadji(id);
        if(!pronadji.IsSuccess || pronadji.Value is null)
        {
            var greska = pronadji.Error ?? new AppError("zapis_nije_pronadjen", "Zapis nije pronadjen");
            _logger.Loguj($"Neuspesna potvrda: {greska.Message}");
            return Result.Failure(greska.Code, greska.Message);
        }
        try
        {
            pronadji.Value.Potvrdi();
            var sacuvaj = _repozitorijum.Sacuvaj(pronadji.Value);
            if(sacuvaj.IsSuccess)
                _logger.Loguj($"Potvrdjen zapis: {pronadji.Value.Naziv} ({pronadji.Value.Id})");
            return sacuvaj;
        }
        catch(InvalidOperationException ex)
        {
            _logger.Loguj($"Neuspesna potvrda: {ex.Message}");
            return Result.Failure("operacija_nedozvoljena", ex.Message);
        }
    }
    public Result PonistiZapis(Guid id)
    {
        var pronadji = _repozitorijum.Pronadji(id);
        if(!pronadji.IsSuccess || pronadji.Value is null)
        {
            var greska = pronadji.Error ?? new AppError("zapis_nije_pronadjen", "Zapis nije pronadjen");
            _logger.Loguj($"Neuspesno ponistavanje: {greska.Message}");
            return Result.Failure(greska.Code, greska.Message);
        }
        try
        {
            pronadji.Value.Ponisti();
            var sacuvaj = _repozitorijum.Sacuvaj(pronadji.Value);
            if(sacuvaj.IsSuccess)
                _logger.Loguj($"Ponisten zapis: {pronadji.Value.Naziv} ({pronadji.Value.Id})");
            return sacuvaj;
        }
        catch(InvalidOperationException ex)
        {
            _logger.Loguj($"Neuspesno ponistavanje: {ex.Message}");
            return Result.Failure("operacija_nedozvoljena", ex.Message);
        }
    }
}
