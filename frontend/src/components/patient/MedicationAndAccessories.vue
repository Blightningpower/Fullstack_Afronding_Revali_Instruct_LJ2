<template>
    <div class="med-acc-wrapper">
        <section class="dossier-section">
            <h3 class="dossier-section-title">💊 Medicatie</h3>
            <div class="info-list" v-if="medications.length">
                <div v-for="m in medications" :key="m.id" class="info-card">
                    <div class="info-card-header"><strong>{{ m.name || m.Name }}</strong><span class="status-pill">{{
                        m.status || m.Status }}</span></div>
                    <p>{{ m.dosage || m.Dosage }} — {{ m.frequency || m.Frequency }}</p>
                </div>
            </div>
        </section>

        <section class="dossier-section">
            <h3 class="dossier-section-title">🦿 Hulpmiddelen</h3>
            <div class="info-list" v-if="accessories.length">
                <div v-for="a in accessories" :key="a.id" class="info-card">
                    <strong>{{ a.name || a.Name }}</strong><br><small>Gebruik: {{ a.expectedUsagePeriod ||
                        a.ExpectedUsagePeriod }}</small>
                </div>
            </div>
        </section>
    </div>
</template>

<script setup>
defineProps(['medications', 'accessories']);
</script>

<style scoped>
/* ==========================================================================
        MEDICATIE & HULPMIDDELEN
   ========================================================================== */
.info-list {
    display: grid;
    gap: 12px;
    margin-top: 10px;
}

.info-card {
    background: #f8fafc;
    padding: 12px;
    border-radius: 8px;
    border: 1px solid #e2e8f0;
}

.info-card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 5px;
}

.info-card-detail {
    font-size: 0.9em;
    color: #2d3748;
    font-weight: 500;
}

.info-card-meta {
    font-size: 0.8em;
    color: #718096;
    margin-top: 4px;
}

.pill-active {
    background: #ebf8ff;
    color: #2b6cb0;
}

.pill-done {
    background: #f7fafc;
    color: #a0aec0;
}
</style>