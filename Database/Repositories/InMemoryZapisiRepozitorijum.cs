using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

namespace Database.Repositories;

// TODO: implementirati skladištenje u memoriji.
public sealed class InMemoryZapisiRepozitorijum : IZapisiRepozitorijum
{
    private readonly List<Zapis> _zapisi = new();

    public Result Sacuvaj(Zapis zapis)
    {
        if(zapis is null)
            return Result.Failure("zapis.null", "Zapis ne sme biti null.");
        for(int i = 0; i < _zapisi.Count; i++)
        {
            if(_zapisi[i].Id == zapis.Id)
            {
                _zapisi[i] = zapis;
                return Result.Success();
            }
        }
        _zapisi.Add(zapis);
        return Result.Success();
    } 

    public Result<Zapis> Pronadji(Guid id) 
    {
        foreach(var zapis in _zapisi)
        {
            if(zapis.Id == id)
                return Result<Zapis>.Success(zapis);
        }
        return Result<Zapis>.Failure("zapis_nije_pronadjen", $"Zapis sa ID-em '{id}' nije pronadjen");
    }

    public Result<IReadOnlyList<Zapis>> Svi() 
    {
        return Result<IReadOnlyList<Zapis>>.Success(_zapisi.ToList());
    }

    public Result Obrisi(Guid id) 
    {
        for(int i = 0; i < _zapisi.Count; i++)
        {
            if(_zapisi[i].Id == id)
            {
                _zapisi.RemoveAt(i);
                return Result.Success();
            }
        }
        return Result.Failure("zapis_nije_pronadjen", $"Zapis sa ID-em '{id}' nije pronadjen");
    }
}
