<template>
    <section class="dossier-section">
        <h3 class="dossier-section-title">📝 Aanvullende Notities</h3>

        <div class="note-input-container card shadow-sm">
            <div class="form-group" :class="{ 'has-error': submitted && !newNoteContent.trim() }">
                <textarea v-model="newNoteContent" placeholder="Voeg een observatie of communicatie toe..."
                    class="styled-textarea" rows="3"></textarea>
            </div>

            <div class="input-footer">
                <span v-if="submitted && !newNoteContent.trim()" class="validation-hint">
                    ⚠️ Voer eerst een tekst in voor de notitie.
                </span>
                <button @click="submitNote" class="btn-add-note">
                    Notitie Toevoegen
                </button>
            </div>
        </div>

        <div class="notes-timeline">
            <div v-for="note in sortedNotes" :key="note.id" class="note-entry"
                :class="{ 'is-editing': editingNoteId === note.id }">

                <div class="note-card-header">
                    <div class="note-author-info">
                        <span class="author-name">{{ note.author?.firstName || 'Revalidatiearts' }}</span>
                        <span class="note-timestamp">{{ formatDateTime(note.timestamp || note.Timestamp) }}</span>
                    </div>

                    <div class="note-management-actions">
                        <button @click="startEdit(note)" class="btn-icon" title="Bewerken">✏️</button>
                        <button @click="deleteNote(note.id)" class="btn-icon" title="Verwijderen">🗑️</button>
                    </div>
                </div>

                <div v-if="editingNoteId === note.id" class="edit-mode-area">
                    <textarea v-model="editContent" class="styled-textarea"></textarea>
                    <div class="edit-actions">
                        <button @click="saveEdit(note.id)" class="btn-confirm-edit">Bijwerken</button>
                        <button @click="editingNoteId = null" class="btn-cancel-edit">Annuleren</button>
                    </div>
                </div>

                <p v-else class="note-text-body">{{ note.content || note.Content }}</p>
            </div>

            <div v-if="!notes.length" class="empty-notes">
                Geen notities gevonden voor deze patiënt.
            </div>
        </div>
    </section>
</template>

<script setup>
import { ref, computed } from 'vue';
import api from '../../api/axios';

const props = defineProps(['patientId', 'notes']);
const emit = defineEmits(['refresh']);

const newNoteContent = ref('');
const submitted = ref(false);
const editingNoteId = ref(null);
const editContent = ref('');

const sortedNotes = computed(() => {
    return [...(props.notes || [])].sort((a, b) =>
        new Date(b.timestamp || b.Timestamp) - new Date(a.timestamp || a.Timestamp)
    );
});

const submitNote = async () => {
    submitted.value = true;

    if (!newNoteContent.value.trim()) return;

    try {
        await api.post(`/patients/${props.patientId}/notes`, {
            content: newNoteContent.value
        });
        newNoteContent.value = '';
        submitted.value = false;
        emit('refresh');
    } catch (err) {
        alert("Fout bij toevoegen: " + err.message);
    }
};

const deleteNote = async (id) => {
    if (!confirm("Weet je zeker dat je deze notitie wilt verwijderen?")) return;
    try {
        await api.delete(`/patients/${props.patientId}/notes/${id}`);
        emit('refresh');
    } catch (err) { alert("Verwijderen mislukt."); }
};

const startEdit = (note) => {
    editingNoteId.value = note.id;
    editContent.value = note.content || note.Content;
};

const saveEdit = async (id) => {
    if (!editContent.value.trim()) return;
    try {
        await api.put(`/patients/${props.patientId}/notes/${id}`, { content: editContent.value });
        editingNoteId.value = null;
        emit('refresh');
    } catch (err) { alert("Bijwerken mislukt."); }
};

const formatDateTime = (d) => {
    return new Date(d).toLocaleString('nl-NL', {
        day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit'
    });
};
</script>

<style scoped>

.note-input-container {
    padding: 1.5rem;
    background: #fafdff;
    border: 1px solid #d2f0f7;
    border-radius: 12px;
    margin-bottom: 2rem;
}

.styled-textarea {
    width: 100%;
    border: 1.5px solid #eaf6fb;
    border-radius: 8px;
    padding: 12px;
    font-family: inherit;
    resize: vertical;
    transition: border-color 0.2s;
}

.styled-textarea:focus {
    border-color: #3bb3ce;
    outline: none;
}

.input-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: 1rem;
}

.btn-add-note {
    background: linear-gradient(135deg, #3bb3ce 0%, #2a9db8 100%);
    color: white;
    border: none;
    padding: 10px 24px;
    border-radius: 8px;
    font-weight: 700;
    cursor: pointer;
}

.validation-hint {
    color: #e53e3e;
    font-size: 0.85rem;
    font-weight: 500;
}

.note-entry {
    background: white;
    border: 1px solid #eaf6fb;
    border-radius: 10px;
    padding: 1rem 1.25rem;
    margin-bottom: 1rem;
    transition: box-shadow 0.2s;
}

.note-entry:hover {
    box-shadow: 0 4px 12px rgba(59, 179, 206, 0.08);
}

.note-card-header {
    display: flex;
    justify-content: space-between;
    margin-bottom: 0.75rem;
    border-bottom: 1px solid #f7fafc;
    padding-bottom: 0.5rem;
}

.author-name {
    font-weight: 700;
    color: #2d3748;
    margin-right: 0.75rem;
}

.note-timestamp {
    color: #a0aec0;
    font-size: 0.8rem;
}

.note-text-body {
    color: #4a5568;
    line-height: 1.5;
    white-space: pre-wrap;
}

.btn-icon {
    background: none;
    border: none;
    cursor: pointer;
    padding: 2px 5px;
    filter: grayscale(1);
    opacity: 0.6;
}

.btn-icon:hover {
    opacity: 1;
    filter: grayscale(0);
}

.edit-actions {
    margin-top: 0.75rem;
    display: flex;
    gap: 10px;
}

.btn-confirm-edit {
    background: #3bb3ce;
    color: white;
    border: none;
    padding: 5px 12px;
    border-radius: 5px;
    font-size: 0.8rem;
    cursor: pointer;
}
</style>