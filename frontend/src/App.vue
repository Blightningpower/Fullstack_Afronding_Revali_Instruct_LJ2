<template>
  <div id="app">
    <nav>
      <div class="nav-left">
        <h1>Revali Instruct</h1>

        <!-- Patients link niet tonen op de login pagina -->
        <router-link v-if="showPatientsLink" to="/patients" class="nav-link">
          Patients
        </router-link>
      </div>

      <!-- login / logout + username (niet tonen op loginpagina) -->
      <div class="auth-actions" v-if="!isLoginPage">
        <span v-if="isLogged && currentUsername" class="user-label">
          Ingelogd als <strong>{{ currentUsername }}</strong>
        </span>

        <router-link
          v-if="!isLogged"
          to="/login"
          class="btn-login"
        >
          Login
        </router-link>

        <button
          v-else
          @click="handleLogout"
          class="btn-logout"
        >
          Logout
        </button>
      </div>
    </nav>

    <main>
      <router-view />
    </main>
  </div>
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

/* titel + links */
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

/* auth area rechtsboven */
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

/* knoppen */
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