<template>
    <section class="dossier-section">
        <h3 class="dossier-section-title">📋 Intakeverslag</h3>

        <div v-if="currentIntake" class="intake-report">
            <div class="report-meta">Opgesteld op: {{ formatDate(currentIntake.date || currentIntake.Date) }}</div>
            <div class="report-grid">
                <div class="report-item"><strong>Diagnose:</strong> {{ currentIntake.diagnosis ||
                    currentIntake.Diagnosis }}</div>
                <div class="report-item"><strong>Ernst:</strong> {{ currentIntake.severity || currentIntake.Severity }}
                </div>
                <div class="report-item full-width"><strong>Behandeldoelen:</strong> {{ currentIntake.goals ||
                    currentIntake.Goals }}</div>
            </div>

            <div class="notes-container">
                <h4>Aanvullende Notities</h4>
                <ul class="notes-list">
                    <li v-for="note in processedNotes" :key="note.id" class="note-item">
                        <small>{{ note.time }}</small>
                        <p>{{ note.content }}</p>
                    </li>
                </ul>
                <div class="add-note-box">
                    <textarea v-model="newNote" placeholder="Nieuwe notitie..."></textarea>
                    <button @click="saveNote" class="btn-small" :disabled="!newNote">Toevoegen</button>
                </div>
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
            <div class="form-group"><label>Doelen</label><textarea v-model="intakeForm.goals"></textarea></div>
            <button @click="saveIntake" class="save-button">Intake Opslaan</button>
        </div>
    </section>
</template>

<script setup>
import { ref, computed } from 'vue';
import api from '../../api/axios';

const props = defineProps(['patientId', 'rawIntakes', 'rawNotes']);
const emit = defineEmits(['refresh']);

const currentIntake = computed(() => props.rawIntakes?.[0] || null);
const intakeForm = ref({ diagnosis: '', severity: 'Licht', goals: '' });
const newNote = ref(''); // Dit is de variabele die je v-model gebruikt

const processedNotes = computed(() => (props.rawNotes || []).map(n => ({
    id: n.id || n.Id,
    content: n.content || n.Content,
    time: new Date(n.timestamp || n.Timestamp).toLocaleString('nl-NL')
})).sort((a, b) => new Date(b.time) - new Date(a.time)));

// 1. Fix voor de Intake (Diagnose/Doelen)
const saveIntake = async () => {
    try {
        await api.post(`/patients/${props.patientId}/intake`, intakeForm.value);
        alert("Intakeverslag succesvol opgeslagen!"); // Melding toegevoegd
        emit('refresh');
    } catch (err) {
        const errorMsg = err.response?.data?.message || err.message;
        alert("Fout bij opslaan intake: " + errorMsg);
    }
};

// 2. Fix voor de Aanvullende Notities
const saveNote = async () => {
    if (!newNote.value.trim()) {
        alert("Voer eerst een tekst in.");
        return;
    }

    try {
        await api.post(`/patients/${props.patientId}/notes`, {
            content: newNote.value
        });

        // DE CRUCIALE FIX: Hier stond eerst geen alert
        alert("Notitie succesvol toegevoegd!");

        newNote.value = ''; // Veld leegmaken
        emit('refresh');    // Lijst verversen
    } catch (err) {
        const errorMsg = err.response?.data?.message || err.message;
        alert("Fout bij toevoegen notitie: " + errorMsg);
    }
};

const formatDate = (d) => new Date(d).toLocaleDateString('nl-NL');
</script>

<style scoped>
/* ==========================================================================
        INTAKE & NOTITIES
   ========================================================================== */
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