<template>
  <nav>
    <div class="nav-left">

      <svg width="240" height="60" viewBox="0 0 240 60" xmlns="http://www.w3.org/2000/svg">
        <defs>
          <linearGradient id="logoGrad" x1="0%" y1="0%" x2="100%" y2="100%">
            <stop offset="0%" style="stop-color:#3bb3ce;stop-opacity:1" />
            <stop offset="100%" style="stop-color:#4ee3c1;stop-opacity:1" />
          </linearGradient>
        </defs>
        <circle cx="30" cy="30" r="25" fill="none" stroke="url(#logoGrad)" stroke-width="4"
          stroke-dasharray="120, 40" />
        <path d="M22 42V18H32C36 18 38 20 38 23C38 26 36 28 32 28H22M32 28L40 42" fill="none" stroke="#3bb3ce"
          stroke-width="5" stroke-linecap="round" stroke-linejoin="round" />

        <text x="65" y="38" font-family="Segoe UI, Arial, sans-serif" font-weight="700" font-size="24" fill="#2a7ca3">
          Revali<tspan fill="#3bb3ce">Instruct</tspan>
        </text>
      </svg>

      <router-link v-if="showPatientsLink" to="/patients" class="nav-link">
        Patients
      </router-link>
    </div>

    <div class="auth-actions" v-if="!isLoginPage">
      <span v-if="isLogged && currentUsername" class="user-label">
        Ingelogd als <strong>{{ currentUsername }}</strong>
      </span>

      <router-link v-if="!isLogged" to="/login" class="btn-login">
        Login
      </router-link>

      <button v-else @click="handleLogout" class="btn-logout">
        Logout
      </button>
    </div>
  </nav>

  <main>
    <router-view />
  </main>
</template>

<script setup>
import { computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
// Importeer de reactieve state en logout functie
import { authState, logout as authLogout } from './services/AuthService';

const router = useRouter();
const route = useRoute();

// Controleer of we op de loginpagina zijn
const isLoginPage = computed(() => {
  const name = route.name ?? '';
  const path = route.path ?? '';
  return name === 'Login' || path === '/login';
});

const showPatientsLink = computed(() => !isLoginPage.value);

// Gebruik de reactieve waarden uit AuthService
const isLogged = computed(() => authState.isLogged);
const currentUsername = computed(() => authState.username);

function handleLogout() {
  authLogout(); // Gebruik de centrale logout logica
  router.push('/login');
}
</script>

<style>
/* CSS Reset en Layout */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

nav {
  padding: 1rem;
  background: #2c3e50;
  color: white;
  display: flex;
  align-items: center;
  gap: 2rem;
  justify-content: space-between;
}

.nav-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.nav-link {
  color: white;
  text-decoration: none;
  font-weight: 600;
  padding: 0.2rem 0.6rem;
  border-radius: 6px;
}

.nav-link:hover {
  background: rgba(255, 255, 255, 0.06);
}

.auth-actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.user-label {
  font-size: 0.9rem;
  opacity: 0.9;
}

.user-label strong {
  font-weight: 600;
}

.btn-login,
.btn-logout {
  background: rgba(255, 255, 255, 0.12);
  color: white;
  border: none;
  padding: 0.4rem 0.8rem;
  border-radius: 8px;
  cursor: pointer;
  text-decoration: none;
}

.btn-login:hover,
.btn-logout:hover {
  background: rgba(255, 255, 255, 0.18);
}
</style>