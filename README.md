
# 🟪 Fullstack – RevaliInstruct

RevaliInstruct is het revalidatie-onderdeel binnen het project **“Van Auw naar Beter!”**.  
De applicatie biedt een web-omgeving voor zorgprofessionals om patiëntgegevens en revalidatietrajecten te beheren.

---

## 🛠️ Tech stack

### Backend

- .NET 10 (Web API, C#)
- Entity Framework Core
- SQL Server (via Docker-container)
- RESTful endpoints onder `/api/...`

### Frontend

- Vue 3 (met `<script setup>`)
- Vue Router
- Vite
- Axios voor HTTP-calls naar de backend

---

## 🚀 Project lokaal draaien

### 1. Repository clonen

```bash
git clone git@github.com:blightningpower/Fullstack_Afronding_Revali_Instruct_LJ2.git
cd Fullstack_Afronding_Revali_Instruct_LJ2
```

---

## ✅ Benodigde software

- [.NET SDK 10](https://dotnet.microsoft.com/)
- [Node.js + npm](https://nodejs.org/)  
- [Docker Desktop](https://www.docker.com/) (voor de SQL Server container en optioneel de API)

---

## 🔹 API starten – Alles lokaal via `npm run dev:full` (ontwikkelmodus)

Dit is de modus die ik zelf gebruikt heb tijdens development.

1. Zorg dat Docker **de SQL Server container draait**, maar **niet** de API:

   ```bash
   docker-compose up revali_mssql db-init
   ```

   - (of start alleen de database-container via Docker Desktop UI)

2. Start vervolgens frontend + backend lokaal:

   ```bash
   cd frontend
   npm install
   npm run dev:full
   ```

   Dit doet:
   - `dotnet watch run --project ../backend/RevaliInstruct.Api`
   - `vite`

3. Toegang:
   - Frontend: [http://localhost:5173]
   - Backend API: [http://localhost:5000]

> ⚠️ In deze modus **niet** ook nog `docker-compose up` voor de API draaien.

---

## 🧪 Tests draaien (optioneel)

_Backend tests:_

```bash
cd backend
dotnet test
```

_Frontend tests (indien aanwezig):_

```bash
cd frontend
npm test
```

---
