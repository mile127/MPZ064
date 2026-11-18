using Domain.Models;
using Shared;

namespace Application.Interfaces.Repositories;

public interface IZapisiUpisRepozitorijum
{
    Result Sacuvaj(Zapis zapis);
}