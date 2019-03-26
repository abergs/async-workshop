# ASYNC .NET Workshop

Examples and explanation on behaviors for Tasks & Exceptions.


Slides:  https://slides.com/abergs/async-net#/

## Manus

Berätta om varför, async i .net är svårt. De som inte programmerar .net kan skippa detta om ni vill.
Flytta er nära projektorn så att ni ser


Vad är ASYNC / AWAIT?
Jag är egentligen noob på detta.
Hela den här presentationen är saker jag nyligen lärt mig - typ.

Men jag ska skulle förklara async/await är keywords i C# som underlättar att arbeta med Tasks (TPL) för att arbeta asynkront. 

Asynkront vs parallellt
Att göra saker asynkront innebär att inte "blocka" medans man gör arbetet. Parallelt är att göra flera saker samtidigt. Ett exempel

Synkron värld:
I .NET har man en threadpool som tar hand om en request.
Tänk er en påse med 20 trådar i.
När ett anrop kommer in tar man en tråd ur påsen och använder för att läsa headers och börja köra kod.
I tråden anropar vi databasen, väntar på svar och returnerar ett resultat.
Efter att resultatet är returnerat så läggs tråden tillbaks i påsen.

Om för många requests kommer in samtidigt tar trådarna slut och requesten får vänta på att trådar returneras.

När vi introducerar async så ser flödet ut så här:
I .NET har man en threadpool som tar hand om en request.
Tänk er en påse med 20 trådar i.
När ett anrop kommer in tar man en tråd ur påsen och använder för att läsa headers och börja köra kod.
I tråden anropar vi databasen. Men istället för att vänta på svaret (blocka tråden) så lägger vi tillbaka tråden i threadpoolen.
När databasen svarar med data så tar den en ny tråd från påsen och använder för köra svars-koden och returnerar ett resultat.
Efter att resultatet är returnerat så läggs tråden tillbaks i påsen.

Det gör att vi kan återanvända tråden och behandla fler requests medans vi väntar på svar från andra resurser som "är asynkrona" - t.ex. Databaser, http-anrop, filsystem, timers osv…

Okej.. Dags för lite quiz.

Förklara reglerna i Kahoot
Man får poäng för rätt svar och mer poäng ju snabbare man svarar.
Ni svarar med mobilerna.
Förklara papper
Berätta om methoden till höger
Det är metoden (endpointen) till vänster som anropar metoden till höger
Förstår alla regelerna?

Fråga dom som hade rätt att förklara varför det blir.

Splitta så vi ser CrashAsync och VoidCrash till höger



/name
Vi glömde awaita resultatet av tasken
(Vi har en icke async metod inte returnerar en task)

/name-correct
vi awaitar resultatet av tasken
(i en async Task method)

/throw
Vi har en async Task metod.
Vi glömde awaita resultatet av tasken

/throw-correct
vi har en async task metod
vi awaitar tasken

/void
vi har en async void method
vi glömde awaita resultatet av tasken
så den returnerar "did not crash"

/void2
vi har en async void metod
den awaitar resultatet av tasken
och fångar därför exceptionet.
...
vi returnerar dock 200 ok ... (för att vi catchar)
Kan vi skriva ut resultet till streamen?
...
(Nej, för asp.net kan inte awaita det vi håller på med, så första gången vi awaitar (släppar tillbaka vår tråd till asp.net) så kommer den stänga vår http stream)

Hela processen krashade då. Varför?
Jo för att vi har ett unhandled exception i en async void metod. Det finns ingen task scheduler eller parent tråd (t.ex. den i asp.net) som hanterar det åt oss.


/void2.1
Vi har en async void method
Vi  awaitar exceptionet
Men hela processen krashar, precis som tidigare.
För att vi har ett unhandled exception i en async void metod. Det finns ingen task scheduler eller parent tråd (t.ex. den i asp.net) som hanterar det åt oss.


/void3
Vi har en async task method
Som gör allt rätt
Den awaitar sin task. Catchar exceptionet och skriver felet till responsen.

/void4
Vi har en async task method
Som anropar en metod som är async void. 
Den väntar 100ms och krashar sen
…vi hinner returnera till streamen och sen krashar processen

/throw3/number
Vi har en async task method som anropar en annan async task method.
Men vi awaitar inte resultatet från tasken?
Vad händer med ett exception då?
Finns det något sett vi kan se det?



Oroa er inte om ni missade något. Jag ville visa "hur mycket som kan gå fel" och att man behöver ha koll på vad man gör.
Det finns ett repo med den här testkoden (jag lärde mig sjukt mycket genom att skriva koden) samt mitt manus med alla förklaringar.

Key Take Aways:

Använd aldrig async void - det kan krasha hela din process.
awaita tasks om du är intresserad av vad som faktiskt händer i anropet.
Om du inte awaitar en task och den kastar exception så hamnar dom i TaskSchedulern.UnobservedTaskException (NÄR GC vill göra sig av med tasken som kommer ligga kvar i minnet tills dess)
Använd cancellation Token om du vill kunna avbryta vad som händer (visa exempel med loop som awaitar)
Akta dig för att starta tasks från en lista av dynamisk längd.

Jag kommer ladda upp all kod + kommentarer i repo!



Obs:
T.ex. via:
var tasks = listOfManyItems.Select(async item => await ProcessItem()) // startar alla tasks samtidigt.

Du kan lätt ta slut på resurser (conenction pool, minne, CPU osv) om du gör för mycket jobb samtidigt.
    Använd ForEachAsync och SelectAsync istället
