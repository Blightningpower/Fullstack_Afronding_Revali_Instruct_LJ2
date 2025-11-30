# Revali Instruct – Frontend

Dit is de Vue 3 + Vite frontend voor **Revali Instruct**, een webapplicatie waarmee zorgprofessionals patiëntdossiers kunnen inzien en beheren.

## 🔧 Vereisten

- Node.js (versie 20+ aanbevolen)
- NPM
- .NET SDK (voor de backend `RevaliInstruct.Api`)
- Backend draait lokaal op `http://localhost:5000`  
  (dit wordt verondersteld door de Vite proxy in `vite.config.js`)

## 🌐 Tech stack

- [Vue 3](https://vuejs.org/) met `<script setup>` SFCs
- [Vue Router](https://router.vuejs.org/) voor routing
- [Vite](https://vitejs.dev/) als bundler/dev server
- [Axios](https://axios-http.com/) voor HTTP-calls naar de .NET backend API

## 📁 Projectstructuur (globaal)

- `src/App.vue` – Shell van de app + navigatie (login/logout, Patients-link)
- `src/main.js` – Entreepunt, mount Vue + router
- `src/router/index.js` – Routes (`/login`, `/patients`, `/patients/:id`) + auth-guard
- `src/views/Login.vue` – Inlogpagina
- `src/pages/PatientsList.vue` – Overzicht met zoek- en statusfilter
- `src/pages/PatientDetail.vue` – Detailweergave van één patiënt
- `src/api/patients.js` – API-calls voor patiënten
- `src/services/api.js` – Axios instance + token & server-instance handling
- `src/services/authService.js` – Login/logout helpers
- `src/App.css` – Globale styling (tabellen, detailcards, statusbadges)

## 🔐 Authenticatie

- JWT-token wordt na inloggen opgeslagen in `localStorage` onder sleutel `token`.
- Routes `/patients` en `/patients/:id` zijn beschermd via een **router-guard** (`meta.requiresAuth`).
- Voorbeeldlogin (zoals op de loginpagina getoond):  
  **gebruikersnaam:** `doctor`  
  **wachtwoord:** `doctor123`  

_Pas deze sectie aan als de echte seedgegevens anders zijn._

## 🌍 API & proxy

Alle API-calls gaan naar `/api/...`.  
Vite proxy in `vite.config.js` stuurt dit door naar de .NET backend:

```js
server: {
  port: 5173,
  proxy: {
    '/api': { target: 'http://localhost:5000', changeOrigin: true }
  }
}
