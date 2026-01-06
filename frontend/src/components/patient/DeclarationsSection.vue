<template>
    <section class="dossier-section declaration-section">
        <div class="section-header">
            <h3 class="dossier-section-title">💶 Declaraties & Kosten</h3>
            <button @click="showForm = !showForm" :class="['btn-toggle', { 'active': showForm }]">
                {{ showForm ? 'Annuleren' : '+ Nieuwe Declaratie' }}
            </button>
        </div>

        <transition name="slide-fade">
            <div v-if="showForm" class="declaration-form card shadow-sm">
                <div class="form-grid">
                    <div class="form-group">
                        <label>Type Behandeling</label>
                        <select v-model="newDec.treatmentType" class="styled-input">
                            <option>Intake revalidatie</option>
                            <option>Controle consult</option>
                            <option>Fysio sessie</option>
                            <option>Evaluatiegesprek</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label>Kostenbedrag (€)</label>
                        <input type="number" v-model.number="newDec.amount" step="0.01" min="0" placeholder="0.00"
                            class="styled-input" :class="{ 'input-error': newDec.amount < 0 }" />
                    </div>
                </div>
                <div class="form-actions">
                    <button @click="saveDeclaration" class="btn-save-declaration" :disabled="!isValid">
                        Registreren
                    </button>
                    <p v-if="newDec.amount < 0" class="validation-hint">⚠️ Bedrag mag niet negatief zijn.</p>
                </div>
            </div>
        </transition>

        <div class="declaration-list">
            <div v-for="dec in sortedDeclarations" :key="dec.id" class="declaration-row"
                :class="{ 'is-declared': dec.status === 'Gedeclareerd' }">

                <div class="dec-info">
                    <div class="dec-date">{{ formatDate(dec.date || dec.Date) }}</div>
                    <div class="dec-type">{{ dec.treatmentType || dec.TreatmentType }}</div>
                </div>

                <div class="dec-status-box">
                    <span class="status-pill" :class="dec.status === 'Gedeclareerd' ? 'pill-done' : 'pill-pending'">
                        {{ dec.status }}
                    </span>
                </div>

                <div class="dec-amount">
                    €{{ (dec.amount || dec.Amount).toFixed(2) }}
                </div>

                <div class="dec-actions">
                    <button v-if="dec.status !== 'Gedeclareerd'" @click="markAsDeclared(dec.id)"
                        class="btn-action-small">
                        Markeer
                    </button>
                </div>
            </div>

            <div v-if="!declarations.length" class="empty-state">
                Geen declaraties geregistreerd.
            </div>
        </div>
    </section>
</template>

<script setup>
import { ref, computed } from 'vue';
import api from '../../api/axios';

const props = defineProps(['patientId', 'declarations']);
const emit = defineEmits(['refresh']);

const showForm = ref(false);
const newDec = ref({ treatmentType: 'Controle consult', amount: null });

const isValid = computed(() => newDec.value.amount !== null && newDec.value.amount >= 0);

const sortedDeclarations = computed(() => {
    return [...(props.declarations || [])].sort((a, b) =>
        new Date(b.date || b.Date) - new Date(a.date || a.Date)
    );
});

const saveDeclaration = async () => {
    try {
        await api.post(`/patients/${props.patientId}/declarations`, newDec.value);
        alert("Kosten succesvol geregistreerd.");
        showForm.value = false;
        newDec.value = { treatmentType: 'Controle consult', amount: null };
        emit('refresh');
    } catch (err) {
        alert("Fout bij opslaan: " + err.message);
    }
};

const markAsDeclared = async (decId) => {
    try {
        await api.patch(`/patients/${props.patientId}/declarations/${decId}/mark-declared`);
        emit('refresh');
    } catch (err) {
        alert("Fout bij bijwerken: " + err.message);
    }
};

const formatDate = (d) => new Date(d).toLocaleDateString('nl-NL', {
    day: '2-digit', month: 'short', year: 'numeric'
});
</script>

<style scoped>
.declaration-section {
    margin-top: 2rem;
}

.declaration-form {
    background: #fafdff;
    border: 1px solid #d2f0f7;
    padding: 20px;
    border-radius: 12px;
    margin-bottom: 20px;
}

.form-grid {
    display: grid;
    grid-template-columns: 2fr 1fr;
    gap: 15px;
    margin-bottom: 15px;
}

.styled-input {
    width: 100%;
    padding: 10px;
    border: 1.5px solid #eaf6fb;
    border-radius: 8px;
    outline: none;
}

.styled-input:focus {
    border-color: #3bb3ce;
}

.btn-save-declaration {
    background: #3bb3ce;
    color: white;
    border: none;
    padding: 10px 20px;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
}

.btn-save-declaration:disabled {
    background: #cbd5e0;
    cursor: not-allowed;
}

.declaration-list {
    border: 1px solid #eaf6fb;
    border-radius: 12px;
    overflow: hidden;
}

.declaration-row {
    display: grid;
    grid-template-columns: 2fr 1fr 1fr 80px;
    padding: 14px 20px;
    background: white;
    border-bottom: 1px solid #f0f4f8;
    align-items: center;
}

.declaration-row:last-child {
    border-bottom: none;
}

.dec-date {
    font-size: 0.85rem;
    color: #718096;
}

.dec-type {
    font-weight: 600;
    color: #2d3748;
}

.dec-amount {
    font-weight: 700;
    color: #2b6cb0;
    text-align: right;
    padding-right: 15px;
}

.status-pill {
    padding: 4px 10px;
    border-radius: 20px;
    font-size: 0.75rem;
    font-weight: 600;
}

.pill-pending {
    background: #ebf8ff;
    color: #2b6cb0;
}

.pill-done {
    background: #f0fff4;
    color: #2f855a;
}

.btn-action-small {
    background: #3bb3ce;
    color: white;
    border: none;
    padding: 5px 10px;
    border-radius: 6px;
    font-size: 0.75rem;
    cursor: pointer;
}

.is-declared {
    background: #fafdfb;
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