<template>
  <div class="container">
    <div class="patient-table-card">
      <h2>Patiënten Overzicht</h2>

      <form class="patient-table-controls" @submit.prevent="load" novalidate>
        <label for="q" class="visually-hidden">Zoek op naam</label>
        <input id="q" name="q" v-model="q" type="text" placeholder="🔍 Zoek op naam..." autocomplete="name" />

        <label for="status" class="visually-hidden">Status filter</label>
        <select id="status" name="status" v-model="status" autocomplete="off">
          <option value="">Alle statussen</option>
          <option v-for="opt in statusOptions" :key="opt.value" :value="opt.value">
            {{ opt.label }}
          </option>
        </select>

        <button type="submit">Zoek</button>
      </form>

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
            <td>
              <span :class="['status-badge', getStatusClass(p.status)]">
                {{ displayStatus(p.status) }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { getPatients } from '../api/patients'

const router = useRouter()
const items = ref([])
const loading = ref(false)
const error = ref('')
const q = ref('')
const status = ref('')

const statusOptions = [
  { value: 'IntakePlanned', label: 'Intake gepland' },
  { value: 'Active', label: 'Actief' },
  { value: 'Completed', label: 'Afgerond' },
  { value: 'OnHold', label: 'On hold' },
]

const formatDate = (d) =>
  d ? new Date(d).toLocaleDateString('nl-NL') : '-'

const STATUS_LABELS = {
  '0': 'Intake gepland',
  '1': 'Actief',
  '2': 'Afgerond',
  '3': 'On hold',
  IntakePlanned: 'Intake gepland',
  Active: 'Actief',
  Completed: 'Afgerond',
  OnHold: 'On hold',
}

const STATUS_CLASSES = {
  '0': 'status-planned',
  '1': 'status-active',
  '2': 'status-completed',
  '3': 'status-hold',
  IntakePlanned: 'status-planned',
  Active: 'status-active',
  Completed: 'status-completed',
  OnHold: 'status-hold',
}

const displayStatus = (status) => STATUS_LABELS[String(status)] || '-'
const getStatusClass = (status) => STATUS_CLASSES[String(status)] || 'status-default'

let debounceTimeout = null;

watch(q, (newValue) => {
  clearTimeout(debounceTimeout);
  debounceTimeout = setTimeout(() => {
    load();
  }, 300);
});

watch(status, () => {
  load();
});

const load = async () => {
  loading.value = true
  error.value = ''
  try {
    items.value = await getPatients({
      // Match de key met de backend: 'searchTerm' i.p.v. 'q'
      searchTerm: q.value?.trim() || undefined,
      status: status.value || undefined,
    })
  } catch (e) {
    error.value = e?.response?.data?.message || e.message || 'Laden mislukt'
  } finally {
    loading.value = false
  }
}

const goDetail = (id) => router.push(`/patients/${id}`)
onMounted(load)
</script>

<style>
/* helper voor verborgen labels t.b.v. a11y */
.visually-hidden {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}
</style>