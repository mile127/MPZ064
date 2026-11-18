using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Database.Repositories;
using Domain.Models;
using NUnit.Framework;
using Presentation.Formatting;
using Shared;
using Moq;

namespace Tests.Application;

[TestFixture]
public class ZapisiTestovi
{
    private sealed class LazniLogger : ILoggerServis
    {
        public List<string> Poruke { get; } = new();
        public bool Loguj(string poruka)
        {
            Poruke.Add(poruka);
            return true;
        }
    }
    private sealed class LazniRepozitorijum : IZapisiRepozitorijum
    {
        public List<Zapis> Stavke { get; } = new();
        public Result Sacuvaj(Zapis zapis)
        {
            var idx = Stavke.FindIndex(z => z.Id == zapis.Id);
            if(idx >= 0) 
                Stavke[idx] = zapis;
            else
                Stavke.Add(zapis);
            return Result.Success();
        }
        public Result<Zapis> Pronadji(Guid id)
        {
            var z = Stavke.FirstOrDefault(x => x.Id == id);
            if(z is null)
                return Result<Zapis>.Failure("nije_pronadjen", "Nije pronadjen");
            return Result<Zapis>.Success(z);
        }
        public Result<IReadOnlyList<Zapis>> Svi() => Result<IReadOnlyList<Zapis>>.Success(Stavke.ToList());
        public Result Obrisi(Guid id)
        {
            var removed = Stavke.RemoveAll(x => x.Id == id);
            return removed > 0 ? Result.Success() : Result.Failure("nije_pronadjen", "Nije pronadjen");
        }
    }
    private sealed class MinimalniFormater : IZapisFormater
    {
        public string Formatiraj(Zapis zapis) => $"{zapis.Id}:{zapis.ZapreminaMl}";
    }

    [Test]
    public void TE1()
    {
        var repo = new InMemoryZapisiRepozitorijum();
        var servis = new ZapisServis(repo, new LazniLogger());
        var pregled = new ZapisPregledServis(repo);

        var r1 = servis.DodajCasuVode("Prva casa", "prva popijena casa", 250);
        var r2 = servis.DodajBocuVode("Aqua viva", "Boca za fakultet", 750);

        Assert.That(r1.IsSuccess, Is.True);
        Assert.That(r2.IsSuccess, Is.True);

        var svi = pregled.VratiSveZapise();
        Assert.That(svi.IsSuccess, Is.True);
        Assert.That(svi.Value, Has.Count.EqualTo(2));
        Assert.That(svi.Value!.OfType<CasaVode>().Single().ZapreminaCase, Is.EqualTo(250));
        Assert.That(svi.Value!.OfType<BocaVode>().Single().ZapreminaBoce, Is.EqualTo(750));
    }

    [TestCase(49)]
    [TestCase(0)]
    [TestCase(-100)]
    [TestCase(2001)]
    public void TE2(int neispravnaZapremina)
    {
        var repo = new InMemoryZapisiRepozitorijum();
        var servis = new ZapisServis(repo, new LazniLogger());
        var rezCasa = servis.DodajCasuVode("Casa", "Opis", neispravnaZapremina);
        var rezBoca = servis.DodajBocuVode("Boca", "Opis", neispravnaZapremina);
        Assert.That(rezCasa.IsFailure, Is.True);
        Assert.That(rezBoca.IsFailure, Is.True);
        Assert.That(repo.Svi().Value, Is.Empty);
    }

    [Test]
    public void TE3()
    {
        var repo = new InMemoryZapisiRepozitorijum();
        var servis = new ZapisServis(repo, new LazniLogger());
        var pregled = new ZapisPregledServis(repo);
        servis.DodajCasuVode("Casa danas", "Opis", 300);
        var danas = DateOnly.FromDateTime(DateTime.UtcNow);
        var sutra = danas.AddDays(1);
        var pogodak = pregled.PretraziPoDatumu(danas);
        var bezPogotka = pregled.PretraziPoDatumu(sutra);
        Assert.That(pogodak.IsSuccess, Is.True);
        Assert.That(pogodak.Value, Has.Count.EqualTo(1));
        Assert.That(bezPogotka.IsSuccess, Is.True);
        Assert.That(bezPogotka.Value, Is.Empty); 
    }

    [Test]
    public void TE4()
    {
        var repo = new InMemoryZapisiRepozitorijum();
        var servis = new ZapisServis(repo, new LazniLogger());
        var dodavanje = servis.DodajCasuVode("Casa", "Opis", 200);
        var id = dodavanje.Value!.Id;
        //Dozvoljena potvrda (F5)
        Assert.That(servis.PotvrdiZapis(id).IsSuccess, Is.True);
        Assert.That(repo.Pronadji(id).Value!.Status, Is.EqualTo(StatusZapisa.Potvrdjen));
        // Dozvoljeno ponistavanje (F6)
        Assert.That(servis.PonistiZapis(id).IsSuccess, Is.True);
        Assert.That(repo.Pronadji(id).Value!.Status, Is.EqualTo(StatusZapisa.Ponisten));
        // Nedozvoljeno ponovno ponistavanje (F6)
        Assert.That(servis.PonistiZapis(id).IsFailure, Is.True);
        // Nedozvoljena potvrda vec ponistenog zapisa (F5)
        Assert.That(servis.PotvrdiZapis(id).IsFailure, Is.True);
        // Nepostojeci ID (F5)
        Assert.That(servis.PotvrdiZapis(Guid.NewGuid()).IsFailure, Is.True);
    }

    [Test]
    public void TE5()
    {
        string tempFajl = Path.Combine(Path.GetTempPath(), $"mpz064_{Guid.NewGuid()}.json");
        try
        {
            var repo1 = new JsonZapisiRepozitorijum(tempFajl);
            var casa = new CasaVode("Casa 1", "Opis 1", 300);
            var boca = new BocaVode("Boca 1", "Opis 2", 750);
            repo1.Sacuvaj(casa);
            repo1.Sacuvaj(boca);
            var repo2 = new JsonZapisiRepozitorijum(tempFajl);
            var svi = repo2.Svi();
            Assert.That(svi.IsSuccess, Is.True);
            Assert.That(svi.Value, Has.Count.EqualTo(2));
            Assert.That(repo2.Pronadji(casa.Id).Value!.ZapreminaMl, Is.EqualTo(300));
            Assert.That(repo2.Pronadji(boca.Id).Value!.ZapreminaMl, Is.EqualTo(750));
        }
        finally
        {
            if(File.Exists(tempFajl))
                File.Delete(tempFajl);
        }
    }

    [Test]
    public void TE6()
    {
        var mockRepo = new Mock<IZapisiRepozitorijum>();
        mockRepo.Setup(r => r.Sacuvaj(It.IsAny<Zapis>())).Returns(Result.Success());
        var servis = new ZapisServis(mockRepo.Object, new LazniLogger());
        var rezultat = servis.DodajCasuVode("Casa", "Opis", 250);
        Assert.That(rezultat.IsSuccess, Is.True);
        mockRepo.Verify(r => r.Sacuvaj(It.Is<CasaVode>(c => c.ZapreminaCase == 250)), Times.Once);
    }

    [Test]
    public void TE7()
    {
        var casa = new CasaVode("Casa", "Opis", 250);
        var boca = new BocaVode("Boca", "Opis", 1500);
        IZapisFormater sazeti = new SazetiZapisFormater();
        IZapisFormater detaljni = new DetaljniZapisFormater();
        IZapisFormater minimalni = new MinimalniFormater();
        Assert.That(sazeti.Formatiraj(casa), Does.Contain(casa.Id.ToString()).And.Not.Contain("250 ml"));
        Assert.That(detaljni.Formatiraj(casa), Does.Contain("Zapremina case: 250 ml"));
        Assert.That(detaljni.Formatiraj(boca), Does.Contain("Zapremina boce: 1500 ml"));
        Assert.That(minimalni.Formatiraj(casa), Is.EqualTo($"{casa.Id}:250"));
    }

    [Test]
    public void TE8()
    {
        string tempFajl = Path.Combine(Path.GetTempPath(), $"mpz064_lsp_{Guid.NewGuid()}.json");
        try
        {
            IZapisiRepozitorijum[] repozitorijumi = 
            {
                new InMemoryZapisiRepozitorijum(),
                new JsonZapisiRepozitorijum(tempFajl)
            };
            foreach(var repo in repozitorijumi)
            {
                var casa = new CasaVode("Casa", "Opis", 200);
                var boca = new BocaVode("Boca", "Opis", 1000);
                Assert.That(repo.Sacuvaj(casa).IsSuccess, Is.True);
                Assert.That(repo.Sacuvaj(boca).IsSuccess, Is.True);
                Assert.That(repo.Svi().Value, Has.Count.EqualTo(2));
                Assert.That(repo.Pronadji(casa.Id).IsSuccess, Is.True);
                Assert.That(repo.Pronadji(Guid.NewGuid()).IsFailure, Is.True);
            }
        }
        finally
        {
            if(File.Exists(tempFajl))
                File.Delete(tempFajl);
        }
    }

    [Test]
    public void TE9()
    {
        var lazniRepo = new LazniRepozitorijum();
        var lazniLogger = new LazniLogger();
        var servis = new ZapisServis(lazniRepo, lazniLogger);
        var rezultat = servis.DodajBocuVode("Boca", "Opis", 500);
        Assert.That(rezultat.IsSuccess, Is.True);
        Assert.That(lazniRepo.Stavke, Has.Count.EqualTo(1));
        Assert.That(lazniLogger.Poruke, Has.Count.EqualTo(1));
    }
}