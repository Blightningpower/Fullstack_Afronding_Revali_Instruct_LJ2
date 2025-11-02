<template>
  <div class="container">
    <div class="patient-table-card">
      <h2>Patiënten Overzicht</h2>

      <div class="patient-table-controls">
        <input v-model="q" type="text" placeholder="🔍 Zoek op naam..." />
        <select v-model="status">
          <option value="">Alle statussen</option>
          <option v-for="s in knownStatuses" :key="s" :value="s">{{ s }}</option>
        </select>
        <button @click="load">Zoek</button>
      </div>

      <div v-if="loading" class="loading-state">⏳ Laden...</div>
      <div v-else-if="error" class="error-state">⚠️ {{ error }}</div>
      <div v-else-if="items.length === 0" class="empty-state">
        Geen patiënten gevonden
      </div>
      <table v-else>
        <thead>
          <tr>
            <th>👤 Naam</th>
            <th>📅 Startdatum</th>
            <th>📊 Status</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="p in items" :key="p.id" @click="goDetail(p.id)">
            <td><strong>{{ p.firstName }} {{ p.lastName }}</strong></td>
            <td>{{ formatDate(p.startDate) }}</td>
            <td><span :class="['status-badge', getStatusClass(p.status)]">{{ p.status }}</span></td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getPatients } from '../api/patients'
import { useRouter } from 'vue-router'

const router = useRouter()
const items = ref([])
const loading = ref(false)
const error = ref('')
const q = ref('')
const status = ref('')
const knownStatuses = ['Intake gepland', 'Actief', 'Afgerond', 'On hold'] // toonlabels; backend levert enum-strings

const formatDate = (d) => d ? new Date(d).toLocaleDateString() : '-'

const getStatusClass = (status) => {
  const map = {
    'Actief': 'status-active',
    'Active': 'status-active',
    'Intake gepland': 'status-planned',
    'IntakePlanned': 'status-planned',
    'Afgerond': 'status-completed',
    'Completed': 'status-completed',
    'On hold': 'status-hold',
    'OnHold': 'status-hold'
  }
  return map[status] || 'status-default'
}

const load = async () => {
  loading.value = true
  error.value = ''
  try {
    items.value = await getPatients({ q: q.value, status: status.value || undefined })
  } catch (e) {
    error.value = e?.response?.data?.message || e.message || 'Laden mislukt'
  } finally {
    loading.value = false
  }
}

const goDetail = (id) => router.push(`/patients/${id}`)

onMounted(load)
</script>