# Instruktioner för att skapa databasen

# Det här behöver du
- .NET 10
- MySQL 8 som är igång på localhost (jag kör den i Docker)
- DataGrip
- Du behöver vara inloggad som root i DataGrip när du kör skripten


## Skapa databasen i DataGrip
### Alternativ 1
- Kör sql/create_database.sql med DataGrip
- Programmet öppnar en konsol med skriptet
- Välj rätt Data Source
- Markera allt
- Tryck på Play-knappen (Execute)
- Kontrollera att schema 'holiday_maker' har skapats med 5 tabeller


### Alternativ 2
- Ta källkoden från sql/create_database.sql
- Öppna DataGrip
- Högerklicka på localhost
- Tryck New - Query Console
- Klistra in källkod
- Markera allt
- Tryck på Play-knappen (Execute)


## Lägga till testdata
### Alternativ 1
- Kör sql/test_data.sql med DataGrip
- Programmet öppnar en konsol med skriptet
- Välj rätt Data Source
- Se till att 'holiday_maker' är vald som schema
- Markera allt
- Tryck på Play-knappen (Execute)
- Kontrollera att testdata har kommit in.

### Alternativ 2
- Ta källkoden från sql/test_data.sql
- Öppna DataGrip
- Högerklicka på localhost
- Tryck New - Query Console
- Kontrollera att rätt Data Source är angiven
- Se till att 'holiday_maker' är vald som schema 
- Klistra in källkod
- Markera allt
- Tryck på Play-knappen (Execute)
- Kontrollera att testdata har kommit in



# Instruktioner för att köra programmet
- Ladda ner från https://github.com/n1cl4s/holiday_maker
- Starta Visual Studio Code och öppna mappen
- Öppna terminal och skriv 'dotnet run'
- För att komma åt databasen så måste du logga in som operator, uppgifter finns i en seperat text-fil på learnpoint
- Mintchokladglass är min favoritsmak. Men väldigt svår att hitta, vilket gör det till ett semi-säkert lösenord.
- Lösenordet står här för att det är ett testkonto som bara finns i din lokala databas. Det ligger inte i koden utan skrivs in när programmet startar.
- Som operator kan du göra allt, detta för att alla tester ska vara möjliga. Så du är egentligen operator/user.


## Hur funkar programmet
- Du har 9 olika alternativ i menyn, om vi inkluderar Avsluta
- Felaktigt val som siffror som inte finns i menyn eller bokstäver blir Ogiltigt val och du kommer tillbaka till menyn
- I och med att det är en kombination av Operator och User så kan du både göra saker som visa kunder, visa rum men även göra bokningar och avbokningar
- Vald kund står ovanför menyn
- Efter varje val trycker du på valfri tangent för att komma tillbaka till menyn
- Du måste ha valt en kund för att boka, visa bokningar, avboka och ändra antal personer. Söka rum går bra utan kund.


### Registrera en kund
1. Ange namn, email, telefonnummer
2. Programmet felhanterar så fältet inte får vara tomma
3. Även om en email inte har en @
4. Därefter blir kunden redan vald och kan använda resten


### Boka ett rum
1. Tryck 3 + enter
2. Välj din en kund
3. Kontrollera över menyn om rätt kund är vald
4. Välj sök och boka rum
5. Skriv in valfritt datum för in- och utcheckning samt antal personer
6. Tryck ok och rummet är bokat på kunden. (Detta går bara om du har valt en kund)
7. Tryck 6. Visa kundens bokningar, för att kontrollera att den har lagts in


### Avboka ett rum
1. Tryck 3 + enter
2. Välj din en kund
3. Kontrollera över menyn om rätt kund är vald
4. Tryck 7. Avboka
5. Programmet frågar efter vilket id som bokningar har som du vill avboka
6. Tryck Enter
7. För att kontrollera gå in på 6. Visa kundens bokningar och se så bokningen är borttagen


### Ändra antal personer på befintlig bokning
1. Tryck 3 + enter
2. Välj din en kund
3. Kontrollera över menyn om rätt kund är vald
4. Välj 8. Ändra antal personer
5. Välj det hotellet du vill byta personer på
5a. Tips om du vill se att det fungerar så byter du ett rum för 2 personer till 1
5b. Om du vill se så att felet hanteras om vi går över rummets kapacitet skriver du 5 på ett rum som står 1 på. 
6. Tryck valfri tangent för att komma tillbaka till menyn
7. Välj 6. Visa kundens bokningar
8. Kontrollera att personer har förändrats, men inte priset (då uppgiften var att antal personer inte påverkade priset)


### Filtrera olika önskemål
Efter antal personer får du frågor om filter. Du behöver inte använda dem, svara n eller tryck bara enter för att hoppa över.
1. Måste ha pool? (j/n)
2. Måste ha restaurang? (j/n)
3. Max avstånd till strand i meter
4. Max avstånd till centrum i meter
5. Sortera på pris eller omdöme? (p/o)

- p visar billigaste rummet först
- o visar högst omdöme först
- I listan ser du omdöme, pool, restaurang och avstånd på varje rum, så du kan se att filtren stämmer
- Tips om du vill se att det fungerar: gör samma sökning som ovan men svara j på pool och restaurang. Då ska bara 2 rum på Hotel Skansen Båstad vara kvar


# Filer i projektet
- Program.cs - inloggning och startar programmet
- Menu.cs - menyn och allt som skrivs ut och matas in
- CustomerDb.cs, RoomDb.cs, BookingDb.cs - alla SQL-frågor
- Customer.cs, Room.cs, Booking.cs - hur en kund, ett rum och en bokning ser ut i koden
- Database.cs - öppnar kopplingen till databasen
- sql/create_database.sql - skapar databasen, vyn och operator
- sql/test_data.sql - testdata
- sql/holiday_maker.jpg - E/R-diagram