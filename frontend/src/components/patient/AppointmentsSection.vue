<template>
    <section class="dossier-section">
        <h3 class="dossier-section-title">📅 Afspraken</h3>
        <div class="appointment-list" v-if="appointments.length">
            <div v-for="a in appointments" :key="a.id" class="appointment-row">
                <strong>{{ format(a.appointmentDateTime || a.AppointmentDateTime) }}</strong>
                <span>{{ a.type || a.Type }}</span>
                <span class="pill">{{ a.status || a.Status }}</span>
            </div>
        </div>
    </section>
</template>

<script setup>
defineProps(['appointments']);
const format = (d) => new Date(d).toLocaleString('nl-NL', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' });
</script>

<style scoped>
/* ==========================================================================
        AFSPRAKEN
   ========================================================================== */
.appointment-list {
    border: 1px solid #edf2f7;
    border-radius: 8px;
    overflow: hidden;
    margin-top: 15px;
}

.appointment-row {
    display: grid;
    grid-template-columns: 2fr 2fr 1fr;
    padding: 12px 20px;
    border-bottom: 1px solid #edf2f7;
    align-items: center;
    background: white;
}

.appointment-row:last-child {
    border-bottom: none;
}

.app-date {
    color: #2d3748;
    margin: 0 3px 0 0 !important;
    max-width: 6vw;
}

.app-type {
    font-weight: 600;
    color: #3182ce;
    margin: 0 3px;
}

.app-duration {
    color: #718096;
    font-size: 0.9em;
}

.pill-planned {
    background: #ebf8ff;
    color: #2b6cb0;
    border: 1px solid #bee3f8;
}
</style>