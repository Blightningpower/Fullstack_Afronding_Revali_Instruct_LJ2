<template>
    <section class="dossier-section">
        <div class="section-header">
            <h3 class="dossier-section-title">🏃 Behandelplan: Oefeningen</h3>
            <button @click="showAssignForm = !showAssignForm" class="btn-assign-toggle">
                {{ showAssignForm ? '✖ Annuleren' : '＋ Oefening toewijzen' }}
            </button>
        </div>

        <div v-if="showAssignForm" class="assignment-card">
            <div class="form-group">
                <label>Selecteer Oefening (Bibliotheek)</label>
                <select v-model="newAssignment.exerciseId" class="form-input">
                    <option :value="null" disabled>Kies een oefening...</option>
                    <option v-for="ex in library" :key="ex.id || ex.Id" :value="ex.id || ex.Id">
                        {{ ex.name || ex.Name }}
                    </option>
                </select>
            </div>

            <div class="form-row-3">
                <div class="form-group"><label>Reps</label><input type="number" v-model="newAssignment.repetitions"
                        class="form-input" /></div>
                <div class="form-group"><label>Sets</label><input type="number" v-model="newAssignment.sets"
                        class="form-input" /></div>
                <div class="form-group"><label>Frequentie</label><input type="number" v-model="newAssignment.frequency"
                        class="form-input" /></div>
            </div>

            <button @click="handleAssign" class="btn-save-assignment">Toewijzing Opslaan</button>
        </div>

        <ul class="dossier-list" v-if="processedExercises.length > 0">
            <li v-for="ex in processedExercises" :key="ex.id" class="section-item exercise-entry">
                <div class="exercise-status-col">
                    <span class="pill" :class="ex.checked ? 'pill-ok' : 'pill-open'">
                        {{ ex.checked ? '✓ Afgevinkt' : 'Nog open' }}
                    </span>
                </div>

                <div class="exercise-main">
                    <span class="dossier-item-title">{{ ex.title }}</span>
                    <p class="exercise-description" v-if="ex.description">{{ ex.description }}</p>
                    <div class="dossier-item-meta">
                        <strong>Planning:</strong> {{ ex.repetitions }}x per set — {{ ex.sets }} sets
                        <div class="exercise-notes" v-if="ex.notes">
                            📝 <i>Opmerking: {{ ex.notes }}</i>
                        </div>
                    </div>
                </div>
            </li>
        </ul>
        <p v-else class="empty-text">Geen oefeningen toegewezen aan deze patiënt.</p>
    </section>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import api from '../../api/axios';

const props = defineProps(['patientId', 'rawExercises']);
const emit = defineEmits(['refresh']);
const showAssignForm = ref(false);
const library = ref([]);

const processedExercises = computed(() => {
    return (props.rawExercises || []).map(ex => ({
        id: ex.id || ex.Id,
        title: ex.exercise?.name || ex.Exercise?.Name || 'Onbekend',
        description: ex.exercise?.description || ex.Exercise?.Description || '',
        checked: ex.clientCheckedOff || ex.ClientCheckedOff || false,
        repetitions: ex.repetitions || ex.Repetitions || 0,
        sets: ex.sets || ex.Sets || 0,
        notes: ex.notes || ex.Notes || ''
    }));
});

const newAssignment = ref({ exerciseId: null, repetitions: 10, sets: 3, frequency: 1, startDate: new Date().toISOString().split('T')[0], endDate: '' });

const handleAssign = async () => {
    const payload = { ...newAssignment.value };
    if (payload.startDate === "") payload.startDate = null;
    if (payload.endDate === "") payload.endDate = null;

    try {
        const response = await api.post(`/patients/${props.patientId}/exercises`, payload);
        
        alert(response.data.message || "Oefening succesvol toegewezen!");
        
        showAssignForm.value = false;
        emit('refresh');
        
        newAssignment.value = { exerciseId: null, repetitions: 10, sets: 3, frequency: 1, startDate: '', endDate: '' };
    } catch (err) { 
        const errorMsg = err.response?.data?.errors 
            ? JSON.stringify(err.response.data.errors) 
            : (err.response?.data?.message || err.message);
        alert("Fout bij opslaan: " + errorMsg); 
    }
};

onMounted(async () => {
    try {
        const res = await api.get('/exercises');
        library.value = res.data;
    } catch (err) { console.error("Kon bibliotheek niet laden", err); }
});
</script>

<style scoped>
    /* ==========================================================================
        OEFENINGEN
   ========================================================================== */
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

.section-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 1rem;
    flex-direction: column;
}

.assignment-card {
  background: #f7fafc;
  padding: 20px;
  border-radius: 12px;
  border: 2px solid #e2e8f0;
  margin-bottom: 25px;
}

.btn-assign-toggle {
  background-color: #3182ce;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  margin-top: 10px;
}

.btn-save-assignment {
  background-color: #38a169;
  color: white;
  border: none;
  padding: 12px;
  border-radius: 8px;
  font-weight: bold;
  width: 100%;
  cursor: pointer;
  margin-top: 10px;
}

.btn-save-assignment:hover {
  background-color: #2f855a;
}
</style>