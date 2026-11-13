# Holiday Maker

Holiday maker är ett bokningssystem för hotell. Kunden kan söka lediga rum, boka, ändra antal personer och avboka sin bokning.

## Det här behövs

- .NET 10 SDK
- MySQL 8 (jag kör den i Docker, containern 'mysql-kurs' på port 3306)
- Ett SQL-verktyg till exempel jag använder DataGrip

## 1. Skapa databasen

Kör de tre skripten i mappen 'sql/' i den här ordningen:

1. 'sql/schema.sql' skapar databasen 'holiday_maker' med alla tabeller, begränsningar och index.
2. 'sql/testdata.sql' lägger in testdata: 4 städer, 5 hotell, 20 rum, 5 kunder och 25 bokningar.
3. 'sql/user.sql' skapar användaren 'holiday_app' som appen loggar in med. Byt 'VALFRITT_LOSENORD' mot ett eget lösenord först.

## 2. Lägg in lösenordet

Lösenordet till databasen finns **inte** i koden, och det finns inte på GitHub.
Skapa en fil som heter 'dbpassword.txt' i projektmappen (bredvid 'Program.cs').
Skriv **bara lösenordet** till MySQL-användaren 'holiday_app' i filen, inget annat
Filen står i '.gitignore', så den laddas aldrig upp.

## 3. Kör appen

Skriv 'dotnet run' i terminalen

## Menyn

| Val | Vad det gör                                                                                                       |
| --- | ----------------------------------------------------------------------------------------------------------------- |
| 1   | Registrera ny kund (loggas in direkt)                                                                             |
| 2   | Sök lediga rum: stad, datum, antal personer + filter (pool, restaurang, avstånd) och sortering (pris eller betyg) |
| 3   | Boka rum (man måste vara inloggad)                                                                                |
| 4   | Visa mina bokningar med totalpris                                                                                 |
| 5   | Ändra antal personer i en bokning                                                                                 |
| 6   | Avboka                                                                                                            |
| 7   | Logga in med e-post                                                                                               |
| 0   | Avsluta                                                                                                           |

Skriv **b** när som helst för att gå tillbaka till menyn.

**Testa snabbt:** välj 7 och logga in med `anna@mail.se`. Välj sedan 2 och sök på Manila, 2026-12-03 till 2026-12-07, 3 personer.

## Filer

| Fil | Innehåll |
| --- | --- |
| `Program.cs` | Kopplingen till databasen och menyn (switch) |
| `Menu.cs` | Menytexten, säker inmatning (ReadRequired, ReadInt, ReadDate, ReadYesNo), b för tillbaka och tabellerna |
| `CustomerRepository.cs` | Registrera kund och logga in |
| `RoomRepository.cs` | Sök lediga rum och hämta hotellöversikten |
| `BookingRepository.cs` | Boka (transaktion), visa, ändra och avboka |
| `AvailableRoom.cs`, `BookingInfo.cs`, `HotelInfo.cs` | Records för resultaten |
| `sql/` | schema.sql, testdata.sql, user.sql och queries.sql |
| `ER-diagram-individuell.webp` | Mitt E/R-diagram |
| `ER-diagram-grupp.webp` | Gruppens E/R-diagram |
| `Testdokumentation.md` | Alla tester (databastester och app-tester A1–A37) |
| `Reflektion.md` | Min reflektion |

## Säkerhet

- Alla SQL-anrop använder `@parametrar`, vilket skyddar mot SQL-injektion.
- Bokningen sker i en **transaktion** som kontrollerar igen att rummet är ledigt innan den sparar.
- En kund kan bara ändra och avboka sina **egna** bokningar.
- Lösenordet ligger utanför koden, i `dbpassword.txt`.
- Appen loggar in som `holiday_app`, som bara får SELECT, INSERT, UPDATE och DELETE. Den kan inte ta bort tabeller (minsta behörighet).
