<template>
  <div class="page-container">
    <div class="card">
      <h1>Inloggen</h1>
      <form @submit.prevent="handleSubmit" class="form">
        <label class="field">
          <span>Gebruikersnaam</span>
          <input v-model="username" type="text" placeholder="bijv. doctor" required />
        </label>

        <label class="field">
          <span>Wachtwoord</span>
          <input v-model="password" type="password" placeholder="bijv. doctor123" required />
        </label>

        <button type="submit" class="btn-primary" :disabled="loading">
          {{ loading ? 'Bezig met inloggen...' : 'Inloggen' }}
        </button>

        <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { login } from '../services/AuthService';

const router = useRouter();
const username = ref('');
const password = ref('');
const errorMessage = ref('');
const loading = ref(false);

async function handleSubmit() {
  try {
    errorMessage.value = '';
    loading.value = true;
    await login(username.value.trim(), password.value.trim());
    router.push('/patients');
  } catch (err) {
    errorMessage.value = 'Inloggen mislukt. Controleer je gegevens.';
    console.error(err);
  } finally {
    loading.value = false;
  }
}

</script>

<style scoped>
.page-container {
  min-height: calc(100vh - 80px);
  display: flex;
  align-items: center;
  justify-content: center;
}

.card {
  background: #fff;
  padding: 2.5rem 3rem;
  border-radius: 18px;
  box-shadow: 0 18px 45px rgba(15, 50, 80, .12);
  max-width: 420px;
  width: 100%;
}

h1 {
  margin: 0 0 1.5rem;
  font-size: 1.8rem;
  text-align: center;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.field {
  display: flex;
  flex-direction: column;
  gap: .35rem;
  font-size: .9rem;
}

input {
  padding: .6rem .85rem;
  border-radius: 10px;
  border: 1px solid #cfd9e6;
  outline: none;
}

input:focus {
  border-color: #2ab3ff;
  box-shadow: 0 0 0 1px rgba(42, 179, 255, .25);
}

.btn-primary {
  margin-top: .75rem;
  padding: .7rem 1rem;
  border-radius: 999px;
  border: none;
  cursor: pointer;
  font-weight: 600;
  background: linear-gradient(90deg, #21c2ff, #1fd8b1);
  color: white;
}

.error {
  margin-top: .75rem;
  font-size: .85rem;
  color: #d9534f;
  text-align: center;
}
</style>
