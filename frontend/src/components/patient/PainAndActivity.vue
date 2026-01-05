<template>
    <div class="pain-activity-wrapper">
        <section class="dossier-section">
            <div class="section-header">
                <h3 class="dossier-section-title">📈 Pijnverloop</h3>
                <div class="filter-controls">
                    <input type="date" v-model="filterDateStart" placeholder="Van" />
                    <input type="date" v-model="filterDateEnd" placeholder="Tot" />
                </div>
            </div>

            <div class="pain-list" v-if="filteredPain.length">
                <div v-for="e in filteredPain" :key="e.id" class="pain-card">
                    <div class="pain-header">
                        <span>{{ formatDate(e.timestamp || e.Timestamp) }}</span>
                        <div class="pain-score-pill" :class="getPainClass(e.score || e.Score)">
                            Score: {{ e.score || e.Score }}/10
                        </div>
                    </div>
                    <div class="pain-bar-bg">
                        <div class="pain-bar-fill" :style="{
                            width: ((e.score || e.Score) * 10) + '%',
                            backgroundColor: getPainColor(e.score || e.Score)
                        }">
                        </div>
                    </div>
                    <p v-if="e.note || e.Note" class="pain-note">💬 {{ e.note || e.Note }}</p>
                </div>
            </div>
            <div v-else class="empty-state">Geen pijngegevens gevonden voor deze periode.</div>
        </section>

        <section class="dossier-section">
            <h3 class="dossier-section-title">📅 Activiteitenlogboek</h3>
            <div class="timeline" v-if="filteredActivity.length">
                <div v-for="l in filteredActivity" :key="l.id" class="timeline-item">
                    <div class="timeline-date">{{ formatDateTime(l.timestamp || l.Timestamp) }}</div>
                    <div class="timeline-content">
                        <span class="timeline-dot"></span>
                        <p>{{ l.activity || l.Activity }}</p>
                    </div>
                </div>
            </div>
            <div v-else class="empty-state">Geen activiteiten geregistreerd.</div>
        </section>
    </div>
</template>

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
    painEntries: { type: Array, default: () => [] },
    activityLogs: { type: Array, default: () => [] }
});

// Filters voor US5 criteria: "Filteren op specifieke periodes"
const filterDateStart = ref('');
const filterDateEnd = ref('');

// Filter- en sorteerlogiek voor Pijn
const filteredPain = computed(() => {
    let data = [...(props.painEntries ?? [])];

    if (filterDateStart.value) {
        data = data.filter(e => new Date(e.timestamp || e.Timestamp) >= new Date(filterDateStart.value));
    }
    if (filterDateEnd.value) {
        data = data.filter(e => new Date(e.timestamp || e.Timestamp) <= new Date(filterDateEnd.value));
    }

    // Sorteren: nieuwste bovenaan voor monitoring
    return data.sort((a, b) => new Date(b.timestamp || b.Timestamp) - new Date(a.timestamp || a.Timestamp));
});

// Filter- en sorteerlogiek voor Activiteiten
const filteredActivity = computed(() => {
    let data = [...(props.activityLogs ?? [])];

    // Toepassen van dezelfde periodefilter
    if (filterDateStart.value) {
        data = data.filter(l => new Date(l.timestamp || l.Timestamp) >= new Date(filterDateStart.value));
    }

    // Chronologische volgorde conform US5 criteria
    return data.sort((a, b) => new Date(b.timestamp || b.Timestamp) - new Date(a.timestamp || a.Timestamp));
});

// Helpers voor styling
const getPainColor = (s) => s >= 8 ? '#e53e3e' : s >= 5 ? '#ed8936' : '#48bb78';
const getPainClass = (s) => s >= 8 ? 'pain-high' : s >= 5 ? 'pain-mid' : 'pain-low';

// Datum formatting
const formatDate = (d) => new Date(d).toLocaleDateString('nl-NL');
const formatDateTime = (d) =>
    new Date(d).toLocaleString('nl-NL', {
        day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit'
    });
</script>

<style scoped>
/* ==========================================================================
        PIJN & ACTIVITEITEN
   ========================================================================== */
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

.filter-controls {
    display: flex;
    gap: 8px;
    margin-bottom: 10px;
}

.filter-controls input {
    padding: 4px 8px;
    border: 1px solid #cbd5e0;
    border-radius: 4px;
    font-size: 0.8em;
}

.empty-state {
    padding: 20px;
    color: #718096;
    text-align: center;
    font-style: italic;
}
</style>