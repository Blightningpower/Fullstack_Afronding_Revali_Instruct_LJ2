<template>
    <div class="pain-activity-wrapper">
        <section class="dossier-section">
            <h3 class="dossier-section-title">Pijnindicaties</h3>
            <div class="pain-list" v-if="pain.length">
                <div v-for="e in pain" :key="e.id" class="pain-card">
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
                    <p v-if="e.note || e.Note">💬 {{ e.note || e.Note }}</p>
                </div>
            </div>
        </section>

        <section class="dossier-section">
            <h3 class="dossier-section-title">Activiteitenlogboek</h3>
            <div class="timeline" v-if="activity.length">
                <div v-for="l in activity" :key="l.id" class="timeline-item">
                    <div class="timeline-date">{{ formatDateTime(l.timestamp || l.Timestamp) }}</div>
                    <div class="timeline-content">
                        <span class="timeline-dot"></span>
                        <p>{{ l.activity || l.Activity }}</p>
                    </div>
                </div>
            </div>
        </section>
    </div>
</template>

<script setup>
import { computed } from 'vue';

const props = defineProps({
    painEntries: { type: Array, default: () => [] },
    activityLogs: { type: Array, default: () => [] }
});

const pain = computed(() => props.painEntries ?? []);
const activity = computed(() => props.activityLogs ?? []);

const getPainColor = (s) => s >= 8 ? '#e53e3e' : s >= 5 ? '#ed8936' : '#48bb78';
const getPainClass = (s) => s >= 8 ? 'pain-high' : s >= 5 ? 'pain-mid' : 'pain-low';
const formatDate = (d) => new Date(d).toLocaleDateString('nl-NL');
const formatDateTime = (d) =>
    new Date(d).toLocaleString('nl-NL', { hour: '2-digit', minute: '2-digit' });
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
</style>