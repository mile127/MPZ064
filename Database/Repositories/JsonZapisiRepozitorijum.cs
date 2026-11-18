using System.Text.Json;
using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

namespace Database.Repositories;

// TODO: implementirati trajno cuvanje u JSON datoteci.
// Nacrtati/odabrati nacin serijalizacije polimorfnih modela konkretnog zadatka.
public sealed class JsonZapisiRepozitorijum : IZapisiRepozitorijum
{
    private readonly string _putanja;
    private static readonly JsonSerializerOptions _opcije = new() { WriteIndented = true };

    public JsonZapisiRepozitorijum(string putanja)
    {
        if(string.IsNullOrWhiteSpace(putanja))
            throw new ArgumentException("Putanja do JSON datoteke ne sme biti prazna.", nameof(putanja));
        _putanja = putanja;
    }

    private List<Zapis> UcitajSveIzFajla()
    {
        if(!File.Exists(_putanja))
            return new List<Zapis>();
        string json = File.ReadAllText(_putanja);
        if(string.IsNullOrWhiteSpace(json))
            return new List<Zapis>();
        return JsonSerializer.Deserialize<List<Zapis>>(json, _opcije) ?? new List<Zapis>();
    }

    private void UpisiSveUFajl(List<Zapis> zapisi)
    {
        string? direktorijum = Path.GetDirectoryName(_putanja);
        if(!string.IsNullOrWhiteSpace(direktorijum) && !Directory.Exists(direktorijum))
            Directory.CreateDirectory(direktorijum);
        string json = JsonSerializer.Serialize(zapisi, _opcije);
        File.WriteAllText(_putanja, json);
    }
    public Result Sacuvaj(Zapis zapis)
    {
        if(zapis is null)
            return Result.Failure("zapis.null", "Zapis ne sme biti null.");
        try
        {
            var zapisi = UcitajSveIzFajla();
            for(int i = 0; i < zapisi.Count; i++)
            {
                if(zapisi[i].Id == zapis.Id)
                {
                    zapisi[i] = zapis;
                    UpisiSveUFajl(zapisi);
                    return Result.Success();
                }
            }
            zapisi.Add(zapis);
            UpisiSveUFajl(zapisi);
            return Result.Success();
        }
        catch(Exception ex)
        {
            return Result.Failure("json_upis_greska", $"Greska pri upisu u JSON datoteku: {ex.Message}");
        }
    }

    public Result<Zapis> Pronadji(Guid id) 
    {
        try
        {
            var zapisi = UcitajSveIzFajla();
            foreach(var zapis in zapisi)
            {
                if(zapis.Id == id)
                    return Result<Zapis>.Success(zapis);
            }
            return Result<Zapis>.Failure("zapis_nije_pronadjen", $"Zapis sa ID-em '{id}' nije pronadjen");
        }
        catch (Exception ex)
        {
            return Result<Zapis>.Failure("json_citanje_gresaka", $"Greska pri citanju JSON datoteke: {ex.Message}");
        }
    }

    public Result<IReadOnlyList<Zapis>> Svi() 
    {
        try
        {
            var zapisi = UcitajSveIzFajla();
            return Result<IReadOnlyList<Zapis>>.Success(zapisi);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<Zapis>>.Failure("json_citanje_gresaka", $"Greska pri citanju JSON datoteke: {ex.Message}");
        }
    }

    public Result Obrisi(Guid id) 
    {
        try
        {
            var zapisi = UcitajSveIzFajla();
            for(int i = 0; i < zapisi.Count; i++)
            {
                if(zapisi[i].Id == id)
                {
                    zapisi.RemoveAt(i);
                    UpisiSveUFajl(zapisi);
                    return Result.Success();
                }
            }
            return Result.Failure("zapis_nije_pronadjen", $"Zapis sa ID-em '{id}' nije pronadjen");
        }
        catch (Exception ex)
        {
            return Result.Failure("json_brisanje_gresaka", $"Greska pri brisanju iz JSON datoteke: {ex.Message}");
        }
    }
}
