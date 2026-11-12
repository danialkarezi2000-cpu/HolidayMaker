# Testdokumentation – Holiday Maker

Alla tester är manuella. Databastesterna kördes i DataGrip, app-testerna i terminalen med `dotnet run`.

## Uppgiftens 8 testpunkter

| Testpunkt i uppgiften | Test |
|---|---|
| Ett ledigt rum visas och ett upptaget rum filtreras bort | A10, A15 |
| Ett rum kan bokas samma dag som en tidigare kund checkar ut | A21 |
| Överlappande bokningar nekas, även när den ena perioden helt omsluter den andra | A22, A23 |
| Ett för stort sällskap kan inte boka rummet | A14 |
| Kombinerade sökfilter och båda sorteringsalternativen fungerar | A10 (pris), A11, A26 (betyg), A27 |
| Ändring av antal personer följer rummets kapacitet | A17, A19, A30 |
| Avbokning frigör rummet | A25 |
| Uppgifterna finns kvar efter att applikationen startats om | A24 |

## Databastester (körda 2026-11-10 i DataGrip)

| Test | Vad testas | Förväntat | Resultat |
|---|---|---|---|
| 1 | Antal rader i alla tabeller | 4 / 5 / 5 / 20 / 25 | ✅ Godkänt (4, 5, 5, 20, 25 rows retrieved) |
| 2 | Sökning: lediga rum 3–7 dec, minst 3 pers, Manila | 4 rader | ✅ Godkänt (4 rows retrieved) |
| 3 | UNIQUE email: samma e-post igen | Fel | ✅ Godkänt: Duplicate entry 'anna@mail.se' for key 'customers_email_uq' |
| 4 | CHECK rating: betyg 6 | Fel | ✅ Godkänt: Check constraint 'hotels_rating_chk' is violated |
| 5 | CHECK datum: utcheckning före incheckning | Fel | ✅ Godkänt: Check constraint 'bookings_dates_chk' is violated |
| 6 | FOREIGN KEY: rum 999 finns inte | Fel | ✅ Godkänt: a foreign key constraint fails (bookings_rooms_id_fk) |
| 7 | UNIQUE (hotel_id, name): rum 101 på hotell 1 igen | Fel | ✅ Godkänt: Duplicate entry '1-101' for key 'rooms_hotel_name_uq' |
| 8 | Index bookings_dates_idx finns | Skapat | ✅ Godkänt: CREATE INDEX completed |

**Slutsats:** databasen stoppar felaktig data med UNIQUE, CHECK och FOREIGN KEY, och sökningen efter lediga rum ger rätt resultat.

## App-tester (C#)

| # | Test | Förväntat | Resultat | Datum |
|---|------|-----------|----------|-------|
| A1 | Menyval 1–6, "abc" och "2 " (med mellanslag) | Rätt val körs, "abc" ger "Felaktigt val", mellanslag tas bort med Trim | ✅ Godkänt | 2026-11-10 |
| A2 | Registrera ny kund (omid@gmail.com) | Sparas, appen visar nytt id | ✅ Godkänt (id 7) | 2026-11-10 |
| A3 | Registrera samma e-post igen, utan try/catch | Databasen stoppar dubbletten (UNIQUE) | ✅ Databasen stoppade, men appen kraschade (Duplicate entry) | 2026-11-10 |
| A4 | Samma e-post igen, med try/catch | Snällt meddelande, menyn kommer tillbaka | ✅ "Den e-posten finns redan. Försök med en annan." | 2026-11-10 |
| A5 | Registrera kund med tomt förnamn och efternamn (bara Enter) | Ska stoppas | ❌ Bugg hittad: sparades (id 10). NOT NULL stoppar inte tom text "". Åtgärd: Menu.ReadRequired frågar igen tills fältet inte är tomt | 2026-11-10 |
| A6 | Tomt förnamn efter åtgärd (bara Enter flera gånger) | Appen frågar igen | ✅ Godkänt: "Fältet får inte vara tomt, försök igen." och frågar på nytt | 2026-11-10 |
| A7 | Logga in med befintlig e-post (omid@gmail.com) | Inloggad med rätt id | ✅ "Inloggad som kund 7" | 2026-11-10 |
| A8 | Logga in med e-post som inte finns (fel@mail.se) | Snällt meddelande | ✅ "Ingen kund med den e-posten. Registrera dig först (val 1)." | 2026-11-10 |
| A9 | Sök rum: skriv "3" som incheckningsdatum | Appen frågar igen | ✅ "Fel format. Skriv till exempel 2026-12-03." och frågar på nytt (Menu.ReadDate) | 2026-11-10 |
| A10 | Sök lediga rum: Manila, 2026-12-03 till 2026-12-07, 3 personer | 4 rum, samma som Q7 i DataGrip | ✅ Rum 19, 20, 3, 4 sorterade på pris (600, 750, 1100, 1400 kr) | 2026-11-10 |
| A11 | Sök Manila 2026-12-03 till 2026-12-07, 3 pers, pool = j | Bara rum på hotell med pool | ✅ Bara rum 3 och 4 (Manila Bay Hotel). Budget Inn (ingen pool) filtrerades bort | 2026-11-10 |
| A12 | Boka rum (val 3) utan att vara inloggad | Stoppas med meddelande | ✅ "Du måste logga in först (val 7) eller registrera dig (val 1)." | 2026-11-10 |
| A13 | Inloggad kund 7 bokar rum 3, 2026-12-03 till 2026-12-07, 3 pers | Bokning sparas, totalpris 4 nätter × 1100 | ✅ "Bokat! Bokningsnummer 27. Totalpris 4400.00 kr." | 2026-11-10 |
| A14 | Boka rum 4 (plats för 4) med 9 personer | Stoppas, ingen bokning sparas (Rollback) | ✅ "Rummet har bara plats för 4 personer." | 2026-11-10 |
| A15 | Sök Manila 2026-12-03 till 2026-12-07 igen efter bokning 27 | Rum 3 ska inte visas längre | ✅ Bara rum 19, 20, 4 visas | 2026-11-10 |
| A16 | Visa mina bokningar (val 4) som kund 7 | Bokning 27 med hotell, rum, datum, personer, totalpris | ✅ "Bokning 27: Manila Bay Hotel, rum 201, 2026-12-03 till 2026-12-07, 3 pers, 4400.00 kr" | 2026-11-10 |
| A17 | Ändra antal personer på bokning 27 till 9 (rummet rymmer 3) | Nekas, ingen ändring | ✅ "Gick inte: ... får inte så många plats i rummet." | 2026-11-10 |
| A18 | Kund 7 försöker ändra bokning 4 (tillhör en annan kund) | Nekas, eftersom WHERE kräver customer_id = inloggad kund | ✅ "Gick inte: bokningen finns inte, är inte din..." | 2026-11-10 |
| A19 | Ändra bokning 27 till 2 personer (rummet rymmer 3) | Ändras, syns i val 4 | ✅ "Antal personer är ändrat." Val 4 visar 2 pers (pris oförändrat 4400 kr, eftersom priset gäller per rum och natt) | 2026-11-10 |
| A20 | Avboka bokning 27 (val 6), sedan visa bokningar (val 4) | Bokningen tas bort | ✅ "Bokningen är avbokad." och sedan "Du har inga bokningar." | 2026-11-10 |
| A21 | Boka rum 2 från 2026-12-08, samma dag som förra gästen checkar ut (bokning 5–8 dec) | Ska gå, eftersom utcheckningsdagen är ledig | ✅ "Bokat! Bokningsnummer 28. Totalpris 1700.00 kr." (2 nätter × 850) | 2026-11-10 |
| A22 | Boka rum 14 2026-12-02 till 2026-12-04 (inuti befintlig bokning 1–8 dec) | Nekas | ✅ "Rummet är redan bokat de datumen." (Rollback) | 2026-11-10 |
| A23 | Boka rum 14 2026-11-25 till 2026-12-20 (omsluter bokningarna 1–8 och 8–12 dec helt) | Nekas | ✅ "Rummet är redan bokat de datumen." | 2026-11-10 |
| A24 | Avsluta appen (0), starta igen, logga in, visa bokningar | Bokning 28 finns kvar (sparad i databasen, inte bara i minnet) | ✅ "Bokning 28: Manila Bay Hotel, rum 102, 2026-12-08 till 2026-12-10, 2 pers, 1700.00 kr" | 2026-11-10 |
| A25 | Avboka bokning 28, sök sedan Manila 2026-12-08 till 2026-12-10, 2 pers | Rum 2 ska vara ledigt igen | ✅ "Bokningen är avbokad." Rum-id 2 syns i sökresultatet igen | 2026-11-10 |
| A26 | Sök Manila 2026-12-03 till 2026-12-07, 3 pers, inga filter, sortera på betyg (j) | Manila Bay Hotel (betyg 4) först, Manila Budget Inn (betyg 2) sist | ✅ Rum-id 3, 4 (Manila Bay, betyg 4) och sedan rum-id 19, 20 (Budget Inn, betyg 2) | 2026-11-11 |
| A27 | Sök Manila 2026-12-03 till 2026-12-07, 3 pers, pool j, restaurang j, max 500 m till stranden, sortera på betyg (j) | Bara Manila Bay Hotel (pool, restaurang, 200 m till stranden). Budget Inn filtreras bort | ✅ Bara rum-id 3 och 4 (Manila Bay Hotel, betyg 4) | 2026-11-11 |
| A28 | Sök en stad som inte finns (Malmö), och Lund med datum 2002-02-02 till 2002-02-03 + pool + restaurang | Inga rum, appen kraschar inte | ✅ "Inga lediga rum hittades." | 2026-11-12 |
| A29 | Ny kund (id 12) bokar rum 7 i Lund 2026-12-13 till 2026-12-15, 2 pers | Bokning sparas, totalpris 2 nätter × 1500 | ✅ "Bokat! Bokningsnummer 29. Totalpris 3000.00 kr." | 2026-11-12 |
| A30 | Ändra bokning 29 till 4 personer (rum 7 rymmer 3) | Nekas | ✅ "Gick inte: ... får inte så många plats i rummet." Val 4 visar fortfarande 2 pers | 2026-11-12 |
| A31 | Avboka bokning 29 | Bokningen tas bort | ✅ "Bokningen är avbokad." | 2026-11-12 |
| A32 | Välj 2 (Sök lediga rum) | En tabell med alla städer och hotell visas först (pris, strand, centrum, pool, restaurang, betyg) | ✅ Tabellen visar 5 hotell i Istanbul, Lund, Manila och Paris | 2026-11-12 |
| A33 | Sök Lund 2026-12-09 till 2026-12-15, 2 pers, inga filter | Lediga rum visas som en tabell | ✅ Tabell med rum-id 7 (rum 201, 1500 kr) och 8 (rum 202, 1800 kr), Lund Stadshotell | 2026-11-12 |
| A34 | Logga in som anna@mail.se (val 7), visa bokningar (val 4) | Bokningarna visas som en tabell med totalpris | ✅ Tabell med 5 bokningar (1, 6, 21, 16, 11) och totalpris | 2026-11-12 |
| A35 | Skriv b vid "Stad:" (val 2) och vid "Förnamn:" (val 1) | Tillbaka till menyn, appen kraschar inte | ✅ "Tillbaka till menyn." och menyn visas igen | 2026-11-12 |
| A36 | Anna bokar rum 7 (Lund, rum 201) 2026-12-09 till 2026-12-15, 2 pers, visar sedan bokningar (val 4) | Bokningen sparas och syns i tabellen, 6 nätter × 1500 = 9000 kr | ✅ "Bokat! Bokningsnummer 30. Totalpris 9000.00 kr." Bokning 30 syns i tabellen | 2026-11-12 |
| A37 | Anna bokar rum 8 (Lund, rum 202) 2026-12-09 till 2026-12-15, 2 pers | Efter "Bokat!" visas den nya bokningen som en tabell, 6 nätter × 1800 = 10800 kr | ✅ "Bokat! Bokningsnummer 31. Totalpris 10800.00 kr." och tabell med bokning 31 | 2026-11-12 |
