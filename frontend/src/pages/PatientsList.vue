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
  
.table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 12px;
  font-size: 14px;
}

.table thead th {
  text-align: left;
  padding: 12px 16px;
  border-bottom: 2px solid var(--border);
  background: #fff;
  font-weight: 700;
}

.table tbody td {
  padding: 12px 16px;
  border-bottom: 1px solid var(--border);
  vertical-align: middle;
}

.empty {
  padding: 28px;
  text-align: center;
  color: var(--muted);
}

.table tbody tr:hover {
  background: #fbfbfd;
}

.table a {
  color: var(--accent);
  text-decoration: none;
  font-weight: 600;
}

.pagination {
  display: flex;
  gap: 10px;
  align-items: center;
  margin-top: 14px;
  color: var(--muted);
}

.page-button {
  padding: 6px 10px;
  border-radius: 6px;
  border: 1px solid var(--border);
  background: #fff;
  cursor: pointer;
}

.page-button[disabled] {
  opacity: 0.5;
  cursor: default;
}

.patient-table-card {
  background: #fff;
  border-radius: 16px;
  box-shadow: 0 2px 20px rgba(59, 179, 206, 0.10);
  padding: 40px 32px;
  margin: 0 auto;
  border: none;
  transition: box-shadow 0.2s;
}

.patient-table-card:hover {
  box-shadow: 0 8px 32px rgba(59, 179, 206, 0.15);
}

.patient-table-controls {
  display: flex;
  gap: 12px;
  margin-bottom: 22px;
  align-items: center;
  flex-wrap: wrap;
}

.patient-table-controls input[type="text"],
.patient-table-controls select {
  padding: 12px 16px;
  border: 2px solid #d2f0f7;
  border-radius: 10px;
  font-size: 1rem;
  background: #fafdff;
  outline: none;
  transition: border 0.2s, box-shadow 0.2s;
  color: #234;
}

.patient-table-controls input[type="text"]:focus,
.patient-table-controls select:focus {
  border: 2px solid #3bb3ce;
  box-shadow: 0 0 0 3px rgba(59, 179, 206, 0.10);
}

.patient-table-controls button {
  background: linear-gradient(90deg, #3bb3ce 70%, #4ee3c1 100%);
  color: #fff;
  border: none;
  border-radius: 10px;
  padding: 12px 28px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.18s, box-shadow 0.18s, transform 0.15s;
  box-shadow: 0 2px 8px rgba(59, 179, 206, 0.15);
}

.patient-table-controls button:hover {
  background: linear-gradient(90deg, #2a7ca3 70%, #3bb3ce 100%);
  box-shadow: 0 4px 16px rgba(59, 179, 206, 0.20);
  transform: translateY(-2px);
}

table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  background: #fff;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: none;
  margin-top: 24px;
  margin-bottom: 0;
  font-size: 1.09rem;
  border: none;
}

th,
td {
  padding: 20px 24px;
  text-align: left;
  vertical-align: middle;
  border: none;
}

th {
  background: #eaf6fb;
  font-weight: 700;
  font-size: 1.12rem;
  border-bottom: none;
  letter-spacing: 0.5px;
  color: #2a7ca3;
}

tr {
  transition: background 0.18s;
  border: none;
}

tr:nth-child(even) {
  background: #f9fcfe;
}

tr:hover {
  background: #d2f0f7;
  transition: background 0.15s;
  cursor: pointer;
}

input[type="text"],
select {
  padding: 10px 14px;
  border: 1.5px solid #bfe7ef;
  border-radius: 8px;
  margin-right: 10px;
  font-size: 1rem;
  background: #fafdff;
  margin-bottom: 0;
  box-shadow: 0 1px 3px rgba(59, 179, 206, 0.04);
  transition: border 0.2s, box-shadow 0.2s;
  color: #234;
}

input[type="text"]:focus,
select:focus {
  border: 2px solid #3bb3ce;
  box-shadow: 0 2px 8px rgba(59, 179, 206, 0.10);
}

button,
input[type="submit"] {
  background: linear-gradient(90deg, #3bb3ce 70%, #4ee3c1 100%);
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 10px 22px;
  font-size: 1rem;
  font-weight: 600;
  letter-spacing: 0.5px;
  margin-top: 0;
  margin-bottom: 0;
  box-shadow: 0 1px 3px rgba(59, 179, 206, 0.04);
  transition: background 0.18s, box-shadow 0.18s;
}

button:active,
input[type="submit"]:active {
  background: #2a7ca3;
}

.empty-state {
  text-align: center;
  color: #7ec9d6;
  font-size: 1.15rem;
  margin: 36px 0;
  opacity: 0.85;
}

.status-badge {
  display: inline-block;
  padding: 6px 14px;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: 600;
  letter-spacing: 0.3px;
}

.status-active {
  background: #d4f4dd;
  color: #1e7e34;
}

.status-planned {
  background: #fff3cd;
  color: #856404;
}

.status-completed {
  background: #d1ecf1;
  color: #0c5460;
}

.status-hold {
  background: #f8d7da;
  color: #721c24;
}

.status-default {
  background: #e2e3e5;
  color: #383d41;
}

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