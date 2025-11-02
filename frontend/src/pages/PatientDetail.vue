<template>
  <div class="container">
    <div class="patient-detail-card">
      <button @click="$router.push('/patients')" class="back-button">← Terug naar overzicht</button>
      
      <div v-if="loading" class="loading-state">⏳ Laden...</div>
      <div v-else-if="error" class="error-state">⚠️ {{ error }}</div>
      <div v-else-if="patient">
        <div class="patient-header">
          <div class="patient-avatar">{{ getInitials(patient.firstName, patient.lastName) }}</div>
          <div class="patient-title">
            <h2>{{ patient.firstName }} {{ patient.lastName }}</h2>
            <span :class="['status-badge', getStatusClass(patient.status)]">{{ patient.status }}</span>
          </div>
        </div>

        <div class="patient-info-grid">
          <div class="info-item">
            <span class="info-label">🎂 Geboortedatum</span>
            <span class="info-value">
              {{ patient?.dateOfBirth ? formatDateLong(patient.dateOfBirth) : 'Niet ingevuld' }}
            </span>
          </div>
          <div class="info-item">
            <span class="info-label">📅 Startdatum behandeling</span>
            <span class="info-value">{{ patient.startDate ? formatDateLong(patient.startDate) : 'Niet ingevuld' }}</span>
          </div>
          <div class="info-item">
            <span class="info-label">📧 E-mail</span>
            <span class="info-value">
              <template v-if="patient?.email">
                <a :href="`mailto:${patient.email}`">{{ patient.email }}</a>
              </template>
              <template v-else>Niet ingevuld</template>
            </span>
          </div>
          <div class="info-item">
            <span class="info-label">📞 Telefoon</span>
            <span class="info-value">
              <template v-if="patient?.phone">
                <a :href="`tel:${patient.phone}`">{{ patient.phone }}</a>
              </template>
              <template v-else>Niet ingevuld</template>
            </span>
          </div>
          <div class="info-item">
            <span class="info-label">🩺 Verwijzend arts</span>
            <span class="info-value">{{ patient?.referringDoctor || 'Niet ingevuld' }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { getPatientDossier } from '../api/patients'

const route = useRoute()
const patient = ref(null)
const loading = ref(false)
const error = ref('')

const formatDateLong = (d) => {
  if (!d) return '-'
  try {
    return new Date(d).toLocaleDateString('nl-NL', { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    })
  } catch (e) {
    return '-'
  }
}

const getInitials = (first, last) => {
  return `${first?.[0] || ''}${last?.[0] || ''}`.toUpperCase()
}

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
    const dossier = await getPatientDossier(route.params.id)
    patient.value = dossier.patient // bevat: firstName, lastName, status, dateOfBirth, startDate, email, phone, referringDoctor
  } catch (e) {
    error.value = e?.response?.data?.message || e.message || 'Laden mislukt'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>