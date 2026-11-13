-- Skapar en egen användare för appen (minsta behörighet).
-- Appen får bara läsa, lägga till, ändra och ta bort rader.
-- Den får INTE ta bort tabeller (DROP) eller ändra strukturen (ALTER).
-- Byt VALFRITT_LOSENORD mot ett eget lösenord och skriv samma lösenord i dbpassword.txt.
CREATE USER holiday_app IDENTIFIED BY 'VALFRITT_LOSENORD';
GRANT SELECT, INSERT, UPDATE, DELETE ON holiday_maker.* TO holiday_app;