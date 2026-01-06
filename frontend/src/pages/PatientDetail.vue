<template>
  <div class="container">
    <div class="patient-detail-card" v-if="!loading">
      <button @click="$router.push('/patients')" class="back-button">← Terug naar overzicht</button>

      <div v-if="error" class="error-state">⚠️ {{ error }}</div>

      <div v-else-if="patient">
        <PatientHeader :patient="patient" />
        <PatientInfoGrid :patient="patient" />

        <div class="patient-dossier">
          <ExerciseSection :patientId="patient.id || patient.Id" :rawExercises="patient.exercises || patient.Exercises"
            @refresh="load" />

          <PainAndActivity :painEntries="patient.painEntries || patient.PainEntries"
            :activityLogs="patient.activityLogs || patient.ActivityLogs" />

          <MedicationAndAccessories :medications="patient.medications || patient.Medications"
            :accessories="patient.accessoryAdvices || patient.AccessoryAdvices" />

          <AppointmentsSection :patientId="patient.id || patient.Id"
            :appointments="patient.appointments || patient.Appointments" @refresh="load" />

          <IntakeSection :patientId="patient.id || patient.Id"
            :rawIntakes="patient.intakeRecords || patient.IntakeRecords"
            :rawNotes="patient.patientNotes || patient.PatientNotes" @refresh="load" />
        </div>
      </div>
    </div>
    <div v-else class="loading-state">⏳ Laden...</div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { getPatientDossier } from '../api/patients';
// Imports van componenten...
import PatientHeader from '../components/patient/PatientHeader.vue';
import PatientInfoGrid from '../components/patient/PatientInfoGrid.vue';
import ExerciseSection from '../components/patient/ExerciseSection.vue';
import PainAndActivity from '../components/patient/PainAndActivity.vue';
import MedicationAndAccessories from '../components/patient/MedicationAndAccessories.vue';
import AppointmentsSection from '../components/patient/AppointmentsSection.vue';
import IntakeSection from '../components/patient/IntakeSection.vue';

const route = useRoute();
const patient = ref(null);
const loading = ref(false);
const error = ref('');

const load = async () => {
  loading.value = true;
  error.value = '';
  try {
    const dossier = await getPatientDossier(route.params.id);
    if (!dossier) {
      error.value = 'Geen patiënt gevonden.';
    } else {
      patient.value = dossier;
    }
  } catch (e) {
    error.value = 'Fout bij laden van dossier.';
    console.error(e);
  } finally {
    loading.value = false;
  }
};

onMounted(load);
</script>

<style scoped>
/* ==========================================================================
    PATIËNT DETAILS
   ========================================================================== */
.patient-detail-card {
  background: #fff;
  border-radius: 16px;
  box-shadow: 0 2px 20px rgba(59, 179, 206, 0.10);
  padding: 40px 36px;
  margin: 0 auto;
  border: none;
}

.patient-header {
  display: flex;
  align-items: center;
  gap: 24px;
  margin-bottom: 36px;
  padding-bottom: 28px;
  border-bottom: 2px solid #eaf6fb;
}

.patient-title {
  flex: 1;
}

.patient-title h2 {
  margin: 0 0 8px 0;
}

.patient-info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 24px;
  margin-top: 28px;
}

.info-item {
  background: #fafdff;
  padding: 24px;
  border-radius: 12px;
  border: 2px solid #eaf6fb;
  display: flex;
  flex-direction: column;
  gap: 10px;
  transition: border 0.2s, box-shadow 0.2s;
}

.info-item:hover {
  border-color: #d2f0f7;
  box-shadow: 0 2px 8px rgba(59, 179, 206, 0.08);
}

.info-label {
  font-size: 0.9rem;
  color: #2a7ca3;
  font-weight: 600;
}

.info-value {
  font-size: 1.1rem;
  color: #234;
  font-weight: 500;
}

.back-button {
  background: #eaf6fb;
  color: #2a7ca3;
  border: 2px solid #d2f0f7;
  padding: 12px 22px;
  border-radius: 10px;
  font-weight: 600;
  cursor: pointer;
  margin-bottom: 28px;
  transition: all 0.2s;
}

.back-button:hover {
  background: #d2f0f7;
  border-color: #3bb3ce;
  transform: translateX(-4px);
}

.loading-state,
.error-state {
  text-align: center;
  padding: 32px;
  font-size: 1.1rem;
}

.loading-state {
  color: #2a7ca3;
}

.error-state {
  color: #c53030;
  background: #fff5f5;
  border-radius: 8px;
  border: 1.5px solid #feb2b2;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(16px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.container,
.patient-table-card,
table,
.empty-state {
  animation: fadeInUp 0.5s;
}

/* ==========================================================================
    RESPONSIVE DESIGN
   ========================================================================== */
@media (max-width:720px) {
  .table thead {
    display: none;
  }

  .table,
  .table tbody,
  .table tr,
  .table td {
    display: block;
    width: 100%;
  }

  .table tr {
    margin-bottom: 12px;
    border: 1px solid var(--border);
    border-radius: 8px;
    padding: 8px;
  }

  .table td {
    padding: 8px 12px;
    border: none;
  }

  .table td::before {
    content: attr(data-label);
    display: block;
    font-weight: 600;
    color: var(--muted);
    margin-bottom: 6px;
  }
}

@media (max-width: 700px) {

  .container,
  main {
    padding: 24px 12px;
  }

  .patient-table-card,
  .patient-detail-card {
    padding: 24px 16px;
  }

  th,
  td {
    padding: 12px 8px;
  }

  .patient-table-controls {
    flex-direction: column;
    gap: 8px;
  }

  h2 {
    font-size: 1.2rem;
  }

  .patient-header {
    flex-direction: column;
    text-align: center;
  }

  .patient-info-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 900px) {
  .patient-dossier {
    grid-template-columns: 1fr;
  }
}
</style>