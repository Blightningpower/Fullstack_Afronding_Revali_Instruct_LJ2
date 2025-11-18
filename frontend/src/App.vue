<template>
  <div id="app">
    <nav>
      <div class="nav-left">
        <h1>Revali Instruct</h1>
        <!-- hide Patients link on the login page -->
        <router-link v-if="showPatientsLink" to="/patients" class="nav-link">Patients</router-link>
      </div>

      <!-- login / logout button -->
      <div class="auth-actions">
        <router-link v-if="!isLogged" to="/login" class="btn-login">Login</router-link>
        <button v-else @click="handleLogout" class="btn-logout">Logout</button>
      </div>
    </nav>

    <main style="max-width:980px;margin:1rem auto;padding:0 1rem;">
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useRoute } from 'vue-router';

const router = useRouter();
const isLogged = ref(false);
const route = useRoute();

// show Patients link except when we're on the Login route
const showPatientsLink = computed(() => {
  // route.name may be undefined for some programmatic routes; fallback to path check
  const name = route.name ?? '';
  const path = route.path ?? '';
  return !(name === 'Login' || path === '/login');
});

function updateLoginState() {
  isLogged.value = !!localStorage.getItem('token');
}

onMounted(() => {
  updateLoginState();
  // update when other tabs set/remove token
  window.addEventListener('storage', (e) => {
    if (e.key === 'token') updateLoginState();
  });
});

function handleLogout() {
  try {
    localStorage.removeItem('token');
  } catch {}
  updateLoginState();
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

/* voeg deze class toe zodat titel + links naast elkaar blijven */
.nav-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

/* kleine styling voor de link zodat het bij de header past */
.nav-link {
  color: white;
  text-decoration: none;
  font-weight: 600;
  padding: 0.2rem 0.6rem;
  border-radius: 6px;
}

.nav-link:hover {
  background: rgba(255,255,255,0.06);
}

/* auth buttons */
.auth-actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.btn-login,
.btn-logout {
  background: rgba(255,255,255,0.12);
  color: white;
  border: none;
  padding: 0.4rem 0.8rem;
  border-radius: 8px;
  cursor: pointer;
  text-decoration: none;
}

.btn-login:hover,
.btn-logout:hover {
  background: rgba(255,255,255,0.18);
}

main {
  padding: 2rem;
}
</style>