<template>
  <div id="app">
    <nav>
      <div class="nav-left">
        <h1>Revali Instruct</h1>
        <!-- hide Patients link on the login page -->
        <router-link v-if="showPatientsLink" to="/patients" class="nav-link">
          Patients
        </router-link>
      </div>

      <!-- login / logout button -->
      <div class="auth-actions">
        <router-link v-if="!isLogged" to="/login" class="btn-login">Login</router-link>
        <button v-else @click="handleLogout" class="btn-logout">Logout</button>
      </div>
    </nav>

    <!-- laat layout verder over aan je globale CSS (App.css) -->
    <main>
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';

const router = useRouter();
const route = useRoute();

// show Patients link behalve op de login pagina
const showPatientsLink = computed(() => {
  const name = route.name ?? '';
  const path = route.path ?? '';
  return !(name === 'Login' || path === '/login');
});

// lees token direct uit localStorage
const isLogged = computed(() => {
  try {
    return !!localStorage.getItem('token');
  } catch {
    return false;
  }
});

function handleLogout() {
  try {
    localStorage.removeItem('token');
  } catch { }
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

/* auth buttons */
.auth-actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
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