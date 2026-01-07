# Revali Instruct – Frontend

Dit is de Vue 3 + Vite frontend voor **Revali Instruct**, een modern platform voor revalidatiebeheer. Hiermee kunnen zorgprofessionals patiëntdossiers inzien, trajecten monitoren en wordt elke medische actie veilig vastgelegd in een audit trail.

## 🔧 Vereisten

- **Node.js** (versie 20+ aanbevolen)
- **NPM** (beheert de dependencies)
- **.NET SDK 8.0** (om de bijbehorende backend `RevaliInstruct.Api` te draaien)
- De backend dient lokaal te draaien op `http://localhost:5000` (dit wordt afgehandeld via de Vite proxy).

## 🚀 Quick Start

1. **naar frontend map gaan:**

   ```bash
   cd frontend
   ```

2. **Dependencies installeren:**

   ```bash
   npm install
   ```

3. **backend en frontend Development server starten:**

   ```bash
   npm run dev:Full
   ```

4. **Applicatie openen:**
   De app is nu bereikbaar op `http://localhost:5173`.

## 🌐 Tech Stack

- **Framework:** [Vue 3](https://vuejs.org/) met de Composition API (`<script setup>`).
- **Routing:** [Vue Router](https://router.vuejs.org/) voor navigatie tussen overzichten en dossiers.
- **Build Tool:** [Vite](https://vitejs.dev/) voor een razendsnelle ontwikkelervaring.
- **HTTP Client:** [Axios](https://axios-http.com/) voor communicatie met de .NET API.
- **Visualisatie:** [Chart.js](https://www.chartjs.org/) voor het weergeven van pijnverloop-trends.

## 📁 Projectstructuur (Modulair)

De frontend is opgebouwd uit herbruikbare modules om een consistente gebruikerservaring te garanderen:

- **`src/pages/`**: Bevat de hoofdpagina's zoals `PatientsList.vue` (overzicht) en `PatientDetail.vue` (het dossier).
- **`src/components/patient/`**: De modulaire dossier-onderdelen:
  - `IntakeSection.vue`: Registratie en onveranderlijke weergave van de intake (US3).
  - `ExerciseSection.vue`: Toewijzen en monitoren van het oefenprogramma.
  - `AppointmentsSection.vue`: Planning van fysio-sessies en evaluaties.
  - `PainAndActivity.vue`: Trend-grafieken voor pijnscores en activiteitenlogs.
  - `NotesSection.vue`: Chronologische tijdlijn voor aanvullende medische notities.
- **`src/admin/`**: Onderdelen specifiek voor systeembeheerders, zoals de `AuditLogView.vue` (US10).
- **`src/services/`**: Bevat de `AuthService.js` voor login-afhandeling en routebescherming.

## 🔐 Authenticatie & Rollen

De applicatie maakt gebruik van beveiligde JWT-tokens die worden opgeslagen in de `localStorage`. De interface past zich automatisch aan op basis van de ingelogde rol:

| Rol                 | Toegang                                                  |
| :------------------ | :------------------------------------------------------- |
| **Revalidatiearts** | Patiëntenlijst, Medische Dossiers, Behandelplannen.      |
| **Admin**           | Systeem-brede Audit Trail (monitoren van alle mutaties). |

**Testaccounts (Seed data):**

- **Arts:** `ra_smit` / `password123`
- **Arts:** `ra_groen` / `password123`
- **Arts:** `ra_visser` / `password123`
- **Admin:** `admin` / `password123`

## 🎨 Design Standaard: De "Revali Card"

Voor een rustige en professionele uitstraling in de zorgomgeving gebruiken alle secties de `dossier-card` standaard:

- **Kaartstijl:** Witte achtergrond, afgeronde hoeken (16px) en een zachte Revali-blauwe rand (`#eaf6fb`).
- **Interactie:** Actieknoppen maken gebruik van de kenmerkende groen-blauwe gradiënt.
- **Status:** Duidelijke "pills" geven de status van afspraken, declaraties en oefeningen aan.

## 🌍 API & Proxy configuratie

Alle API-calls vanuit de frontend gaan naar `/api/...`. De Vite configuratie stuurt deze verzoeken automatisch door naar de backend server:

```js
// vite.config.js snippet
server: {
  proxy: {
    '/api': {
      target: 'http://localhost:5000',
      changeOrigin: true
    }
  }
}
```
