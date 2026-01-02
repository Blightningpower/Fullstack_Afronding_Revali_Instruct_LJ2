<template>
    <div class="patient-info-grid">
        <div class="info-item">
            <span class="info-label">🎂 Geboortedatum</span>
            <span class="info-value">
                {{ (patient.birthDate || patient.BirthDate) ? formatDateLong(patient.birthDate || patient.BirthDate) :
                'Niet ingevuld' }}
            </span>
        </div>
        <div class="info-item">
            <span class="info-label">📅 Startdatum behandeling</span>
            <span class="info-value">
                {{ (patient.startDate || patient.StartDate) ? formatDateLong(patient.startDate || patient.StartDate) :
                'Niet ingevuld' }}
            </span>
        </div>
        <div class="info-item">
            <span class="info-label">📧 E-mail</span>
            <span class="info-value">
                <template v-if="patient?.email || patient?.Email">
                    <a :href="`mailto:${patient.email || patient.Email}`">{{ patient.email || patient.Email }}</a>
                </template>
                <template v-else>Niet ingevuld</template>
            </span>
        </div>
        <div class="info-item">
            <span class="info-label">📞 Telefoon</span>
            <span class="info-value">
                {{ patient?.phone || patient?.Phone || 'Niet ingevuld' }}
            </span>
        </div>
        <div class="info-item">
            <span class="info-label">🩺 Verwijzend arts</span>
            <span class="info-value">
                {{ patient?.referringDoctor?.firstName || patient?.ReferringDoctor?.FirstName }}
                {{ patient?.referringDoctor?.lastName || patient?.ReferringDoctor?.LastName }}
            </span>
        </div>
    </div>
</template>

<script setup>
const props = defineProps(['patient']);

const formatDateLong = (dateString) => {
    if (!dateString) return 'Nog niet bekend';
    const date = new Date(dateString);
    return date.toLocaleDateString('nl-NL', {
        day: 'numeric',
        month: 'long',
        year: 'numeric'
    });
};
</script>