<template>
    <div class="patient-header">
        <div class="patient-avatar">{{ initials }}</div>
        <div class="patient-title">
            <h2>{{ patient.firstName || patient.FirstName }} {{ patient.lastName || patient.LastName }}</h2>
            <span :class="['status-badge', statusClass]">{{ statusLabel }}</span>
        </div>
    </div>
</template>

<script setup>
import { computed } from 'vue';
const props = defineProps(['patient']);

const initials = computed(() => {
    const f = props.patient.firstName || props.patient.FirstName || '';
    const l = props.patient.lastName || props.patient.LastName || '';
    return `${f[0] || ''}${l[0] || ''}`.toUpperCase();
});

const STATUS_MAP = {
    '0': { label: 'Intake gepland', class: 'status-planned' },
    '1': { label: 'Actief', class: 'status-active' },
    '2': { label: 'Afgerond', class: 'status-completed' },
    '3': { label: 'On hold', class: 'status-hold' }
};

const statusLabel = computed(() => STATUS_MAP[String(props.patient.status)]?.label || 'Onbekend');
const statusClass = computed(() => STATUS_MAP[String(props.patient.status)]?.class || 'status-default');
</script>