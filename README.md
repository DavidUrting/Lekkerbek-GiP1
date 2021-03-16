__Tip__: om deze .md file goed te kunnen lezen kan je de 'Markdown Editor' extension toevoegen aan jouw Visual Studio.
Dat kan je installeren via ```Extensions - Manage Extensions```.
In de ```Online``` tab zoek je dan op 'Markdown Editor'. 
Na de download zal je wel even Visual Studio moeten afsluiten waarna een VSIX installatie dialog verschijnt.
Na de VSIX installatie kan je Visual Studio weer starten en zal de extension beschikbaar zijn.

# Algemene info
De project home van deze solution is https://dev.azure.com/UCLL-2020-2021-GiP-TeamDocenten/Lekkerbek.  
Er wordt op Toledo of Github ook een publiek toegankelijke versie voorzien.  
De gepublishte versie van deze solution kan teruggevonden worden op https://delekkerbekweb.azurewebsites.net/.

# Startup.cs
Aan startup.cs werden nagenoeg geen wijzigingen gedaan.  

Er wordt wel een LekkerbekDbContext geregistreerd die zijn connection string uit appsettings.json haalt:
```
   services.AddDbContext<LekkerbekDbContext>(options =>
   options.UseSqlServer(Configuration.GetConnectionString("LekkerbekDb")));
```

En er gebeurt ook volgende aanroep:
```
     .AddRazorRuntimeCompilation();
```

Dat zorgt ervoor dat je at runtime wijzigingen kan doen aan .cshtml bestanden zonder dat je Visual Studio steeds moet stoppen en weer starten.
Om deze extension te gebruiken moest wel ```Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation``` NuGet package geïnstalleerd worden.  
Heel handig!

# Models\Entities
Er werden, vrij voor de hand liggende, entiteiten voorzien zoals __Klant__, __Gerecht__ en __Bestelling__.
Opdat een bestelling uit meerdere soorten gerechten zou kunnen bestaan werd een __BesteldGerecht__ entiteit voorzien.
Deze heeft een koppeling met de Bestelling waarvan het deel uitmaakt, en een koppeling met het Gerecht.
Verder werden op die entiteit  ```Aantal``` en ```Opmerkingen``` properties voorzien.

Er werd ook geopteerd voor een __Korting__ entiteit, zodat kortingen in de database kunnen geconfigureerd worden en bijgevolg niet hardcoded in de solution zitten.

Een __Bestelling__ bevindt zich steeds in één van de volgende statussen:
  * __Nieuw__: een bestelling in deze toestand is aangemaakt maar nog niet ingepland (= tijdslot is dus nog niet ingevuld).
  * __Besteld__: een bestelling in deze toestand is ingepland en al dan niet wijzigbaar/annuleerbaar afhankelijk van de duurtijd tot het tijdslot.
  * __Afgeleverd__: een bestelling in deze toestand moet betaald zijn en kan verder niet meer aangepast worden (tenzij het opmerkingen veld)
  * __Geannuleerd__: dit stelt een verwijderde bestelling voor. Het record bestaat dus nog steeds in de database maar wordt niet meer getoond in de views.
  
De __Bestelling__ entiteit werd verder ook voorzien van functies die bepalen welke velden mogen gewijzigd worden en welke statusovergangen toegelaten zijn.

## Automatisch inserten van data
In de **/Models/Entities/LekkerbekDbContext.cs** wordt er ook gebruik gemaakt van ```HastData()```. 
Daarmee kan je entiteiten laten toevoegen aan je database bij uitvoering van ```Update-Database```.
(Dat is handig voor 'referentie' data.)

# Controllers
De __KlantController__ en __GerechtController__ werden automatisch gegenereerd en lichtjes aangepast zodat klanten of gerechten niet kunnen verwijderd worden wanneer deze gekoppeld zijn aan een bestelling.

De belangrijkste controller is de __BestellingController__. Deze werd automatisch gegenereerd en vervolgens uitgebreid en aangepast.
Bemerk dat bestellingen kunnen aangemaakt en aangepast worden.
Het verwijderen van bestellingen werd niet voorzien, deze kunnen echter wel geannuleerd worden.
Onderliggend blijft het record dus bestaan in de database, maar de ```Index``` action zal die standaard niet meer tonen.

Verder is er ook nog de __BesteldGerechtController__. Deze laatste controller heeft geen ```Index``` action.
De reden is dat bestelde gerechten child entiteiten van een bestelling zijn. 
Er werd daarom geopteerd om de bestelde gerechten te tonen in een HTML ```<table>``` in de ```Detail``` en ```Edit``` views van de __BestellingController__.
NB: alle acties op de __BesteldGerechtController__ redirecten terug naar de de bestelling.

Uiteindelijk is er nog de __HomeController__, die zorgt voor de ontvangst van de gebruiker via de ```Index``` view.
In deze view werden 2 Bootstrap tiles voorzien voor de meest voorkomende acties in GiP1: aanmaken van bestellingen en het afrekenen/afleveren ervan.
Hoe minder interacties/clicks een gebruiker moet doen om een frequente handeling te doen hoe beter.

Eventuele (meer) gedetailleerde informatie kan je terugvinden als code commentaar in de controllers en views.

# Factuur
Opdat de factuur een ideale vorm zou hebben voor afdrukken werd een specifieke _PrintLayout.cshtml voorzien.
Deze layout kan gebruik worden voor Lekkerbek 'documenten'.
Standaard wordt ook een __Print__ knop getoond, die niet mee afgedrukt wordt op het document.
Hiervoor wordt gebruik gemaakt van de ```d-print-none``` Bootstrap CSS class. 

# JavaScript
In deze GiP1 referentiesolution werd zeer weinig custom JavaScript ontwikkeld, vandaar werd er ook nog geen Webpack pipeline opgezet.  
In GiP2 zal er wel wat meer JavaScript nodig zijn waardoor de Webpack pipeline wel interessant wordt!

## Ondersteuning van , in getalvelden
Om een komma te ondersteunen bij het ingeven van bedragen (vb. de prijs van een gerecht) werd dit scriptje:
```
<script>
    // https://stackoverflow.com/questions/48066208/mvc-jquery-validation-does-not-accept-comma-as-decimal-separator
    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)?(?:,\d+)?$/.test(value);
    }
</script>
```

toegevoegd aan __/Lekkerbek.Web/Views/Shared/\_ValidationScriptsPartial__.
Die file is een 'partial view' die standaard wordt toegevoegd in ```Edit``` views zodat er jQuery validations kunnen gebeuren.
Door bovenstaand scriptje toe te voegen zorgen we ervoor dat de jQuery validation niet valt over een komma.

# Assets
Het beeldmateriaal komt hoofdzakelijk van pexels.com.
  * __/Lekkerbek.Web/wwwroot/img/bestelling.jpg__: zie /Assets/README.md
  * __/Lekkerbek.Web/wwwroot/img/kassa.jpg__: zie /Assets/README.md
  * __/Lekkerbek.Web/wwwroot/img/DeLekkerbek.jpg__: aangeleverd door klant

