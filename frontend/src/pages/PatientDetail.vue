<template>
  <div class="page">
    <button @click="$router.back()">Terug</button>
    <div v-if="loading">Laden...</div>
    <div v-else-if="!patient">Patiënt niet gevonden.</div>
    <div v-else>
      <h2>{{ patient.firstName }} {{ patient.lastName }}</h2>
      <p><strong>Startdatum:</strong> {{ patient.startDate }}</p>
      <p><strong>Status:</strong> {{ patient.status }}</p>
      <p><strong>Notes:</strong> {{ patient.notes || '-' }}</p>
      <!-- voeg meer velden toe als je die hebt -->
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { fetchPatient } from '../api/patients'
import { useRoute } from 'vue-router'

const route = useRoute()
const id = route.params.id
const loading = ref(false)
const patient = ref(null)

async function load() {
  loading.value = true
  try {
    patient.value = await fetchPatient(id)
  } catch (err) {
    console.error(err)
    alert('Kon patiënt niet laden: ' + err.message)
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<style>
.page { padding: 20px; }
</style>