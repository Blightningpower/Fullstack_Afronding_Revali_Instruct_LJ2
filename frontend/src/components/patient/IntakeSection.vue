<template>
    <section class="dossier-section">
        <h3 class="dossier-section-title">📋 Intakeverslag</h3>

        <div v-if="intakeData" class="intake-report">
            <div class="report-meta">
                Opgesteld op: {{ formatDate(intakeData.date || intakeData.Date) }}
            </div>
            <div class="report-grid">
                <div class="report-item">
                    <strong>Diagnose:</strong> {{ intakeData.diagnosis || intakeData.Diagnosis }}
                </div>
                <div class="report-item">
                    <strong>Ernst:</strong> {{ intakeData.severity || intakeData.Severity }}
                </div>
                <div class="report-item full-width">
                    <strong>Behandeldoelen:</strong> {{ intakeData.initialGoals || intakeData.InitialGoals }}
                </div>
            </div>

            <div class="notes-container">
                <NoteSection :patientId="patientId" :notes="rawNotes" @refresh="$emit('refresh')" />
            </div>
        </div>

        <div v-else class="intake-form">
            <div class="form-group"><label>Diagnose</label><input v-model="intakeForm.diagnosis" /></div>
            <div class="form-group">
                <label>Ernst</label>
                <select v-model="intakeForm.severity">
                    <option>Licht</option>
                    <option>Matig</option>
                    <option>Ernstig</option>
                </select>
            </div>
            <div class="form-group">
                <label>Doelen</label>
                <textarea v-model="intakeForm.initialGoals"></textarea>
            </div>
            <button @click="saveIntake" class="save-button" :disabled="loading">
                {{ loading ? 'Bezig met opslaan...' : 'Intake Opslaan' }}
            </button>
        </div>
    </section>
</template>

<script setup>
import { ref, computed } from 'vue';
import api from '../../api/axios';
import NoteSection from './NotesSection.vue';

const props = defineProps(['patientId', 'rawIntakes', 'rawNotes']);
const emit = defineEmits(['refresh']);

const loading = ref(false);

const intakeData = computed(() => (props.rawIntakes && props.rawIntakes.length > 0) ? props.rawIntakes[0] : null);

const intakeForm = ref({
    diagnosis: '',
    severity: 'Matig',
    initialGoals: ''
});

const saveIntake = async () => {
    loading.value = true;
    try {
        await api.post(`/patients/${props.patientId}/intake`, intakeForm.value);
        alert("✅ Intakeverslag succesvol opgeslagen!");
        emit('refresh');
    } catch (err) {
        const errorMsg = err.response?.data?.message || err.message;
        alert("Fout bij opslaan intake: " + errorMsg);
    } finally {
        loading.value = false;
    }
};

const formatDate = (d) => d ? new Date(d).toLocaleString('nl-NL') : '';
</script>

<style scoped>

.intake-report {
    background: #fff;
    border: 2px solid #e2e8f0;
    padding: 20px;
    border-radius: 12px;
}

.report-meta {
    font-size: 0.8em;
    color: #718096;
    margin-bottom: 15px;
    border-bottom: 1px solid #edf2f7;
    padding-bottom: 8px;
}

.report-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 20px;
}

.full-width {
    grid-column: span 2;
}

.intake-form {
    background: #f7fafc;
    padding: 20px;
    border-radius: 12px;
    border: 1px dashed #cbd5e0;
}

.form-group {
    margin-bottom: 15px;
}

.form-group label {
    display: block;
    font-weight: bold;
    margin-bottom: 5px;
    color: #4a5568;
}

.form-group input,
.form-group select,
.form-group textarea {
    width: 100%;
    padding: 10px;
    border: 1px solid #cbd5e0;
    border-radius: 6px;
}

.save-button {
    background: #38a169;
    color: white;
    padding: 12px 24px;
    border-radius: 8px;
    font-weight: bold;
    cursor: pointer;
    border: none;
    width: 100%;
}

.save-button:disabled {
    background: #a0aec0;
    cursor: not-allowed;
}

.notes-container {
    margin-top: 25px;
    padding-top: 20px;
    border-top: 1px solid #edf2f7;
}

.note-item {
    background: #fdfdfd;
    padding: 10px;
    border-left: 3px solid #3182ce;
    margin-bottom: 10px;
    list-style: none;
}

.add-note-box textarea {
    width: 100%;
    padding: 10px;
    border: 1px solid #cbd5e0;
    border-radius: 6px;
    margin-top: 10px;
}

.btn-small {
    background: #3182ce;
    color: white;
    padding: 6px 12px;
    border-radius: 4px;
    border: none;
    margin-top: 5px;
    cursor: pointer;
}

.form-row-3 {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    gap: 15px;
    margin-bottom: 15px;
}

.form-row-2 {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 15px;
    margin-bottom: 15px;
}
</style>