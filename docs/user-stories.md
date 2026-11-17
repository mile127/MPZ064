# Корисничке приче

<!-- Самостално написати шест прича према додељеној спецификацији. -->
<!-- За сваку причу навести улогу, могућност, корист, F-ознаку и >= 2 критеријума прихватања. -->

## US-01 (F1)

Kao korisnik aplikacije, zelim da evidentiram unos case vode sa nazivom, opisom i zapreminom case, kako bih pratio unos vode tokom dana.

Kriterijumi prihvatanja:
- Uspesan slucaj: Korisnik unese naziv, opis i zapreminu vrednosti od 50 ml do 2000 ml, sistem cuva zapis "Casa vode" i prikazuje dodeljeni identifikator.
- Neuspesan slucaj: Korisnik ne unese naziv ili opis, ili unese zapreminu manju od 50 ml ili vecu od 2000 ml, sistem odbija unos i prikazuje poruku o gresci.

## US-02 (F2)

Kao korisnik aplikacije, zelim da evidentiram unos vode iz boce sa nazivom, opisom i zapreminom boce, kako bih pratio unos vode tokom dana.

Kriterijumi prihvatanja:
- Uspesan slucaj: Korisnik unese naziv, opis i zapreminu vrednosti od 50 ml do 2000 ml, sistem cuva zapis "Boca vode" i prikazuje dodeljeni identifikator.
- Neuspesan slucaj: Korisnik ne unese naziv ili opis, ili unese zapreminu manju od 50 ml ili vecu od 2000 ml, sistem odbija unos i prikazuje poruku o gresci.

## US-03 (F3)

Kao korisnik aplikacije, zelim da prikazem sve evidentirane zapise sa tipom, identifikatorom i statusom, kao i dnevni zbir zapremine, kako bih imao pregled ukupnog unosa vode.

Kriterijumi prihvatanja:
- Uspesan slucaj: Kada u sistemu postoje evidentirani zapisi, sistem prikazuje listu svih zapisa sa tipom. identifikatorom i statusom, kao i dnevni zbir zapremine.
- Granicni slucaj: Kada u sistemu nema evidentiranih zapisa, sistem prikazuje odgovarajuce obavestenje da nema zapisa i zbir zapremine 0 ml.

## US-04 (F4)

Kao korisnik aplikacije, zelim da pretrazim zapise prema datumu unosa kako bih mogao da proverim unos za taj dan.

Kriterijumi prihvatanja: 
- Uspesan slucaj: Kada korisnik unese datum u formatu GGGG-MM-DD za koji postoje zapisi, sistem prikazuje sve zapise kreirane tog datuma.
- Neuspesan slucaj: Kada korisnik unese datum u pogresnom formatu, sistem odbija pretragu i prikazuje poruku o gresci.
- Granicni slucaj: Kada za ispravno unet datum ne postoje evidentirani zapisi, sistem prikazuje poruku da nema rezultata.

## US-05 (F5)

Kao korisnik aplikacije, zelim da potvrdim unos tecnosti, kako bih oznacio da je taj unos validan i zavrsen.

Kriterijumi prihvatanja:
- Uspesan slucaj: Kada korisnik unese postojeci identifikator cija je kolicina veca od nule, sistem potvrdjuje unos i menja njegov status u potvrdjen.
- Neuspesan slucaj: Kada korisnik unese nepostojeci identifikator ili je kolicina nula (ili manja), sistem odbija operaciju i prikazuje poruku o gresci bez izmene podataka.

## US-06 (F6)

Kao korisnik aplikacije, zelim da ponistim pogresno unet zapis, kako bih uklonio neispravne unose iz svoje evidencije.

Kriterijumi prihvatanja:
- Uspesan slucaj: Kada korisnik unese identifikator postojeceg zapisa koji prethodno nije ponisten, sistem ponistava zapis i menja njegov status u ponisten.
- Neuspesan slucaj: Kada je zapis vec ponisten ili uneti identifikator ne postoji, sistem odbija operaciju i prikazuje poruku o gresci bez promene stanja.