# Reflektion – Holiday Maker

## 1. Datamodellen

Jag ändrade gruppens ER-diagram. Jag lade till en extra tabell, cities, för normalisering. Jag ändrade också pool och restaurang till boolean i stället för int, eftersom boolean passar bäst: det är bara sant eller falskt, antingen finns de eller så finns de inte.

Om stadens namn skrivs i varje hotell och någon stavar fel, till exempel "Manilla", kommer det hotellet inte att vara med i sökningen. Det syns inte i resultatlistan. Med tabellen cities skrivs "Manila" bara en gång, och hotellen pekar på stadens id. Då kan det inte bli olika stavningar.

Jag sparar inte totalpriset i databasen, eftersom det räknas ut automatiskt: antal nätter × rummets pris per natt (inte per person). Om priset var sparat och någon ändrade datumen, kunde det sparade priset bli fel, eftersom det inte räknas om.

## 2. Säkerhet

- @parametrar i alla SQL-anrop. De skyddar mot SQL-injektion, alltså att någon skriver SQL-kod i en ruta (till exempel e-postrutan) och försöker ändra eller ta bort data.
- Lösenordet ligger i dbpassword.txt och inte i koden. Repot på GitHub är publikt. Om lösenordet låg i koden kunde vem som helst se det. Filen står i .gitignore.
- Bokningen sker i en transaktion. Utan den kan ett rum bli dubbelbokat om två personer bokar samtidigt. Transaktionen kollar igen att rummet är ledigt och låser det tills bokningen är sparad.
- customer_id = @customerId när man ändrar eller avbokar. Det hindrar en kund från att ändra eller avboka någon annans bokning (test A18).
- Appen loggar in som holiday_app i stället för root. Den får bara läsa, lägga till, ändra och ta bort rader (SELECT, INSERT, UPDATE, DELETE). Den kan inte ta bort eller ändra tabellerna, så om något går fel kan appen inte förstöra databasen.

## 3. Resultat och tester

Appen kan registrera kunder, logga in med e-post, söka lediga rum (stad, datum, antal personer) med filter för pool, restaurang och avstånd, och sortera på pris eller betyg. Kunden kan boka, se sina bokningar med totalpris, ändra antal personer och avboka.

Jag har testat databasens begränsningar (CHECK, UNIQUE och FOREIGN KEY), och de stoppade felaktig data som de skulle. Jag har också gjort 37 tester i appen (A1–A37), och alla blev godkända. Alla tester finns i Testdokumentation.md. Några exempel: man kan boka samma dag som förra gästen checkar ut, en bokning som krockar med eller omsluter en annan nekas, en kund kan inte ändra någon annans bokning, och bokningar finns kvar efter omstart.

Efter min presentation fick jag förslag från seminariegruppen: att visa sökresultat och bokningar som tabeller och att kunna gå tillbaka till menyn. Jag lade till det och testade varje ändring själv (test A32–A37).

Det svåraste var att koppla C# till databasen. Det jag är mest nöjd med är att jag lärde mig hur smidigt man kan skriva SQL med hjälp av tabellverktyget i DataGrip. Jag har också testat att skriva koden i konsolen, men det var inte lika spännande som att göra det med hjälp av tabellen. 🙂

## 4. Förbättringar

Om jag hade mer tid skulle jag göra ett grafiskt gränssnitt i stället för en konsol. I konsolen måste man skriva startdatum och slutdatum för hand i rätt format. Med ett grafiskt gränssnitt blir det lättare och snabbare att välja datumen.

Jag skulle också vilja ha riktig inloggning med lösenord. Nu räcker det att skriva någons e-post för att se hens bokningar, och det är en säkerhetsbrist. Det fanns inte med i uppgiften, men i en riktig app skulle det behövas.

## 5. Hur jag har använt AI

Jag har använt AI (Claude) under projektet. Mest har jag använt det för att lära mig, inte bara för att göra uppgiften färdig utan att förstå varför jag gjorde den. Claude har ställt frågor och gett exempel, och jag har själv svarat och fattat besluten. Till exempel visade Claude ett exempel med ett hotell där orten var felstavad, och jag kom själv fram till att det hotellet missas i sökningen. Därför valde jag en egen tabell för städer.
Där jag hade svårt att komma vidare själv har jag också fått färdig kod av AI. Jag kontrollerade AI:ns kod genom att köra den och testa allt. AI hjälpte mig att planera testerna, men jag körde dem själv i appen och i DataGrip. Alla 37 tester finns i Testdokumentation.md.
