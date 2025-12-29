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
              {{ patient.birthDate ? formatDateLong(patient.birthDate) : 'Niet ingevuld' }}
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
              {{ patient?.phone || 'Niet ingevuld' }}
            </span>
          </div>
          <div class="info-item">
            <span class="info-label">🩺 Verwijzend arts</span>
            <span class="info-value">
              {{ patient?.referringDoctor?.firstName }} {{ patient?.referringDoctor?.lastName }}
            </span>
          </div>
        </div>

        <!-- Dossier-blokken -->
        <div class="patient-dossier">
          <!-- Toegewezen oefeningen -->
          <section class="dossier-section">
            <h3 class="dossier-section-title">Toegewezen oefeningen</h3>
            <ul class="dossier-list" v-if="exercises.length">
              <li v-for="ex in exercises" :key="ex.id" class="section-item exercise-entry">
                <div class="exercise-status-col">
                  <span class="pill" :class="ex.clientCheckedOff ? 'pill-ok' : 'pill-open'">
                    {{ ex.clientCheckedOff ? '✓ Afgevinkt' : 'Nog open' }}
                  </span>
                  <br>
                  <small class="checked-off-time" v-if="ex.checkedOffAtDisplay">Gedaan op: {{ ex.checkedOffAtDisplay
                  }}</small>
                </div>

                <div class="exercise-main">
                  <span class="dossier-item-title">{{ ex.title }}</span>
                  <p class="exercise-description" v-if="ex.description">{{ ex.description }}</p>
                  <div class="dossier-item-meta">
                    <strong>Planning:</strong> {{ ex.repetitions }}x per set — {{ ex.sets }} sets — <strong>{{
                      ex.frequencyText
                    }}</strong>
                    <div class="exercise-notes" v-if="ex.notes">
                      📝 <i>Opmerking arts: {{ ex.notes }}</i>
                    </div>
                  </div>
                </div>
              </li>
            </ul>
            <p v-else>Geen oefeningen toegewezen.</p>
          </section>

          <!-- Pijnindicaties -->
          <section class="dossier-section">
            <h3 class="dossier-section-title">Pijnindicaties overloop</h3>
            <div class="pain-list" v-if="painEntries.length">
              <div v-for="entry in painEntries" :key="entry.id" class="pain-card">
                <div class="pain-header">
                  <span class="pain-date">{{ formatDateLong(entry.timestamp) }}</span>
                  <div class="pain-score-pill" :class="getPainClass(entry.score)">
                    Score: {{ entry.score }}/10
                  </div>
                </div>
                <div class="pain-visual">
                  <div class="pain-bar-bg">
                    <div class="pain-bar-fill" :style="{
                      width: (entry.score * 10) + '%',
                      backgroundColor: getPainColor(entry.score)
                    }"></div>
                  </div>
                </div>
                <p class="pain-note" v-if="entry.note">💬 {{ entry.note }}</p>
              </div>
            </div>
            <p v-else class="empty-text">Geen pijnregistraties gevonden.</p>
          </section>

          <!-- Dagelijks activiteitenlogboek -->
          <section class="dossier-section">
            <h3 class="dossier-section-title">Dagelijks activiteitenlogboek</h3>
            <div class="timeline" v-if="activityLogs.length">
              <div v-for="log in activityLogs" :key="log.id" class="timeline-item">
                <div class="timeline-date">{{ formatDateTime(log.timestamp) }} uur</div>
                <div class="timeline-content">
                  <span class="timeline-dot"></span>
                  <p>{{ log.activity }}</p>
                </div>
              </div>
            </div>
            <p v-else class="empty-text">Nog geen activiteiten gelogd.</p>
          </section>

          <!-- Medicatie & accessoires -->
          <section class="dossier-section">
            <h3 class="dossier-section-title">💊 Voorgeschreven Medicatie</h3>
            <div class="info-list" v-if="medications.length">
              <div v-for="med in medications" :key="med.id" class="info-card">
                <div class="info-card-header">
                  <strong>{{ med.name }}</strong>
                  <span :class="['status-pill', med.status === 'Actief' ? 'pill-active' : 'pill-done']">{{ med.status
                  }}</span>
                </div>
                <p class="info-card-detail">{{ med.dosageInfo }}</p>
                <p class="info-card-meta">📅 {{ med.period }}</p>
              </div>
            </div>
            <p v-else class="empty-text">Geen medicatie geregistreerd.</p>
          </section>

          <section class="dossier-section">
            <h3 class="dossier-section-title">🦿 Hulpmiddelen & Accessoires</h3>
            <div class="info-list" v-if="accessoryAdvices.length">
              <div v-for="acc in accessoryAdvices" :key="acc.id" class="info-card">
                <div class="info-card-header">
                  <strong>{{ acc.name }}</strong>
                  <span :class="['status-pill', acc.status === 'Actief' ? 'pill-active' : 'pill-done']">{{ acc.status
                  }}</span>
                </div>
                <p class="info-card-detail">Geadviseerd op: {{ acc.adviceDate }}</p>
                <p class="info-card-meta">⏱ Gebruiksperiode: {{ acc.usage }}</p>
              </div>
            </div>
            <p v-else class="empty-text">Geen accessoires geadviseerd.</p>
          </section>

          <!-- Afspraken -->
          <section class="dossier-section">
            <h3 class="dossier-section-title">📅 Afsprakenoverzicht</h3>
            <div class="appointment-list" v-if="appointments.length">
              <div v-for="app in appointments" :key="app.id" class="appointment-row">
                <div class="app-date">
                  <strong>{{ app.dateTime }} uur</strong>
                </div>
                <div class="app-info">
                  <span class="app-type">{{ app.type }}</span>
                  <span class="app-duration">({{ app.duration }})</span>
                </div>
                <div class="app-status">
                  <span :class="['pill', app.status === 'Gepland' ? 'pill-planned' : 'pill-done']">
                    {{ app.status }}
                  </span>
                </div>
              </div>
            </div>
            <p v-else class="empty-text">Geen afspraken gepland of geweest.</p>
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

const exercises = computed(() => {
  const raw = normalizeArray(patient.value, 'exercises', 'Exercises')
  return raw.map(ex => ({
    id: ex.id ?? ex.Id,
    title: ex.exercise?.name ?? ex.Exercise?.Name ?? 'Onbekende oefening',
    description: ex.exercise?.description ?? ex.Exercise?.Description ?? '',
    repetitions: ex.repetitions ?? ex.Repetitions,
    sets: ex.sets ?? ex.Sets,
    notes: ex.notes ?? ex.Notes,
    frequencyText: `${ex.frequency ?? ex.Frequency} keer per dag`,
    clientCheckedOff: ex.clientCheckedOff ?? ex.ClientCheckedOff,
    checkedOffAtDisplay: ex.checkedOffAt || ex.CheckedOffAt
      ? new Date(ex.checkedOffAt || ex.CheckedOffAt).toLocaleString('nl-NL', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' })
      : null
  }))
})

const painEntries = computed(() => {
  const raw = normalizeArray(patient.value, 'painEntries', 'PainEntries');
  return raw.map(e => ({
    id: e.id ?? e.Id,
    timestamp: e.timestamp ?? e.Timestamp,
    score: e.score ?? e.Score,
    note: e.note ?? e.Note
  })).sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp)); // Nieuwste bovenaan
});

const getPainColor = (score) => {
  if (score >= 8) return '#e53e3e'; // Rood
  if (score >= 5) return '#ed8936'; // Oranje
  return '#48bb78'; // Groen
};

const getPainClass = (score) => {
  if (score >= 8) return 'pain-high';
  if (score >= 5) return 'pain-mid';
  return 'pain-low';
};

const activityLogs = computed(() => {
  const raw = normalizeArray(patient.value, 'activityLogs', 'ActivityLogs');
  return raw.map(l => ({
    id: l.id ?? l.Id,
    timestamp: l.timestamp ?? l.Timestamp,
    activity: l.activity ?? l.Activity
  })).sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp)); // Nieuwste bovenaan
});

const formatDateTime = (d) => {
  if (!d) return '-';
  return new Date(d).toLocaleString('nl-NL', {
    day: 'numeric',
    month: 'long',
    hour: '2-digit',
    minute: '2-digit'
  });
};

const medications = computed(() => {
  const raw = normalizeArray(patient.value, 'medications', 'Medications');
  return raw.map(m => ({
    id: m.id ?? m.Id,
    name: m.name ?? m.Name,
    dosageInfo: `${m.dosage ?? m.Dosage} — ${m.frequency ?? m.Frequency}`,
    period: `${formatDateLong(m.startDate ?? m.StartDate)} t/m ${m.endDate ? formatDateLong(m.endDate) : 'heden'}`,
    status: m.status ?? m.Status
  }));
});

const accessoryAdvices = computed(() => {
  const raw = normalizeArray(patient.value, 'accessoryAdvices', 'AccessoryAdvices');
  return raw.map(a => ({
    id: a.id ?? a.Id,
    name: a.name ?? a.Name,
    adviceDate: formatDateLong(a.adviceDate ?? a.AdviceDate),
    usage: a.expectedUsagePeriod ?? a.ExpectedUsagePeriod,
    status: a.status ?? a.Status
  }));
});

const appointments = computed(() => {
  const raw = normalizeArray(patient.value, 'appointments', 'Appointments');
  return raw.map(a => ({
    id: a.id ?? a.Id,
    dateTime: formatDateTime(a.appointmentDateTime ?? a.AppointmentDateTime),
    type: a.type ?? a.Type,
    duration: `${a.durationMinutes ?? a.DurationMinutes} min`,
    status: a.status ?? a.Status
  })).sort((a, b) => new Date(b.appointmentDateTime) - new Date(a.appointmentDateTime)); // Nieuwste bovenaan
});

const load = async () => {
  loading.value = true
  error.value = ''
  try {
    const dossier = await getPatientDossier(route.params.id)
    patient.value = dossier
  } catch (e) {
    error.value = e?.response?.data?.message || e.message || 'Laden mislukt'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<style scoped>
.exercise-entry {
  display: flex;
  justify-content: space-between;
  padding: 20px;
  border-bottom: 1px solid #edf2f7;
  flex-direction: column;
}

.exercise-notes {
  margin-top: 10px;
  padding: 8px;
  background: #f7fafc;
  border-left: 3px solid #cbd5e0;
}

.exercise-status-col {
  text-align: right;
  min-width: 150px;
  max-height: fit-content;
}

.checked-off-time {
  margin-top: 8px;
  color: #718096;
  font-size: 0.7em;
}

.pain-list {
  display: grid;
  gap: 15px;
  margin-top: 20px;
}

.pain-card {
  background: white;
  padding: 15px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.pain-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 10px;
}

.pain-score-pill {
  padding: 2px 10px;
  border-radius: 12px;
  font-weight: bold;
  font-size: 0.85em;
}

.pain-bar-bg {
  height: 12px;
  background: #edf2f7;
  border-radius: 6px;
  overflow: hidden;
}

.pain-bar-fill {
  height: 100%;
  border-radius: 6px;
  transition: width 0.6s ease-in-out;
}

.pain-note {
  font-size: 0.9em;
  color: #4a5568;
  margin-top: 10px;
  font-style: italic;
}

.pill {
  min-width: fit-content;
  max-height: fit-content;
  border-radius: 5px;
  padding: 2px;
  margin: 0 0.2vw;
}

.pill-ok {
  background: #c6f6d5;
  color: #22543d;
}

.pill-open {
  background: #feebc8;
  color: #744210;
}

.timeline {
  border-left: 2px solid #edf2f7;
  margin-left: 10px;
  padding-left: 20px;
}

.timeline-item {
  position: relative;
  margin-bottom: 20px;
}

.timeline-dot {
  position: absolute;
  left: -27px;
  top: 5px;
  width: 12px;
  height: 12px;
  background: #3182ce;
  border-radius: 50%;
  border: 2px solid #fff;
}

.timeline-date {
  font-size: 0.85em;
  font-weight: bold;
  color: #718096;
  margin-bottom: 4px;
}

.timeline-content p {
  background: #f8fafc;
  padding: 10px 15px;
  border-radius: 6px;
  color: #2d3748;
  margin: 0;
}

.info-list {
  display: grid;
  gap: 12px;
  margin-top: 10px;
}

.info-card {
  background: #f8fafc;
  padding: 12px;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
}

.info-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 5px;
}

.info-card-detail {
  font-size: 0.9em;
  color: #2d3748;
  font-weight: 500;
}

.info-card-meta {
  font-size: 0.8em;
  color: #718096;
  margin-top: 4px;
}

.status-pill {
  font-size: 0.75em;
  padding: 2px 8px;
  border-radius: 10px;
  font-weight: bold;
}

.pill-active {
  background: #ebf8ff;
  color: #2b6cb0;
}

.pill-done {
  background: #f7fafc;
  color: #a0aec0;
}

.appointment-list {
  border: 1px solid #edf2f7;
  border-radius: 8px;
  overflow: hidden;
  margin-top: 15px;
}

.appointment-row {
  display: grid;
  grid-template-columns: 2fr 2fr 1fr;
  padding: 12px 20px;
  border-bottom: 1px solid #edf2f7;
  align-items: center;
  background: white;
}

.appointment-row:last-child {
  border-bottom: none;
}

.app-date {
  color: #2d3748;
  margin: 0 3px 0 0 !important;
  max-width: 6vw;
}

.app-type {
  font-weight: 600;
  color: #3182ce;
  margin-right: 8px;
}

.app-duration {
  color: #718096;
  font-size: 0.9em;
}

.pill-planned {
  background: #ebf8ff;
  color: #2b6cb0;
  border: 1px solid #bee3f8;
}

.pill-done {
  background: #f7fafc;
  color: #718096;
  border: 1px solid #e2e8f0;
}
</style>