using Domain.Models;
using Shared;

namespace Application.Interfaces.Repositories;

// Polazni ugovor: student ga menja ili razdvaja prema operacijama svog zadatka.
public interface IZapisiRepozitorijum : IZapisiCitanjeRepozitorijum, IZapisiUpisRepozitorijum
{
    Result Obrisi(Guid id);
}
