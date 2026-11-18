# Архитектура решења

За сваку ставку навести конкретне класе/интерфејсе из сопственог решења и доказ о примени (метод, тест или могућност замене имплементације). Наводи без везе са кодом се не прихватају.

## SRP — једна одговорност
* **Konkretne klase:** 'Zapis', 'CasaVode' (domenski modeli), 'BocaVode', 'Zapis servis' (operacije upisa i promene statusa), 'ZapisPregledServis' (pregled i pretraga), 'InMemoryZapisiRepozitorijum' i 'JsonZapisiRepozitorijum' (perzistencija), 'SazetiZapisFormater' i 'DetaljniZapisFormater' (formatiranje ispisa), 'ConsoleMenu' (interakcija sa korisnikom).
* **Dokaz o primeni:** Domenska validacija zapremine (50-2000 ml) i pravila promene statusa('Potvrdi()', 'Ponisti()') izolovani su u klasama 'Zapis', 'CasaVode' i 'BocaVode', dok je logika cuvanja u JSON fajlu potpuna odvojena u 'JsonZapisiRepozitorijum' ('UcitajSveIzFajla()', 'UpisiSveUFajl()'), a prikaz u formaterima 'Presentation/Formatting/'.

## OCP — отвореност за проширење
* **Konkretni interfejs i klase:** Interfejs 'IZapisFormater' i implementacije 'SazetiZapisFormater' i 'DetaljniZapisFormater'.
* **Dokaz o primeni:** Metoda 'Formatiraj(Zapis zapis)' omogucava dodavanje novog nacina prikaza podataka bez ikakve izmene poslovnih servisa ('ZapisServis', 'ZapisPregledServis') ili postojecih formatera. Dokaz je jedinicni test 'TE7' u 'Tests/Application/ZapisiTestovi.cs', gde je uvedena nova implementacija 'Minimalni formater : IZapisFormater' i odmah koriscena nad modelima 'CasaVode' i 'BocaVode'.

## LSP — заменљивост имплементација
* **Konkretne klase i interfejsi:** Apstraktna klasa 'Zapis' i izvedene klase 'CasaVode' i 'BocaVode'; interfejs 'IZapisiRepozitorijum' i implementacije 'InMemoryZapisiRepozitorijum' i 'JsonZapisiRepozitorijum'.
* **Dokaz o primeni:** Oba izvedena modela ('CasaVode', 'BocaVode') postuju ugovor bazne klase 'Zapis' (svojstva 'TipZapisa', 'ZapreminaMl' i metode 'Potvrdi()', 'Ponisti()'). Oba repozitorijuma ispunjavaju isti ugovor metoda 'Sacuvaj', 'Pronadji', 'Svi' i 'Obrisi', sto je direktno dokazano ugovorom u testu 'TE8' ('ZapisiTestovi.cs') koji pokrece identicne provere nad obe implementacije repozitorijuma.

## ISP — раздвајање интерфејса
* **Konkretni interfejsi i klase:** 'IZapisiCitanjeRepozitorijum' ('Pronadji', 'Svi'), 'IZapisiUpisRepozitorijum('Sacuvaj', 'Obrisi'), 'IZapisiRepozitorijum' (nasledjuje oba) i servis 'ZapisPregledServis'.
* **Dokaz o primeni:** Klasa 'ZapisPregledServis' u svom konstruktoru prima iskljucivo 'IZapisiCitanjeRepozitorijum', tako da nema pristup metodama za upis ili brisanje ('Sacuvaj', 'Obrisi'). U 'Tests/Application/ZapisiTestovi.cs' ('TE9') refleksijom je provereno da konstruktor 'zapisPregledServis' zavisi samo od 'IZapisiCitanjeRepozitorijum'.

## DIP — инверзија зависности
* **Konkretni interfejsi i klase:** 'ZapisServis' zavisi od apstrakcija 'IZapisiRepozitorijum' i 'ILoggerServis'; 'AppContainer' ('Presentation/Composition/AppContainer.cs') sastavlja konkretne implementacije ('InMemoryZapisiRepozitorijum' / 'JsonZapisiRepozitorijum' i 'FileLoggerServis' / 'ConsoleLoggerServis').
* **Dokaz o primeni:** Mogucnost zamene implementacija na jednom mestu u metodi 'AppContainer.Create(rezim, loggerTip)' bez izmene poslovne logike, kao i testovi 'TE6' i 'TE9' u 'ZapisiTestovi.cs', gde se 'ZapisServis' testira izolovano uz lazni repozitorijum('Mock<IzapisiRepozitorijum>', 'LazniRepozitorijum') i lazni loger ('LazniLogger') bez pristupa fajl sistemu

## Clean Architecture — смер зависности

* **Konkretni projekti/slojevi:** 'Domain' i 'Shared' (unutrasnji slojevi), 'Application' (sloj aplikacione logike i interfejsa), 'Database', 'Infrastructure' i 'Presentation' (spoljasnji slojevi)
* **Dokaz o primeni:** Sve zavisnosti su usmerene iskljucivo ka unutra ('Presentation' / 'Database' / 'Infrastructure' -> 'Application' -> 'Domain', 'Shared'). Sloj 'Domain' nema referencu ni na jedan drugi projekat, dok 'Application' referencira samo 'Domain' i 'Shared' (verifikovano arhitektonskim testovima u  'Tests/Architecture/LayeringTests.cs' i referencama u '.csproj' fajlovima).
