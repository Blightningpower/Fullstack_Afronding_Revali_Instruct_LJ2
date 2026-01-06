<template>
  <section class="dossier-section">
    <div class="appointment-header">
      <h3 class="dossier-section-title">📅 Afspraken & Sessies</h3>
      <button @click="toggleForm" :class="['btn-toggle', { 'is-active': showForm }]">
        {{ showForm ? 'Annuleren' : '+ Nieuwe Afspraak' }}
      </button>
    </div>

    <transition name="slide-fade">
      <div v-if="showForm" class="appointment-form-card">
        <div class="form-grid">
          <div class="form-group" :class="{ 'has-error': submitted && !newApp.dateTime }">
            <label class="form-label">Datum & Tijd <span class="required-mark">*</span></label>
            <input 
              type="datetime-local" 
              v-model="newApp.dateTime" 
              :min="minDateTime"
              class="form-input"
            />
          </div>
          <div class="form-group">
            <label class="form-label">Type Sessie</label>
            <select v-model="newApp.type" class="form-input">
              <option>Controle</option>
              <option>Fysio sessie</option>
              <option>Evaluatie</option>
              <option>Intake</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">Duur (minuten)</label>
            <input 
              type="number" 
              v-model="newApp.duration" 
              min="5" 
              step="5"
              class="form-input"
            />
          </div>
        </div>
        
        <div class="form-footer">
          <button 
            @click="saveAppointment" 
            class="btn-submit" 
            :disabled="submitted && !isFormValid"
          >
            Afspraak Inplannen
          </button>
          <p v-if="submitted && !isFormValid" class="error-text">
            ⚠️ Kies een geldig moment in de toekomst.
          </p>
        </div>
      </div>
    </transition>

    <div class="appointment-list">
      <div v-for="app in sortedAppointments" :key="app.id || app.Id" class="appointment-item"
        :class="{ 'is-cancelled': (app.status || app.Status) === 'Geannuleerd' }">

        <div class="item-main">
          <div class="item-date">{{ formatDateTime(app.dateTime || app.DateTime) }}</div>
          <div class="item-details">
            <span class="type-tag">{{ app.type || app.Type }}</span>
            <span class="duration-tag">{{ app.duration || app.Duration }} min</span>
          </div>
        </div>
        
        <div class="item-status">
          <span class="status-pill"
            :class="(app.status || app.Status) === 'Geannuleerd' ? 'status-red' : 'status-blue'">
            {{ app.status || app.Status }}
          </span>
        </div>

        <div class="item-actions">
          <button v-if="(app.status || app.Status) !== 'Geannuleerd'" 
                  @click="cancelApp(app.id || app.Id)"
                  class="btn-action-cancel">
            Annuleren
          </button>
        </div>
      </div>

      <div v-if="!sortedAppointments.length" class="empty-list">
        <span class="empty-icon">📭</span>
        <p>Geen afspraken gevonden voor deze patiënt.</p>
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
const submitted = ref(false);
const newApp = ref({ dateTime: '', type: 'Fysio sessie', duration: 30 });

const minDateTime = computed(() => {
  const now = new Date();
  return now.toISOString().slice(0, 16);
});

const isFormValid = computed(() => {
  if (!newApp.value.dateTime) return false;
  // US6 AC 1: Blokkeer verleden tijd
  return new Date(newApp.value.dateTime) >= new Date();
});

const toggleForm = () => {
  showForm.value = !showForm.value;
  submitted.value = false;
};

const sortedAppointments = computed(() => {
  return [...(props.appointments || [])].sort((a, b) =>
    new Date(b.dateTime || b.DateTime) - new Date(a.dateTime || a.DateTime)
  );
});

const saveAppointment = async () => {
  submitted.value = true;
  if (!isFormValid.value) return;

  try {
    const payload = {
      DateTime: newApp.value.dateTime,
      Duration: parseInt(newApp.value.duration),
      Type: newApp.value.type,
      Status: "Gepland"
    };

    await api.post(`/patients/${props.patientId}/appointments`, payload);
    showForm.value = false;
    submitted.value = false;
    newApp.value = { dateTime: '', type: 'Fysio sessie', duration: 30 };
    emit('refresh');
  } catch (err) {
    console.error(err);
  }
};

const cancelApp = async (appId) => {
  if (!confirm("Weet je zeker dat je deze afspraak wilt annuleren?")) return;
  try {
    await api.patch(`/patients/${props.patientId}/appointments/${appId}/cancel`);
    emit('refresh');
  } catch (err) {
    console.error(err);
  }
};

const formatDateTime = (d) => {
  return new Date(d).toLocaleString('nl-NL', {
    day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit'
  });
};
</script>

<style scoped>
/* Sectie Layout */
.appointment-section {
  margin-top: 32px;
  background: white;
  border-radius: 12px;
}

.appointment-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 20px;
  border-bottom: 2px solid #eaf6fb;
  margin-bottom: 24px;
}

.section-title {
  color: #2a7ca3;
  font-weight: 700;
  margin: 0;
}

/* Knoppen */
.btn-toggle {
  background: #fafdff;
  color: #2a7ca3;
  border: 2px solid #d2f0f7;
  padding: 8px 16px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-toggle.is-active {
  background: #fff5f5;
  color: #c53030;
  border-color: #feb2b2;
}

.btn-submit {
  background: #3bb3ce;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-weight: 700;
  cursor: pointer;
  box-shadow: 0 4px 6px rgba(59, 179, 206, 0.2);
}

.btn-submit:disabled {
  background: #cbd5e0;
  box-shadow: none;
  cursor: not-allowed;
}

/* Formulier */
.appointment-form-card {
  background: #fafdff;
  border: 2px solid #eaf6fb;
  padding: 24px;
  border-radius: 12px;
  margin-bottom: 32px;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 24px;
}

.form-label {
  display: block;
  font-size: 0.9rem;
  font-weight: 600;
  color: #2a7ca3;
  margin-bottom: 8px;
}

.form-input {
  width: 100%;
  padding: 12px;
  border: 2px solid #eaf6fb;
  border-radius: 8px;
  font-size: 1rem;
  background: white;
  outline: none;
}

.form-input:focus {
  border-color: #3bb3ce;
}

.required-mark {
  color: #e53e3e;
}

.error-text {
  color: #e53e3e;
  font-size: 0.85rem;
  margin-top: 12px;
  font-weight: 600;
}

/* Lijst items */
.appointment-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 18px 24px;
  border: 1px solid #eaf6fb;
  border-radius: 10px;
  margin-bottom: 12px;
  transition: transform 0.2s;
}

.appointment-item:hover {
  transform: translateX(4px);
  border-color: #d2f0f7;
}

.item-date {
  font-weight: 700;
  font-size: 1.05rem;
  color: #2d3748;
}

.item-details {
  display: flex;
  gap: 12px;
  margin-top: 4px;
}

.type-tag {
  color: #718096;
  font-size: 0.85rem;
}

.duration-tag {
  background: #f7fafc;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 0.8rem;
  color: #718096;
}

/* Status Pillen */
.status-pill {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 700;
}

.status-blue {
  background: #ebf8ff;
  color: #2b6cb0;
  border: 1px solid #bee3f8;
}

.status-red {
  background: #fff5f5;
  color: #c53030;
  border: 1px solid #feb2b2;
}

.btn-action-cancel {
  background: transparent;
  color: #e53e3e;
  border: 1.5px solid #feb2b2;
  padding: 6px 14px;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
}

.btn-action-cancel:hover {
  background: #fff5f5;
}

.is-cancelled {
  opacity: 0.6;
  background: #f8fafc;
}

.empty-list {
  text-align: center;
  padding: 48px;
  background: #fafdff;
  border: 2px dashed #eaf6fb;
  border-radius: 12px;
}

.empty-icon {
  font-size: 2rem;
  margin-bottom: 12px;
  display: block;
}

/* Animatie */
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