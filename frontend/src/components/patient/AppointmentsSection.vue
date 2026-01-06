<template>
  <section class="dossier-section appointment-section">
    <div class="section-header">
      <h3 class="dossier-section-title">📅 Afspraken & Sessies</h3>
      <button @click="toggleForm" :class="['btn-toggle', { 'active': showForm }]">
        {{ showForm ? 'Annuleren' : '+ Nieuwe Afspraak' }}
      </button>
    </div>

    <transition name="slide-fade">
      <div v-if="showForm" class="appointment-form card shadow-sm">
        <div class="form-grid">
          <div class="form-group" :class="{ 'has-error': submitted && !newApp.dateTime }">
            <label>Datum & Tijd <span class="required">*</span></label>
            <input 
              type="datetime-local" 
              v-model="newApp.dateTime" 
              :min="minDateTime"
              class="styled-input"
            />
          </div>
          <div class="form-group">
            <label>Type Sessie</label>
            <select v-model="newApp.type" class="styled-input">
              <option>Controle</option>
              <option>Fysio sessie</option>
              <option>Evaluatie</option>
              <option>Intake</option>
            </select>
          </div>
          <div class="form-group">
            <label>Duur (minuten)</label>
            <input 
              type="number" 
              v-model="newApp.duration" 
              min="5" 
              step="5"
              class="styled-input"
            />
          </div>
        </div>
        
        <div class="form-actions">
          <button 
            @click="saveAppointment" 
            class="btn-save-appointment" 
            :class="{ 'btn-disabled': submitted && !isFormValid }"
          >
            Afspraak Inplannen
          </button>
          <p v-if="submitted && !isFormValid" class="validation-hint">
            ⚠️ Kies een geldig moment in de toekomst.
          </p>
        </div>
      </div>
    </transition>

    <div class="appointment-list">
      <div v-for="app in sortedAppointments" :key="app.id || app.Id" class="appointment-row"
        :class="{ 'status-cancelled': (app.status || app.Status) === 'Geannuleerd' }">

        <div class="app-info">
          <div class="app-date">{{ formatDateTime(app.dateTime || app.DateTime) }}</div>
          <div class="app-meta">
            <span class="app-type">{{ app.type || app.Type }}</span>
            <span class="app-duration">({{ app.duration || app.Duration }} min)</span>
          </div>
        </div>
        
        <div class="app-status-box">
          <span class="status-pill"
            :class="(app.status || app.Status) === 'Geannuleerd' ? 'pill-cancelled' : 'pill-planned'">
            {{ app.status || app.Status }}
          </span>
        </div>

        <div class="app-actions">
          <button v-if="(app.status || app.Status) !== 'Geannuleerd'" 
                  @click="cancelApp(app.id || app.Id)"
                  class="btn-cancel-small">
            Annuleren
          </button>
        </div>
      </div>
      <div v-if="!sortedAppointments.length" class="empty-state">
        📭 Geen afspraken gevonden.
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, computed } from 'vue';
import api from '../../api/axios';

const props = defineProps(['patientId', 'appointments']);
const emit = defineEmits(['refresh']);

const showForm = ref(false);
const submitted = ref(false); // Houdt bij of er op 'opslaan' is geklikt
const newApp = ref({ dateTime: '', type: 'Fysio sessie', duration: 30 });

const minDateTime = computed(() => {
  const now = new Date();
  return now.toISOString().slice(0, 16);
});

const isFormValid = computed(() => {
  if (!newApp.value.dateTime) return false;
  // Blokkeer datums in het verleden (US6 AC 1)
  return new Date(newApp.value.dateTime) >= new Date();
});

const toggleForm = () => {
  showForm.value = !showForm.value;
  submitted.value = false; // Reset de foutmeldingen bij het sluiten/openen
};

const sortedAppointments = computed(() => {
  return [...(props.appointments || [])].sort((a, b) =>
    new Date(b.dateTime || b.DateTime) - new Date(a.dateTime || a.DateTime)
  );
});

const saveAppointment = async () => {
  submitted.value = true; // Activeer de validatie-weergave

  if (!isFormValid.value) {
    return; // Stop als het formulier niet geldig is
  }

  try {
    const payload = {
      DateTime: newApp.value.dateTime,
      Duration: parseInt(newApp.value.duration),
      Type: newApp.value.type,
      Status: "Gepland"
    };

    await api.post(`/patients/${props.patientId}/appointments`, payload);
    alert("Afspraak succesvol opgeslagen.");
    showForm.value = false;
    submitted.value = false;
    newApp.value = { dateTime: '', type: 'Fysio sessie', duration: 30 };
    emit('refresh');
  } catch (err) {
    alert("Fout bij opslaan: " + (err.response?.data?.title || err.message));
  }
};

const cancelApp = async (appId) => {
  if (!confirm("Weet je zeker dat je deze afspraak wilt annuleren?")) return;
  try {
    await api.patch(`/patients/${props.patientId}/appointments/${appId}/cancel`);
    emit('refresh');
  } catch (err) {
    alert("Fout bij annuleren: " + err.message);
  }
};

const formatDateTime = (d) => {
  return new Date(d).toLocaleString('nl-NL', {
    day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit'
  });
};
</script>

<style scoped>
.appointment-form {
    background: #fafdff;
    border: 1px solid #d2f0f7;
    padding: 24px;
    border-radius: 12px;
    margin-bottom: 24px;
}

.form-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
    gap: 16px;
    margin-bottom: 20px;
}

.styled-input {
    width: 100%;
    padding: 10px;
    border: 1.5px solid #eaf6fb;
    border-radius: 8px;
    font-size: 1rem;
    transition: border-color 0.2s;
}

.styled-input:focus {
    border-color: #3bb3ce;
    outline: none;
}

.btn-save-appointment {
    background: #3bb3ce;
    color: white;
    border: none;
    padding: 12px 24px;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    transition: opacity 0.2s;
}

.btn-save-appointment:disabled {
    background: #cbd5e0;
    cursor: not-allowed;
}

.validation-hint {
    font-size: 0.85rem;
    color: #e53e3e;
    margin-top: 8px;
}

.appointment-row {
    display: grid;
    grid-template-columns: 2fr 1fr 1fr;
    padding: 16px 20px;
    border-bottom: 1px solid #eaf6fb;
    align-items: center;
}

.app-date {
    font-weight: 600;
    color: #2d3748;
}

.app-meta {
    font-size: 0.85rem;
    color: #718096;
}

.pill-planned {
    background: #ebf8ff;
    color: #2b6cb0;
    border: 1px solid #bee3f8;
}

.pill-cancelled {
    background: #fff5f5;
    color: #c53030;
    border: 1px solid #feb2b2;
}

.btn-cancel-small {
    background: transparent;
    color: #e53e3e;
    border: 1px solid #feb2b2;
    padding: 4px 12px;
    border-radius: 6px;
    font-size: 0.8rem;
    cursor: pointer;
}

.btn-cancel-small:hover {
    background: #fff5f5;
}

.status-cancelled {
    opacity: 0.5;
    background: #f8fafc;
}

.slide-fade-enter-active,
.slide-fade-leave-active {
    transition: all 0.3s ease;
}

.slide-fade-enter-from,
.slide-fade-leave-to {
    transform: translateY(-10px);
    opacity: 0;
}
</style>