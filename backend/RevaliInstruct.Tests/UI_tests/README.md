# 🧪 UI Automation Tests

Deze map bevat de End-to-End (E2E) tests voor RevaliInstruct, geschreven in C# met Selenium en NUnit. Deze tests simuleren echte gebruikersinteracties in de browser.

🚀 Voorbereiding: De applicatie starten
Voordat je de tests kunt uitvoeren, moet de volledige stack (Frontend + Backend) draaien. De tests verwachten dat de frontend bereikbaar is op <http://localhost:5173>.

Open een nieuwe terminal in de root van het project.

Navigeer naar de frontend map:

Bash
``
cd frontend
Start de applicatie (Backend API + Vite Frontend):
``
Bash
``
npm run dev:full
``
Dit commando start gelijktijdig de .NET API en de Vite development server.

🛠️ De UI Tests uitvoeren
Zodra de applicatie draait, kun je de tests starten vanuit de backend testomgeving.

Via de Terminal
Open een tweede terminalvenster.

Ga naar de map van het testproject:

Bash
``
cd backend/RevaliInstruct.Tests
``
Voer de tests uit:

Bash
``
dotnet test
``
Via VS Code (Aanbevolen)
Open de Test Explorer (het 'flesje' icoon in de zijbalk).

Zoek de tests onder de namespace RevaliInstruct.Tests.UI.

Klik op de Play-knop naast LoginTests of PatientOverviewTests om ze individueel te draaien.

⚠️ Belangrijke aandachtspunten
Browser: De tests maken gebruik van de ChromeDriver. Zorg dat je een recente versie van Google Chrome hebt geïnstalleerd.

Database: De tests verwachten dat de gebruiker ra_smit met wachtwoord password123 aanwezig is in de database.

Timing: De tests maken gebruik van WebDriverWait om te wachten op async acties van de API.
