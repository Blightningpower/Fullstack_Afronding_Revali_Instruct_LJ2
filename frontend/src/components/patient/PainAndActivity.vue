<template>
    <div class="pain-activity-wrapper gap-4 d-flex flex-column">
        <section class="dossier-section">
            <div class="section-header d-flex flex-wrap justify-content-between align-items-center mb-4 gap-3">
                <h3 class="dossier-section-title mb-0">📈 Pijnverloop (Trend)</h3>

                <div class="filter-controls d-flex gap-2 align-items-center">
                    <div class="date-input-group">
                        <span class="input-icon">📅</span>
                        <input type="date" v-model="filterDateStart" class="styled-date-input" placeholder="Van" />
                    </div>
                    <span class="text-muted">-</span>
                    <div class="date-input-group">
                        <span class="input-icon">📅</span>
                        <input type="date" v-model="filterDateEnd" class="styled-date-input" placeholder="Tot" />
                    </div>
                </div>
            </div>

            <div class="chart-wrapper mb-4" v-if="filteredPain.length">
                <div class="chart-container-inner">
                    <Line :data="chartData" :options="chartOptions" />
                </div>
            </div>
            <div v-else class="empty-state py-5">
                <div class="empty-icon">📉</div>
                <p>Geen pijngegevens gevonden voor deze periode.</p>
            </div>

            <div class="pain-detail-list mt-4" v-if="filteredPain.length">
                <h4 class="h6 text-primary fw-bold mb-3 border-bottom pb-2">Recente registraties</h4>
                <div class="d-flex flex-column gap-3">
                    <div v-for="e in [...filteredPain].reverse().slice(0, 3)" :key="e.id"
                        class="registratie-row shadow-xs">
                        <div class="d-flex justify-content-between align-items-center w-100">
                            <div class="d-flex align-items-center gap-2">
                                <span class="calendar-icon">📅</span>
                                <span class="pain-date-text">{{ formatDate(e.timestamp || e.Timestamp) }}</span>
                            </div>
                            <div class="pain-badge-container">
                                <span class="vas-label">VAS:</span>
                                <span class="pain-badge" :class="getPainClass(e.score || e.Score)">
                                    {{ e.score || e.Score }}<small>/10</small>
                                </span>
                            </div>
                        </div>
                        <div v-if="e.note || e.Note" class="pain-note-bubble mt-2">
                            <span class="quote-icon">"</span>
                            {{ e.note || e.Note }}
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <section class="dossier-section">
            <h3 class="dossier-section-title">📅 Activiteitenlogboek</h3>

            <div class="timeline-container pl-2" v-if="filteredActivity.length">
                <div class="timeline">
                    <div v-for="l in filteredActivity" :key="l.id" class="timeline-item">
                        <div class="timeline-marker">
                            <div class="timeline-dot"></div>
                        </div>
                        <div class="timeline-content-wrapper">
                            <div class="timeline-header">
                                <span class="timeline-date">{{ formatDateTime(l.timestamp || l.Timestamp) }}</span>
                            </div>
                            <div class="timeline-card">
                                <p class="mb-0 text-dark">{{ l.activity || l.Activity }}</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div v-else class="empty-state py-5">
                <div class="empty-icon">📋</div>
                <p>Geen activiteiten geregistreerd in deze periode.</p>
            </div>
        </section>
    </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { Line } from 'vue-chartjs';
import {
    Chart as ChartJS,
    Title,
    Tooltip,
    Legend,
    LineElement,
    LinearScale,
    PointElement,
    CategoryScale,
    Filler
} from 'chart.js';

ChartJS.register(Title, Tooltip, Legend, LineElement, LinearScale, PointElement, CategoryScale, Filler);

const props = defineProps(['patientId', 'activities', 'painEntries']);

const filterDateStart = ref('');
const filterDateEnd = ref('');

// Filter logica voor Pijn (Oud -> Nieuw voor grafiek)
const filteredPain = computed(() => {
    let data = [...(props.painEntries ?? [])];
    if (filterDateStart.value) data = data.filter(e => new Date(e.timestamp || e.Timestamp) >= new Date(filterDateStart.value));
    if (filterDateEnd.value) data = data.filter(e => new Date(e.timestamp || e.Timestamp) <= new Date(filterDateEnd.value));
    return data.sort((a, b) => new Date(a.timestamp || a.Timestamp) - new Date(b.timestamp || b.Timestamp));
});

// Filter logica voor Activiteiten (Nieuw -> Oud voor tijdlijn)
const filteredActivity = computed(() => {
    let data = [...(props.activities ?? [])];
    if (filterDateStart.value) data = data.filter(l => new Date(l.timestamp || l.Timestamp) >= new Date(filterDateStart.value));
    if (filterDateEnd.value) data = data.filter(l => new Date(l.timestamp || l.Timestamp) <= new Date(filterDateEnd.value));
    return data.sort((a, b) => new Date(b.timestamp || b.Timestamp) - new Date(a.timestamp || a.Timestamp));
});

const chartData = computed(() => ({
    labels: filteredPain.value.map(e => formatDate(e.timestamp || e.Timestamp)),
    datasets: [{
        label: 'Pijnscore (VAS)',
        backgroundColor: 'rgba(59, 179, 206, 0.15)',
        borderColor: '#3bb3ce',
        borderWidth: 2.5,
        data: filteredPain.value.map(e => e.score || e.Score),
        tension: 0.3,
        pointRadius: 5,
        pointBackgroundColor: '#ffffff',
        pointBorderColor: '#3bb3ce',
        pointBorderWidth: 2,
        fill: true
    }]
}));

const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    layout: {
        padding: { left: 10, right: 20, top: 20, bottom: 10 }
    },
    scales: {
        y: {
            min: 0, max: 10,
            ticks: { stepSize: 1, font: { size: 11 } },
            grid: { color: '#eaf6fb' },
            title: { display: true, text: 'VAS Score', font: { weight: 'bold' } }
        },
        x: {
            grid: { display: false },
            ticks: { maxRotation: 45, minRotation: 45, font: { size: 10 } }
        }
    },
    plugins: {
        legend: { display: false },
        tooltip: {
            mode: 'index', intersect: false, backgroundColor: 'rgba(255,255,255,0.95)',
            titleColor: '#234', bodyColor: '#234', borderColor: '#d2f0f7', borderWidth: 1
        }
    }
};

const getPainClass = (s) => s >= 7 ? 'badge-high' : s >= 4 ? 'badge-mid' : 'badge-low';

const formatDate = (d) => new Date(d).toLocaleDateString('nl-NL', {
    day: '2-digit', month: 'short', year: 'numeric'
});
const formatDateTime = (d) => new Date(d).toLocaleString('nl-NL', {
    day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit'
});
</script>

<style scoped>

/* ================= ALGEMENE STIJLEN ================= */
.revali-card {
    border: none;
    border-radius: 16px;
    overflow: hidden;
}

.dossier-section-title {
    color: #2a7ca3;
    font-weight: 700;
}

/* ================= FILTERS ================= */
.date-input-group {
    position: relative;
    display: flex;
    align-items: center;
}

.input-icon {
    position: absolute;
    left: 10px;
    font-size: 0.9rem;
    opacity: 0.7;
    pointer-events: none;
}

.styled-date-input {
    border: 2px solid #eaf6fb;
    border-radius: 8px;
    padding: 6px 10px 6px 32px;
    font-size: 0.9rem;
    color: #234;
    background: #fafdff;
    transition: all 0.2s;
    outline: none;
}

.styled-date-input:focus {
    border-color: #3bb3ce;
    background: #fff;
    box-shadow: 0 0 0 3px rgba(59, 179, 206, 0.1);
}

/* ================= GRAFIEK ================= */
.chart-wrapper {
    background: linear-gradient(to bottom, #ffffff, #fafdff);
    border-radius: 12px;
    border: 1px solid #eaf6fb;
    padding: 1.5rem;
    box-shadow: inset 0 0 20px rgba(59, 179, 206, 0.05);
}

.chart-container-inner {
    height: 320px;
    position: relative;
}

/* ================= MINI CARDS (Details) ================= */
.registratie-row {
    background: #ffffff;
    border: 1px solid #eaf6fb;
    border-radius: 12px;
    padding: 16px;
    margin-bottom: 12px;
}

.registratie-row:hover {
    transform: scale(1.01);
    border-color: #3bb3ce;
}

.pain-date-text {
    font-weight: 600;
    color: #4a5568;
}

.vas-label {
    font-size: 0.75rem;
    font-weight: 800;
    color: #a0aec0;
    margin-right: 6px;
    text-transform: uppercase;
}

.pain-badge {
    min-width: 50px;
    height: 28px;
    padding: 0 10px;
    border-radius: 14px;
    font-weight: 800;
    font-size: 0.9rem;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    float: inline-end;
}

/* Kleur-definities voor de VAS-badges */
.badge-high {
    background: linear-gradient(135deg, #e53e3e, #c53030);
    color: white !important;
}

.badge-mid {
    background: linear-gradient(135deg, #ed8936, #dd6b20);
    color: white !important;
}

.badge-low {
    background: linear-gradient(135deg, #48bb78, #38a169);
    color: white !important;
}

.pain-note-bubble {
    font-size: 0.9rem;
    color: #2d3748;
    font-style: italic;
    background: #f7fafc;
    padding: 8px 12px;
    border-radius: 6px;
    border-left: 3px solid #cbd5e0;
    margin-top: 15px;
}

/* ================= TIJDLIJN ================= */
.timeline {
    position: relative;
    padding: 20px 0;
}

.timeline::before {
    content: '';
    position: absolute;
    left: 9px;
    top: 0;
    bottom: 0;
    width: 3px;
    background: #eaf6fb;
    border-radius: 2px;
}

.timeline-item {
    position: relative;
    margin-bottom: 30px;
    display: flex;
}

.timeline-marker {
    position: relative;
    width: 20px;
    flex-shrink: 0;
}

.timeline-dot {
    position: absolute;
    left: 0;
    top: 6px;
    width: 20px;
    height: 20px;
    background: #fff;
    border: 4px solid #3bb3ce;
    border-radius: 50%;
    box-shadow: 0 0 0 3px rgba(59, 179, 206, 0.15);
    z-index: 2;
}

.timeline-content-wrapper {
    margin-left: 25px;
    flex-grow: 1;
}

.timeline-header {
    margin-bottom: 6px;
}

.timeline-date {
    font-size: 0.8rem;
    font-weight: 700;
    color: #2a7ca3;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.timeline-card {
    background: #fff;
    border: 2px solid #f0f7fa;
    padding: 14px 20px;
    border-radius: 12px;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.02);
    transition: all 0.2s;
    position: relative;
}

.timeline-card:hover {
    border-color: #eaf6fb;
    box-shadow: 0 4px 10px rgba(59, 179, 206, 0.06);
}

/* ================= EMPTY STATE ================= */
.empty-state {
    text-align: center;
    background: #fafdff;
    border-radius: 12px;
    border: 2px dashed #eaf6fb;
}

.empty-icon {
    font-size: 2.5rem;
    margin-bottom: 10px;
    opacity: 0.6;
}

.empty-state p {
    color: #718096;
    font-weight: 500;
    margin-bottom: 0;
}
</style>