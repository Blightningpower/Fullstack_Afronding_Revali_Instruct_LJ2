<template>
  <div class="container">
    <div class="patient-detail-card">
      <button @click="$router.push('/patients')" class="back-button">
        ← Terug naar overzicht
      </button>

      <div v-if="loading" class="loading-state">⏳ Laden...</div>
      <div v-else-if="error" class="error-state">⚠️ {{ error }}</div>

      <div v-else-if="patient">
        <!-- Kop met naam en status -->
        <div class="patient-header">
          <div class="patient-avatar">
            {{ getInitials(patient.firstName, patient.lastName) }}
          </div>
          <div class="patient-title">
            <h2>{{ patient.firstName }} {{ patient.lastName }}</h2>
            <span :class="['status-badge', getStatusClass(patient.status)]">
              {{ displayStatus(patient.status) }}
            </span>
          </div>
        </div>

        <!-- Basisgegevens -->
        <div class="patient-info-grid">
          <div class="info-item">
            <span class="info-label">🎂 Geboortedatum</span>
            <span class="info-value">
              {{ patient?.dateOfBirth ? formatDateLong(patient.dateOfBirth) : 'Niet ingevuld' }}
            </span>
          </div>
          <div class="info-item">
            <span class="info-label">📅 Startdatum behandeling</span>
            <span class="info-value">
              {{ patient.startDate ? formatDateLong(patient.startDate) : 'Niet ingevuld' }}
            </span>
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
            <span class="info-value">
              {{ patient?.referringDoctor || 'Niet ingevuld' }}
            </span>
          </div>
        </div>

        <!-- Dossier-blokken -->
        <div class="patient-dossier">
          <!-- Toegewezen oefeningen -->
          <section class="dossier-section" v-if="exercises.length">
            <h3 class="dossier-section-title">Toegewezen oefeningen</h3>
            <ul class="dossier-list">
              <li v-for="ex in exercises" :key="ex.id" class="section-item">
                <span class="dossier-item-title">{{ ex.title }}</span>
                <span class="dossier-item-meta">
                  <span v-if="ex.repetitions"> – {{ ex.repetitions }}x</span>
                  <span v-if="ex.sets"> in {{ ex.sets }} sets</span>
                  <span v-if="ex.frequency"> ({{ ex.frequency }})</span>
                  <span
                    class="pill"
                    :class="ex.clientCheckedOff ? 'pill-ok' : 'pill-open'"
                  >
                    {{ ex.clientCheckedOff ? 'Afgevinkt door cliënt' : 'Nog open' }}
                  </span>
                </span>
              </li>
            </ul>
          </section>

          <!-- Pijnindicaties -->
          <section class="dossier-section" v-if="painEntries.length">
            <h3 class="dossier-section-title">Pijnindicaties</h3>
            <ul class="dossier-list">
              <li v-for="e in painEntries" :key="e.id" class="section-item">
                <span class="dossier-item-title">
                  {{ formatDateLong(e.recordedAtUtc) }} – score {{ e.score }}/10
                </span>
                <span class="dossier-item-meta">
                  <span v-if="e.location">({{ e.location }}) </span>
                  <span v-if="e.note">– {{ e.note }}</span>
                </span>
              </li>
            </ul>
          </section>

          <!-- Dagelijks activiteitenlogboek -->
          <section class="dossier-section" v-if="activityLogs.length">
            <h3 class="dossier-section-title">Dagelijks activiteitenlogboek</h3>
            <ul class="dossier-list">
              <li v-for="l in activityLogs" :key="l.id" class="section-item">
                <span class="dossier-item-title">
                  {{ formatDateLong(l.loggedAtUtc) }} – {{ l.activity }}
                </span>
                <span class="dossier-item-meta" v-if="l.details">
                  – {{ l.details }}
                </span>
              </li>
            </ul>
          </section>

          <!-- Medicatie & accessoires -->
          <section class="dossier-section" v-if="accessoryAdvices.length">
            <h3 class="dossier-section-title">Medicatie & accessoires</h3>
            <ul class="dossier-list">
              <li v-for="a in accessoryAdvices" :key="a.id" class="section-item">
                <span class="dossier-item-title">{{ a.name }}</span>
                <span class="dossier-item-meta">
                  – geadviseerd op {{ formatDateLong(a.adviceDateUtc) }}
                  (gebruik: {{ a.expectedUsagePeriod }}, status: {{ a.status }})
                </span>
              </li>
            </ul>
          </section>

          <!-- Afspraken -->
          <section class="dossier-section" v-if="appointments.length">
            <h3 class="dossier-section-title">Afspraken</h3>
            <ul class="dossier-list">
              <li v-for="a in appointments" :key="a.id" class="section-item">
                <span class="dossier-item-title">
                  {{ formatDateLong(a.startUtc) }} – {{ a.type }}
                </span>
                <span class="dossier-item-meta">
                  (status: {{ a.status }}, duur: {{ a.duration }})
                </span>
              </li>
            </ul>
          </section>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { getPatientDossier } from '../api/patients'

const route = useRoute()
const patient = ref(null)
const loading = ref(false)
const error = ref('')

// Helpers om casing-verschillen (camelCase / PascalCase) op te vangen
const normalizeArray = (obj, camelName, pascalName) => {
  if (!obj) return []
  return obj[camelName] || obj[pascalName] || []
}

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

const STATUS_LABELS = {
  '0': 'Intake gepland',
  '1': 'Actief',
  '2': 'Afgerond',
  '3': 'On hold',
  IntakePlanned: 'Intake gepland',
  Active: 'Actief',
  Completed: 'Afgerond',
  OnHold: 'On hold'
}

const STATUS_CLASSES = {
  '0': 'status-planned',
  '1': 'status-active',
  '2': 'status-completed',
  '3': 'status-hold',
  IntakePlanned: 'status-planned',
  Active: 'status-active',
  Completed: 'status-completed',
  OnHold: 'status-hold'
}

const displayStatus = (status) => {
  return STATUS_LABELS[String(status)] || '-'
}

const getStatusClass = (status) => {
  return STATUS_CLASSES[String(status)] || 'status-default'
}

/**
 * Genormaliseerde collections voor de template
 */
const exercises = computed(() => {
  const raw = normalizeArray(patient.value, 'exercises', 'Exercises')
  return raw.map(ex => ({
    id: ex.id ?? ex.Id,
    title: ex.exerciseTitle ?? ex.ExerciseTitle ?? 'Oefening',
    repetitions: ex.repetitions ?? ex.Repetitions ?? null,
    sets: ex.sets ?? ex.Sets ?? null,
    frequency: ex.frequency ?? ex.Frequency ?? null,
    duration: ex.duration ?? ex.Duration ?? null,
    clientCheckedOff: (ex.clientCheckedOff ?? ex.ClientCheckedOff) ?? false,
    startDateUtc: ex.startDateUtc ?? ex.StartDateUtc ?? null,
    endDateUtc: ex.endDateUtc ?? ex.EndDateUtc ?? null
  }))
})

const painEntries = computed(() => {
  const raw = normalizeArray(patient.value, 'painEntries', 'PainEntries')
  return raw.map(e => ({
    id: e.id ?? e.Id,
    recordedAtUtc: e.recordedAtUtc ?? e.RecordedAtUtc,
    score: e.score ?? e.Score,
    location: e.location ?? e.Location,
    note: e.note ?? e.Note
  }))
})

const activityLogs = computed(() => {
  const raw = normalizeArray(patient.value, 'activityLogs', 'ActivityLogs')
  return raw.map(l => ({
    id: l.id ?? l.Id,
    loggedAtUtc: l.loggedAtUtc ?? l.LoggedAtUtc,
    activity: l.activity ?? l.Activity,
    details: l.details ?? l.Details
  }))
})

const accessoryAdvices = computed(() => {
  const raw = normalizeArray(patient.value, 'accessoryAdvices', 'AccessoryAdvices')
  return raw.map(a => ({
    id: a.id ?? a.Id,
    name: a.name ?? a.Name,
    adviceDateUtc: a.adviceDateUtc ?? a.AdviceDateUtc,
    expectedUsagePeriod: a.expectedUsagePeriod ?? a.ExpectedUsagePeriod,
    status: a.status ?? a.Status
  }))
})

const appointments = computed(() => {
  const raw = normalizeArray(patient.value, 'appointments', 'Appointments')
  return raw.map(a => ({
    id: a.id ?? a.Id,
    startUtc: a.startUtc ?? a.StartUtc,
    duration: a.duration ?? a.Duration,
    type: a.type ?? a.Type,
    status: a.status ?? a.Status
  }))
})

const load = async () => {
  loading.value = true
  error.value = ''
  try {
    const dossier = await getPatientDossier(route.params.id)
    // API geeft een flat object terug, geen { patient: {...} }
    patient.value = dossier
  } catch (e) {
    error.value = e?.response?.data?.message || e.message || 'Laden mislukt'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>