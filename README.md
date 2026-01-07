# 🟪 Fullstack – RevaliInstruct

RevaliInstruct is het revalidatie-onderdeel binnen het overkoepelende project **"Van Auw naar Beter!"**. De applicatie biedt een integrale web-omgeving voor zorgprofessionals om patiëntgegevens, medische intakes en revalidatietrajecten veilig te beheren.

---

## 🛠️ Tech Stack

### Backend

* **Runtime**: .NET 10 (Web API, C#)
* **ORM**: Entity Framework Core
* **Database**: SQL Server (gedraaid via Docker-container)
* **API**: RESTful endpoints onder `/api/...`

### Frontend

* **Framework**: Vue 3 (Composition API met `<script setup>`)
* **Tooling**: Vite & Vue Router
* **Communicatie**: Axios voor HTTP-calls naar de backend
* **Visualisatie**: Chart.js voor trend-monitoring

---

## 🚀 Project Lokaal Draaien

### 1. Repository clonen

```bash
git clone git@github.com:blightningpower/Fullstack_Afronding_Revali_Instruct_LJ2.git
cd Fullstack_Afronding_Revali_Instruct_LJ2
```

### 2. Benodigde software

* [.NET SDK 10](https://dotnet.microsoft.com/)
* [Node.js + npm](https://nodejs.org/)
* [Docker Desktop](https://www.docker.com/) (voor de SQL Server container)

---

## 🔹 Opstartinstructies (Ontwikkelmodus)

Dit project maakt gebruik van een gecombineerd opstartproces om zowel de frontend als de backend gelijktijdig te draaien.

1. **Start de SQL Server container:**

   Zorg dat Docker Desktop actief is en start alleen de database-onderdelen:

   ```bash
   docker-compose up revali_mssql db-init
   ```

2. **Start de applicatie (Full Stack):**

   Navigeer naar de frontend map en gebruik het gecombineerde script:

   ```bash
   cd frontend
   npm install
   npm run dev:full
   ```

   **Wat dit commando doet:**

   * Start de backend API via `dotnet watch run`.
   * Start de frontend via de Vite dev-server op poort 5173.

3. **Toegang:**

   * **Frontend**: <http://localhost:5173>
   * **Backend API**: <http://localhost:5000>

> [!IMPORTANT]
> Draai in deze modus **niet** handmatig `docker-compose up` voor de API-container om poort-conflicten te voorkomen. Gebruik Docker alleen voor de database.

---

## 🔐 Authenticatie & Testaccounts

De interface past zich automatisch aan op basis van de ingelogde rol. De volgende seed-gegevens zijn beschikbaar:

| Gebruikersnaam | Wachtwoord    | Rol              |
| ---            | ---           | ---              |
| **ra_smit**    | `password123` | Revalidatiearts  |
| **ra_groen**   | `password123` | Revalidatiearts  |
| **ra_visser**  | `password123` | Revalidatiearts  |
| **admin**      | `password123` | Systeembeheerder |

---

## 🧪 Tests Draaien (Optioneel)

**Backend tests:**

```bash
cd backend
dotnet test
```

**Frontend tests:**

```bash
cd frontend
npm test
```
